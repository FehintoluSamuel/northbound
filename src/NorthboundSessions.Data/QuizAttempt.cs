using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NorthboundSessions.Data
{
    public class QuizAttempt
    {
        public int Id {get; set;}
        public int QuizId {get; set;}
        public Quiz? Quiz {get; set;}
        public required string StudentId {get; set;}
        public int Score {get; set;}
        public DateTimeOffset SubmittedAt {get; set;} = DateTimeOffset.UtcNow;
    }
    
}

