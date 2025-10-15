using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public interface IConceptStrategy
  {
    bool Applicable(ElementModel concept);
    PayrollDetailModel Apply(EmployeePayrollModel employeePayroll, ElementModel concept, PayrollContext ctx);
  }
}
