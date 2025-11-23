namespace Planilla_Backend.CleanArchitecture.Application.Ports
{
  using Microsoft.Data.SqlClient;
  using Planilla_Backend.CleanArchitecture.Domain.Entities;
  public interface IPayrollRepository
  {
    // Queries
    Task<CompanyModel> GetCompany(int companyId, SqlConnection? conn = null, SqlTransaction? tx = null);
    Task<IEnumerable<EmployeeModel>> GetEmployees(int companyId, DateTime dateFrom, DateTime dateTo, SqlConnection? conn = null, SqlTransaction? tx = null);
    Task<IEnumerable<ContractModel>> GetContracts(int companyId, DateTime dateFrom, DateTime dateTo, SqlConnection? conn = null, SqlTransaction? tx = null);
    Task<IEnumerable<ElementModel>> GetElementsForEmployee(int companyId, int employeeId, DateTime dateFrom, DateTime dateTo, SqlConnection? conn = null, SqlTransaction? tx = null);
    Task<IDictionary<int, decimal>> GetEmployeeTimesheets(int companyId, DateTime dateFrom, DateTime dateTo, SqlConnection? conn = null, SqlTransaction? tx = null);
    Task<IEnumerable<TaxModel>> GetTaxes(DateTime dateFrom, DateTime dateTo, SqlConnection? conn = null, SqlTransaction? tx = null);
    Task<IEnumerable<CCSSModel>> GetCCSS(DateTime dateFrom, DateTime dateTo, SqlConnection? conn = null, SqlTransaction? tx = null);
    Task<bool> ExistsPayrollForPeriod(int companyId, DateTime dateFrom, DateTime dateTo, SqlConnection? conn = null, SqlTransaction? tx = null);

    // Commands
    Task<int> SaveCompanyPayroll(CompanyPayrollModel header, SqlConnection? conn = null, SqlTransaction? tx = null);
    Task<int> SaveEmployeePayroll(EmployeePayrollModel employeePayroll, SqlConnection? conn = null, SqlTransaction? tx = null);
    Task SavePayrollDetails(int employeePayrollId, IEnumerable<PayrollDetailModel> details, SqlConnection? conn = null, SqlTransaction? tx = null);
    Task UpdateEmployeePayrollTotals(int employeePayrollId, EmployeePayrollModel totalsAndStatus, SqlConnection? conn = null, SqlTransaction? tx = null);
    Task UpdateCompanyPayrollTotals(int companyPayrollId, CompanyPayrollModel totalsAndStatus, SqlConnection? conn = null, SqlTransaction? tx = null);
    Task SavePayment(int employeePayrollId, PaymentModel payment, SqlConnection? conn = null, SqlTransaction? tx = null);
  }
}
