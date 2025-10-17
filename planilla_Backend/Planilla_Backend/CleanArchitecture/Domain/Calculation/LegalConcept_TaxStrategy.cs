using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public class LegalConcept_TaxStrategy : ILegalConceptStrategy
  {
    public IEnumerable<PayrollDetailModel> Apply(EmployeePayrollModel employeePayroll, PayrollContext ctx)
    {
      // TODO: implement the logic
      throw new NotImplementedException();
    }
  }
}
