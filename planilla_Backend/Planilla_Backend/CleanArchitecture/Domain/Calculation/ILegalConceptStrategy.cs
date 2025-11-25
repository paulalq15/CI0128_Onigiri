using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public interface ILegalConceptStrategy
  {
    public bool Applicable(ContractModel contract)
    {
      return contract != null && contract.ContractType != EmployeeType.ProfessionalServices;
    }

    IEnumerable<PayrollDetailModel> Apply(EmployeePayrollModel employeePayroll, PayrollContext ctx);
  }
}
