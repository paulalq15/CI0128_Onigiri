using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;
using Planilla_Backend.CleanArchitecture.Domain.Calculation;

namespace Planilla_Backend.CleanArchitecture.Application.UseCases
{
  public class CreatePayrollCommand : ICreatePayrollCommand
  {
    private readonly IPayrollRepository _repo;
    private readonly PayrollTemplate _template;

    public CreatePayrollCommand(IPayrollRepository repo, PayrollTemplate template)
    {
      _repo = repo;
      _template = template;
    }

    public Task<PayrollSummary> Execute(int companyId, DateOnly DateFrom, DateOnly DateTo)
    {

      // TODO: implement payroll creation logic

      return Task.FromResult(new PayrollSummary());
    }
  }
}
