using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public class Salary_FixedStrategy
  {
    public bool Applicable(ContractModel contract)
    {
      return contract != null && contract.ContractType == ContractType.FixedSalary;
    }

    public PayrollDetailModel CreateBaseLine(EmployeePayrollModel employeePayroll, ContractModel contract, PayrollContext ctx)
    {
      if (employeePayroll == null) throw new ArgumentNullException(nameof(employeePayroll));
      if (contract == null) throw new ArgumentNullException(nameof(contract));

      var line = new PayrollDetailModel();
      line.EmployeePayrollId = employeePayroll.Id;
      line.Description = "Salario bruto";
      line.Type = PayrollItemType.Base;
      line.Amount = contract.Salary;
      line.IdCCSS = null;
      line.IdTax = null;
      line.IdElement = null;

      return line;
    }
  }
}
