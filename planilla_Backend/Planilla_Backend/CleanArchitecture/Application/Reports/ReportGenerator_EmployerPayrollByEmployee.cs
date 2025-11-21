using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Application.Reports
{
  public class ReportGenerator_EmployerPayrollByEmployee : IReportGenerator
  {
    public string ReportCode
    {
      get { return ReportCodes.EmployerEmployeePayroll; }
    }
    public async Task<ReportResultDto> GenerateAsync(ReportRequestDto request, IReportRepository repository, CancellationToken ct = default)
    {
      if (request is null) throw new ArgumentNullException(nameof(request));
      if (repository is null) throw new ArgumentNullException(nameof(repository));
      if (!request.CompanyId.HasValue || request.CompanyId.Value <= 0) throw new ArgumentException("El parámetro CompanyId es requerido y debe ser mayor que cero", nameof(request));
      if (!request.DateFrom.HasValue) throw new ArgumentException("El parámetro de fecha de inicio es requerido", nameof(request));
      if (!request.DateTo.HasValue) throw new ArgumentException("El parámetro de fecha fin es requerido", nameof(request));
      if (request.EmployeeNationalId is not null && request.EmployeeNationalId.Trim().Length != 11) throw new ArgumentException("El parámetro Cedula debe ser de 11 caracteres", nameof(request));
      if (request.EmployeeType is not null && !Enum.IsDefined(typeof(EmployeeType), request.EmployeeType.Value)) throw new ArgumentException("El parámetro TipoEmpleado tiene un valor inválido", nameof(request));

      int companyId = request.CompanyId.Value;
      var dateFrom = request.DateFrom.Value;
      var dateTo = request.DateTo.Value;
      string? employeeNatId = request.EmployeeNationalId is not null ? request.EmployeeNationalId.Trim(): null;
      string? employeeType = request.EmployeeType switch
      { 
        EmployeeType.FullTime => "Tiempo Completo",
        EmployeeType.PartTime => "Medio Tiempo",
        EmployeeType.ProfessionalServices => "Servicios Profesionales",
        null => null,
        _ => null
      };

    var items = await repository.GetEmployerPayrollByPersonAsync(companyId, dateFrom, dateTo, employeeType, employeeNatId, ct);

      var rows = new List<Dictionary<string, object?>>();
      decimal totalGross = 0;
      decimal totalEmployerContrib = 0;
      decimal totalBenefits = 0;
      decimal totalCost = 0;

      foreach (var item in items)
      {
        rows.Add(new Dictionary<string, object?>
        {
          ["EmployeeName"] = item.EmployeeName,
          ["NationalId"] = item.NationalId,
          ["EmployeeType"] = item.EmployeeType,
          ["PaymentPeriod"] = item.PaymentPeriod,
          ["PaymentDate"] = item.PaymentDate,
          ["GrossSalary"] = item.GrossSalary,
          ["EmployerContributions"] = item.EmployerContributions,
          ["EmployeeBenefits"] = item.EmployeeBenefits,
          ["EmployerCost"] = item.EmployerCost
        });

        totalGross += item.GrossSalary;
        totalEmployerContrib += item.EmployerContributions;
        totalBenefits += item.EmployeeBenefits;
        totalCost += item.EmployerCost;
      }

      if (rows.Count > 0)
      {
        rows.Add(new Dictionary<string, object?>
        {
          ["EmployeeName"] = "Total",
          ["NationalId"] = string.Empty,
          ["EmployeeType"] = string.Empty,
          ["PaymentPeriod"] = string.Empty,
          ["PaymentDate"] = null,
          ["GrossSalary"] = totalGross,
          ["EmployerContributions"] = totalEmployerContrib,
          ["EmployeeBenefits"] = totalBenefits,
          ["EmployerCost"] = totalCost
        });
      }

      return new ReportResultDto
      {
        ReportCode = ReportCodes.EmployerEmployeePayroll,
        DisplayName = "Detalle de Planilla Por Empleado",
        Columns = new List<string>
        {
          "EmployeeName",
          "NationalId",
          "EmployeeType",
          "PaymentPeriod",
          "PaymentDate",
          "GrossSalary",
          "EmployerContributions",
          "EmployeeBenefits",
          "EmployerCost"
        },
        Rows = rows
      };
    }
  }
}