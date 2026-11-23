using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NorthboundSessions.Data;

namespace NorthboundSessions.Web.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Quiz> Quizzes {get; set;}
    public DbSet<QuizQuestion> QuizQuestions {get; set;}
    public DbSet<QuizOption> QuizOptions {get; set;}
    public DbSet<QuizAttempt> QuizAttempts {get; set;}
    public DbSet<LiveSession> LiveSessions {get; set;}
    public DbSet<Attendance> Attendances {get; set;}
    public DbSet<Lesson> Lessons {get; set;}
    public DbSet<LessonProgress> LessonProgresses { get; set; }
    public DbSet<TopicBankItem> TopicBankItems { get; set; } 
    public DbSet<BankQuestion> BankQuestions { get; set; } 
    public DbSet<BankOption> BankOptions { get; set; }
    public DbSet<LandingPageContent> LandingPageContents { get; set; }
    public DbSet<Instructor> Instructors { get; set; }
    public DbSet<Testimonial> Testimonials { get; set; }
    public DbSet<FaqItem> FaqItems { get; set; }
    public DbSet<CurriculumModule> CurriculumModules { get; set; }
    public DbSet<RealtimeProbe> RealtimeProbes { get; set; }
    
}
