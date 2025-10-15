using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Application.UseCases
{
  public interface IGetPayrollSummaryQuery
  {
    Task<PayrollSummary> Execute(int payrollId);
  }
}
