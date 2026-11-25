using Microsoft.EntityFrameworkCore;
using NorthboundSessions.Data;
using NorthboundSessions.Web.Data;

namespace NorthboundSessions.Web.Services;

public sealed record LessonState(
    int LessonId,
    int SortOrder,
    string Title,
    DateOnly ReleaseDate,
    bool IsLiveDay,
    string? MarketSymbol,
    bool IsReleased,
    bool IsCompleted,
    bool QuizPassed,
    bool IsComplete,
    bool IsUnlocked,
    string? LockReason);

public class LessonAccessService(IDbContextFactory<ApplicationDbContext> dbFactory)
{
    public async Task<IReadOnlyList<LessonState>> GetLessonStatesAsync(
        string studentId,
        CancellationToken cancellationToken = default)
    {
        await using var context = await dbFactory.CreateDbContextAsync(cancellationToken);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var lessons = await context.Lessons
            .AsNoTracking()
            .OrderBy(l => l.SortOrder)
            .ThenBy(l => l.ReleaseDate)
            .ThenBy(l => l.Id)
            .ToListAsync(cancellationToken);

        var quizzes = await context.Quizzes
            .AsNoTracking()
            .Select(q => new { q.Id, q.LessonId, q.PassingScorePercent })
            .ToListAsync(cancellationToken);

        var quizById = quizzes.ToDictionary(q => q.Id);
        var quizByLessonId = quizzes
            .GroupBy(q => q.LessonId)
            .ToDictionary(g => g.Key, g => g.OrderBy(q => q.Id).First());

        var quizIds = quizById.Keys.ToHashSet();
        var questionCounts = quizIds.Count == 0
            ? new Dictionary<int, int>()
            : await context.QuizQuestions
                .AsNoTracking()
                .Where(q => quizIds.Contains(q.QuizId))
                .GroupBy(q => q.QuizId)
                .Select(g => new { QuizId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.QuizId, x => x.Count, cancellationToken);

        var hasStudent = !string.IsNullOrWhiteSpace(studentId);
        var progressByLessonId = hasStudent
            ? await context.LessonProgresses
                .AsNoTracking()
                .Where(p => p.StudentId == studentId)
                .ToDictionaryAsync(p => p.LessonId, cancellationToken)
            : new Dictionary<int, LessonProgress>();

        var attempts = hasStudent
            ? await context.QuizAttempts
                .AsNoTracking()
                .Where(a => a.StudentId == studentId)
                .ToListAsync(cancellationToken)
            : new List<QuizAttempt>();

        bool HasPassed(QuizAttempt attempt)
        {
            if (attempt.Passed)
            {
                return true;
            }

            if (!quizById.TryGetValue(attempt.QuizId, out var quiz))
            {
                return false;
            }

            var questionCount = questionCounts.GetValueOrDefault(attempt.QuizId);
            return questionCount > 0
                && attempt.Score * 100.0 / questionCount >= quiz.PassingScorePercent;
        }

        var passedLessonIds = attempts
            .Where(HasPassed)
            .Select(a => quizById[a.QuizId].LessonId)
            .ToHashSet();

        var states = new List<LessonState>(lessons.Count);

        for (var i = 0; i < lessons.Count; i++)
        {
            var lesson = lessons[i];
            var isReleased = lesson.ReleaseDate <= today;
            var progress = progressByLessonId.GetValueOrDefault(lesson.Id);
            var quizPassed = passedLessonIds.Contains(lesson.Id);
            var isCompleted = progress?.IsCompleted == true || quizPassed;
            var hasQuiz = quizByLessonId.ContainsKey(lesson.Id);
            var isComplete = hasQuiz ? isCompleted && quizPassed : isCompleted;

            bool isUnlocked;
            string? lockReason = null;

            if (!isReleased)
            {
                isUnlocked = false;
                lockReason = "Not released yet";
            }
            else if (i == 0)
            {
                isUnlocked = true;
            }
            else
            {
                var previous = states[i - 1];
                var previousHasQuiz = quizByLessonId.ContainsKey(previous.LessonId);
                var previousQuizPassed = passedLessonIds.Contains(previous.LessonId);
                var requirementsMet = previous.IsCompleted
                    && (!previousHasQuiz || previousQuizPassed);

                isUnlocked = requirementsMet;

                if (!requirementsMet)
                {
                    lockReason = !previous.IsCompleted && !previousQuizPassed
                        ? $"Finish {previous.Title} and pass its quiz to unlock this lesson"
                        : !previous.IsCompleted
                            ? $"Finish {previous.Title} to unlock this lesson"
                            : $"Pass the quiz for {previous.Title} to unlock this lesson";
                }
            }

            states.Add(new LessonState(
                lesson.Id,
                lesson.SortOrder,
                lesson.Title,
                lesson.ReleaseDate,
                lesson.IsLiveDay,
                lesson.MarketSymbol,
                isReleased,
                isCompleted,
                quizPassed,
                isComplete,
                isUnlocked,
                lockReason));
        }

        return states;
    }

    public async Task<LessonState?> GetLessonStateAsync(
        string studentId,
        int lessonId,
        CancellationToken cancellationToken = default)
    {
        var states = await GetLessonStatesAsync(studentId, cancellationToken);
        return states.FirstOrDefault(s => s.LessonId == lessonId);
    }

    public async Task<LessonState?> GetNextLessonAsync(
        string studentId,
        CancellationToken cancellationToken = default)
    {
        var states = await GetLessonStatesAsync(studentId, cancellationToken);
        return states.FirstOrDefault(s => s.IsUnlocked && !s.IsComplete);
    }

    public async Task<bool> CanAccessLessonAsync(
        string studentId,
        int lessonId,
        CancellationToken cancellationToken = default)
    {
        var state = await GetLessonStateAsync(studentId, lessonId, cancellationToken);
        return state?.IsUnlocked == true;
    }

    public async Task<bool> CanTakeQuizAsync(
        string studentId,
        int quizId,
        CancellationToken cancellationToken = default)
    {
        await using var context = await dbFactory.CreateDbContextAsync(cancellationToken);
        var quiz = await context.Quizzes
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == quizId, cancellationToken);

        if (quiz is null)
        {
            return false;
        }

        var state = await GetLessonStateAsync(studentId, quiz.LessonId, cancellationToken);
        return state?.IsUnlocked == true && state.IsCompleted;
    }
}
