using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Application.UseCases
{
  public class PayPayrollCommand : IPayPayrollCommand
  {
    private readonly IPayrollRepository _repo;

    public PayPayrollCommand(IPayrollRepository repo)
    {
      _repo = repo;
    }

    public async Task<PayrollSummary> Execute(int payrollId, int personId)
    {
      if (payrollId <= 0) throw new ArgumentException("El parámetro payrollId debe ser mayor que cero");
      if (personId <= 0) throw new ArgumentException("El parámetro payrollId debe ser mayor que cero");

      var now = DateTime.Now;
      var companyPayroll = await _repo.GetCompanyPayrollById(payrollId);
      if (companyPayroll == null) throw new KeyNotFoundException("La planilla no existe");

      var summary = new PayrollSummary
      {
        CompanyPayrollId = companyPayroll.Id,
        TotalGrossSalaries = companyPayroll.Gross,
        TotalEmployerDeductions = companyPayroll.EmployerDeductions,
        TotalEmployeeDeductions = companyPayroll.EmployeeDeductions,
        TotalBenefits = companyPayroll.Benefits,
        TotalNetEmployee = companyPayroll.Net,
        TotalEmployerCost = companyPayroll.Cost,
        DateFrom = companyPayroll.DateFrom,
        DateTo = companyPayroll.DateTo,
      };

      var pendingEmployeePayrolls = (await _repo.GetUnpaidEmployeePayrolls(payrollId))?.ToList();
      if (pendingEmployeePayrolls == null || pendingEmployeePayrolls.Count == 0)
      {
        await _repo.UpdatePaidCompanyPayroll(payrollId);
        summary.PayDate = now;
        return summary;
      }

      for (int i = 0; i < pendingEmployeePayrolls.Count; i++)
      {
        var employeePayroll = pendingEmployeePayrolls[i];
        if (employeePayroll == null || employeePayroll.Id <= 0) continue;

        var payment = new PaymentModel
        {
          EmployeePayrollId = employeePayroll.Id,
          Amount = employeePayroll.Net,
          PaymentDate = now,
          PaymentRef = BuildPaymentRef(payrollId, employeePayroll.Id, now),
          CreatedBy = personId
        };

        await _repo.SavePayment(employeePayroll.Id, payment);
      }

      await _repo.UpdatePaidCompanyPayroll(payrollId);
      summary.PayDate = now;

      return summary;
    }

    private static string BuildPaymentRef(int companyPayrollId, int employeePayrollId, DateTime date)
    {
      return "PAY-" + companyPayrollId + "-" + employeePayrollId + "-" + date.ToString("yyyyMMdd");
    }
  }
}
