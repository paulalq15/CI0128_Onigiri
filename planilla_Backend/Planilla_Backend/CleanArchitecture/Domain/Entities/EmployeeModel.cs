namespace Planilla_Backend.CleanArchitecture.Domain.Entities
{
  public class EmployeeModel
  {
    public int Id { get; set; }
    required public string PersonType { get; set; }
  }
}
