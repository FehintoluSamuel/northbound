using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NorthboundSessions.Web.Data;
using NorthboundSessions.Data;


namespace NorthboundSessions.Web.Services
{
    public class SlideService
    {
        public List<Slide> GenerateSlides(Lesson lesson)
        {
            var slides = new List<Slide>();
            if (string.IsNullOrWhiteSpace(lesson.OutlineContent))
            {
                return slides;
            }
            var paragraphs = lesson.OutlineContent.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < paragraphs.Length; i++)
            {
                var slideDisplay = new Slide{SlideNumber = i + 1, Title = i ==0? lesson.Title : $"{lesson.Title} (continued)", BodyText = paragraphs[i]};
                slides.Add(slideDisplay);
            }
            return slides;
        }
    }
}
