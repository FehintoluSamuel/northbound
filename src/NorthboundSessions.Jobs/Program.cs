using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.DependencyInjection; 
using Microsoft.Extensions.Hosting; 
using NorthboundSessions.Web.Data; 
using NorthboundSessions.Web.Services; 
using Microsoft.Extensions.Configuration;

var builder = Host.CreateApplicationBuilder(args); 
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string not found."); 

builder.Services.AddDbContextFactory<ApplicationDbContext>(options => options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure())); 
builder.Services.AddScoped<LessonGeneratorService>(); 

var host = builder.Build(); 
using var scope = host.Services.CreateScope(); 
var generator = scope.ServiceProvider.GetRequiredService<LessonGeneratorService>(); 
var lesson = await generator.GenerateNextLessonAsync(); 
if (lesson is null) 
    { 
    Console.WriteLine("No unused topics remain in the bank — nothing generated today."); 
    } 
else 
    { 
    Console.WriteLine($"Generated lesson: {lesson.Title} (Id: {lesson.Id})"); 
    }