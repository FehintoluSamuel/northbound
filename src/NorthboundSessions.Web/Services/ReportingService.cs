using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; 
using NorthboundSessions.Web.Data; 

namespace NorthboundSessions.Web.Services
{
    public class ReportingService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory; 
        public ReportingService(IDbContextFactory<ApplicationDbContext> dbFactory) 
        {
             _dbFactory = dbFactory; 
        }

        public async Task<List<StudentProgressSummary>> GetStudentProgressAsync() 
        { 
            await using var context = await _dbFactory.CreateDbContextAsync(); 
            var instructorRoleId = await context.Roles 
            .Where(r => r.Name == "Instructor") 
            .Select(r => r.Id) .FirstOrDefaultAsync(); 
            
            var instructorUserIds = await context.UserRoles 
            .Where(ur => ur.RoleId == instructorRoleId) 
            .Select(ur => ur.UserId) .ToListAsync(); 
            
            var students = await context.Users 
            .Where(u => !instructorUserIds.Contains(u.Id)) 
            .ToListAsync(); 
            
            var summaries = new List<StudentProgressSummary>(); 
            foreach (var student in students) 
            { 
                var attempts = await context.QuizAttempts 
                .Where(a => a.StudentId == student.Id) 
                .ToListAsync(); 
                
                var attendanceCount = await context.Attendances 
                .Where(a => a.StudentId == student.Id) 
                .CountAsync(); 
                
                summaries.Add(new StudentProgressSummary 
                { 
                    Email = student.Email ?? "(no email)", 
                    QuizzesAttempted = attempts.Count, 
                    AverageScore = attempts.Count > 0 ? attempts.Average(a => a.Score) : 0, 
                    SessionsAttended = attendanceCount 
                    
                }
                ); 
                    
                } 
             return summaries; 
        } 
    }
}
