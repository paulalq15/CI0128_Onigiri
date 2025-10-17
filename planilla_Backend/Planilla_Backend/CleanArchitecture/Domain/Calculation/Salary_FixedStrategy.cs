using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public class Salary_FixedStrategy : ISalaryBaseStrategy
  {
    public PayrollDetailModel CreateBaseLine(EmployeePayrollModel employeePayroll, ContractModel contract, PayrollContext ctx)
    {
      if (employeePayroll == null) throw new ArgumentNullException(nameof(employeePayroll));
      if (contract == null) throw new ArgumentNullException(nameof(contract));
      if (ctx == null) throw new ArgumentNullException(nameof(ctx));

      var firstDayMonth = 1;
      var halfDayMonth = 15;

      // payroll period, month range, contract range
      var periodStartDate = DateOnly.FromDateTime(ctx.DateFrom);
      var periodEndDate = DateOnly.FromDateTime(ctx.DateTo);
      var monthStart = new DateOnly(periodStartDate.Year, periodStartDate.Month, firstDayMonth);
      var monthEnd = new DateOnly(periodStartDate.Year, periodStartDate.Month,DateTime.DaysInMonth(periodStartDate.Year, periodStartDate.Month));
      var totalDaysInMonth = monthEnd.Day;
      var contractStart = DateOnly.FromDateTime(contract.StartDate);
      var contractEnd = contract.EndDate.HasValue ? DateOnly.FromDateTime(contract.EndDate.Value) : monthEnd;

      // monthly base to calculate payroll elements
      var workedDaysInMonth = OverlapInclusiveDays(monthStart, monthEnd, contractStart, contractEnd);
      var baseMonthly = ComputeMonthlyBase(contract.Salary, workedDaysInMonth, totalDaysInMonth);
      employeePayroll.BaseSalaryForPeriod = baseMonthly;

      // salary for the current payroll period
      var paymentFrequency = ctx.Company?.PaymentFrequency ?? PaymentFrequency.Monthly;
      var amountForPeriod = baseMonthly;

      if (paymentFrequency == PaymentFrequency.Biweekly)
      {
        var isFirstHalf = periodStartDate.Day == firstDayMonth && periodEndDate.Day == halfDayMonth;
        var halfStart = isFirstHalf
          ? new DateOnly(periodStartDate.Year, periodStartDate.Month, firstDayMonth)
          : new DateOnly(periodStartDate.Year, periodStartDate.Month, halfDayMonth + 1);
        var halfEnd = isFirstHalf
          ? new DateOnly(periodStartDate.Year, periodStartDate.Month, halfDayMonth)
          : monthEnd;

        var workedDaysInHalf = OverlapInclusiveDays(halfStart, halfEnd, contractStart, contractEnd);
        amountForPeriod = ComputePeriodAmount(baseMonthly, workedDaysInMonth, workedDaysInHalf);
      }

      // Salary line for the period
      return new PayrollDetailModel
      {
        EmployeePayrollId = employeePayroll.Id,
        Description = "Salario bruto",
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

    private static decimal ComputeMonthlyBase(decimal contractMonthlySalary, int workedDaysInMonth, int totalDaysInMonth)
    {
      if (workedDaysInMonth <= 0) return 0m;
      return Math.Round(contractMonthlySalary * workedDaysInMonth / totalDaysInMonth, 2);
    }

    private static decimal ComputePeriodAmount(decimal baseMonthly, int workedDaysInMonth, int workedDaysInHalf)
    {
      if (workedDaysInMonth <= 0) return 0m;
      if (workedDaysInHalf == workedDaysInMonth) return Math.Round(baseMonthly / 2m, 2);
      return Math.Round(baseMonthly * workedDaysInHalf / workedDaysInMonth, 2);
    }
  }
}