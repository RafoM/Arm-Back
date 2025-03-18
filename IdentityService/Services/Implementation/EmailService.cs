using IdentityService.Services.Interface;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using IdentityService.Models.ConfigModels;

namespace IdentityService.Services.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettingsConfigModel _smtpSettings;

        public EmailService(IOptions<SmtpSettingsConfigModel> smtpOptions)
        {
            _smtpSettings = smtpOptions.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlContent)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.Username, _smtpSettings.SenderName),
                Subject = subject,
                Body = htmlContent,
                IsBodyHtml = true
            };
            mailMessage.To.Add(to);

            using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                smtpClient.EnableSsl = true; 
                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}
