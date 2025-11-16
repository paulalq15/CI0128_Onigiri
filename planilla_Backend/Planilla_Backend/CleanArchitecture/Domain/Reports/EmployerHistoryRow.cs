using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Reports
{
  public class EmployerHistoryRow
  {
    public string CompanyName { get; set; } = null!;
    public PaymentFrequency PaymentFrequency { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal EmployerContributions { get; set; }
    public decimal EmployeeBenefits { get; set; }
    public decimal EmployerCost { get; set; }
  }
}
