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
    public DbSet<TopicBankItem> TopicBankItems { get; set; } 
    public DbSet<BankQuestion> BankQuestions { get; set; } 
    public DbSet<BankOption> BankOptions { get; set; }
}
