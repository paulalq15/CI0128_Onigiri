using Planilla_Backend.CleanArchitecture.Application.Ports;

namespace Planilla_Backend.CleanArchitecture.Application.Reports
{
  public class ReportGenerator_EmployerPayrollDetail : IReportGenerator
  {
    public string ReportCode
    {
      get { return ReportCodes.EmployerDetailPayroll; }
    }
    public async Task<ReportResultDto> GenerateAsync(ReportRequestDto request, IReportRepository repository, CancellationToken ct = default)
    {
      throw new NotImplementedException("ReportGenerator_EmployerPayrollDetail aún no está implementado");
    }
  }
}