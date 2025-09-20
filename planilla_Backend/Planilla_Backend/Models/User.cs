namespace Planilla_Backend.Models
{
    public class User
    {
        public int IdUsuario { get; set; }
        public string Correo { get; set; } = "";
        public string Contrasena { get; set; } = "";  // texto plano por ahora
        public string Estado { get; set; } = "";      // 'Activo' | 'Inactivo'
        public int IdPersona { get; set; }
        public Person? Persona { get; set; }
    }
}
