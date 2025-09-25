// Tabla Persona & Usuario
namespace Planilla_Backend.Models
{
  public class PersonUser
  {
    public int IdUser { get; set; }
    public string Email { get; set; }
    public string Status { get; set; }
    public int IdPerson { get; set; }
    public string IdCard { get; set; }
    public string Name1 { get; set; }
    public string Name2 { get; set; }
    public string Surname1 { get; set; }
    public string Surname2 { get; set; }
    public string Number { get; set; }
    public DateTime BirthdayDate { get; set; }
    public string TypePerson { get; set; }
  }
}
