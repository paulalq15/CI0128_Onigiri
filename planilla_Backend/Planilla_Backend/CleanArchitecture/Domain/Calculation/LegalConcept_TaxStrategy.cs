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
      var monthlyBase = employeePayroll.BaseSalaryForPeriod;
      var acumulatedTax = 0m;
      var taxId = 0;
      var coverage = 0m;

      foreach (var bracket in ctx.TaxBrackets)
      {
        if (bracket.Max != null && monthlyBase > bracket.Max.Value)
        {
          acumulatedTax += (bracket.Max.Value - bracket.Min) * bracket.Rate / 100;
          continue;
        }
        else if (bracket.Max == null || monthlyBase <= bracket.Max.Value)
        {
          acumulatedTax += (monthlyBase - bracket.Min) * bracket.Rate / 100;
          taxId = bracket.Id;
          break;
        }
      }

      if (monthlyBase > 0)
      {
        acumulatedTax = Math.Round(acumulatedTax, 2);

        coverage = Math.Round(employeePayroll.Gross / monthlyBase, 2);
        acumulatedTax = Math.Round(acumulatedTax * coverage, 2);
      }

      var line = new PayrollDetailModel
      {
        EmployeePayrollId = employeePayroll.Id,
        Description = "Impuesto sobre la renta",
        Type = PayrollItemType.EmployeeDeduction,
        Amount = acumulatedTax,
        IdCCSS = null,
        IdTax = taxId,
        IdElement = null,
      };
      detailList.Add(line);

      return detailList;
    }
  }
}
