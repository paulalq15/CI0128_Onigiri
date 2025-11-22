using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Application.UseCases
{
  public interface IPayrollElementCommand
  {
    Task<int> Update(PayrollElementEntity payrollElement);
    Task<int> Delete(int elementId);
  }
}
