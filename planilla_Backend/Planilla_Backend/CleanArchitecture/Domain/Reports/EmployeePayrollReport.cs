using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Reports
{
  public class EmployeePayrollReport
  {
    public string CompanyName { get; set; } = null!;
    public string EmployeeName { get; set; } = null!;
    public EmployeeType EmployeeType { get; set; }
    public DateTime PaymentDate { get; set; }

    public IReadOnlyList<PayrollDetailLine> Lines { get; set; } = Array.Empty<PayrollDetailLine>();
  }
}
