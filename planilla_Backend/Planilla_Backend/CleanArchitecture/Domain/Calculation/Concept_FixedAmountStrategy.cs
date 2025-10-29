using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public class Concept_FixedAmountStrategy : IConceptStrategy
  {
    public IEnumerable<PayrollDetailModel> Apply(EmployeePayrollModel employeePayroll, ElementModel concept, PayrollContext ctx, EmployeeModel employee)
    {
      if (employeePayroll == null) throw new ArgumentNullException("La planilla del empleado es requerida");
      if (concept == null) throw new ArgumentNullException("El elemento de planilla es requerido");

      var detailList = new List<PayrollDetailModel>();

      var line = new PayrollDetailModel
      {
        EmployeePayrollId = employeePayroll.Id,
        Description = concept.Name,
        Type = concept.ItemType,
        Amount = concept.Value,
        IdCCSS = null,
        IdTax = null,
        IdElement = concept.Id,
      };
      detailList.Add(line);

      return detailList;
    }
  }
}
