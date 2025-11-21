using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Application.UseCases
{
  public interface IPayrollElementQuery
  {
    Task<PayrollElementEntity?> Execute(int payElementId);
  }
}
