using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Application.UseCases
{
  public interface ICreatePayrollCommand
  {
    Task<PayrollSummary> Execute(int companyId, int personId, DateOnly dateFrom, DateOnly dateTo);
  }
}
