using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Application.UseCases
{
  public class GetPayrollSummaryQuery : IGetPayrollSummaryQuery
  {
    private readonly IPayrollRepository _repo;

    public GetPayrollSummaryQuery(IPayrollRepository repo)
    {
      _repo = repo;
    }

    public Task<PayrollSummary> Execute(int payrollId)
    {
      // TODO: implement payroll summary retrieval

      return Task.FromResult(new PayrollSummary());
    }
  }
}
