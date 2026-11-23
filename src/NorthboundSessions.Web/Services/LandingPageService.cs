using Microsoft.EntityFrameworkCore;
using NorthboundSessions.Data;
using NorthboundSessions.Web.Data;

namespace NorthboundSessions.Web.Services;

public class LandingPageService(IDbContextFactory<ApplicationDbContext> dbFactory)
{
    public async Task<LandingContentDto> GetAsync()
    {
        await using var context = await dbFactory.CreateDbContextAsync();
        await EnsureSeededAsync(context);

        var page = await context.LandingPageContents.AsNoTracking().FirstAsync();
        var instructors = await context.Instructors.AsNoTracking()
            .Where(i => i.IsActive).OrderBy(i => i.DisplayOrder).ThenBy(i => i.Id).ToListAsync();
        var testimonials = await context.Testimonials.AsNoTracking()
            .Where(t => t.IsActive).OrderBy(t => t.DisplayOrder).ThenBy(t => t.Id).ToListAsync();
        var faqs = await context.FaqItems.AsNoTracking()
            .Where(f => f.IsActive).OrderBy(f => f.DisplayOrder).ThenBy(f => f.Id).ToListAsync();
        var curriculum = await context.CurriculumModules.AsNoTracking()
            .Where(c => c.IsActive).OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id).ToListAsync();

        return new LandingContentDto
        {
            Hero = new HeroContentDto
            {
                Eyebrow = page.HeroEyebrow,
                Title = page.HeroTitle,
                Accent = page.HeroAccent,
                Subheadline = page.HeroSubheadline,
                ImageUrl = page.HeroImageUrl,
                CurriculumHeading = page.CurriculumHeading,
                CurriculumIntro = page.CurriculumIntro,
                FaqHeading = page.FaqHeading,
                FinalCtaHeading = page.FinalCtaHeading,
                FinalCtaText = page.FinalCtaText
            },
            Instructors = instructors.Select(i => new InstructorDto
            {
                Id = i.Id,
                FullName = i.FullName,
                Headline = i.Headline,
                Bio = i.Bio,
                PhotoUrl = i.PhotoUrl,
                YearsTeaching = i.YearsTeaching,
                YearsTrading = i.YearsTrading,
                MarketFocus = i.MarketFocus,
                ContactEmail = i.ContactEmail,
                LinkedInUrl = i.LinkedInUrl,
                GithubUrl = i.GithubUrl,
                DisplayOrder = i.DisplayOrder
            }).ToList(),
            Testimonials = testimonials.Select(t => new TestimonialDto
            {
                Id = t.Id,
                Quote = t.Quote,
                Attribution = t.Attribution,
                DisplayOrder = t.DisplayOrder
            }).ToList(),
            Faqs = faqs.Select(f => new FaqItemDto
            {
                Id = f.Id,
                Question = f.Question,
                Answer = f.Answer,
                DisplayOrder = f.DisplayOrder
            }).ToList(),
            Curriculum = curriculum.Select(c => new CurriculumModuleDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                DisplayOrder = c.DisplayOrder
            }).ToList()
        };
    }

    public async Task SaveAsync(LandingContentDto dto)
    {
        await using var context = await dbFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        await EnsureSeededAsync(context);

        var page = await context.LandingPageContents.FirstAsync();
        page.HeroEyebrow = dto.Hero.Eyebrow;
        page.HeroTitle = dto.Hero.Title;
        page.HeroAccent = dto.Hero.Accent;
        page.HeroSubheadline = dto.Hero.Subheadline;
        page.HeroImageUrl = dto.Hero.ImageUrl;
        page.CurriculumHeading = dto.Hero.CurriculumHeading;
        page.CurriculumIntro = dto.Hero.CurriculumIntro;
        page.FaqHeading = dto.Hero.FaqHeading;
        page.FinalCtaHeading = dto.Hero.FinalCtaHeading;
        page.FinalCtaText = dto.Hero.FinalCtaText;

        context.Instructors.RemoveRange(context.Instructors);
        context.Instructors.AddRange(dto.Instructors.Select(i => new Instructor
        {
            FullName = i.FullName,
            Headline = i.Headline,
            Bio = i.Bio,
            PhotoUrl = string.IsNullOrWhiteSpace(i.PhotoUrl) ? "images/instructor.jpg" : i.PhotoUrl,
            YearsTeaching = i.YearsTeaching,
            YearsTrading = i.YearsTrading,
            MarketFocus = i.MarketFocus,
            ContactEmail = i.ContactEmail,
            LinkedInUrl = i.LinkedInUrl,
            GithubUrl = i.GithubUrl,
            DisplayOrder = i.DisplayOrder
        }));

        context.Testimonials.RemoveRange(context.Testimonials);
        context.Testimonials.AddRange(dto.Testimonials.Select(t => new Testimonial
        {
            Quote = t.Quote,
            Attribution = t.Attribution,
            DisplayOrder = t.DisplayOrder
        }));

        context.FaqItems.RemoveRange(context.FaqItems);
        context.FaqItems.AddRange(dto.Faqs.Select(f => new FaqItem
        {
            Question = f.Question,
            Answer = f.Answer,
            DisplayOrder = f.DisplayOrder
        }));

        context.CurriculumModules.RemoveRange(context.CurriculumModules);
        context.CurriculumModules.AddRange(dto.Curriculum.Select(c => new CurriculumModule
        {
            Title = c.Title,
            Description = c.Description,
            DisplayOrder = c.DisplayOrder
        }));

        await context.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    private static async Task EnsureSeededAsync(ApplicationDbContext context)
    {
        if (await context.LandingPageContents.AnyAsync())
        {
            return;
        }

        context.LandingPageContents.Add(new LandingPageContent
        {
            HeroEyebrow = "Stock-market education for beginners",
            HeroTitle = "Build the judgment to navigate the stock market with",
            HeroAccent = "clarity.",
            HeroSubheadline = "Northbound Sessions is a structured learning experience for people who want to understand stocks, analyze opportunities, manage risk, and make independent decisions without relying on blind signals or get-rich-quick promises.",
            HeroImageUrl = "images/hero.jpg",
            CurriculumHeading = "Four modules, built to compound on each other",
            CurriculumIntro = "Each module builds on the last, so you can connect market concepts instead of studying them in isolation.",
            FaqHeading = "Before you enroll",
            FinalCtaHeading = "Ready to start?",
            FinalCtaText = "Applications are reviewed on a rolling basis for the current cohort."
        });

        context.Instructors.Add(new Instructor
        {
            FullName = "Fehintolu Samuel",
            Headline = "Stock-market educator and software engineer",
            Bio = string.Join("\n\n",
                "Fehintolu Samuel is a Stock Market Educator and Software Engineer with several years of experience teaching software development, AI and machine learning, and stock-market education.",
                "His background includes Metallurgical and Materials Engineering, computational neuroscience research through the ThinkNeuro program at the University of California, Berkeley, and work at Cyart. He applies AI to biomedical problems and contributes to disease detection solutions.",
                "Northbound Sessions brings that practical, structured approach to stocks and equities. The goal is to help students understand what they are seeing, manage risk responsibly, and build their own decision-making process."),
            PhotoUrl = "images/instructor.jpg",
            YearsTeaching = "Several",
            YearsTrading = "5 yrs",
            MarketFocus = "Stocks",
            DisplayOrder = 0
        });

        var faqs = new[]
        {
            ("Do I need prior trading experience?", "No. The curriculum begins with market fundamentals and develops from there. No stock-market background is assumed."),
            ("When are the live sessions?", "Live sessions take place on Thursday, Saturday, and Sunday. Each session lasts two hours."),
            ("How much time should I set aside?", "Plan for about two hours of study on each weekday, in addition to the live sessions."),
            ("How do I get enrollment and pricing information?", "Apply through the main application. Enrollment and pricing details are provided there."),
            ("What will I learn?", "You will study market foundations, stock analysis, risk management, and practical decision-making for equities.")
        };
        for (var i = 0; i < faqs.Length; i++)
        {
            context.FaqItems.Add(new FaqItem { Question = faqs[i].Item1, Answer = faqs[i].Item2, DisplayOrder = i });
        }

        var modules = new[]
        {
            ("Foundations", "Market structure, order types, and the vocabulary everything else depends on."),
            ("Technical analysis", "Reading charts, identifying trends, and recognizing patterns with actual context."),
            ("Risk management", "Position sizing, stop-losses, and the discipline that separates a plan from a guess."),
            ("Applied practice", "Paper-trading drills and live-session walkthroughs using real, current market examples.")
        };
        for (var i = 0; i < modules.Length; i++)
        {
            context.CurriculumModules.Add(new CurriculumModule { Title = modules[i].Item1, Description = modules[i].Item2, DisplayOrder = i });
        }

        await context.SaveChangesAsync();
    }
}
