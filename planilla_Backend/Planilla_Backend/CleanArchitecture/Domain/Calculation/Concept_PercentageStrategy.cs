using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public class Concept_PercentageStrategy : IConceptStrategy
  {
    public IEnumerable<PayrollDetailModel> Apply(EmployeePayrollModel employeePayroll, ElementModel concept, PayrollContext ctx)
    {
      // TODO: implement the logic

      // Dummy data for testing
      var detailList = new List<PayrollDetailModel>();

      var line = new PayrollDetailModel
      {
        EmployeePayrollId = employeePayroll.Id,
        Description = "Elemento de planilla",
        Type = PayrollItemType.Benefit,
        Amount = employeePayroll.Gross * 0.025m,
        IdCCSS = null,
        IdTax = null,
        IdElement = 2,
      };
      detailList.Add(line);

      return detailList;
    }
  }
}
