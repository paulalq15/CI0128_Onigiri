
namespace Planilla_Backend.Services.EmailService
{
  public interface IEmailService
  {
    Task SendEmail(string emailReceiver, string title, string body);
  }
}