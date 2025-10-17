using Planilla_Backend.LayeredArchitecture.Models;

namespace Planilla_Backend.LayeredArchitecture.Models
{
  public class RegisterEmpleador
  {
    public PersonUser personData { get; set; }
    public string password { get; set; }
    public string otherSigns { get; set; }
    public string zipCode { get; set; }

  }
}
