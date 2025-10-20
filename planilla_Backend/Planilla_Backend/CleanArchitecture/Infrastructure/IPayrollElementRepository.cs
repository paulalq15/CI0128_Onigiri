using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Infrastructure
{
  public interface IPayrollElementRepository
  {
    Task<PayrollElementEntity?> GetPayrollElementByElementId(int elementId);

    Task<int> UpdatePayrollElement(PayrollElementEntity payrollElement);
  }
}
