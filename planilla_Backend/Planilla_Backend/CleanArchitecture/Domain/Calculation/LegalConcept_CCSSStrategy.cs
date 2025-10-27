using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public class LegalConcept_CCSSStrategy : ILegalConceptStrategy
  {
    public IEnumerable<PayrollDetailModel> Apply(EmployeePayrollModel employeePayroll, PayrollContext ctx)
    {
      if (employeePayroll == null) throw new ArgumentNullException("La planilla del empleado es requerida");
      if (ctx == null) throw new ArgumentNullException("El contexto de planilla es requerido");
      if (ctx.CCSSRates == null || ctx.CCSSRates.Count == 0) throw new InvalidOperationException("Las tasas de la CCSS son requeridas");

      var detailList = new List<PayrollDetailModel>();

      foreach (CCSSModel actualCCSSRate in ctx.CCSSRates)
      {
        var line = new PayrollDetailModel
        {
          EmployeePayrollId = employeePayroll.Id,
          Description = actualCCSSRate.Category + " - " + actualCCSSRate.Concept,
          Type = actualCCSSRate.ItemType,
          Amount = Math.Round(employeePayroll.Gross * (actualCCSSRate.Rate / 100), 2),
          IdCCSS = actualCCSSRate.Id,
          IdTax = null,
          IdElement = null,
        };
        
        detailList.Add(line);
      }

      return detailList;
    }
  }
}
