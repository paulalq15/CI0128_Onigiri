using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public interface ISalaryBaseStrategy
  {
    PayrollDetailModel CreateBaseLine(EmployeeModel employee, ContractModel contract, PayrollContext ctx);
  }
}
