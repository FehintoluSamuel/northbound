using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.DependencyInjection; 
using Microsoft.Extensions.Hosting; 
using NorthboundSessions.Web.Data; 
using NorthboundSessions.Web.Services; 
using Microsoft.Extensions.Configuration;

NeonEnvironment.LoadDotEnv();

var builder = Host.CreateApplicationBuilder(args); 
var connectionString = NeonEnvironment.GetNpgsql("DATABASE_URL")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContextFactory<ApplicationDbContext>(options => options.UseNpgsql(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure())); 
builder.Services.AddScoped<LessonGeneratorService>(); 
builder.Services.AddScoped<LessonBankSeeder>();
builder.Services.AddScoped<EmailService>(); 

var host = builder.Build(); 
using var scope = host.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<LessonBankSeeder>();
var seedResult = await seeder.SeedAsync();
Console.WriteLine($"Lesson bank seeded: {seedResult.Added} added, {seedResult.Updated} refreshed, {seedResult.Total} ready.");

if (args.Contains("--seed-only", StringComparer.OrdinalIgnoreCase))
{
    return;
}

var generator = scope.ServiceProvider.GetRequiredService<LessonGeneratorService>(); 
var lesson = await generator.GenerateNextLessonAsync(); 
if (lesson is null) 
    { 
        Console.WriteLine("No unused topics remain in the bank — nothing generated today."); 
    } 
else {
        Console.WriteLine($"Generated lesson: {lesson.Title} (Id: {lesson.Id})");
        var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();
        await emailService.NotifyAllStudentsAsync
            ( subject: $"New lesson available: {lesson.Title}", body: $"A new lesson is ready: {lesson.Title}. Log in to view it." 
            );
      }