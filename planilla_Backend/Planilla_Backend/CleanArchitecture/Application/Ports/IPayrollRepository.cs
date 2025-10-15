namespace Planilla_Backend.CleanArchitecture.Application.Ports
{
  using Planilla_Backend.CleanArchitecture.Domain.Entities;
  public interface IPayrollRepository
  {
    // Queries
    Task<CompanyModel> GetCompany(int companyId);
    Task<IEnumerable<EmployeeModel>> GetEmployees(int companyId);
    Task<IEnumerable<ContractModel>> GetContracts(int companyId, DateOnly DateFrom, DateOnly DateTo);
    Task<IEnumerable<ElementModel>> GetElementsForEmployee(int companyId, int employeeId, DateOnly dateFrom, DateOnly dateTo);
    Task<IEnumerable<TaxModel>> GetTaxes(int companyId, DateOnly dateFrom, DateOnly dateTo);
    Task<IEnumerable<CCSSModel>> GetCCSS(int companyId, DateOnly dateFrom, DateOnly dateTo);

    // Commands
    Task<int> SaveCompanyPayroll(CompanyPayrollModel header);
    Task<int> SaveEmployeePayroll(EmployeePayrollModel employeePayroll);
    Task SavePayrollDetails(int employeePayrollId, IEnumerable<PayrollDetailModel> details);
    Task UpdateEmployeePayrollTotals(int employeePayrollId, EmployeePayrollModel totalsAndStatus);
    Task UpdateCompanyPayrollTotals(int companyPayrollId, CompanyPayrollModel totalsAndStatus);
    Task SavePayment(int employeePayrollId, PaymentModel payment);
  }
}
