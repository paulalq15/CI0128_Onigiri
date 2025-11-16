using Planilla_Backend.CleanArchitecture.Application.Ports;

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
      throw new NotImplementedException("ReportGenerator_EmployeePayrollHistory aún no está implementado");
    }
  }
}