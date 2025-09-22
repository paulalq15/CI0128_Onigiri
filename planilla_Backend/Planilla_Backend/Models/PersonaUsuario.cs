// Tabla Persona & Usuario
namespace Planilla_Backend.Models
{
  public class PersonaUsuario
  {
    public int IdUsuario { get; set; }
    public string Correo { get; set; }
    public string Estado { get; set; }
    public int IdPersona { get; set; }
    public string Cedula { get; set; }
    public string Nombre1 { get; set; }
    public string Nombre2 { get; set; }
    public string Apellido1 { get; set; }
    public string Apellido2 { get; set; }
    public string Telefono { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public string TipoPersona { get; set; }
  }
}
