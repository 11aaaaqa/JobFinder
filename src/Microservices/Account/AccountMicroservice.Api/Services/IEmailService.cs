using MimeKit;

namespace AccountMicroservice.Api.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(MailboxAddress to, string title, string content);
    }
}