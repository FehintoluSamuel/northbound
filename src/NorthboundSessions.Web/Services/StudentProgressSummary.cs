using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NorthboundSessions.Web.Services
{
    public class StudentProgressSummary
    {
        public required string Email { get; set; } 
        public int QuizzesAttempted { get; set; } 
        public double AverageScore { get; set; } 
        public int SessionsAttended { get; set; }
    }
}
