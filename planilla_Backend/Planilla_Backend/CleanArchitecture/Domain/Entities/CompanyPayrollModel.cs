using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Planilla_Backend.CleanArchitecture.Domain.Entities
{
  public class CompanyPayrollModel
  {
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    required public string PayrollStatus { get; set; }
    public decimal Gross { get; set; }
    public decimal EmployeeDeductions { get; set; }
    public decimal EmployerDeductions { get; set; }
    public decimal Benefits { get; set; }
    public decimal Net { get; set; }
    public decimal Cost { get; set; }
    public int CreatedBy { get; set; }
  }
}