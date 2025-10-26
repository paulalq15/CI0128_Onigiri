using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Application.UseCases
{
  public interface IUpdatePayrollElement
  {
    Task<int> Execute(PayrollElementEntity payrollElement);
  }
}
