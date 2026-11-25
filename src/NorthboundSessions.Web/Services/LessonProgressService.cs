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

        public async Task<bool> IsLessonCompletedAsync(string studentId, int lessonId)
            {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.LessonProgresses
                .AnyAsync(p => p.StudentId == studentId && p.LessonId == lessonId && p.IsCompleted);
            }

        public async Task RecordProgressAsync(string studentId, int lessonId, int slideIndex, int? lastSlideIndex = null)
            { await using var context = await _dbFactory.CreateDbContextAsync();
            var existing = await context.LessonProgresses
            .FirstOrDefaultAsync(p => p.StudentId == studentId && p.LessonId == lessonId);

            var isCompleted = lastSlideIndex.HasValue && slideIndex >= lastSlideIndex.Value;
            var now = DateTimeOffset.UtcNow;

            if (existing is null)
            {
                context.LessonProgresses.Add(new LessonProgress
                {
                StudentId = studentId,
                LessonId = lessonId,
                FurthestSlideIndex = slideIndex,
                IsCompleted = isCompleted,
                CompletedAt = isCompleted ? now : null,
                UpdatedAt = now
                }
                );
            }
            else
            {
                if (slideIndex > existing.FurthestSlideIndex)
                {
                    existing.FurthestSlideIndex = slideIndex;
                    existing.UpdatedAt = now;
                }

                if (isCompleted && !existing.IsCompleted)
                {
                    existing.IsCompleted = true;
                    existing.CompletedAt = now;
                    existing.UpdatedAt = now;
                }
            }
            await context.SaveChangesAsync();
            }
    }
}
