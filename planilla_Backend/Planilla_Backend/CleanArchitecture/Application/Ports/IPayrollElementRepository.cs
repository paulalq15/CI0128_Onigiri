using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Application.Ports
{
  public interface IPayrollElementRepository
  {
    Task<PayrollElementEntity?> GetPayrollElementByElementId(int elementId);

    Task<int> UpdatePayrollElement(PayrollElementEntity payrollElement);
  }
}
