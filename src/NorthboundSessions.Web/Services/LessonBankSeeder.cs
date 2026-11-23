using Microsoft.EntityFrameworkCore;
using NorthboundSessions.Data;
using NorthboundSessions.Web.Data;

namespace NorthboundSessions.Web.Services;

public class LessonBankSeeder(IDbContextFactory<ApplicationDbContext> dbFactory)
{
    public async Task<LessonBankSeedResult> SeedAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await dbFactory.CreateDbContextAsync(cancellationToken);
        var strategy = context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

            var titles = LessonBankContent.Items.Select(item => item.Title).ToList();
            var existingTopics = await context.TopicBankItems
                .Where(item => titles.Contains(item.Title))
                .Include(item => item.Questions)
                .ThenInclude(item => item.Options)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            var added = 0;
            var updated = 0;
            foreach (var seed in LessonBankContent.Items)
            {
                var questions = seed.Questions.Select((question, questionIndex) => new BankQuestion
                {
                    QuestionText = question.Text,
                    DisplayOrder = questionIndex,
                    Options = question.Options.Select((option, optionIndex) => new BankOption
                    {
                        OptionText = option,
                        IsCorrect = optionIndex == question.CorrectIndex,
                        DisplayOrder = optionIndex
                    }).ToList()
                }).ToList();

                var topic = existingTopics.FirstOrDefault(item => item.Title == seed.Title);
                if (topic is null)
                {
                    context.TopicBankItems.Add(new TopicBankItem
                    {
                        Title = seed.Title,
                        OutlineContent = seed.OutlineContent,
                        MarketSymbol = seed.MarketSymbol,
                        IsUsed = false,
                        Questions = questions
                    });
                    added++;
                    continue;
                }

                topic.OutlineContent = seed.OutlineContent;
                topic.MarketSymbol = seed.MarketSymbol;
                topic.Questions.Clear();
                topic.Questions = questions;
                updated++;
            }

            var lessonOne = await context.Lessons.FirstOrDefaultAsync(
                lesson => lesson.Title == "Lesson 1 — Foundations: How Markets and Orders Actually Work",
                cancellationToken);

            if (lessonOne is not null && lessonOne.MarketSymbol == "SPY")
            {
                lessonOne.MarketSymbol = "AMEX:SPY";
            }

            await context.SaveChangesAsync(cancellationToken);
            var total = await context.TopicBankItems.CountAsync(item => titles.Contains(item.Title), cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return new LessonBankSeedResult(added, updated, total);
        });
    }
}

public sealed record LessonBankSeedResult(int Added, int Updated, int Total);
