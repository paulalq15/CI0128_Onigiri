using Planilla_Backend.CleanArchitecture.Application.Ports;

namespace Planilla_Backend.CleanArchitecture.Application.UseCases
{
  public class PayPayrollCommand : IPayPayrollCommand
  {
    private readonly IPayrollRepository _repo;

    public PayPayrollCommand(IPayrollRepository repo)
    {
      _repo = repo;
    }

    public Task Execute(int payrollId)
    {
      // TODO: implement payroll payment

      return Task.CompletedTask;
    }
  }
}
