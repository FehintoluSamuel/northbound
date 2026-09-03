using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NorthboundSessions.Data
{
    [Index(nameof(StudentId), nameof(LessonId), IsUnique = true)]
    public class LessonProgress
    {
        public int Id { get; set; } 
        
        public required string StudentId { get; set; } 
        
        public int LessonId { get; set; } 
        
        public int FurthestSlideIndex { get; set; } 
        
        public DateTimeOffset UpdatedAt { get; set; } 
    }
}
