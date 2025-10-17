using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;

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
          @"SELECT IdEmpresa AS Id, DiaPago1 AS PayDay1, DiaPago2 AS PayDay2,
              CASE FrecuenciaPago
                WHEN 'Quincenal' THEN 'Biweekly'
                WHEN 'Mensual' THEN 'Monthly'
              END AS PaymentFrequency
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

    public async Task<IEnumerable<EmployeeModel>> GetEmployees(int companyId, DateTime dateFrom, DateTime dateTo)
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

    public async Task<IEnumerable<ContractModel>> GetContracts(int companyId, DateTime dateFrom, DateTime dateTo)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);
        const string query =
          @"SELECT c.IdContrato AS Id, c.IdPersona AS EmployeeId, c.FechaInicio AS StartDate, c.FechaFin AS EndDate, c.Salario AS Salary, c.CuentaPago AS PaymentAccount,
              CASE c.Tipo
                WHEN 'Tiempo Completo' THEN 'FixedSalary'
                WHEN 'Medio Tiempo' THEN 'FixedSalary'
                WHEN 'Servicios Profesionales' THEN 'ProfessionalServices'
              END AS ContractType
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

    public async Task<IEnumerable<ElementModel>> GetElementsForEmployee(int companyId, int employeeId, DateTime dateFrom, DateTime dateTo)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);
        const string query =
          @"SELECT ea.IdElementoAplicado AS Id, e.Nombre AS Name, e.Valor AS Value, p.IdPersona AS EmployeeId,
              CASE 
                WHEN e.Tipo = 'Monto' THEN 'FixedAmount'
                WHEN e.Tipo = 'Porcentaje' THEN 'Percentage'
                WHEN e.Tipo = 'API' THEN 'ExternalAPI'
              END AS CalculationType,
              CASE 
                WHEN e.PagadoPor = 'Empleador' THEN 'Benefit'
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

    public async Task<IDictionary<int, decimal>> GetEmployeeTimesheets(int companyId, DateTime dateFrom, DateTime dateTo)
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

    public async Task<IEnumerable<TaxModel>> GetTaxes(DateTime dateFrom, DateTime dateTo)
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

    public async Task<IEnumerable<CCSSModel>> GetCCSS(DateTime dateFrom, DateTime dateTo)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);
        const string query = 
          @"SELECT IdCCSS AS Id, Categoria AS Category, Concepto AS Concept, Porcentaje AS Rate,
              CASE 
                WHEN PagadoPor = 'Empleado' THEN 'EmployeeDeduction'
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

    public async Task<CompanyPayrollModel?> GetLatestOpenCompanyPayroll(int companyId)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);
        const string query =
          @"SELECT TOP 1 IdNominaEmpresa AS Id, IdEmpresa AS CompanyId, FechaInicio AS DateFrom,
              FechaFin AS DateTo, Estado AS PayrollStatus, MontoBruto AS Gross,
              DeduccionesEmpleado AS EmployeeDeductions, DeduccionesEmpleador AS EmployerDeductions,
              Beneficios AS Benefits, MontoNeto AS Net, Costo AS Cost, CreadoPor AS CreatedBy
            FROM NominaEmpresa
            WHERE Estado = 'Creado' AND IdEmpresa = @companyId
            ORDER BY FechaCreacion DESC";

        var companyPayroll = await connection.QuerySingleOrDefaultAsync<CompanyPayrollModel>(query, new { companyId });
        return companyPayroll;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "GetLatestOpenCompanyPayroll failed. companyId: {CompanyId}", companyId);
        throw;
      }
    }

    public async Task<CompanyPayrollModel?> GetCompanyPayrollById(int companyPayrollId)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);
        const string query =
          @"SELECT IdNominaEmpresa AS Id, IdEmpresa AS CompanyId, FechaInicio AS DateFrom,
              FechaFin AS DateTo, Estado AS PayrollStatus, MontoBruto AS Gross,
              DeduccionesEmpleado AS EmployeeDeductions, DeduccionesEmpleador AS EmployerDeductions,
              Beneficios AS Benefits, MontoNeto AS Net, Costo AS Cost, CreadoPor AS CreatedBy
            FROM NominaEmpresa
            WHERE IdNominaEmpresa = @companyPayrollId";

        var companyPayroll = await connection.QuerySingleOrDefaultAsync<CompanyPayrollModel>(query, new { companyPayrollId });
        return companyPayroll;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "GetCompanyPayrollById failed. companyPayrollId: {CompanyPayrollId}", companyPayrollId);
        throw;
      }
    }

    public async Task<IEnumerable<EmployeePayrollModel>> GetEmployeePayrolls(int companyPayrollId)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);
        const string query =
          @"SELECT IdNominaEmpleado AS Id, IdNominaEmpresa AS CompanyPayrollId, IdEmpleado AS EmployeeId,
              MontoBruto AS Gross, DeduccionesEmpleado AS EmployeeDeductions, DeduccionesEmpleador AS EmployerDeductions,
              Beneficios AS Benefits, MontoNeto AS Net, Costo AS Cost
            FROM NominaEmpleado
            WHERE IdNominaEmpresa = @companyPayrollId";

        var employeePayrolls = await connection.QueryAsync<EmployeePayrollModel>(query, new { companyPayrollId });
        return employeePayrolls;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "GetEmployeePayrolls failed. companyPayrollId: {CompanyPayrollId}", companyPayrollId);
        throw;
      }
    }

    // WRITE METHODS

    public async Task<int> SaveCompanyPayroll(CompanyPayrollModel header)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);
        const string sql =
          @"INSERT INTO NominaEmpresa(FechaInicio, FechaFin, MontoBruto, MontoNeto, DeduccionesEmpleado, DeduccionesEmpleador, Beneficios, CreadoPor, IdEmpresa, Costo)
            VALUES (@DateFrom, @DateTo, @Gross, @Net, @EmployeeDeductions, @EmployerDeductions, @Benefits, @CreatedBy, @CompanyId, @Cost);
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
          header.Cost,
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
    public async Task<int> SaveEmployeePayroll(EmployeePayrollModel employeePayroll)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);
        const string sql =
          @"INSERT INTO NominaEmpleado(IdNominaEmpresa, IdEmpleado, MontoBruto, MontoNeto, DeduccionesEmpleado, DeduccionesEmpleador, Beneficios, Costo)
            VALUES (@CompanyPayrollId, @EmployeeId, @Gross, @Net, @EmployeeDeductions, @EmployerDeductions, @Benefits, @Cost);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        var id = await connection.ExecuteScalarAsync<int>(sql, new
        {
          employeePayroll.CompanyPayrollId,
          employeePayroll.EmployeeId,
          employeePayroll.Gross,
          employeePayroll.EmployeeDeductions,
          employeePayroll.EmployerDeductions,
          employeePayroll.Benefits,
          employeePayroll.Net,
          employeePayroll.Cost,
        });

        return id;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "SaveEmployeePayroll failed");
        throw;
      }
    }
    public async Task SavePayrollDetails(int employeePayrollId, IEnumerable<PayrollDetailModel> details)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);
        const string sql =
          @"INSERT INTO DetalleNomina(IdNominaEmpleado, IdCCSS, IdImpuestoRenta, IdElementoAplicado, Descripcion, Monto, Tipo)
            VALUES (@EmployeePayrollId, @IdCCSS, @IdTax, @IdElement, @Description, @Amount,
              CASE @Type
                WHEN 'Base' THEN 'Salario'
                WHEN 'Benefit' THEN 'Beneficio Empleado'
                WHEN 'EmployeeDeduction' THEN 'Deduccion Empleado'
                WHEN 'EmployerContribution' THEN 'Deduccion Empleador'
                WHEN 'Tax' THEN 'Deduccion Empleado'
              END 
            );";

        var args = new List<object>();
        foreach (var line in details)
        {
          args.Add(new
          {
            EmployeePayrollId = employeePayrollId,
            line.Description,
            Type = line.Type.ToString(),
            line.Amount,
            line.IdCCSS,
            line.IdTax,
            line.IdElement
          });
        }

        if (args.Count > 0)
        {
          await connection.ExecuteAsync(sql, args);
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "SavePayrollDetails failed for EmployeePayrollId {EmployeePayrollId}", employeePayrollId);
        throw;
      }
    }
    public Task UpdateEmployeePayrollTotals(int employeePayrollId, EmployeePayrollModel totalsAndStatus)
    {
      return Task.CompletedTask;
    }
    public Task UpdateCompanyPayrollTotals(int companyPayrollId, CompanyPayrollModel totalsAndStatus)
    {
      return Task.CompletedTask;
    }
    public async Task SavePayment(int employeePayrollId, PaymentModel payment)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);
        const string sql =
          @"INSERT INTO ComprobantePago(Referencia, FechaPago, Monto, IdNominaEmpleado, IdCreadoPor)
            VALUES (@PaymentRef, @PaymentDate, @Amount, @EmployeePayrollId, @CreatedBy);";

        await connection.ExecuteAsync(sql, new
        {
          EmployeePayrollId = employeePayrollId,
          payment.PaymentRef,
          payment.PaymentDate,
          payment.Amount,
          payment.CreatedBy,
        });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "SavePayment failed");
        throw;
      }
    }

    public async Task UpdatePaidCompanyPayroll(int companyPayrollId)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);
        const string sql =
          @"UPDATE NominaEmpresa
            SET Estado = 'Pagado'
            WHERE IdNominaEmpresa = @CompanyPayrollId;";

        await connection.ExecuteAsync(sql, new
        {
          CompanyPayrollId = companyPayrollId,
        });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "UpdatePaidCompanyPayroll failed");
        throw;
      }
    }

  }
}
