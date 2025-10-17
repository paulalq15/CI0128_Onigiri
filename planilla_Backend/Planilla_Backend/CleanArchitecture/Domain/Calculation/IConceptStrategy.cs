using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public interface IConceptStrategy
  {
    bool Applicable(ContractModel contract);
    PayrollDetailModel Apply(EmployeePayrollModel employeePayroll, ElementModel concept, PayrollContext ctx);
  }
}
