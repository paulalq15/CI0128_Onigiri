using Planilla_Backend.CleanArchitecture.Application.Ports;

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
      throw new NotImplementedException("ReportGenerator_EmployerPayrollHistory aún no está implementado");
    }
  }
}