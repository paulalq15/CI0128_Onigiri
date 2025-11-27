namespace Planilla_Backend.CleanArchitecture.Domain.Reports
{
  public class EmployerPayrollReport
  {
    public string CompanyName { get; set; } = null!;
    public string EmployerName { get; set; } = null!;
    public DateTime PaymentDate { get; set; }
    public decimal Cost { get; set; }

    public IReadOnlyList<PayrollDetailLine> Lines { get; set; } = Array.Empty<PayrollDetailLine>();
  }
}
