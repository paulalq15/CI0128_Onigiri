namespace Planilla_Backend.CleanArchitecture.Application.Reports
{
  public interface IGenerateReportDataQuery
  {
      Task<ReportResultDto> GenerateReportAsync(ReportRequestDto request, CancellationToken ct = default);
      Task<IEnumerable<ReportPayrollPeriodDto>> GetEmployeePayrollPeriodsAsync(int companyId, int employeeId, int top);
      Task<IEnumerable<ReportPayrollPeriodDto>> GetEmployerPayrollPeriodsAsync(int companyId, int top);
    }
}
