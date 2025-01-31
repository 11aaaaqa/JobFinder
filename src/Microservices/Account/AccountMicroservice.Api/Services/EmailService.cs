using MailKit.Net.Smtp;
using MimeKit;

namespace AccountMicroservice.Api.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task SendEmailAsync(MailboxAddress to, string title, string content)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(configuration["Email:CompanyName"], configuration["Email:From"]));
            emailMessage.To.Add(to);
            emailMessage.Subject = title;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = content
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(configuration["Email:Smtp"], int.Parse(configuration["Email:Port"]), true);
            await client.AuthenticateAsync(configuration["Email:UserName"], configuration["Email:Password"]);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
    }
}