namespace Planilla_Backend.CleanArchitecture.Domain.Entities
{
  public class PayrollSummary
  {
    public int CompanyPayrollId { get; set; }
    public DateOnly PayDate { get; set; }
    public decimal TotalGrossSalaries { get; set; }
    public decimal TotalEmployerDeductions { get; set; }
    public decimal TotalEmployeeDeductions { get; set; }
    public decimal TotalBenefits { get; set; }
    public decimal TotalNetEmployee { get; set; }
    public decimal TotalEmployerCost { get; set; }
  }
}
