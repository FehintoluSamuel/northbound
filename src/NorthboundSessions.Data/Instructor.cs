namespace NorthboundSessions.Data;

public class LandingPageContent
{
    public int Id { get; set; } = 1;
    public string HeroEyebrow { get; set; } = "";
    public string HeroTitle { get; set; } = "";
    public string HeroAccent { get; set; } = "";
    public string HeroSubheadline { get; set; } = "";
    public string HeroImageUrl { get; set; } = "images/hero.jpg";
    public string CurriculumHeading { get; set; } = "";
    public string CurriculumIntro { get; set; } = "";
    public string FaqHeading { get; set; } = "";
    public string FinalCtaHeading { get; set; } = "";
    public string FinalCtaText { get; set; } = "";
}

public class Instructor
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public string Headline { get; set; } = "";
    public required string Bio { get; set; }
    public string PhotoUrl { get; set; } = "images/instructor.jpg";
    public string YearsTeaching { get; set; } = "";
    public string YearsTrading { get; set; } = "";
    public string MarketFocus { get; set; } = "";
    public string? ContactEmail { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GithubUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class Testimonial
{
    public int Id { get; set; }
    public required string Quote { get; set; }
    public required string Attribution { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class FaqItem
{
    public int Id { get; set; }
    public required string Question { get; set; }
    public required string Answer { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class CurriculumModule
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
