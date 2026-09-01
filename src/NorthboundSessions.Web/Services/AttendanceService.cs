using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; 
using NorthboundSessions.Data; 
using NorthboundSessions.Web.Data; 


namespace NorthboundSessions.Web.Services
{
    public class AttendanceService
    {
        //Perform the database handshake
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory; 
        public AttendanceService(IDbContextFactory<ApplicationDbContext> dbFactory) 
        { 
            _dbFactory = dbFactory; 
        }

        //Create attendance checkings for students
        public async Task<Attendance> CheckInAsync (int liveSessionId, string studentId)
        {
            
            await using var context = await _dbFactory.CreateDbContextAsync(); //Access the db asynchronously
            var existing = await context.Attendances.FirstOrDefaultAsync(a => a.LiveSessionId == liveSessionId && a.StudentId == studentId); //Extract LiveSesseion and student Ids from the db
            //Check if attendance has been marked already
            if (existing is not null)
            {
                return existing; //Return LiveSesseion and student Ids if they exist
            }
            //Create new attendance if not marked
            var attendance = new Attendance
            {
                LiveSessionId = liveSessionId,
                StudentId = studentId,
                CheckedInAt = DateTimeOffset.UtcNow
            };
            //Add the attendance
            context.Attendances.Add(attendance);
            await context.SaveChangesAsync();
            return attendance;
        }

        //Create method for tutor to get attendance
        public async Task<List<Attendance>> GetattendanceForSessionAsync(int liveSessionId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.Attendances.Where(a =>a.LiveSessionId == liveSessionId).ToListAsync();
        }
    }
}
