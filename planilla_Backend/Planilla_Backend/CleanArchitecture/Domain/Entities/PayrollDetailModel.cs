using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Entities
{
  public class PayrollDetailModel
  {
    public int Id { get; set; }
    public int EmployeePayrollId { get; set; }
    required public string Description { get; set; }
    public PayrollItemType Type { get; set; }
    public decimal Amount { get; set; }
    public int? IdCCSS { get; set; }
    public int? IdTax { get; set; }
    public int? IdElement { get; set; }
    public int? NumberOfDependents { get; set; }
    public string? PensionType { get; set; }
  }
}