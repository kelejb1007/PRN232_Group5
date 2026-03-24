using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using BLL.Services.Interfaces;

namespace BLL.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string content)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var portString = _configuration["EmailSettings:Port"];
            var senderName = _configuration["EmailSettings:SenderName"];
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var password = _configuration["EmailSettings:Password"];

            if (string.IsNullOrWhiteSpace(senderEmail) || senderEmail == "YOUR_GMAIL_HERE@gmail.com")
            {
                // Fallback to console print if user hasn't configured it yet
                Console.WriteLine("\n[WARNING] Real Email sending skipped because appsettings.json uses dummy credentials.");
                Console.WriteLine($"MOCK EMAIL SENT TO: {toEmail}\nOTP: {content}\n");
                return;
            }

            int port = int.TryParse(portString, out int p) ? p : 587;

            using var smtpClient = new SmtpClient(smtpServer)
            {
                Port = port,
                Credentials = new NetworkCredential(senderEmail, password),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail!, senderName),
                Subject = subject,
                Body = content,
                IsBodyHtml = false, 
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
