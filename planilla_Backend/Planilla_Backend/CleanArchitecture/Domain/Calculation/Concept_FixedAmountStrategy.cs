using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public class Concept_FixedAmountStrategy : IConceptStrategy
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
        Type = PayrollItemType.EmployeeDeduction,
        Amount = 50000m,
        IdCCSS = null,
        IdTax = null,
        IdElement = 4,
      };
      detailList.Add(line);

      return detailList;
    }
  }
}
