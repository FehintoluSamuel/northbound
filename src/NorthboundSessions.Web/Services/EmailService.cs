using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace NorthboundSessions.Web.Services
{
    //Create an email service for email confirmation
    public class EmailService
    {
        //Read the stored Gmail adress/app passowrd and talk to Gmail servers
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

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
    }

}
