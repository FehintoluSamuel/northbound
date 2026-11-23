namespace NorthboundSessions.Web.Services;

public class LandingContentDto
{
    public HeroContentDto Hero { get; set; } = new();
    public List<InstructorDto> Instructors { get; set; } = new();
    public List<TestimonialDto> Testimonials { get; set; } = new();
    public List<FaqItemDto> Faqs { get; set; } = new();
    public List<CurriculumModuleDto> Curriculum { get; set; } = new();
}

public class HeroContentDto
{
    public string Eyebrow { get; set; } = "";
    public string Title { get; set; } = "";
    public string Accent { get; set; } = "";
    public string Subheadline { get; set; } = "";
    public string ImageUrl { get; set; } = "";
    public string CurriculumHeading { get; set; } = "";
    public string CurriculumIntro { get; set; } = "";
    public string FaqHeading { get; set; } = "";
    public string FinalCtaHeading { get; set; } = "";
    public string FinalCtaText { get; set; } = "";
}

public class InstructorDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Headline { get; set; } = "";
    public string Bio { get; set; } = "";
    public string PhotoUrl { get; set; } = "";
    public string YearsTeaching { get; set; } = "";
    public string YearsTrading { get; set; } = "";
    public string MarketFocus { get; set; } = "";
    public string? ContactEmail { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GithubUrl { get; set; }
    public int DisplayOrder { get; set; }
}

public class TestimonialDto
{
    public int Id { get; set; }
    public string Quote { get; set; } = "";
    public string Attribution { get; set; } = "";
    public int DisplayOrder { get; set; }
}

public class FaqItemDto
{
    public int Id { get; set; }
    public string Question { get; set; } = "";
    public string Answer { get; set; } = "";
    public int DisplayOrder { get; set; }
}

public class CurriculumModuleDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public int DisplayOrder { get; set; }
}
