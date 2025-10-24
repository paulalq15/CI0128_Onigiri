using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public class LegalConcept_TaxStrategy : ILegalConceptStrategy
  {
    public IEnumerable<PayrollDetailModel> Apply(EmployeePayrollModel employeePayroll, PayrollContext ctx)
    {
      if (employeePayroll == null) throw new ArgumentNullException("La planilla del empleado es requerida");
      if (ctx == null) throw new ArgumentNullException("El contexto de planilla es requerido");
      if (ctx.TaxBrackets == null || ctx.TaxBrackets.Count == 0) throw new InvalidOperationException("Se requieren tramos de impuesto en el contexto");
      
      var detailList = new List<PayrollDetailModel>();
      var acumulatedTax = 0m;
      var taxId = 0;

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
          taxId = bracket.Id;
          break;
        }
      }

      var line = new PayrollDetailModel
      {
        EmployeePayrollId = employeePayroll.Id,
        Description = "Impuesto sobre la renta",
        Type = PayrollItemType.EmployeeDeduction,
        Amount = Math.Round(acumulatedTax, 2),
        IdCCSS = null,
        IdTax = taxId,
        IdElement = null,
      };
      detailList.Add(line);

      return detailList;
    }
  }
}
