namespace Planilla_Backend.CleanArchitecture.Application.UseCases
{
  public interface IPayPayrollCommand
  {
    Task Execute(int payrollId);
  }
}
