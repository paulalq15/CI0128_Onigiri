using Planilla_Backend.Models;

namespace Planilla_Backend.Models
{
  public class RegisterEmpleador
  {
    public PersonUser personData { get; set; }
    public string password { get; set; }
    public string otherSigns { get; set; }
    public string zipCode { get; set; }

  }
}
