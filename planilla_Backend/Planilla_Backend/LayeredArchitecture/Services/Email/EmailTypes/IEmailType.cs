namespace Planilla_Backend.LayeredArchitecture.Services.Email.EmailTypes
{
  public interface IEmailType
  {
    public string GetSubject();
    public string GetBody(object model);
  }
}
