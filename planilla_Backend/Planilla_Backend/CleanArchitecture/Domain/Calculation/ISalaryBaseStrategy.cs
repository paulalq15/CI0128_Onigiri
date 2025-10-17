using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public interface ISalaryBaseStrategy
  {
    public bool Applicable(ContractModel contract)
    {
      return contract != null && contract.ContractType == ContractType.FixedSalary;
    }

    PayrollDetailModel CreateBaseLine(EmployeePayrollModel employeePayroll, ContractModel contract, PayrollContext ctx);
  }
}
