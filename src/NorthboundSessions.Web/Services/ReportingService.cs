using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; 
using NorthboundSessions.Web.Data; 
using NorthboundSessions.Data;

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
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync();

                var instructorUserIds = await context.UserRoles
                    .Where(ur => ur.RoleId == instructorRoleId)
                    .Select(ur => ur.UserId)
                    .ToListAsync();

                var students = await context.Users
                    .Where(u => !instructorUserIds.Contains(u.Id))
                    .ToListAsync();

                var studentIds = students.Select(s => s.Id).ToList();

                // Pull ALL attempts and attendance rows once, instead of two
                // queries per student inside the loop (was causing N+1 queries —
                // 84+ round trips for 42 students; now it's 3 queries total).
                var allAttempts = await context.QuizAttempts
                    .Where(a => studentIds.Contains(a.StudentId))
                    .ToListAsync();

                var allAttendances = await context.Attendances
                    .Where(a => studentIds.Contains(a.StudentId))
                    .ToListAsync();

                // Score is a raw correct-answer count, not a percentage — convert
                // per-attempt using that quiz's actual question count, since
                // different quizzes can have different totals.
                var quizIds = allAttempts.Select(a => a.QuizId).Distinct().ToList();
                var questionCounts = await context.QuizQuestions
                    .Where(q => quizIds.Contains(q.QuizId))
                    .GroupBy(q => q.QuizId)
                    .Select(g => new { QuizId = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.QuizId, x => x.Count);

                double ToPercent(QuizAttempt a) =>
                    questionCounts.TryGetValue(a.QuizId, out var total) && total > 0
                        ? a.Score * 100.0 / total
                        : 0;

                var summaries = new List<StudentProgressSummary>();

                foreach (var student in students)
                {
                    var attempts = allAttempts.Where(a => a.StudentId == student.Id).ToList();
                    var attendanceCount = allAttendances.Count(a => a.StudentId == student.Id);

                    summaries.Add(new StudentProgressSummary
                    {
                        Email = student.Email ?? "(no email)",
                        QuizzesAttempted = attempts.Count,
                        AverageScore = attempts.Count > 0 ? attempts.Average(ToPercent) : 0,
                        SessionsAttended = attendanceCount
                    });
                }

                return summaries;
            }
        }
    }
