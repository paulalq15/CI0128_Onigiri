namespace Planilla_Backend.Models
{
    public class Person
    {
        public int PersonID { get; set; }
        public string Cedula { get; set; } = "";
        public string Name1 { get; set; } = "";
        public string? Name2 { get; set; }
        public string Surname1 { get; set; } = "";
        public string? Surname2 { get; set; }
        public string? Phone { get; set; }
        public DateTime BirthDate { get; set; }
        public string PersonType { get; set; } = "";
    }
}
