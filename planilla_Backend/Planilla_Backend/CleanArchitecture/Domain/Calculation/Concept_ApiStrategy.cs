using Planilla_Backend.CleanArchitecture.Application.External.Models;
using Planilla_Backend.CleanArchitecture.Domain.Entities;
using Planilla_Backend.CleanArchitecture.Infrastructure.External;
using System.Text.Json;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public readonly record struct APIAmounts(decimal EE, decimal ER);
  public class Concept_ApiStrategy : IConceptStrategy
  {
    private readonly ExternalPartnersService _partners;
    public Concept_ApiStrategy(ExternalPartnersService partners)
    {
      _partners = partners;
    }
    public IEnumerable<PayrollDetailModel> Apply(EmployeePayrollModel employeePayroll, ElementModel concept, PayrollContext ctx, EmployeeModel employee)
    {
      if (employeePayroll == null) throw new ArgumentNullException("La planilla del empleado es requerida");
      if (concept == null) throw new ArgumentNullException("El elemento de planilla es requerido");
      if (ctx == null) throw new ArgumentNullException("El contexto de planilla es requerido");
    
      ApiResponse? apiResponse = null;

      switch (concept.Value)
      {
        case 1: //Asociación Solidarista
          apiResponse = _partners
            .GetAsociacionSolidaristaAsync(ctx.Company.LegalID!, employeePayroll.Gross, CancellationToken.None)
            .GetAwaiter().GetResult();
          break;

        case 2: //Seguro Privado
          if (employee.Age is null)
            throw new InvalidOperationException("La edad del empleado es requerida para Seguro Privado.");
          if (concept.NumberOfDependents is null)
            throw new InvalidOperationException("El número de dependientes es requerido para Seguro Privado.");
          apiResponse = _partners
            .GetSeguroPrivadoAsync(employee.Age.Value, concept.NumberOfDependents.Value, CancellationToken.None)
            .GetAwaiter().GetResult();
          break;

        case 3: //Pensión voluntaria
          apiResponse = _partners
            .GetPensionesVoluntariasAsync(concept.PensionType!, employeePayroll.Gross, CancellationToken.None)
            .GetAwaiter().GetResult();
          break;

        default:
          throw new InvalidOperationException("Elemento API no reconocido");
      }

      var apiAmounts = ParseAPIResponse(apiResponse);

      var detailList = new List<PayrollDetailModel>();

      if (apiAmounts.EE > 0)
      {
        if (concept.Value == 2 && ctx.Company.PaymentFrequency == PaymentFrequency.Biweekly)
        {
          apiAmounts = apiAmounts with { EE = Math.Round(apiAmounts.EE / 2, 2) };
        }
        detailList.Add(new PayrollDetailModel
        {
          EmployeePayrollId = employeePayroll.Id,
          Description = $"{concept.Name} (Empleado)",
          Type = PayrollItemType.EmployeeDeduction,
          Amount = apiAmounts.EE,
          IdElement = concept.Id
        });
      }

      if (apiAmounts.ER > 0)
      {
        if (concept.Value == 2 && ctx.Company.PaymentFrequency == PaymentFrequency.Biweekly)
        {
          apiAmounts = apiAmounts with { ER = Math.Round(apiAmounts.ER / 2, 2) };
        }
        detailList.Add(new PayrollDetailModel
        {
          EmployeePayrollId = employeePayroll.Id,
          Description = $"{concept.Name} (Empleador)",
          Type = PayrollItemType.Benefit,
          Amount = apiAmounts.ER,
          IdElement = concept.Id
        });
      }

      return detailList;
    }
    private static APIAmounts ParseAPIResponse(ApiResponse? api)
    {
      if (api == null || api.Deductions == null)
        return new APIAmounts(0, 0);

      decimal ee = 0m, er = 0m;

      foreach (var d in api.Deductions)
      {
        if (string.Equals(d.Type, "EE", StringComparison.OrdinalIgnoreCase))
          ee = d.Amount;
        else if (string.Equals(d.Type, "ER", StringComparison.OrdinalIgnoreCase))
          er = d.Amount;
      }

      return new APIAmounts(ee, er);
    }
  }
}

