using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;
using Planilla_Backend.CleanArchitecture.Domain.Reports;

namespace Planilla_Backend.CleanArchitecture.Application.Reports
{
  public class ReportGenerator_EmployeePayrollHistory : IReportGenerator
  {
    public string ReportCode
    {
      get { return ReportCodes.EmployeeHistoryPayroll; }
    }
    public async Task<ReportResultDto> GenerateAsync(ReportRequestDto request, IReportRepository repository, CancellationToken ct = default)
    {
      if (request is null) throw new ArgumentNullException(nameof(request));
      if (repository is null) throw new ArgumentNullException(nameof(repository));
      if (!request.CompanyId.HasValue || request.CompanyId.Value <= 0) throw new ArgumentException("El parámetro CompanyId es requerido y debe ser mayor que cero", nameof(request));
      if (!request.EmployeeId.HasValue || request.EmployeeId.Value <= 0) throw new ArgumentException("El parámetro EmployeeId es requerido y debe ser mayor que cero", nameof(request));
      if (!request.DateFrom.HasValue) throw new ArgumentException("El parámetro de fecha inicial es requerido", nameof(request));
      if (!request.DateTo.HasValue) throw new ArgumentException("El parámetro de fecha final es requerido", nameof(request));

      EmployeePayrollHistoryReport report = await repository.GetEmployeePayrollHistoryInDateRange(request.EmployeeId.Value, request.DateFrom.Value, request.DateTo.Value);
      if (report.Rows == null || !report.Rows.Any()) throw new KeyNotFoundException("No se encontró información de pago para el empleado en el rango seleccionado");
      if (report.CompanyId != request.CompanyId.Value) throw new KeyNotFoundException("La empresa solicitante no coincide con la empresa registrada en la planilla");

      var rows = BuildRows(report);

      var result = new ReportResultDto
      {
        ReportCode = ReportCodes.EmployeeHistoryPayroll,
        DisplayName = "Histórico pago de planilla",
        Columns = new List<string> {
          "Tipo de contrato",
          "Puesto",
          "Fecha de pago",
          "Salario bruto",
          "Deducciones obligatorias",
          "Deducciones voluntarias",
          "Salario neto"
        },
        Rows = rows,
        ReportInfo = new Dictionary<string, object?>
        {
          ["EmployeeName"] = report.EmployeeName,
          ["CompanyName"] = report.CompanyName,
          ["DateFrom"] = request.DateFrom.Value,
          ["DateTo"] = request.DateTo.Value
        }
      };

      return result;
    }

    private List<Dictionary<string, object?>> BuildRows(EmployeePayrollHistoryReport report)
    {
      var rows = new List<Dictionary<string, object?>>();

      decimal totalGrossSalary = 0;
      decimal totalLegalDeductions = 0;
      decimal totalVoluntaryDeductions = 0;
      decimal totalNetSalary = 0;

      foreach (var row in report.Rows)
      {
        totalGrossSalary += row.GrossSalary;
        totalLegalDeductions += row.LegalDeductions;
        totalVoluntaryDeductions += row.VoluntaryDeductions;
        totalNetSalary += row.NetSalary;
        rows.Add(new Dictionary<string, object?>
        {
          ["paymentDate"] = row.PaymentDate,
          ["contractType"] = GetContractTypeDisplayName(row.ContractType),
          ["position"] = row.Role,
          ["grossSalary"] = row.GrossSalary,
          ["mandatoryDeductions"] = row.LegalDeductions,
          ["voluntaryDeductions"] = row.VoluntaryDeductions,
          ["netSalary"] = row.NetSalary
        });
      }

      rows.Add(new Dictionary<string, object?>
      {
        ["totalGrossSalary"] = totalGrossSalary,
        ["totalLegalDeductions"] = totalLegalDeductions,
        ["totalVoluntaryDeductions"] = totalVoluntaryDeductions,
        ["totalNetSalary"] = totalNetSalary,
      });

      return rows;
    }
    private static string GetContractTypeDisplayName(EmployeeType type)
    {
      switch (type)
      {
        case EmployeeType.FullTime:
          return "Tiempo Completo";
        case EmployeeType.PartTime:
          return "Medio Tiempo";
        case EmployeeType.ProfessionalServices:
          return "Servicios Profesionales";
        default:
          return type.ToString();
      }
    }
  }
}