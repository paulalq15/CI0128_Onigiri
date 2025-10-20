using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Application
{
  public interface IPayrollElementUseCase
  {
    Task<PayrollElementEntity?> GetPayrollElementByElementId(int id);

    Task<int> UpdatePayrollElement(PayrollElementEntity payrollElement);
  }
}
