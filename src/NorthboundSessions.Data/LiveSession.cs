using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NorthboundSessions.Data
{
    public class LiveSession
    {
        public int Id {get; set;}
        public DateTimeOffset ScheduledAt {get; set;}
        public required string Topic {get; set;}
        public required string MeetLink {get; set;}
        public ICollection<Attendance> Attendance {get; set;} = new List<Attendance>(); 
    }

    
    
}
