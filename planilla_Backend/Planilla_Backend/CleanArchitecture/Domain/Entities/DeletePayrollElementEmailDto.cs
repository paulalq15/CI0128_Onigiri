namespace Planilla_Backend.CleanArchitecture.Domain.Entities
{
  public class DeletePayrollElementEmailDto
  {
    public string EmployeeName { get; set; }
    public string EmployeeEmail { get; set; }
    public string Benefit { get; set; }
    public DateTime EffectiveDate { get; set; }
  }

  public class DeletePayrollElementEmailListDto
  {
    public List<DeletePayrollElementEmailDto> Entries { get; set; } = new();
  }
}
