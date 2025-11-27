using Azure.Core;
using Planilla_Backend.CleanArchitecture.Application.Ports;

namespace Planilla_Backend.CleanArchitecture.Application.Reports
{
  public class GenerateReportDataQuery : IGenerateReportDataQuery
  {
    private readonly IReportFactory _reportFactory;
    private readonly IReportRepository _reportRepository;

    public GenerateReportDataQuery(IReportFactory reportFactory, IReportRepository reportRepository)
    {
      _reportFactory = reportFactory;
      _reportRepository = reportRepository;
    }

    public Task<ReportResultDto> GenerateReportAsync(ReportRequestDto request, CancellationToken ct = default)
    {
      if (request is null) throw new ArgumentNullException(nameof(request));
      if (string.IsNullOrWhiteSpace(request.ReportCode)) throw new ArgumentException("El código de reporte es requerido", nameof(request));

      var generator = _reportFactory.GetGenerator(request.ReportCode);

      if (generator is null) throw new InvalidOperationException($"No existe un generador configurado para el reporte '{request.ReportCode}'");

      return generator.GenerateAsync(request, _reportRepository, ct);
    }

    public Task<IEnumerable<ReportPayrollPeriodDto>> GetEmployeePayrollPeriodsAsync(int companyId, int employeeId, int top)
    {
      if (companyId <= 0) throw new ArgumentException("El parámetro CompanyId es requerido y debe ser mayor que cero");
      if (employeeId <= 0) throw new ArgumentException("El parámetro EmployeeId es requerido y debe ser mayor que cero");
      if (top <= 0) throw new ArgumentException("El parámetro top es requerido y debe ser mayor que cero");
      return _reportRepository.GetEmployeePayrollPeriodsAsync(companyId, employeeId, top);
    }

    public Task<IEnumerable<ReportPayrollPeriodDto>> GetEmployerPayrollPeriodsAsync(int companyId, int top)
    {
      if (companyId <= 0) throw new ArgumentException("El parámetro CompanyId es requerido y debe ser mayor que cero");
      if (top <= 0) throw new ArgumentException("El parámetro top es requerido y debe ser mayor que cero");
      return _reportRepository.GetEmployerPayrollPeriodsAsync(companyId, top);
    }
  }
}
