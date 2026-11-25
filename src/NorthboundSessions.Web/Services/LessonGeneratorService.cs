using Microsoft.EntityFrameworkCore;
using NorthboundSessions.Data;
using NorthboundSessions.Web.Data;

namespace NorthboundSessions.Web.Services
{
    public class LessonGeneratorService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public LessonGeneratorService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<Lesson?> GenerateNextLessonAsync()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            var strategy = context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await context.Database.BeginTransactionAsync();

                var nextTopic = await context.TopicBankItems
                    .Include(t => t.Questions)
                    .ThenInclude(q => q.Options)
                    .Where(t => !t.IsUsed)
                    .OrderBy(t => t.Id)
                    .FirstOrDefaultAsync();

                if (nextTopic is null)
                {
                    return null;
                }

                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var isLiveDay = DateTime.UtcNow.DayOfWeek == DayOfWeek.Tuesday || DateTime.UtcNow.DayOfWeek == DayOfWeek.Thursday;
                var nextSortOrder = (await context.Lessons.MaxAsync(l => (int?)l.SortOrder) ?? 0) + 1;
                var lesson = new Lesson
                {
                    Title = nextTopic.Title,
                    OutlineContent = nextTopic.OutlineContent,
                    MarketSymbol = nextTopic.MarketSymbol,
                    ImageBytes = nextTopic.ImageBytes,
                    ReleaseDate = today,
                    SortOrder = nextSortOrder,
                    IsLiveDay = isLiveDay
                };
                context.Lessons.Add(lesson);
                await context.SaveChangesAsync();

                var quiz = new Quiz
                {
                    Title = $"{nextTopic.Title} Quiz",
                    LessonId = lesson.Id
                };

                foreach (var bankQuestion in nextTopic.Questions)
                {
                    var quizQuestion = new QuizQuestion
                    {
                        QuestionText = bankQuestion.QuestionText,
                        DisplayOrder = bankQuestion.DisplayOrder
                    };

                    foreach (var bankOption in bankQuestion.Options)
                    {
                        quizQuestion.Options.Add(new QuizOption
                        {
                            OptionText = bankOption.OptionText,
                            IsCorrect = bankOption.IsCorrect,
                            DisplayOrder = bankOption.DisplayOrder
                        });
                    }

                    quiz.Questions.Add(quizQuestion);
                }

                context.Quizzes.Add(quiz);
                nextTopic.IsUsed = true;
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return lesson;
            });
        }

    }
}
