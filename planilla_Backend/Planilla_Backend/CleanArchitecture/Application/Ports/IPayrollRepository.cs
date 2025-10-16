namespace Planilla_Backend.CleanArchitecture.Application.Ports
{
  using Planilla_Backend.CleanArchitecture.Domain.Entities;
  public interface IPayrollRepository
  {
    // Queries
    Task<CompanyModel> GetCompany(int companyId);
    Task<IEnumerable<EmployeeModel>> GetEmployees(int companyId, DateOnly dateFrom, DateOnly dateTo);
    Task<IEnumerable<ContractModel>> GetContracts(int companyId, DateOnly dateFrom, DateOnly dateTo);
    Task<IEnumerable<ElementModel>> GetElementsForEmployee(int companyId, int employeeId, DateOnly dateFrom, DateOnly dateTo);
    Task<IDictionary<int, decimal>> GetEmployeeTimesheets(int companyId, DateOnly dateFrom, DateOnly dateTo);
    Task<IEnumerable<TaxModel>> GetTaxes(DateOnly dateFrom, DateOnly dateTo);
    Task<IEnumerable<CCSSModel>> GetCCSS(DateOnly dateFrom, DateOnly dateTo);
    Task<CompanyPayrollModel?> GetLatestOpenCompanyPayroll(int companyId);

    // Commands
    Task<int> SaveCompanyPayroll(CompanyPayrollModel header);
    Task<int> SaveEmployeePayroll(EmployeePayrollModel employeePayroll);
    Task SavePayrollDetails(int employeePayrollId, IEnumerable<PayrollDetailModel> details);
    Task UpdateEmployeePayrollTotals(int employeePayrollId, EmployeePayrollModel totalsAndStatus);
    Task UpdateCompanyPayrollTotals(int companyPayrollId, CompanyPayrollModel totalsAndStatus);
    Task SavePayment(int employeePayrollId, PaymentModel payment);
  }
}
