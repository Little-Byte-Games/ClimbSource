using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Climb.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly string apiKey;

        public EmailSender(IConfiguration configuration)
        {
            apiKey = configuration.GetSection("Email")["Key"];
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var msg = new SendGridMessage();
            msg.SetFrom(new EmailAddress("developer@littlebytegames.com", "Climb"));
            msg.AddTo(email);
            msg.SetSubject($"Climb - {subject}");
            msg.AddContent(MimeType.Html, message);

            var client = new SendGridClient(apiKey);
            await client.SendEmailAsync(msg);
        }
    }
}