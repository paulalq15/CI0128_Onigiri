using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Reports
{
  public class EmployeeHistoryRow
  {
    public EmployeeType ContractType { get; set; }
    public string Role { get; set; } = null!;
    public DateTime PaymentDate { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal LegalDeductions { get; set; }
    public decimal VoluntaryDeductions { get; set; }
    public decimal NetSalary { get; set; }
  }
}
