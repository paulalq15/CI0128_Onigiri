using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public class Concept_ApiStrategy : IConceptStrategy
  {
    public IEnumerable<PayrollDetailModel> Apply(EmployeePayrollModel employeePayroll, ElementModel concept)
    {
      // TODO: implement the logic

      // Dummy data for testing
      var detailList = new List<PayrollDetailModel>();

      var line = new PayrollDetailModel
      {
        EmployeePayrollId = employeePayroll.Id,
        Description = "Elemento de planilla API",
        Type = PayrollItemType.EmployeeDeduction,
        Amount = 25000m,
        IdCCSS = null,
        IdTax = null,
        IdElement = 7,
      };
      detailList.Add(line);

      line = new PayrollDetailModel
      {
        EmployeePayrollId = employeePayroll.Id,
        Description = "Elemento de planilla API",
        Type = PayrollItemType.Benefit,
        Amount = 50000m,
        IdCCSS = null,
        IdTax = null,
        IdElement = 7,
      };
      detailList.Add(line);

      return detailList;
    }
  }
}

