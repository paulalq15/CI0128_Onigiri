namespace Planilla_Backend.CleanArchitecture.Application.Ports
{
  using Planilla_Backend.CleanArchitecture.Domain.Entities;
  public interface IPayrollRepository
  {
    // Queries
    Task<CompanyModel> GetCompany(int companyId);
    Task<IEnumerable<EmployeeModel>> GetEmployees(int companyId, DateTime dateFrom, DateTime dateTo);
    Task<IEnumerable<ContractModel>> GetContracts(int companyId, DateTime dateFrom, DateTime dateTo);
    Task<IEnumerable<ElementModel>> GetElementsForEmployee(int companyId, int employeeId, DateTime dateFrom, DateTime dateTo);
    Task<IDictionary<int, decimal>> GetEmployeeTimesheets(int companyId, DateTime dateFrom, DateTime dateTo);
    Task<IEnumerable<TaxModel>> GetTaxes(DateTime dateFrom, DateTime dateTo);
    Task<IEnumerable<CCSSModel>> GetCCSS(DateTime dateFrom, DateTime dateTo);
    Task<CompanyPayrollModel?> GetLatestOpenCompanyPayroll(int companyId);
    Task<CompanyPayrollModel?> GetCompanyPayrollById(int companyPayrollId);
    Task<IEnumerable<EmployeePayrollModel>> GetEmployeePayrolls(int companyPayrollId);

    // Commands
    Task<int> SaveCompanyPayroll(CompanyPayrollModel header);
    Task<int> SaveEmployeePayroll(EmployeePayrollModel employeePayroll);
    Task SavePayrollDetails(int employeePayrollId, IEnumerable<PayrollDetailModel> details);
    Task UpdateEmployeePayrollTotals(int employeePayrollId, EmployeePayrollModel totalsAndStatus);
    Task UpdateCompanyPayrollTotals(int companyPayrollId, CompanyPayrollModel totalsAndStatus);
    Task SavePayment(int employeePayrollId, PaymentModel payment);
    Task UpdatePaidCompanyPayroll(int companyPayrollId, int personId, DateTime paymentDate);
  }
}
