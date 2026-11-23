using System.Linq;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NorthboundSessions.Web.Data;

namespace NorthboundSessions.Web.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public EmailService(IConfiguration configuration, IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _configuration = configuration;
            _dbFactory = dbFactory;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var fromAddress = _configuration["Email:Address"];
            var appPassword = _configuration["Email:AppPassword"];

            if (string.IsNullOrWhiteSpace(fromAddress) || string.IsNullOrWhiteSpace(appPassword))
            {
                throw new InvalidOperationException(
                    "Email is not configured. Set Email:Address and Email:AppPassword (e.g. Email__Address and Email__AppPassword environment variables).");
            }

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(fromAddress));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(fromAddress, appPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task NotifyAllStudentsAsync(string subject, string body) 
        { 
            await using var context = await _dbFactory.CreateDbContextAsync(); 

            var instructorRoleId = await context.Roles 
            .Where(r => r.Name == "Instructor") 
            .Select(r => r.Id) 
            .FirstOrDefaultAsync(); 

            var instructorUserIds = await context.UserRoles 
            .Where(ur => ur.RoleId == instructorRoleId) 
            .Select(ur => ur.UserId) .ToListAsync(); 

            var studentEmails = await context.Users 
            .Where(u => !instructorUserIds
            .Contains(u.Id) && u.Email != null) 
            .Select(u => u.Email!) .ToListAsync(); 

            foreach (var email in studentEmails) 
            { 
                await SendEmailAsync(email, subject, body); 
            } 
        }
    }

}
