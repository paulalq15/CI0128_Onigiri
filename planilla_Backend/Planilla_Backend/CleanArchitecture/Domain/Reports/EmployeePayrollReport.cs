using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Reports
{
  public class EmployeePayrollReport
  {
    public string CompanyName { get; set; } = null!;
    public int CompanyId { get; set; }
    public string EmployeeName { get; set; } = null!;
    public int EmployeeId { get; set; }
    public EmployeeType EmployeeType { get; set; }
    public DateTime PaymentDate { get; set; }

    public IReadOnlyList<PayrollDetailLine> Lines { get; set; } = Array.Empty<PayrollDetailLine>();
  }
}
