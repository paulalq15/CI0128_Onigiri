using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Application.UseCases
{
  public interface IGetPayrollElement
  {
    Task<PayrollElementEntity?> Execute(int payElementId);
  }
}
