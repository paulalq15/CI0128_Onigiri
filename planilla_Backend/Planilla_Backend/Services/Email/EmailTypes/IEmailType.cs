namespace Planilla_Backend.Services.Email.EmailTypes
{
  public interface IEmailType
  {
    public string GetSubject();
    public string GetBody(object model);
  }
}
