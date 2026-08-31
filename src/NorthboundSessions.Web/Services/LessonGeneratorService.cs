using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; 
using NorthboundSessions.Data; 
using NorthboundSessions.Web.Data; 


namespace NorthboundSessions.Web.Services
{
    public class LessonGeneratorService
    {
        //Create the service shell using the db
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public LessonGeneratorService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }
        //Create a lesson
        public async Task<Lesson?>GenerateNextLessonAsync()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            //Find the next unused topic
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
            
            //Create the real lesson and save it
            var today = DateOnly.FromDateTime(DateTime.UtcNow); 
            var isLiveDay = DateTime.UtcNow.DayOfWeek == DayOfWeek.Tuesday || DateTime.UtcNow.DayOfWeek == DayOfWeek.Thursday; 
            var lesson = new Lesson { 
                Title = nextTopic.Title, 
                OutlineContent = nextTopic.OutlineContent, 
                MarketSymbol = nextTopic.MarketSymbol, 
                ReleaseDate = today, 
                IsLiveDay = isLiveDay }; 
            context.Lessons.Add(lesson); 
            await context.SaveChangesAsync(); 

            //Building the real quiz questions, and options from template
            var quiz = new Quiz{
                Title = $"{nextTopic.Title} Quiz",
                LessonId = lesson.Id
            };
            foreach (var bankQuestion in nextTopic.Questions) 
                { 
                var quizQuestion = new QuizQuestion { QuestionText = bankQuestion.QuestionText, DisplayOrder = bankQuestion.DisplayOrder }; 
                foreach (var bankOption in bankQuestion.Options)
                    { 
                        quizQuestion.Options.Add(new QuizOption { 
                            OptionText = bankOption.OptionText, 
                            IsCorrect = bankOption.IsCorrect, 
                            DisplayOrder = bankOption.DisplayOrder }); 
                    } 
                    quiz.Questions.Add(quizQuestion); 
                } 
            context.Quizzes.Add(quiz); 
            
            //Mark the topic used, save everything, return the lesson
            nextTopic.IsUsed = true; 
            await context.SaveChangesAsync(); 
            return lesson;
        }

    }
}
