using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NorthboundSessions.Web.Data;
using NorthboundSessions.Data;

namespace NorthboundSessions.Web.Services
{
    public class Slide
    {
        public int SlideNumber { get; set; }
        public required string Title { get; set; }
        public required string BodyText { get; set; }
    }
}
