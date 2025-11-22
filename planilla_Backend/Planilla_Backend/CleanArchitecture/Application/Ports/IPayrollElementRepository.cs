using Planilla_Backend.CleanArchitecture.Domain.Entities;
using Planilla_Backend.CleanArchitecture.Domain.Reports;

namespace Planilla_Backend.CleanArchitecture.Application.Ports
{
  public interface IPayrollElementRepository
  {
    Task<PayrollElementEntity?> GetPayrollElementByElementId(int elementId);
    Task<int> UpdatePayrollElement(PayrollElementEntity payrollElement);
    Task<IEnumerable<DeletePayrollElementEmailDto>> GetEmployeeEmailsByAssignedElement(int elementId);
    Task<int> DeletePayrollElement(int elementId);
  }
}
