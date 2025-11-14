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

    public Task<ReportResultDto> ExecuteAsync(ReportRequestDto request, CancellationToken ct = default)
    {
      if (request is null) throw new ArgumentNullException(nameof(request));
      if (string.IsNullOrWhiteSpace(request.ReportCode)) throw new ArgumentException("El código de reporte es requerido", nameof(request));

      var generator = _reportFactory.GetGenerator(request.ReportCode);

      if (generator is null)throw new InvalidOperationException($"No existe un generador configurado para el reporte '{request.ReportCode}'");

      return generator.GenerateAsync(request, _reportRepository, ct);
    }
  }
}
