using System.Net;
using System.Net.Mail;

namespace Planilla_Backend.LayeredArchitecture.Services.EmailService
{
  public class EmailService : IEmailService
  {
    private readonly IConfiguration configuration;
    private SmtpClient smtpClient;

    private string emailSender = "";
    private string emailPassword = "";
    public EmailService(IConfiguration configuration)
    {
      this.configuration = configuration;
      if (this.configuration != null)
      {
        this.emailSender = configuration.GetValue<string>("CONFIGURACIONES_EMAIL:EMAIL")!;
        this.emailPassword = configuration.GetValue<string>("CONFIGURACIONES_EMAIL:PASSWORD")!;
        var host = configuration.GetValue<string>("CONFIGURACIONES_EMAIL:HOST");
        var port = configuration.GetValue<int>("CONFIGURACIONES_EMAIL:PORT");

        this.smtpClient = new SmtpClient(host, port);
        this.smtpClient.EnableSsl = true;
        this.smtpClient.UseDefaultCredentials = false;

        this.smtpClient.Credentials = new NetworkCredential(emailSender, emailPassword);
      }
    }

    // Envía el correo como HTML
    public async Task SendEmail(string emailReceiver, string subject, string body)
    {
      var message = new MailMessage(this.emailSender, emailReceiver, subject, body);

      message.IsBodyHtml = true;

      try
      {
        await smtpClient.SendMailAsync(message);
      }
      catch (Exception ex)
      {
        Console.WriteLine("Error sending email: " + ex.Message);
      }
    }
  }
}
