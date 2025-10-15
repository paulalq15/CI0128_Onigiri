using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public class Concept_FixedAmountStrategy : IConceptStrategy
  {
    public bool Applicable(ElementModel concept)
    {
      return false;
    }
    public PayrollDetailModel Apply(EmployeePayrollModel employeePayroll, ElementModel concept, PayrollContext ctx)
    {
      // TODO: implement the logic
      throw new NotImplementedException();
    }
  }
}
