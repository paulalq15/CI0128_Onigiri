using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public class Salary_FixedStrategy : ISalaryBaseStrategy
  {
    public bool Applicable(ContractModel contract)
    {
      return contract != null && contract.ContractType == ContractType.FixedSalary;
    }

    public PayrollDetailModel CreateBaseLine(EmployeePayrollModel employeePayroll, ContractModel contract, PayrollContext ctx)
    {
      if (employeePayroll == null) throw new ArgumentNullException(nameof(employeePayroll));
      if (contract == null) throw new ArgumentNullException(nameof(contract));
      if (ctx == null) throw new ArgumentNullException(nameof(ctx));

      var daysOfMonth = 30;
      var calculateDays = false;
      var payrollDays = 0;
      var salaryAmount = 0m;

      var periodStart = ctx.DateFrom;
      var periodEnd = ctx.DateTo;
      var contractStart = contract.StartDate;
      var contractEnd = contract.EndDate.HasValue ? contract.EndDate.Value : periodEnd;

      if (contractStart > periodStart)
      {
        periodStart = contractStart;
        calculateDays = true;
      }
        
      if (contractEnd < periodEnd) {
        periodEnd = contractEnd;
        calculateDays = true;
      }

      if (periodStart <= periodEnd && calculateDays)
      {
        payrollDays = (periodEnd - periodStart).Days + 1;
        salaryAmount = Math.Round(contract.Salary / daysOfMonth * payrollDays, 2);
      }
      else
      {
        salaryAmount = contract.Salary;
      }

      var line = new PayrollDetailModel();
      line.EmployeePayrollId = employeePayroll.Id;
      line.Description = "Salario bruto";
      line.Type = PayrollItemType.Base;
      line.Amount = salaryAmount;
      line.IdCCSS = null;
      line.IdTax = null;
      line.IdElement = null;

      return line;
    }
  }
}
