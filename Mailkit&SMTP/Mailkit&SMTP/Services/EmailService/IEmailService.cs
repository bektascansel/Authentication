using Mailkit_SMTP.Models;

namespace Mailkit_SMTP.Services.EmailService
{
    public interface IEmailService
    {
        void SendEmail(EmailDto request);
    }
}
