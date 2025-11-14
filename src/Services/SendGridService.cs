using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace order_service.src.Services
{
    public class SendGridService
    {
        private readonly string _apiKey;

        public SendGridService()
        {
            _apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY") ?? throw new ArgumentNullException("SendGrid API Key is not configured.");
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("aromadecantsantofa@gmail.com", "Order Service");
            var to = new EmailAddress(toEmail);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, htmlContent, htmlContent);

            var response = await client.SendEmailAsync(msg);
        }
        
    }
}