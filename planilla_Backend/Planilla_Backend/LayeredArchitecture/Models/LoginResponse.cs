namespace Planilla_Backend.LayeredArchitecture.Models
{
  public class LoginResponse
  {
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public int? UserId { get; set; }
    public int? PersonID { get; set; }
    public string? FullName { get; set; }
    public string? PersonType { get; set; }
    public string? Email { get; set; }
    public int? CompanyUniqueId { get; set; }
  }
}
