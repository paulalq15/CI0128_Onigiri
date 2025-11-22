using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Application.UseCases
{
  public interface IPayrollElementQuery
  {
    Task<PayrollElementEntity?> GetPayrollElement(int payElementId);
    Task<DeletePayrollElementEmailListDto> GetEmployeeEmails(int elementId);
  }
}
