/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

 // src/NorthboundSessions.Data/Instructor.cs
namespace NorthboundSessions.Data
{
    public class Instructor
    {

    public int Id { get; set; }                    // EF Core treats "Id" as the primary key by convention — no attribute needed
    public required string Name { get; set; }       // "required" (C# 11+) forces callers to set this — catches bugs at compile time
    public required string Bio { get; set; }
    public string? PhotoPath { get; set; }           // nullable ("?") because a photo might not be uploaded yet
    public int YearsTeaching { get; set; }
    public int YearsTrading { get; set; }
    public int StudentsTaught { get; set; }
    public int DisplayOrder { get; set; }            // controls sort order on the landing page
    public bool IsActive { get; set; } = true;        // lets you hide an instructor without deleting their record
    }

    public class Testimonial
    {
        public string QuoteText {get; set;}
        public string Attribution {get; set;}
        public string DisplayOrder {get; set;}
        public string attribution {get; set;}
        public bool IsActive {get; set;} = true;
    }

    public class FaqItem
    {
        public string Question {get; set;}
        public string Answer {get; set;}
        public int DisplayOrder { get; set; }            
        public bool IsActive { get; set; } = true;       
    }

    public class CurriculumModule
    {
        public string Title {get; set;}
        public string Description {get; set;}
        public string SortOrder {get; set;}

    }

    public class HeroContent
    {
        public string HeroContent {get; set;}
        public string AccentWord {get; set;}
        public string Subheadline {get; set;}
        public string CtaText {get; set;}
        public string HeroImagePath {get; set;}

    }

    public class SiteSettings
    {
        
    }

}
*/