using Microsoft.EntityFrameworkCore;
using NorthboundSessions.Data;
using NorthboundSessions.Web.Data;

namespace NorthboundSessions.Web.Services
{
    public class QuizService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
        private readonly LessonAccessService _lessonAccess;

        public QuizService(IDbContextFactory<ApplicationDbContext> dbFactory, LessonAccessService lessonAccess)
        {
            _dbFactory = dbFactory;
            _lessonAccess = lessonAccess;
        }

        public async Task<Quiz?> GetQuizWithDetailsAsync(int quizId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == quizId);
        }

        public async Task<QuizAttempt> SubmitQuizAsync(int quizId, string studentId, Dictionary<int, int> selectedOptions)
        {
            if (string.IsNullOrWhiteSpace(studentId))
            {
                throw new InvalidOperationException("A signed-in student is required to submit a quiz.");
            }

            if (!await _lessonAccess.CanTakeQuizAsync(studentId, quizId))
            {
                throw new InvalidOperationException("Finish this lesson and pass the previous quiz before taking this one.");
            }

            await using var context = await _dbFactory.CreateDbContextAsync();

            var quiz = await context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == quizId)
                ?? throw new InvalidOperationException($"Quiz {quizId} does not exist.");

            var correctCount = quiz.Questions.Count(question =>
            {
                var correct = question.Options.FirstOrDefault(o => o.IsCorrect);
                return correct is not null
                    && selectedOptions.TryGetValue(question.Id, out var selected)
                    && selected == correct.Id;
            });

            var questionCount = quiz.Questions.Count;
            var scorePercent = questionCount == 0 ? 0 : correctCount * 100.0 / questionCount;
            var passed = scorePercent >= quiz.PassingScorePercent;
            var now = DateTimeOffset.UtcNow;

            var attempt = new QuizAttempt
            {
                QuizId = quizId,
                StudentId = studentId,
                Score = correctCount,
                Passed = passed,
                SubmittedAt = now
            };

            context.QuizAttempts.Add(attempt);
            await context.SaveChangesAsync();

            if (passed)
            {
                var progress = await context.LessonProgresses
                    .FirstOrDefaultAsync(p => p.StudentId == studentId && p.LessonId == quiz.LessonId);

                if (progress is null)
                {
                    context.LessonProgresses.Add(new LessonProgress
                    {
                        StudentId = studentId,
                        LessonId = quiz.LessonId,
                        FurthestSlideIndex = 0,
                        IsCompleted = true,
                        CompletedAt = now,
                        UpdatedAt = now
                    });
                }
                else if (!progress.IsCompleted)
                {
                    progress.IsCompleted = true;
                    progress.CompletedAt = now;
                    progress.UpdatedAt = now;
                }

                await context.SaveChangesAsync();
            }

            return attempt;
        }
    }
}
