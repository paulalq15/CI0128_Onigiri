using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public class LegalConcept_CCSSStrategy : ILegalConceptStrategy
  {
    public IEnumerable<PayrollDetailModel> Apply(EmployeePayrollModel employeePayroll, PayrollContext ctx)
    {
      // TODO: implement the logic

      // Dummy data for testing
      var detailList = new List<PayrollDetailModel>();

      var line = new PayrollDetailModel
      {
        EmployeePayrollId = employeePayroll.Id,
        Description = "CCSS - Concepto XX",
        Type = PayrollItemType.EmployeeDeduction,
        Amount = employeePayroll.BaseSalaryForPeriod * 0.05m,
        IdCCSS = 2,
        IdTax = null,
        IdElement = null,
      };
      detailList.Add(line);

      return detailList;
    }
  }
}
