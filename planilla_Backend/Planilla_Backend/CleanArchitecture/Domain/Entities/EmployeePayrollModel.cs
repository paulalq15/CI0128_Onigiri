namespace Planilla_Backend.CleanArchitecture.Domain.Entities
{
  public class EmployeePayrollModel
  {
    public int Id { get; set; }
    public int CompanyPayrollId { get; set; }
    public int EmployeeId { get; set; }
    public decimal Gross { get; set; }
    public decimal EmployeeDeductions { get; set; }
    public decimal EmployerDeductions { get; set; }
    public decimal Benefits { get; set; }
    public decimal Net { get; set; }
    public decimal Cost { get; set; }
    public decimal BaseSalaryForPeriod { get; set; }
  }
}
