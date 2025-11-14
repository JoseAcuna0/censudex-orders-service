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

        public SendGridService(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("orders@taller2.com", "Order Service");
            var to = new EmailAddress(toEmail);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, htmlContent, htmlContent);

            var response = await client.SendEmailAsync(msg);
        }
        
    }
}