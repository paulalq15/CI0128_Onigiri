namespace Planilla_Backend.Models
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public int? IdUsuario { get; set; }
        public int? IdPersona { get; set; }
        public string? NombreCompleto { get; set; }
        public string? TipoPersona { get; set; }
        public string? Correo { get; set; }
    }
}
