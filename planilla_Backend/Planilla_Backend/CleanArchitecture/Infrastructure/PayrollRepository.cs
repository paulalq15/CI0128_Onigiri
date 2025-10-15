using Dapper;
using Microsoft.Data.SqlClient;
using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Infrastructure
{
  public class PayrollRepository : IPayrollRepository
  {
    private readonly string _connectionString;
    public PayrollRepository(IConfiguration config)
    {
      _connectionString = config.GetConnectionString("OnigiriContext");
    }

    // =======================
    //  TODO: READ METHODS
    // =======================

    public async Task<CompanyModel> GetCompany(int companyId)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);
        const string query = @"SELECT IdEmpresa AS Id, FrecuenciaPago AS PaymentFrequency, DiaPago1 AS PayDay1, DiaPago2 AS PayDay2
                             FROM Empresa
                             WHERE IdEmpresa = @companyId";

        var company = await connection.QuerySingleOrDefaultAsync<CompanyModel>(query, new { companyId });
        return company ?? new CompanyModel();
      }
      catch (Exception ex)
      {
        Console.WriteLine("Error getting company: " + ex.Message);
        return new CompanyModel();
      }      
    }

    public Task<IEnumerable<EmployeeModel>> GetEmployees(int companyId)
    {
      return Task.FromResult<IEnumerable<EmployeeModel>>(new List<EmployeeModel>());
    }

    public Task<IEnumerable<ContractModel>> GetContracts(int companyId, DateOnly DateFrom, DateOnly DateTo)
    {
      return Task.FromResult<IEnumerable<ContractModel>>(new List<ContractModel>());
    }

    public Task<IEnumerable<ElementModel>> GetElementsForEmployee(int companyId, int employeeId, DateOnly dateFrom, DateOnly dateTo)
    {
      return Task.FromResult<IEnumerable<ElementModel>>(new List<ElementModel>());
    }

    public Task<IEnumerable<TaxModel>> GetTaxes(int companyId, DateOnly dateFrom, DateOnly dateTo)
    {
      return Task.FromResult<IEnumerable<TaxModel>>(new List<TaxModel>());
    }

    public Task<IEnumerable<CCSSModel>> GetCCSS(int companyId, DateOnly dateFrom, DateOnly dateTo)
    {
      return Task.FromResult<IEnumerable<CCSSModel>>(new List<CCSSModel>());
    }

    // =======================
    //  TODO: WRITE METHODS
    // =======================

    public Task<int> SaveCompanyPayroll(CompanyPayrollModel header)
    {
      return Task.FromResult(0);
    }
    public Task<int> SaveEmployeePayroll(EmployeePayrollModel employeePayroll)
    {
      return Task.FromResult(0);
    }
    public Task SavePayrollDetails(int employeePayrollId, IEnumerable<PayrollDetailModel> details)
    {
      return Task.CompletedTask;
    }
    public Task UpdateEmployeePayrollTotals(int employeePayrollId, EmployeePayrollModel totalsAndStatus)
    {
      return Task.CompletedTask;
    }
    public Task UpdateCompanyPayrollTotals(int companyPayrollId, CompanyPayrollModel totalsAndStatus)
    {
      return Task.CompletedTask;
    }
    public Task SavePayment(int employeePayrollId, PaymentModel payment)
    {
      return Task.CompletedTask;
    }

  }
}
