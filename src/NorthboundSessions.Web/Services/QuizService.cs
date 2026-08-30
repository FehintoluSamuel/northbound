using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NorthboundSessions.Web.Data;
using NorthboundSessions.Data;
//using NorthboundSessions.ApplicationDbContext;

namespace NorthboundSessions.Web.Services
{
    public class QuizService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        // This is the constructor
        public QuizService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        

        // This is a regular method 
        public async Task<Quiz?> GetQuizWithDetailsAsync(int quizId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            // logic 
            var quiz = await context.Quizzes
            .Include(q=> q.Questions)
            .ThenInclude(q=> q.Options)
            .FirstOrDefaultAsync(q => q.Id == quizId);

            return quiz;
        }

        public async Task<QuizAttempt> SubmitQuizAsync(int quizId, string studentId, Dictionary<int, int> selectedOptions)
        {
            // logic 
            await using var context = await _dbFactory.CreateDbContextAsync();
            var quiz = await GetQuizWithDetailsAsync(quizId);
            int correctCount = 0; //setting up a counter that holds the running total as each question is checked
            foreach (var quizQuestion in quiz.Questions)
            {
                var selectedOptionId = selectedOptions[quizQuestion.Id]; //pulls out which option the student picked for THIS specific question, using the question's Id as the dictionary key.
                var correctOption = quizQuestion.Options.FirstOrDefault(o=> o.IsCorrect);//asks the database-loaded data (the source of truth) which option was actually correct
                if (correctOption != null && correctOption.Id == selectedOptionId) 
                { 
                    correctCount++; 
                }

            }

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
