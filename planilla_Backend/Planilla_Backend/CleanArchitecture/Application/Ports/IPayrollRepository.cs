namespace Planilla_Backend.CleanArchitecture.Application.Ports
{
  using Planilla_Backend.CleanArchitecture.Domain.Entities;
  public interface IPayrollRepository
  {
    // Queries
    CompanyModel GetCompany(int companyId);
    IEnumerable<EmployeeModel> GetEmployees(int companyId);
    IEnumerable<ContractModel> GetContracts(int companyId, DateOnly DateFrom, DateOnly DateTo);
    IEnumerable<ElementModel> GetElementsForEmployee(int companyId, int employeeId, DateOnly dateFrom, DateOnly dateTo);
    IEnumerable<TaxModel> GetTaxes(int companyId, DateOnly dateFrom, DateOnly dateTo);
    IEnumerable<CCSSModel> GetCCSS(int companyId, DateOnly dateFrom, DateOnly dateTo);

    // Commands
    int SaveCompanyPayroll(CompanyPayrollModel header);
    int SaveEmployeePayroll(EmployeePayrollModel employeePayroll);
    void SavePayrollDetails(int employeePayrollId, IEnumerable<PayrollDetailModel> details);
    void UpdateEmployeePayrollTotals(int employeePayrollId, EmployeePayrollModel totalsAndStatus);
    void UpdateCompanyPayrollTotals(int companyPayrollId, CompanyPayrollModel totalsAndStatus);
    void SavePayment(int employeePayrollId, PaymentModel payment);
  }
}
