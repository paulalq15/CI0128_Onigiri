using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public class LegalConcept_TaxStrategy : ILegalConceptStrategy
  {
    public IEnumerable<PayrollDetailModel> Apply(EmployeePayrollModel employeePayroll, PayrollContext ctx)
    {
      // TODO: implement the logic

      // Dummy data for testing
      var detailList = new List<PayrollDetailModel>();

      var line = new PayrollDetailModel
      {
        EmployeePayrollId = employeePayroll.Id,
        Description = "Impuesto sobre la renta",
        Type = PayrollItemType.EmployeeDeduction,
        Amount = employeePayroll.BaseSalaryForPeriod * 0.1m,
        IdCCSS = null,
        IdTax = 2,
        IdElement = null,
      };
      detailList.Add(line);

      return detailList;
    }
  }
}
