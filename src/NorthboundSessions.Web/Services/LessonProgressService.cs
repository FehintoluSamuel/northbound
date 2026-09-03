using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; 
using NorthboundSessions.Data; 
using NorthboundSessions.Web.Data; 


namespace NorthboundSessions.Web.Services
{
    public class LessonProgressService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory; 
        public LessonProgressService(IDbContextFactory<ApplicationDbContext> dbFactory) 
            { _dbFactory = dbFactory; } 

        public async Task<int> GetFurthestSlideAsync(string studentId, int lessonId) 
            { await using var context = await _dbFactory.CreateDbContextAsync(); 
            var progress = await context.LessonProgresses 
            .FirstOrDefaultAsync(p => p.StudentId == studentId && p.LessonId == lessonId); 

            return progress?.FurthestSlideIndex ?? 0; 
            } 
            
        public async Task RecordProgressAsync(string studentId, int lessonId, int slideIndex) 
            { await using var context = await _dbFactory.CreateDbContextAsync(); 
            var existing = await context.LessonProgresses 
            .FirstOrDefaultAsync(p => p.StudentId == studentId && p.LessonId == lessonId); 
            
            if (existing is null) 
            { 
                context.LessonProgresses.Add(new LessonProgress 
                { 
                StudentId = studentId, 
                LessonId = lessonId, 
                FurthestSlideIndex = slideIndex, 
                UpdatedAt = DateTimeOffset.UtcNow 
                }
                ); 
            } 
            else if (slideIndex > existing.FurthestSlideIndex) 
            { 
                existing.FurthestSlideIndex = slideIndex; 
                existing.UpdatedAt = DateTimeOffset.UtcNow; 
            } 
            // else: client reported a slideIndex that isn't further than what's 
            // already stored — silently ignored, exactly as the contract requires. 
            await context.SaveChangesAsync(); 
            } 
    }
}
