namespace Planilla_Backend.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";  // texto plano por ahora
        public string Status { get; set; } = "";      // 'Activo' | 'Inactivo'
        public int PersonID { get; set; }
        public Person? Person { get; set; }
        public int? CompanyUniqueId { get; set; }
    }
}
