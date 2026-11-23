using Microsoft.EntityFrameworkCore;
using NorthboundSessions.Data;
using NorthboundSessions.Web.Data;

namespace NorthboundSessions.Web.Services
{
    public class AttendanceService
    {
        public static readonly TimeSpan OpensBefore = TimeSpan.FromMinutes(30);
        public static readonly TimeSpan ClosesAfter = TimeSpan.FromHours(2);

        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public AttendanceService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public static bool IsCheckInOpen(LiveSession session, DateTimeOffset now) =>
            now >= session.ScheduledAt - OpensBefore && now <= session.ScheduledAt + ClosesAfter;

        public async Task<Attendance> CheckInAsync(int liveSessionId, string studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId))
            {
                throw new InvalidOperationException("A signed-in student is required to check in.");
            }

            await using var context = await _dbFactory.CreateDbContextAsync();

            var existing = await context.Attendances
                .FirstOrDefaultAsync(a => a.LiveSessionId == liveSessionId && a.StudentId == studentId);

            if (existing is not null)
            {
                return existing;
            }

            var session = await context.LiveSessions.FindAsync(liveSessionId)
                ?? throw new InvalidOperationException($"Session {liveSessionId} does not exist.");

            if (!IsCheckInOpen(session, DateTimeOffset.UtcNow))
            {
                throw new InvalidOperationException(
                    "Check-in is only open from 30 minutes before the session starts until 2 hours after it starts.");
            }

            var attendance = new Attendance
            {
                LiveSessionId = liveSessionId,
                StudentId = studentId,
                CheckedInAt = DateTimeOffset.UtcNow
            };

            context.Attendances.Add(attendance);
            await context.SaveChangesAsync();
            return attendance;
        }

        public async Task<List<Attendance>> GetAttendanceForSessionAsync(int liveSessionId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.Attendances.Where(a => a.LiveSessionId == liveSessionId).ToListAsync();
        }
    }
}
