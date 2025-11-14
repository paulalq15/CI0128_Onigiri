namespace Planilla_Backend.CleanArchitecture.Application.Reports
{
  public interface IGenerateReportDataQuery
  {
      Task<ReportResultDto> ExecuteAsync(ReportRequestDto request, CancellationToken ct = default);
  }
}
