using Planilla_Backend.CleanArchitecture.Application.Ports;

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
      throw new NotImplementedException("ReportGenerator_EmployerPayrollByEmployee aún no está implementado");
    }
  }
}