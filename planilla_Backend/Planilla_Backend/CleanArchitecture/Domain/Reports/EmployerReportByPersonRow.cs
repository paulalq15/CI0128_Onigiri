using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Reports
{
  public class EmployerReportByPersonRow
  {
    public string EmployeeName { get; set; } = null!;
    public string NationalId { get; set; } = null!;
    public EmployeeType EmployeeType { get; set; }
    public string PaymentPeriod { get; set; } = null!;
    //public DateTime DateFrom { get; set; }
    //public DateTime DateTo { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal EmployerContributions { get; set; }
    public decimal EmployeeBenefits { get; set; }
    public decimal EmployerCost { get; set; }
  }
}
