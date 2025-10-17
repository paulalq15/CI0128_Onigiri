using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public class Salary_HourlyStrategy : ISalaryBaseStrategy
  {
    public PayrollDetailModel CreateBaseLine(EmployeePayrollModel employeePayroll, ContractModel contract, PayrollContext ctx)
    {
      if (employeePayroll == null) throw new ArgumentNullException("La planilla del empleado es requerida");
      if (contract == null) throw new ArgumentNullException("El contrato es requerido");
      if (ctx == null) throw new ArgumentNullException("El contexto de planilla es requerido");

      int employeeId = employeePayroll.EmployeeId;

      if (ctx.HoursByEmployee == null) throw new InvalidOperationException("La cantidad de horas por empleado es requerido");

      decimal totalHours;
      if (!ctx.HoursByEmployee.TryGetValue(employeeId, out totalHours)) throw new InvalidOperationException("No se encontraron horas para el empleado con Id " + employeeId);
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
