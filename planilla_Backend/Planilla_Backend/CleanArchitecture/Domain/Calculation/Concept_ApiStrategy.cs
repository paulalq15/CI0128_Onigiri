using Planilla_Backend.CleanArchitecture.Domain.Entities;
using System.Text.Json;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public readonly record struct APIAmounts(decimal EE, decimal ER);
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

      //Create URL with parameters
      //...

      //Call external API to get values
      //...

      //Simulate API response
      string jsonResponse = @"{""deductions"": [{ ""type"": ""EE"", ""amount"": 35},{ ""type"": ""ER"", ""amount"": 30}]}";
      var apiAmounts = ParseAPIResponse(jsonResponse);

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
    private static APIAmounts ParseAPIResponse(string json)
    {
      using var doc = JsonDocument.Parse(json);
      decimal ee = 0m, er = 0m;

      if (doc.RootElement.TryGetProperty("deductions", out var arr) && arr.ValueKind == JsonValueKind.Array)
      {
        foreach (var item in arr.EnumerateArray())
        {
          var t = item.GetProperty("type").GetString();
          var amount = item.TryGetProperty("amount", out var a) ? a.GetDecimal() : item.GetProperty("Amount").GetDecimal();
          if (string.Equals(t, "EE", StringComparison.OrdinalIgnoreCase)) ee = amount;
          else if (string.Equals(t, "ER", StringComparison.OrdinalIgnoreCase)) er = amount;
        }
      }
      return new APIAmounts(ee, er);
    }
  }
}

