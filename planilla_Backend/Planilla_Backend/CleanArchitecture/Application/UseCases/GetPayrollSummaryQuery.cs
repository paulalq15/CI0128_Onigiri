using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Application.UseCases
{
  public class GetPayrollSummaryQuery : IGetPayrollSummaryQuery
  {
    private readonly IPayrollRepository _repo;

    public GetPayrollSummaryQuery(IPayrollRepository repo)
    {
      _repo = repo;
    }

    public async Task<PayrollSummary?> Execute(int companyId)
    {
      if (companyId <= 0) throw new ArgumentException("El parámetro companyId debe ser mayor que cero");

      var companyPayroll = await _repo.GetLatestOpenCompanyPayroll(companyId);
      if (companyPayroll == null) return null;

      var summary = new PayrollSummary();
      summary.CompanyPayrollId = companyPayroll.Id;
      summary.TotalGrossSalaries = companyPayroll.Gross;
      summary.TotalEmployerDeductions = companyPayroll.EmployerDeductions;
      summary.TotalEmployeeDeductions = companyPayroll.EmployeeDeductions;
      summary.TotalBenefits = companyPayroll.Benefits;
      summary.TotalNetEmployee = companyPayroll.Net;
      summary.TotalEmployerCost = companyPayroll.Cost;
      summary.DateFrom = companyPayroll.DateFrom;
      summary.DateTo = companyPayroll.DateTo;

      return summary;
    }
  }
}
