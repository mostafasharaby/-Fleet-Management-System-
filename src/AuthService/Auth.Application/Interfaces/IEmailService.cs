using Auth.Application.Response;

namespace Auth.Application.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}
