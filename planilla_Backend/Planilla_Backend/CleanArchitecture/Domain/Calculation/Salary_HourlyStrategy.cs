using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public class Salary_HourlyStrategy : ISalaryBaseStrategy
  {
    public PayrollDetailModel CreateBaseLine(EmployeePayrollModel employeePayroll, ContractModel contract, PayrollContext ctx)
    {
      if (employeePayroll == null) throw new ArgumentNullException(nameof(employeePayroll));
      if (contract == null) throw new ArgumentNullException(nameof(contract));
      if (ctx == null) throw new ArgumentNullException(nameof(ctx));

      int employeeId = employeePayroll.EmployeeId;

      if (ctx.HoursByEmployee == null) throw new InvalidOperationException("HoursByEmployee is required");

      decimal totalHours;
      if (!ctx.HoursByEmployee.TryGetValue(employeeId, out totalHours)) throw new InvalidOperationException("No hours found for employeeId " + employeeId);
      decimal amount = totalHours * contract.Salary;

      return new PayrollDetailModel
      {
        EmployeePayrollId = employeePayroll.Id,
        Description = "Servicios Profesionales",
        Type = PayrollItemType.Base,
        Amount = amount,
        IdCCSS = null,
        IdTax = null,
        IdElement = null,
      };
    }
  }
}
