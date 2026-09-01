using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NorthboundSessions.Data; 
using NorthboundSessions.Web.Data; 

namespace NorthboundSessions.Web.Services
{
    //Create an email service for email confirmation
    public class EmailService
    {
        //Read the stored Gmail adress/app passowrd and talk to Gmail servers
        private readonly IConfiguration _configuration;
        private readonly IDbContextFactory <ApplicationDbContext> _dbFactory; 

        public EmailService(IConfiguration configuration, IDbContextFactory<ApplicationDbContext> dbFactory) 
        { 
            _configuration = configuration; 
            _dbFactory = dbFactory; 
        }
        /*public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }*/

        //Create a method that sends confirmation emails
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var fromAddress = _configuration["Email:Address"]; //access the email address
            var appPassword = _configuration["Email: AppPassword"]; //access the email password
            var message = new MimeMessage(); //Create an instance

            message.From.Add(MailboxAddress.Parse(fromAddress)); //add email to object
            message.To.Add(MailboxAddress.Parse(toEmail)); //parse user email and add to pbject 
            message.Subject = subject; //add subject
            message.Body = new TextPart("plain") //add text
            {
                Text = body
            };
            using var client = new SmtpClient(); //instantiae smtp object
            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls); //establish connection
            await client.AuthenticateAsync(fromAddress, appPassword); //fill in authentication 
            await client.SendAsync(message); //send mail
            await client.DisconnectAsync(true); //disconnect
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
