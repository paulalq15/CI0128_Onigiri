using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public class LegalConcept_TaxStrategy : ILegalConceptStrategy
  {
    public IEnumerable<PayrollDetailModel> Apply(EmployeePayrollModel employeePayroll, PayrollContext ctx)
    {
      var detailList = new List<PayrollDetailModel>();
      var acumulatedTax = 0m;

      foreach (var bracket in ctx.TaxBrackets)
      {
        if (bracket.Max != null && employeePayroll.BaseSalaryForPeriod > bracket.Max.Value)
        {
          acumulatedTax += (bracket.Max.Value - bracket.Min) * bracket.Rate;
          continue;
        }
        else if (bracket.Max == null || employeePayroll.BaseSalaryForPeriod <= bracket.Max.Value)
        {
          acumulatedTax += (employeePayroll.BaseSalaryForPeriod - bracket.Min) * bracket.Rate;
          break;
        }
      }

      var line = new PayrollDetailModel
      {
        EmployeePayrollId = employeePayroll.Id,
        Description = "Impuesto sobre la renta",
        Type = PayrollItemType.EmployeeDeduction,
        Amount = acumulatedTax,
        IdCCSS = null,
        IdTax = 1,
        IdElement = null,
      };
      detailList.Add(line);

      return detailList;
    }
  }
}
