namespace Planilla_Backend.CleanArchitecture.Application.Reports
{
  public class ReportFactory : IReportFactory
  {
    private readonly IDictionary<string, IReportGenerator> _generators;
    public ReportFactory(IEnumerable<IReportGenerator> generators)
    {
      if (generators is null) throw new ArgumentNullException(nameof(generators));
      _generators = generators.ToDictionary(g => g.ReportCode, g => g, StringComparer.OrdinalIgnoreCase);
    }

    public IReportGenerator GetGenerator(string reportCode)
    {
      if (string.IsNullOrWhiteSpace(reportCode)) throw new ArgumentException("El código de reporte es requerido", nameof(reportCode));
      if (!_generators.TryGetValue(reportCode, out var generator)) throw new InvalidOperationException($"No existe un generador configurado para el reporte '{reportCode}'");
      return generator;
    }
  }
}
