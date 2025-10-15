using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Infrastructure
{
  public class PayrollRepository : IPayrollRepository
  {
    public PayrollRepository()
    {
      //Inject dependencies
    }

    // =======================
    //  TODO: READ METHODS
    // =======================

    public CompanyModel GetCompany(int companyId)
    {
      return new CompanyModel();
    }

    public IEnumerable<EmployeeModel> GetEmployees(int companyId)
    {
      return new List<EmployeeModel>();
    }

    public IEnumerable<ContractModel> GetContracts(int companyId, DateOnly DateFrom, DateOnly DateTo)
    {
      return new List<ContractModel>();
    }

    public IEnumerable<ElementModel> GetElementsForEmployee(int companyId, int employeeId, DateOnly dateFrom, DateOnly dateTo)
    {
      return new List<ElementModel>();
    }

    public IEnumerable<TaxModel> GetTaxes(int companyId, DateOnly dateFrom, DateOnly dateTo)
    {
      return new List<TaxModel>();
    }

    public IEnumerable<CCSSModel> GetCCSS(int companyId, DateOnly dateFrom, DateOnly dateTo)
    {
      return new List<CCSSModel>();
    }

    // =======================
    //  TODO: WRITE METHODS
    // =======================

    public int SaveCompanyPayroll(CompanyPayrollModel header)
    {
      return 0;
    }
    public int SaveEmployeePayroll(EmployeePayrollModel employeePayroll)
    {
      return 0;
    }
    public void SavePayrollDetails(int employeePayrollId, IEnumerable<PayrollDetailModel> details)
    {
    }
    public void UpdateEmployeePayrollTotals(int employeePayrollId, EmployeePayrollModel totalsAndStatus)
    {
    }
    public void UpdateCompanyPayrollTotals(int companyPayrollId, CompanyPayrollModel totalsAndStatus)
    {
    }
    public void SavePayment(int employeePayrollId, PaymentModel payment)
    {
    }

  }
}
