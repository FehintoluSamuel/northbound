using Microsoft.EntityFrameworkCore;
using NorthboundSessions.Web.Data;

namespace NorthboundSessions.Web.Services;

public record StudentStats(
    int CompletedLessons,
    int ReleasedLessons,
    double? AverageQuizScore,
    int SessionsAttended,
    int CompletionPercent);

public class StudentStatsService(IDbContextFactory<ApplicationDbContext> dbFactory)
{
    public async Task<StudentStats> GetStatsAsync(string studentId)
    {
        await using var context = await dbFactory.CreateDbContextAsync();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var releasedLessonIds = await context.Lessons
            .Where(l => l.ReleaseDate <= today)
            .Select(l => l.Id)
            .ToListAsync();

        var completedLessonIds = await CompletedLessonIdsAsync(context, studentId, today);
        var percentages = await ScorePercentagesAsync(context, studentId);
        var sessionsAttended = await context.Attendances.CountAsync(a => a.StudentId == studentId);

        var completedLessons = completedLessonIds.Intersect(releasedLessonIds).Count();

        return new StudentStats(
            completedLessons,
            releasedLessonIds.Count,
            percentages.Count > 0 ? percentages.Values.Average() : null,
            sessionsAttended,
            releasedLessonIds.Count == 0 ? 0 : (int)Math.Round(completedLessons * 100.0 / releasedLessonIds.Count));
    }

    public async Task<HashSet<int>> GetCompletedLessonIdsAsync(string studentId)
    {
        await using var context = await dbFactory.CreateDbContextAsync();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return await CompletedLessonIdsAsync(context, studentId, today);
    }

    public async Task<Dictionary<int, double>> GetQuizScorePercentagesAsync(string studentId)
    {
        await using var context = await dbFactory.CreateDbContextAsync();
        return await ScorePercentagesAsync(context, studentId);
    }

    private static async Task<HashSet<int>> CompletedLessonIdsAsync(
        ApplicationDbContext context, string studentId, DateOnly today)
    {
        var ids = await context.QuizAttempts
            .Where(a => a.StudentId == studentId
                && a.Quiz != null
                && a.Quiz.Lesson != null
                && a.Quiz.Lesson.ReleaseDate <= today)
            .Select(a => a.Quiz!.LessonId)
            .Distinct()
            .ToListAsync();

        return ids.ToHashSet();
    }

    private static async Task<Dictionary<int, double>> ScorePercentagesAsync(
        ApplicationDbContext context, string studentId)
    {
        var attempts = await context.QuizAttempts
            .AsNoTracking()
            .Where(a => a.StudentId == studentId)
            .ToListAsync();

        if (attempts.Count == 0)
        {
            return new Dictionary<int, double>();
        }

        var quizIds = attempts.Select(a => a.QuizId).Distinct().ToList();
        var questionCounts = await context.QuizQuestions
            .AsNoTracking()
            .Where(q => quizIds.Contains(q.QuizId))
            .GroupBy(q => q.QuizId)
            .Select(g => new { QuizId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.QuizId, x => x.Count);

        return attempts
            .GroupBy(a => a.QuizId)
            .ToDictionary(
                g => g.Key,
                g =>
                {
                    var latest = g.OrderByDescending(a => a.SubmittedAt).First();
                    return questionCounts.TryGetValue(g.Key, out var total) && total > 0
                        ? Math.Round(latest.Score * 100.0 / total, 1)
                        : 0;
                });
    }
}
