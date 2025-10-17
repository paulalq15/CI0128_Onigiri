using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public class Concept_PercentageStrategy : IConceptStrategy
  {
    public bool Applicable(ContractModel contract)
    {
      return contract != null && contract.ContractType == ContractType.FixedSalary;
    }
    public PayrollDetailModel Apply(EmployeePayrollModel employeePayroll, ElementModel concept, PayrollContext ctx)
    {
      // TODO: implement the logic
      throw new NotImplementedException();
    }
  }
}
