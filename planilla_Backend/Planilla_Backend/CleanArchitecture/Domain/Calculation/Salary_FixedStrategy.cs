using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public class Salary_FixedStrategy : ISalaryBaseStrategy
  {
    public bool Applicable(ContractModel contract)
    {
      return contract != null && contract.ContractType != EmployeeType.ProfessionalServices;
    }
    public PayrollDetailModel CreateBaseLine(EmployeePayrollModel employeePayroll, ContractModel contract, PayrollContext ctx)
    {
      if (employeePayroll == null) throw new ArgumentNullException("La planilla del empleado es requerida");
      if (contract == null) throw new ArgumentNullException("El contrato es requerido");
      if (ctx == null) throw new ArgumentNullException("El contexto de planilla es requerido");

      var firstDayMonth = 1;
      var fixedMonthDays = 30;

      // payroll period, month range, contract range
      var periodStartDate = DateOnly.FromDateTime(ctx.DateFrom);
      var periodEndDate = DateOnly.FromDateTime(ctx.DateTo);
      var contractStart = DateOnly.FromDateTime(contract.StartDate);
      var contractEnd = contract.EndDate.HasValue ? DateOnly.FromDateTime(contract.EndDate.Value) : DateOnly.MaxValue;
      var workedDaysInPeriod = OverlapInclusiveDays(periodStartDate, periodEndDate, contractStart, contractEnd);

      var monthStart = new DateOnly(periodStartDate.Year, periodStartDate.Month, firstDayMonth);
      var monthEnd = new DateOnly(periodStartDate.Year, periodStartDate.Month, DateTime.DaysInMonth(periodStartDate.Year, periodStartDate.Month));
      var workedDaysInMonth = OverlapInclusiveDays(monthStart, monthEnd, contractStart, contractEnd);
      var totalDaysInMonth = monthEnd.Day;

      var totalPayrollDays = 0;
      var amountForPeriod = 0m;
      var amountForOtherPeriod = 0m;
      var workedDaysInOtherPeriod = 0;
      var halfMonthDays = 0;
      if (ctx.Company.PaymentFrequency == PaymentFrequency.Biweekly)
      {
        totalPayrollDays = (periodEndDate.DayNumber - periodStartDate.DayNumber) + 1;
        amountForPeriod = Math.Round(contract.Salary / 2, 2);
        amountForOtherPeriod = Math.Round(contract.Salary / 2, 2);

        if (periodStartDate.Day == 1)
        {
          workedDaysInOtherPeriod = OverlapInclusiveDays(periodStartDate.AddDays(15), monthEnd, contractStart, contractEnd);
          halfMonthDays = OverlapInclusiveDays(monthStart.AddDays(15), monthEnd, periodStartDate.AddDays(15), monthEnd);
        }
        else
        {
          workedDaysInOtherPeriod = OverlapInclusiveDays(periodStartDate.AddDays(-15), periodStartDate.AddDays(-1), contractStart, contractEnd);
          halfMonthDays = OverlapInclusiveDays(monthStart.AddDays(-15), monthStart.AddDays(-1), periodStartDate.AddDays(-15), periodStartDate.AddDays(-1));
        }
        if (workedDaysInOtherPeriod != halfMonthDays) amountForOtherPeriod = ComputeBase(contract.Salary, workedDaysInOtherPeriod, fixedMonthDays);
      }
      else
      {
        totalPayrollDays = periodEndDate.Day;
        amountForPeriod = contract.Salary;
      }

      // adjust period salary based on worked days
      if (workedDaysInPeriod != totalPayrollDays) amountForPeriod = ComputeBase(contract.Salary, workedDaysInPeriod, fixedMonthDays);
      employeePayroll.Gross = amountForPeriod;

      //adjust monthly base based on expected worked days
      var expectedMonthlySalary = 0m;
      if (ctx.Company.PaymentFrequency == PaymentFrequency.Biweekly) expectedMonthlySalary = amountForPeriod + amountForOtherPeriod;
      else expectedMonthlySalary = amountForPeriod;
      employeePayroll.BaseSalaryForPeriod = Math.Round(expectedMonthlySalary, 2);

      // Salary line for the period
      return new PayrollDetailModel
      {
        EmployeePayrollId = employeePayroll.Id,
        Description = "Salario " + GetContractTypeDisplayName(contract.ContractType),
        Type = PayrollItemType.Base,
        Amount = amountForPeriod,
        IdCCSS = null,
        IdTax = null,
        IdElement = null
      };
    }

    private static int OverlapInclusiveDays(DateOnly aFrom, DateOnly aTo, DateOnly bFrom, DateOnly bTo)
    {
      var from = aFrom > bFrom ? aFrom : bFrom;
      var to = aTo < bTo ? aTo : bTo;
      if (to < from) return 0;
      return (to.DayNumber - from.DayNumber) + 1;
    }

    private static decimal ComputeBase(decimal contractMonthlySalary, int workedDaysInMonth, int totalDaysInMonth)
    {
      if (workedDaysInMonth <= 0) return 0m;
      return Math.Round(contractMonthlySalary * workedDaysInMonth / totalDaysInMonth, 2);
    }

    private static string GetContractTypeDisplayName(EmployeeType type)
    {
      switch (type)
      {
        case EmployeeType.FullTime:
          return "Tiempo Completo";
        case EmployeeType.PartTime:
          return "Medio Tiempo";
        case EmployeeType.ProfessionalServices:
          return "Servicios Profesionales";
        default:
          return type.ToString();
      }
    }
  }
}