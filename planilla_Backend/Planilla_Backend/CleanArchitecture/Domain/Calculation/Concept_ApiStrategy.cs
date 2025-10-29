using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public class Concept_ApiStrategy : IConceptStrategy
  {
    public IEnumerable<PayrollDetailModel> Apply(EmployeePayrollModel employeePayroll, ElementModel concept, PayrollContext ctx, EmployeeModel employee)
    {
      if (employeePayroll == null) throw new ArgumentNullException("La planilla del empleado es requerida");
      if (concept == null) throw new ArgumentNullException("El elemento de planilla es requerido");
      if (ctx == null) throw new ArgumentNullException("El contexto de planilla es requerido");

      string baseUrl, p1Name, p1Value, p2Name, p2Value;

      switch (concept.Name)
      {
        case "Asociación Solidarista":
          baseUrl = "";
          p1Name = "cedulaEmpresa"; 
          p1Value = ctx.Company.LegalID ?? string.Empty;
          p2Name = "salarioBruto"; 
          p2Value = employeePayroll.Gross.ToString("0.##");
          break;

        case "Seguro Privado":
          baseUrl = "";
          p1Name = "edad";
          p1Value = employee.Age.ToString() ?? string.Empty;
          p2Name = "cantidadDependientes";
          p2Value = concept.NumberOfDependents.ToString() ?? string.Empty;
          break;

        case "Pensión voluntaria":
          baseUrl = "";
          p1Name = "planType";
          p1Value = concept.PensionType ?? string.Empty;
          p2Name = "grossSalary";
          p2Value = employeePayroll.Gross.ToString("0.##");
          break;

        default:
          throw new InvalidOperationException("Elemento API no reconocido");
      }

      //Call external API to get values, and parse response
      //...

      //Dummy data for testing
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

