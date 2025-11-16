namespace Planilla_Backend.CleanArchitecture.Domain.Reports
{
  public class EmployerPayrollReport
  {
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = null!;
    public string EmployerName { get; set; } = null!;
    public DateTime PaymentDate { get; set; }

    public IReadOnlyList<PayrollDetailLine> Lines { get; set; } = Array.Empty<PayrollDetailLine>();
  }
}
