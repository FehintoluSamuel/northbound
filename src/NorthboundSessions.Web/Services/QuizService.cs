using Microsoft.EntityFrameworkCore;
using NorthboundSessions.Data;
using NorthboundSessions.Web.Data;

namespace NorthboundSessions.Web.Services
{
    public class QuizService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public QuizService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
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

            await using var context = await _dbFactory.CreateDbContextAsync();

            var quiz = await context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == quizId)
                ?? throw new InvalidOperationException($"Quiz {quizId} does not exist.");

            if (await context.QuizAttempts.AnyAsync(a => a.QuizId == quizId && a.StudentId == studentId))
            {
                throw new InvalidOperationException("This quiz has already been attempted.");
            }

            var correctCount = quiz.Questions.Count(question =>
            {
                var correct = question.Options.FirstOrDefault(o => o.IsCorrect);
                return correct is not null
                    && selectedOptions.TryGetValue(question.Id, out var selected)
                    && selected == correct.Id;
            });

            var attempt = new QuizAttempt
            {
                QuizId = quizId,
                StudentId = studentId,
                Score = correctCount,
                SubmittedAt = DateTimeOffset.UtcNow
            };

            context.QuizAttempts.Add(attempt);
            await context.SaveChangesAsync();
            return attempt;
        }
    }
}
