namespace Planilla_Backend.CleanArchitecture.Application.Reports
{
  public interface IReportFactory
  {
    IReportGenerator GetGenerator(string reportCode);
  }
}
