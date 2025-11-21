using Dapper;
using Microsoft.Data.SqlClient;
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

    public async Task<CompanyModel> GetCompany(int companyId, SqlConnection? conn = null, SqlTransaction? tx = null)
    {
      var connection = conn ?? new SqlConnection(_connectionString);
      var shouldClose = conn == null;
      if (shouldClose) await connection.OpenAsync();
      try
      {
        const string query =
          @"SELECT IdEmpresa AS Id, DiaPago1 AS PayDay1, DiaPago2 AS PayDay2, CedulaJuridica AS LegalID,
              CASE FrecuenciaPago
                WHEN 'Quincenal' THEN 'Biweekly'
                WHEN 'Mensual' THEN 'Monthly'
              END AS PaymentFrequency
            FROM Empresa
            WHERE IdEmpresa = @companyId;";

        var company = await connection.QuerySingleOrDefaultAsync<CompanyModel>(query, new { companyId }, transaction: tx);
        if (company == null) throw new KeyNotFoundException("La empresa no existe.");

        return company;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "GetCompany failed. companyId: {CompanyId}", companyId);
        throw;
      }
      finally
      {
        if (shouldClose)
          await connection.CloseAsync();
      }
    }

    public async Task<IEnumerable<EmployeeModel>> GetEmployees(int companyId, DateTime dateFrom, DateTime dateTo, SqlConnection? conn = null, SqlTransaction? tx = null)
    {
      var connection = conn ?? new SqlConnection(_connectionString);
      var shouldClose = conn == null;
      if (shouldClose) await connection.OpenAsync();
      try
      {
        const string query =
          @"SELECT p.IdPersona AS Id, e.IdEmpresa AS CompanyId, p.TipoPersona AS PersonType, 
            CASE 
              WHEN DATEADD(YEAR, DATEDIFF(YEAR, FechaNacimiento, GETDATE()), FechaNacimiento) > GETDATE() THEN DATEDIFF(YEAR, FechaNacimiento, GETDATE()) - 1
              ELSE DATEDIFF(YEAR, FechaNacimiento, GETDATE())
          END AS Age
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
              AND EXISTS (
                SELECT 1
                FROM dbo.fn_GetPayrollTimesheets(@companyId, @dateFrom, @dateTo) AS fn
                WHERE fn.EmployeeId = p.IdPersona
              );";

        var employees = await connection.QueryAsync<EmployeeModel>(query, new { companyId, dateFrom, dateTo }, transaction: tx);
        return employees;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "GetEmployees failed. companyId: {CompanyId}", companyId);
        throw;
      }
      finally
      {
        if (shouldClose)
          await connection.CloseAsync();
      }
    }

    public async Task<IEnumerable<ContractModel>> GetContracts(int companyId, DateTime dateFrom, DateTime dateTo, SqlConnection? conn = null, SqlTransaction? tx = null)
    {
      var connection = conn ?? new SqlConnection(_connectionString);
      var shouldClose = conn == null;
      if (shouldClose) await connection.OpenAsync();
      try
      {
        const string query =
          @"SELECT c.IdContrato AS Id, c.IdPersona AS EmployeeId, c.FechaInicio AS StartDate, c.FechaFin AS EndDate, c.Salario AS Salary, c.CuentaPago AS PaymentAccount, c.Puesto as Role,
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
            AND EXISTS (
              SELECT 1
              FROM dbo.fn_GetPayrollTimesheets(@companyId, @dateFrom, @dateTo) AS fn
              WHERE fn.EmployeeId = c.IdPersona
            );";

        var contracts = await connection.QueryAsync<ContractModel>(query, new { companyId, dateFrom, dateTo }, transaction: tx);
        return contracts;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "GetContracts failed. companyId: {CompanyId}", companyId);
        throw;
      }
      finally
      {
        if (shouldClose)
          await connection.CloseAsync();
      }
    }

    public async Task<IEnumerable<ElementModel>> GetElementsForEmployee(int companyId, int employeeId, DateTime dateFrom, DateTime dateTo, SqlConnection? conn = null, SqlTransaction? tx = null)
    {
      var connection = conn ?? new SqlConnection(_connectionString);
      var shouldClose = conn == null;
      if (shouldClose) await connection.OpenAsync();
      try
      {
        const string query =
          @"SELECT ea.IdElementoAplicado AS Id, e.Nombre AS Name, e.Valor AS Value, p.IdPersona AS EmployeeId, ea.TipoPlan AS PensionType, ea.CantidadDependientes AS NumberOfDependents,
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

        var elements = await connection.QueryAsync<ElementModel>(query, new { companyId, employeeId, dateFrom, dateTo }, transaction: tx);
        return elements;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "GetElementsForEmployee failed. companyId: {CompanyId}", companyId);
        throw;
      }
      finally
      {
        if (shouldClose)
          await connection.CloseAsync();
      }
    }

    public async Task<IDictionary<int, decimal>> GetEmployeeTimesheets(int companyId, DateTime dateFrom, DateTime dateTo, SqlConnection? conn = null, SqlTransaction? tx = null)
    {
      var connection = conn ?? new SqlConnection(_connectionString);
      var shouldClose = conn == null;
      if (shouldClose) await connection.OpenAsync();
      try
      {
        const string query =
          @"SELECT EmployeeId, TotalHours
            FROM dbo.fn_GetPayrollTimesheets(@companyId, @dateFrom, @dateTo);";

        var rows = await connection.QueryAsync<(int EmployeeId, decimal TotalHours)>(
          new CommandDefinition(query, new { companyId, dateFrom, dateTo }, transaction: tx));

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
      finally
      {
        if (shouldClose)
          await connection.CloseAsync();
      }
    }

    public async Task<IEnumerable<TaxModel>> GetTaxes(DateTime dateFrom, DateTime dateTo, SqlConnection? conn = null, SqlTransaction? tx = null)
    {
      var connection = conn ?? new SqlConnection(_connectionString);
      var shouldClose = conn == null;
      if (shouldClose) await connection.OpenAsync();
      try
      {
        const string query =
          @"SELECT IdImpuestoRenta AS Id, MontoMinimo AS Min, MontoMaximo AS Max, Porcentaje AS Rate, 'Tax' AS ItemType
            FROM ImpuestoRenta
            WHERE FechaInicio <= @dateFrom
              AND (FechaFin IS NULL OR FechaFin >= @dateTo)";

        var taxBrackets = await connection.QueryAsync<TaxModel>(query, new { dateFrom, dateTo }, transaction: tx);
        return taxBrackets;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "GetTaxes failed.");
        throw;
      }
      finally
      {
        if (shouldClose)
          await connection.CloseAsync();
      }
    }

    public async Task<IEnumerable<CCSSModel>> GetCCSS(DateTime dateFrom, DateTime dateTo, SqlConnection? conn = null, SqlTransaction? tx = null)
    {
      var connection = conn ?? new SqlConnection(_connectionString);
      var shouldClose = conn == null;
      if (shouldClose) await connection.OpenAsync();
      try
      {
        const string query =
          @"SELECT IdCCSS AS Id, Categoria AS Category, Concepto AS Concept, Porcentaje AS Rate,
              CASE 
                WHEN PagadoPor = 'Empleado' THEN 'EmployeeDeduction'
                WHEN PagadoPor = 'Empleador' THEN 'EmployerContribution'
              END AS ItemType
            FROM CCSS
            WHERE FechaInicio <= @dateFrom
              AND (FechaFin IS NULL OR FechaFin >= @dateTo)";

        var ccssLines = await connection.QueryAsync<CCSSModel>(query, new { dateFrom, dateTo }, transaction: tx);
        return ccssLines;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "GetCCSS failed.");
        throw;
      }
      finally
      {
        if (shouldClose)
          await connection.CloseAsync();
      }
    }

    public async Task<bool> ExistsPayrollForPeriod(int companyId, DateTime dateFrom, DateTime dateTo, SqlConnection? conn = null, SqlTransaction? tx = null)
    {
      var connection = conn ?? new SqlConnection(_connectionString);
      var shouldClose = conn == null;
      if (shouldClose) await connection.OpenAsync();
      try
      { 
        const string sql =
          @"SELECT COUNT(1)
            FROM NominaEmpresa
            WHERE IdEmpresa = @companyId AND FechaInicio = @dateFrom AND FechaFin = @dateTo";
        var count = await connection.ExecuteScalarAsync<int>(sql, new { companyId, dateFrom, dateTo }, transaction: tx);
        return count > 0;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "ExistsPayrollForPeriod failed");
        throw;
      }
      finally
      {
        if (shouldClose)
          await connection.CloseAsync();
      }
    }

    // WRITE METHODS

    public async Task<int> SaveCompanyPayroll(CompanyPayrollModel header, SqlConnection? conn = null, SqlTransaction? tx = null)
    {
      var connection = conn ?? new SqlConnection(_connectionString);
      var shouldClose = conn == null;
      if (shouldClose) await connection.OpenAsync();
      try
      {
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
        }, transaction: tx);

        return id;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "SaveCompanyPayroll failed");
        throw;
      }
      finally
      {
        if (shouldClose)
          await connection.CloseAsync();
      }
    }
    public async Task<int> SaveEmployeePayroll(EmployeePayrollModel employeePayroll, SqlConnection? conn = null, SqlTransaction? tx = null)
    {
      var connection = conn ?? new SqlConnection(_connectionString);
      var shouldClose = conn == null;
      if (shouldClose) await connection.OpenAsync();
      try
      {
        const string sql =
          @"INSERT INTO NominaEmpleado(IdNominaEmpresa, IdEmpleado, PuestoEnMomento, MontoBruto, MontoNeto, DeduccionesEmpleado, DeduccionesEmpleador, Beneficios, Costo)
            VALUES (@CompanyPayrollId, @EmployeeId, @EmployeeRole, @Gross, @Net, @EmployeeDeductions, @EmployerDeductions, @Benefits, @Cost);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        var id = await connection.ExecuteScalarAsync<int>(sql, new
        {
          employeePayroll.CompanyPayrollId,
          employeePayroll.EmployeeId,
          employeePayroll.EmployeeRole,
          employeePayroll.Gross,
          employeePayroll.EmployeeDeductions,
          employeePayroll.EmployerDeductions,
          employeePayroll.Benefits,
          employeePayroll.Net,
          employeePayroll.Cost,
        }, transaction: tx);

        return id;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "SaveEmployeePayroll failed");
        throw;
      }
      finally
      {
        if (shouldClose)
          await connection.CloseAsync();
      }
    }
    public async Task SavePayrollDetails(int employeePayrollId, IEnumerable<PayrollDetailModel> details, SqlConnection? conn = null, SqlTransaction? tx = null)
    {
      var connection = conn ?? new SqlConnection(_connectionString);
      var shouldClose = conn == null;
      if (shouldClose) await connection.OpenAsync();
      try
      {
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

        var args = details.Select(line => new
        {
          EmployeePayrollId = employeePayrollId,
          line.Description,
          Type = line.Type.ToString(),
          line.Amount,
          line.IdCCSS,
          line.IdTax,
          line.IdElement
        }).ToList();

        if (args.Count > 0)
          await connection.ExecuteAsync(sql, args, transaction: tx);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "SavePayrollDetails failed for EmployeePayrollId {EmployeePayrollId}", employeePayrollId);
        throw;
      }
      finally
      {
        if (shouldClose)
          await connection.CloseAsync();
      }
    }
    public async Task UpdateEmployeePayrollTotals(int employeePayrollId, EmployeePayrollModel totalsAndStatus, SqlConnection? conn = null, SqlTransaction? tx = null)
    {
      var connection = conn ?? new SqlConnection(_connectionString);
      var shouldClose = conn == null;
      if (shouldClose) await connection.OpenAsync();
      try
      {
        const string sql = @" UPDATE NominaEmpleado
                              SET
                                MontoBruto = @Gross,
                                DeduccionesEmpleado = @EmployeeDeductions,
                                DeduccionesEmpleador = @EmployerDeductions,
                                Beneficios = @Benefits,
                                MontoNeto = @Net,
                                Costo = @Cost
                              WHERE IdNominaEmpleado = @employeePayrollId;";
        var affected = await connection.ExecuteAsync(sql, new
        {
          employeePayrollId,
          totalsAndStatus.Gross,
          totalsAndStatus.EmployeeDeductions,
          totalsAndStatus.EmployerDeductions,
          totalsAndStatus.Benefits,
          totalsAndStatus.Net,
          totalsAndStatus.Cost,
          totalsAndStatus.BaseSalaryForPeriod
        }, transaction: tx);
        if (affected != 1)
          throw new InvalidOperationException($"No se pudo actualizar la Nomina de Empleado Id={employeePayrollId} (registros afectados: {affected}).");
      }
      finally
      {
        if (shouldClose) await connection.CloseAsync();
      }
    }
    public async Task UpdateCompanyPayrollTotals(int companyPayrollId, CompanyPayrollModel totalsAndStatus, SqlConnection? conn = null, SqlTransaction? tx = null)
    {
      var connection = conn ?? new SqlConnection(_connectionString);
      var shouldClose = conn == null;
      if (shouldClose) await connection.OpenAsync();
      try
      {        
        const string sql = @" UPDATE NominaEmpresa
                              SET
                                MontoBruto = @Gross,
                                DeduccionesEmpleado = @EmployeeDeductions,
                                DeduccionesEmpleador = @EmployerDeductions,
                                Beneficios = @Benefits,
                                MontoNeto = @Net,
                                Costo = @Cost,
                                Estado = 'Pagado'
                              WHERE IdNominaEmpresa = @CompanyPayrollId;";
        var affected = await connection.ExecuteAsync(sql, new
        {
          companyPayrollId,
          totalsAndStatus.Gross,
          totalsAndStatus.EmployeeDeductions,
          totalsAndStatus.EmployerDeductions,
          totalsAndStatus.Benefits,
          totalsAndStatus.Net,
          totalsAndStatus.Cost
        }, transaction: tx);
        if (affected != 1)
          throw new InvalidOperationException($"No se pudo actualizar la Nomina de Empleado Id={companyPayrollId} (registros afectados: {affected}).");
      }
      finally
      {
        if (shouldClose) await connection.CloseAsync();
      }
    }
    public async Task SavePayment(int employeePayrollId, PaymentModel payment, SqlConnection? conn = null, SqlTransaction? tx = null)
    {
      var connection = conn ?? new SqlConnection(_connectionString);
      var shouldClose = conn == null;
      if (shouldClose) await connection.OpenAsync();
      try
      {
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
        },transaction: tx);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "SavePayment failed");
        throw;
      }
      finally
      {
        if (shouldClose)
          await connection.CloseAsync();
      }
    }
  }
}
