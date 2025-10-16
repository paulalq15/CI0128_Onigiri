using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;
using System.ComponentModel.Design;

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
        const string query =
          @"SELECT IdEmpresa AS Id, FrecuenciaPago AS PaymentFrequency, DiaPago1 AS PayDay1, DiaPago2 AS PayDay2
            FROM Empresa
            WHERE IdEmpresa = @companyId;";

        var company = await connection.QuerySingleOrDefaultAsync<CompanyModel>(query, new { companyId });
        return company ?? new CompanyModel();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "GetCompany failed. companyId: {CompanyId}", companyId);
        throw;
      }      
    }

    public async Task<IEnumerable<EmployeeModel>> GetEmployees(int companyId, DateOnly dateFrom, DateOnly dateTo)
    {
      try {
        using var connection = new SqlConnection(_connectionString);
        const string query =
          @"SELECT p.IdPersona AS Id, p.TipoPersona AS PersonType
            FROM Persona AS p
            JOIN Usuario AS u ON p.IdPersona = u.IdPersona
            JOIN UsuariosPorEmpresa AS ue ON u.IdUsuario = ue.IdUsuario
            JOIN Empresa AS e ON ue.IdEmpresa = e.IdEmpresa
            WHERE e.IdEmpresa = @companyId
            AND p.TipoPersona IN ('Empleado', 'Aprobador')
            AND p.IdPersona IN (
	            SELECT h.IdEmpleado
	            FROM HojaHoras AS h
	            WHERE h.Fecha >= @dateFrom AND h.Fecha <= @dateTo
	            GROUP BY h.IdEmpleado
	            HAVING SUM(CAST(h.Horas AS decimal(10,2))) > 0)
            AND p.IdPersona IN (
	            SELECT c.IdPersona
	            FROM Contrato AS c
	            WHERE c.FechaInicio <= @dateTo
	            AND (c.FechaFin IS NULL OR c.FechaFin >= @dateFrom));";

        var employees = await connection.QueryAsync<EmployeeModel>(query, new { companyId, dateFrom, dateTo });
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
        const string query =
          @"SELECT c.IdContrato AS Id, c.IdPersona AS EmployeeId, c.Tipo AS ContractType, c.FechaInicio AS StartDate, c.FechaFin AS EndDate, c.Salario AS Salary, c.CuentaPago AS PaymentAccount
            FROM Contrato AS c
            JOIN Persona AS p ON c.IdPersona = p.IdPersona
            JOIN Usuario AS u ON p.IdPersona = u.IdPersona
            JOIN UsuariosPorEmpresa AS ue ON u.IdUsuario = ue.IdUsuario
            JOIN Empresa AS e ON ue.IdEmpresa = e.IdEmpresa
            WHERE e.IdEmpresa = @companyId
            AND c.FechaInicio <= @dateTo
            AND (c.FechaFin IS NULL OR c.FechaFin >= @dateFrom)
            AND p.IdPersona IN (
	            SELECT h.IdEmpleado
	            FROM HojaHoras AS h
	            WHERE h.Fecha >= @dateFrom AND h.Fecha <= @dateTo
	            GROUP BY h.IdEmpleado
	            HAVING SUM(CAST(h.Horas AS decimal(10,2))) > 0);";

        var contracts = await connection.QueryAsync<ContractModel>(query, new { companyId, dateFrom, dateTo });
        return contracts;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "GetContracts failed. companyId: {CompanyId}", companyId);
        throw;
      }
    }

    public async Task<IEnumerable<ElementModel>> GetElementsForEmployee(int companyId, int employeeId, DateOnly dateFrom, DateOnly dateTo)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);
        const string query =
          @"SELECT ea.IdElementoAplicado AS Id, e.Nombre AS Name, e.Valor AS Value, p.IdPersona AS EmployeeId,
            CASE WHEN e.Tipo = 'Monto' THEN 'FixedAmount'
            WHEN e.Tipo = 'Porcentaje' THEN 'Percentage'
            WHEN e.Tipo = 'API' THEN 'ExternalAPI'
            END AS CalculationType,
            CASE WHEN e.PagadoPor = 'Empleador' THEN 'Benefit'
            WHEN e.PagadoPor = 'Empleado' THEN 'EmployeeDeduction'
            END AS ItemType
            FROM ElementoAplicado AS ea
            JOIN ElementoPlanilla AS e ON ea.IdElemento = e.IdElemento
            JOIN Usuario AS u ON ea.IdUsuario = u.IdUsuario
            JOIN Persona AS p ON u.IdPersona = p.IdPersona
            WHERE e.IdEmpresa = @companyId
            AND p.IdPersona = @employeeId
            AND ea.FechaInicio <= @dateTo
            AND (ea.FechaFin IS NULL OR ea.FechaFin >= @dateFrom);";

        var elements = await connection.QueryAsync<ElementModel>(query, new { companyId, employeeId, dateFrom, dateTo });
        return elements;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "GetElementsForEmployee failed. companyId: {CompanyId}", companyId);
        throw;
      }
    }

    public async Task<IDictionary<int, decimal>> GetEmployeeTimesheets(int companyId, DateOnly dateFrom, DateOnly dateTo)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);

        const string query = 
          @"SELECT h.IdEmpleado AS EmployeeId, SUM(CAST(h.Horas AS decimal(10,2))) AS TotalHours
            FROM HojaHoras AS h
            JOIN Persona AS p ON p.IdPersona = h.IdEmpleado
            JOIN Usuario AS u ON u.IdPersona = p.IdPersona
            JOIN UsuariosPorEmpresa AS ue ON ue.IdUsuario = u.IdUsuario
            JOIN Empresa AS e ON ue.IdEmpresa = e.IdEmpresa
            WHERE e.IdEmpresa = @companyId AND h.Fecha >= @dateFrom AND h.Fecha <= @dateTo
            GROUP BY h.IdEmpleado;";

        var rows = await connection.QueryAsync<(int EmployeeId, decimal TotalHours)>(
          new CommandDefinition(query, new { companyId, dateFrom, dateTo }));

        var dict = new Dictionary<int, decimal>();
        foreach (var r in rows)
          dict[r.EmployeeId] = r.TotalHours;

        return dict;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "GetEmployeeTimesheets failed. companyId: {CompanyId}", companyId);
        throw;
      }
    }

    public async Task<IEnumerable<TaxModel>> GetTaxes(DateOnly dateFrom, DateOnly dateTo)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);
        const string query = 
          @"SELECT IdImpuestoRenta AS Id, MontoMinimo AS Min, MontoMaximo AS Max, Porcentaje AS Rate, 'Tax' AS ItemType
            FROM ImpuestoRenta
            WHERE FechaInicio <= @dateFrom
            AND (FechaFin IS NULL OR FechaFin >= @dateTo)";

        var taxBrackets = await connection.QueryAsync<TaxModel>(query, new {dateFrom, dateTo });
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
        const string query = 
          @"SELECT IdCCSS AS Id, Categoria AS Category, Concepto AS Concept, Porcentaje AS Rate, 
              CASE WHEN PagadoPor = 'Empleado' THEN 'EmployeeDeduction'
                    WHEN PagadoPor = 'Empleador' THEN 'EmployerContribution'
              END AS ItemType
              FROM CCSS
              WHERE FechaInicio <= @dateFrom
              AND (FechaFin IS NULL OR FechaFin >= @dateTo)";

        var ccssLines = await connection.QueryAsync<CCSSModel>(query, new { dateFrom, dateTo });
        return ccssLines;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "GetCCSS failed.");
        throw;
      }
    }

    // WRITE METHODS

    public async Task<int> SaveCompanyPayroll(CompanyPayrollModel header, int personId)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);
        const string sql =
          @"INSERT INTO CompanyPayroll(FechaInicio, FechaFin, FechaCreacion, MontoBruto, MontoNeto, DeduccionesEmpleado, DeduccionesEmpleador, Beneficios, CreadoPor, IdEmpresa)
            VALUES (@DateFrom, @DateTo, SYSUTCDATETIME(), @Gross, @Net, @EmployeeDeductions, @EmployerDeductions, @Benefits, @personId, @CompanyId);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        var id = await connection.ExecuteScalarAsync<int>(sql, new
        {
          header.CompanyId,
          header.DateFrom,
          header.DateTo,
          header.Gross,
          header.EmployeeDeductions,
          header.EmployerDeductions,
          header.Benefits,
          header.Net,
          header.CreatedBy,
        });

        return id;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "SaveCompanyPayroll failed");
        throw;
      }
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
    public Task SavePayment(int employeePayrollId, PaymentModel payment, int personId)
    {
      return Task.CompletedTask;
    }

  }
}
