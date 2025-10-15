using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Planilla_Backend.CleanArchitecture.Infrastructure
{
  public class PayrollRepository : IPayrollRepository
  {
    private readonly string _connectionString;
    private readonly ILogger<PayrollRepository> _logger;
    public PayrollRepository(IConfiguration config, ILogger<PayrollRepository> logger)
    {
      _connectionString = config.GetConnectionString("OnigiriContext");
      _logger = logger;
    }

    // READ METHODS

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
        _logger.LogError(ex, "GetCompany failed. companyId: {CompanyId}", companyId);
        throw;
      }      
    }

    public async Task<IEnumerable<EmployeeModel>> GetEmployees(int companyId)
    {
      try {
        using var connection = new SqlConnection(_connectionString);
        const string query = @"SELECT IdPersonas AS Id, TipoPersona AS PersonType
                              FROM Personas
                              WHERE IdEmpresa = @companyId AND TipoPersona IN ('Empleado', 'Aprobador')";
        var employees = await connection.QueryAsync<EmployeeModel>(query, new { companyId });
        return employees;
      }
      catch(Exception ex)
      {
        _logger.LogError(ex, "GetEmployees failed. companyId: {CompanyId}", companyId);
        throw;
      }
    }

    public async Task<IEnumerable<ContractModel>> GetContracts(int companyId, DateOnly dateFrom, DateOnly dateTo)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);
        const string query = @"SELECT c.IdContrato AS Id, c.IdPersona AS EmployeeId, c.Tipo AS ContractType, c.FechaInicio AS StartDate, c.FechaFin AS EndDate
                              FROM Contrato AS c
                              JOIN Personas AS p ON c.IdPersona = p.IdPersona
                              JOIN Usuario AS u ON p.IdPersona = u.IdPersona
                              JOIN UsuariosPorEmpresa AS ue ON u.IdUsuario = ue.IdUsuario
                              JOIN Empresa AS e ON ue.IdEmpresa = e.IdEmpresa
                              WHERE e.IdEmpresa = @companyId
                              AND CAST(StartDate AS date) <= @dateFrom
                              AND (EndDate IS NULL OR CAST(EndDate AS date) >= @dateTo";
        var contracts = await connection.QueryAsync<ContractModel>(query, new { companyId });
        return contracts;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "GetContracts failed. companyId: {CompanyId}", companyId);
        throw;
      }
    }

    public Task<IEnumerable<ElementModel>> GetElementsForEmployee(int companyId, int employeeId, DateOnly dateFrom, DateOnly dateTo)
    {
      return Task.FromResult<IEnumerable<ElementModel>>(new List<ElementModel>());
    }

    public async Task<IEnumerable<TaxModel>> GetTaxes(DateOnly dateFrom, DateOnly dateTo)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);
        const string query = @"SELECT IdImpuestoRenta AS Id, MontoMinimo AS Min, MontoMaximo AS Max, Porcentaje AS Rate, 'Tax' AS ItemType
                              FROM ImpuestoRenta
                              WHERE AND CAST(FechaInicio AS date) <= @dateFrom
                              AND (FechaFin IS NULL OR CAST(FechaFin AS date) >= @dateTo)";
        var taxBrackets = await connection.QueryAsync<TaxModel>(query);
        return taxBrackets;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "GetTaxes failed.");
        throw;
      }
    }

    public async Task<IEnumerable<CCSSModel>> GetCCSS(DateOnly dateFrom, DateOnly dateTo)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);
        const string query = @"SELECT IdCCSS AS Id, Categoria AS Category, Concepto AS Concept, Porcentaje AS Rate, 
                              CASE WHEN PagadoPor = 'Empleado' THEN 'EmployeeDeduction'
                                   WHEN PagadoPor = 'Empleador' THEN 'EmployerContribution'
                              END AS ItemType
                              FROM ImpuestoRenta
                              WHERE AND CAST(FechaInicio AS date) <= @dateFrom
                              AND (FechaFin IS NULL OR CAST(FechaFin AS date) >= @dateTo)";
        var ccssLines = await connection.QueryAsync<CCSSModel>(query);
        return ccssLines;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "GetCCSS failed.");
        throw;
      }
    }

    // WRITE METHODS

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
