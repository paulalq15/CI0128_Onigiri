
namespace Planilla_Backend.LayeredArchitecture.Services.EmailService
{
  public interface IEmailService
  {
    Task SendEmail(string emailReceiver, string title, string body);
  }
}