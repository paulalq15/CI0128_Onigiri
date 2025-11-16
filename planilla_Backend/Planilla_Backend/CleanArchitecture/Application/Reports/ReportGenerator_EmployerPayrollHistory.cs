using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Application.Reports
{
  public class ReportGenerator_EmployerPayrollHistory : IReportGenerator
  {
    public string ReportCode
    {
      get { return ReportCodes.EmployerHistoryPayroll; }
    }
    public async Task<ReportResultDto> GenerateAsync(ReportRequestDto request, IReportRepository repository, CancellationToken ct = default)
    {
      if (request is null) throw new ArgumentNullException(nameof(request));
      if (repository is null) throw new ArgumentNullException(nameof(repository));
      if (!request.EmployeeId.HasValue || request.EmployeeId.Value <= 0) throw new ArgumentException("El parámetro EmployeeId es requerido y debe ser mayor que cero", nameof(request));
      if (!request.DateFrom.HasValue) throw new ArgumentException("El parámetro de fecha de inicio es requerido", nameof(request));
      if (!request.DateTo.HasValue) throw new ArgumentException("El parámetro de fecha fin es requerido", nameof(request));

      var dateFrom = request.DateFrom.Value.Date;
      var dateTo = request.DateTo.Value.Date;
      var employeeId = request.EmployeeId.Value;
      int? companyId = request.CompanyId.HasValue && request.CompanyId.Value > 0 ? request.CompanyId.Value : null;

      var items = await repository.GetEmployerHistoryCompaniesAsync(companyId, employeeId, dateFrom, dateTo, ct);
      var rows = new List<Dictionary<string, object?>>();

      foreach (var item in items)
      {
        rows.Add(new Dictionary<string, object?>
        {
          ["CompanyName"] = item.CompanyName,
          ["PaymentFrequency"] = GetFrequencyDisplayName(item.PaymentFrequency),
          ["Period"] = new { item.DateFrom, item.DateTo },
          ["PaymentDate"] = item.PaymentDate,
          ["GrossSalary"] = item.GrossSalary,
          ["EmployerContributions"] = item.EmployerContributions,
          ["EmployeeBenefits"] = item.EmployeeBenefits,
          ["EmployerCost"] = item.EmployerCost
        });
      }

      return new ReportResultDto
      {
        ReportCode = ReportCodes.EmployerHistoryPayroll,
        DisplayName = "Histórico pago de planilla empresas",
        Columns = new List<string>
        {
          "CompanyName",
          "PaymentFrequency",
          "Period",
          "PaymentDate",
          "GrossSalary",
          "EmployerContributions",
          "EmployeeBenefits",
          "EmployerCost"
        },
        Rows = rows,
      };
    }

    private static string GetFrequencyDisplayName(PaymentFrequency frequency)
    {
      switch (frequency)
      {
        case PaymentFrequency.Monthly:
          return "Mensual";
        case PaymentFrequency.Biweekly:
          return "Quincenal";
        default:
          return frequency.ToString();
      }
    }
  }
}