using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Application.Reports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;
using Planilla_Backend.CleanArchitecture.Domain.Reports;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Planilla_Backend.CleanArchitecture.Infrastructure
{
  public class ReportRepository: IReportRepository
  {
    private readonly string _connectionString;
    private readonly ILogger<ReportRepository> _logger;
    public ReportRepository(IConfiguration config, ILogger<ReportRepository> logger)
    {
      _connectionString = config.GetConnectionString("OnigiriContext");
      _logger = logger;
    }

    public async Task<IEnumerable<ReportPayrollPeriodDto>> GetEmployeePayrollPeriodsAsync(int companyId, int employeeId, int top)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);
        const string query =
            @"SELECT TOP (@top)
              ne.IdNominaEmpleado AS PayrollId,
              nem.FechaInicio AS DateFrom,
              nem.FechaFin AS DateTo, 
              CONCAT(FORMAT(nem.FechaInicio, 'dd MMMM yyyy', 'es-ES'), ' - ', FORMAT(nem.FechaFin, 'dd MMMM yyyy', 'es-ES')) AS PeriodLabel 
            FROM NominaEmpleado AS ne
              JOIN NominaEmpresa AS nem ON ne.IdNominaEmpresa = nem.IdNominaEmpresa
            WHERE nem.IdEmpresa = @companyId
             AND ne.IdEmpleado = @employeeId
            ORDER BY nem.FechaInicio DESC;";

        var payrolls = await connection.QueryAsync<ReportPayrollPeriodDto>(query, new { companyId, employeeId, top });
        return payrolls;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "GetEmployeePayrollPeriodsAsync failed. companyId: {CompanyId}", companyId);
        throw;
      }
    }

    public async Task<IEnumerable<ReportPayrollPeriodDto>> GetEmployerPayrollPeriodsAsync(int companyId, int top)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);
        const string query =
          @"SELECT TOP(@top)
            IdNominaEmpresa AS PayrollId,
            FechaInicio AS DateFrom,
            FechaFin AS DateTo,
            FORMAT(FechaInicio, 'dd/MM/yyyy') + ' - ' + FORMAT(FechaFin, 'dd/MM/yyyy') AS PeriodLabel
          FROM NominaEmpresa
          WHERE IdEmpresa = @companyId
          ORDER BY FechaInicio DESC;";

        var payrolls = await connection.QueryAsync<ReportPayrollPeriodDto>(query, new { companyId, top });
        return payrolls;
      }

      catch (Exception ex)
      {
        _logger.LogError(ex, "GetEmployerPayrollPeriodsAsync failed. companyId: {CompanyId}", companyId);
        throw;
      }
    }

    public async Task<EmployeePayrollReport> GetEmployeePayrollReport(int payrollId, CancellationToken ct = default)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);
        const string headerQuery =
          @"SELECT 
            e.Nombre AS CompanyName,
            e.IdEmpresa AS CompanyId,
            CONCAT_WS(' ', p.Nombre1, NULLIF(p.Nombre2, ''), p.Apellido1, NULLIF(p.Apellido2, '')) AS EmployeeName,
            p.IdPersona AS EmployeeId,
            CASE c.Tipo
              WHEN 'Tiempo Completo' THEN 'FullTime'
              WHEN 'Medio Tiempo' THEN 'PartTime'
              WHEN 'Servicios Profesionales' THEN 'ProfessionalServices'
            END AS EmployeeType,
            cp.FechaPago AS PaymentDate,
            ne.MontoNeto AS NetAmount
          FROM NominaEmpleado AS ne
            JOIN NominaEmpresa AS nem ON ne.IdNominaEmpresa = nem.IdNominaEmpresa
            JOIN Empresa AS e ON nem.IdEmpresa = e.IdEmpresa
            JOIN Persona AS p ON ne.IdEmpleado = p.IdPersona
            JOIN Contrato AS c ON p.IdPersona = c.IdPersona
            LEFT JOIN ComprobantePago AS cp ON cp.IdNominaEmpleado = ne.IdNominaEmpleado
          WHERE ne.IdNominaEmpleado = @payrollId 
            AND c.FechaInicio <= nem.FechaFin
            AND (c.FechaFin IS NULL OR c.FechaFin >= nem.FechaFin);";

        const string detailsQuery =
          @"SELECT
            dn.Descripcion AS Description,
            CASE
              WHEN dn.Tipo = 'Salario' THEN 'Salario'
              WHEN dn.Tipo = 'Beneficio Empleado' THEN 'Beneficio'
              WHEN dn.Tipo = 'Deduccion Empleado' AND dn.IdElementoAplicado IS NOT NULL THEN 'Deduccion voluntaria'
              WHEN dn.Tipo = 'Deduccion Empleado' AND (dn.IdCCSS IS NOT NULL OR dn.IdImpuestoRenta IS NOT NULL) THEN 'Deduccion obligatoria'
              ELSE dn.Tipo
            END AS Category,
            CASE
              WHEN dn.Tipo = 'Deduccion Empleado'
                THEN -dn.Monto
              ELSE dn.Monto
            END AS Amount
          FROM DetalleNomina AS dn
          WHERE dn.IdNominaEmpleado = @payrollId AND dn.Tipo <> 'Deduccion Empleador' AND dn.Tipo <> 'Beneficio Empleado'
          ORDER BY dn.IdDetalleNomina;";

        var header = await connection.QuerySingleOrDefaultAsync<EmployeePayrollReport>(new CommandDefinition(headerQuery, new { payrollId }, cancellationToken: ct));
        if (header == null) throw new KeyNotFoundException("No se encontró información de la planilla seleccionada");
        var detailLines = await connection.QueryAsync<PayrollDetailLine>(new CommandDefinition(detailsQuery, new { payrollId }, cancellationToken: ct));

        header.Lines = detailLines.ToList();
        return header;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "GetEmployeePayrollReportAsync failed. PayrollId: {payrollId}", payrollId);
        throw;
      }
    }

    public async Task<EmployerPayrollReport> GetEmployerPayrollReport(int payrollId, int employerId, CancellationToken ct = default)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);

        const string headerQuery =
          @"SELECT
              CONCAT_WS(' ', p.Nombre1, NULLIF(p.Nombre2, ''), p.Apellido1, NULLIF(p.Apellido2, '')) AS EmployerName
            FROM Persona p
            WHERE p.IdPersona = @employerId;

            SELECT
              FORMAT(ne.FechaInicio, 'dd/MM/yyyy') AS PaymentDate
            FROM NominaEmpresa ne
            WHERE ne.IdNominaEmpresa = @payrollId;";

        const string detailsQuery =
          @"SELECT
            dn.Descripcion AS Description,
            dn.Tipo AS Category,
            SUM(dn.Monto) AS Amount
          FROM NominaEmpresa neEmp

          JOIN NominaEmpleado ne
            ON ne.IdNominaEmpresa = neEmp.IdNominaEmpresa
          JOIN DetalleNomina dn
            ON dn.IdNominaEmpleado = ne.IdNominaEmpleado
          JOIN Persona p
            ON p.IdPersona = ne.IdEmpleado
          JOIN Contrato c
            ON c.IdPersona = ne.IdEmpleado
          AND c.FechaInicio <= neEmp.FechaFin
          AND (c.FechaFin IS NULL OR c.FechaFin >= neEmp.FechaInicio)
          WHERE
            neEmp.IdNominaEmpresa = @payrollId
          AND dn.Tipo IN ('Salario', 'Deduccion Empleador', 'Beneficio Empleado')
          GROUP BY
            dn.Descripcion,
            dn.Tipo
          ORDER BY
            dn.Tipo,
            dn.Descripcion;";

        var header = await connection.QuerySingleOrDefaultAsync<EmployerPayrollReport>(new CommandDefinition(headerQuery, new { payrollId, employerId }, cancellationToken: ct));

        if (header == null) throw new KeyNotFoundException("No se encontró información de la planilla seleccionada");

        var detailLines = await connection.QueryAsync<PayrollDetailLine>(new CommandDefinition(detailsQuery, new { payrollId }, cancellationToken: ct));

        header.Lines = detailLines.ToList();

        return header;
      }

      catch (Exception ex)
      {
        _logger.LogError(ex, "GetEmployerPayrollReportAsync failed. PayrollId: {payrollId}", payrollId);
        throw;
      }
    }

    public async Task<IEnumerable<EmployerHistoryRow>> GetEmployerHistoryCompaniesAsync(int? companyId, int employeeId, DateTime dateFrom, DateTime dateTo, CancellationToken ct = default)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);
        const string query =
            @"SELECT 
              e.Nombre AS CompanyName,
              CASE e.FrecuenciaPago
                WHEN 'Quincenal' THEN 'Biweekly'
                WHEN 'Mensual' THEN 'Monthly'
              END AS PaymentFrequency,
              ne.FechaInicio AS DateFrom,
              ne.FechaFin AS DateTo,
              MAX(cp.FechaPago) AS PaymentDate,
              ne.MontoBruto AS GrossSalary,
              ne.DeduccionesEmpleador AS EmployerContributions,
              ne.Beneficios AS EmployeeBenefits,
              ne.Costo AS EmployerCost
            FROM NominaEmpresa AS ne
              JOIN Empresa AS e ON e.IdEmpresa = ne.IdEmpresa
              JOIN UsuariosPorEmpresa AS ue ON ue.IdEmpresa = e.IdEmpresa
              JOIN Usuario AS u ON u.IdUsuario = ue.IdUsuario
              LEFT JOIN NominaEmpleado AS nem ON nem.IdNominaEmpresa = ne.IdNominaEmpresa
              LEFT JOIN ComprobantePago AS cp ON cp.IdNominaEmpleado = nem.IdNominaEmpleado
            WHERE ne.FechaFin <= @dateTo
              AND ne.FechaInicio >= @dateFrom
              AND (@companyId IS NULL OR e.IdEmpresa = @companyId)
              AND u.IdPersona = @employeeId
            GROUP BY e.Nombre, e.FrecuenciaPago, ne.FechaInicio, ne.FechaFin, ne.MontoBruto, ne.DeduccionesEmpleador, ne.Beneficios, ne.Costo
            ORDER BY e.Nombre, ne.FechaInicio;";

        var payrolls = await connection.QueryAsync<EmployerHistoryRow>(query, new { companyId, employeeId, dateFrom, dateTo });
        return payrolls;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "GetEmployerHistoryCompaniesAsync failed. companyId: {CompanyId}", companyId);
        throw;
      }
    }

    public async Task<IEnumerable<EmployerReportByPersonRow>> GetEmployerPayrollByPersonAsync(int companyId, DateTime start, DateTime end, string? employeeType, string? nationalId, CancellationToken ct)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        const string storedProc = "dbo.sp_GetPayrollReport";

        var parameters = new
        {
          companyId,
          start,
          end,
          employeeType = string.IsNullOrWhiteSpace(employeeType) ? null : employeeType,
          nationalId = string.IsNullOrWhiteSpace(nationalId) ? null : nationalId
        };

        var reportRows = await connection.QueryAsync<EmployerReportByPersonRow>(storedProc, parameters, commandType: CommandType.StoredProcedure);

        return reportRows;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "GetEmployerPayrollByPersonAsync failed. companyId: {CompanyId}", companyId);
        throw;
      }
    }

    public async Task<EmployeePayrollHistoryReport> GetEmployeePayrollHistoryInDateRange(int employeeId, DateTime startPayrollDate, DateTime finalPayrollDate)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);

        const string headerQuery = @"
            Select Top 1
                e.Nombre AS CompanyName,
                e.IdEmpresa AS CompanyId,
                CONCAT_WS(' ', p.Nombre1, NULLIF(p.Nombre2, ''), p.Apellido1, NULLIF(p.Apellido2, '')) AS EmployeeName,
                p.IdPersona AS EmployeeId
            From Persona p
            Inner Join Usuario u On u.IdPersona = p.IdPersona
            Inner Join UsuariosPorEmpresa upe On upe.IdUsuario = u.IdUsuario
            Inner Join Empresa e On e.IdEmpresa = upe.IdEmpresa
            Where p.IdPersona = @employeeId;
        ";

        const string getEmpPayHistorySqlQuery = @"
            Select
              Case c.Tipo
                When 'Tiempo Completo' THEN 'FullTime'
                When 'Medio Tiempo' THEN 'PartTime'
                When 'Servicios Profesionales' THEN 'ProfessionalServices'
              End As ContractType,
              ne.PuestoEnMomento As Role,
              cp.FechaPago As PaymentDate,
              ne.MontoBruto As GrossSalary,
              ISNULL(SUM(
                CASE 
                  WHEN dn.Tipo = 'Deduccion Empleado' AND (dn.IdCCSS IS NOT NULL OR dn.IdImpuestoRenta IS NOT NULL) THEN dn.Monto
                  ELSE 0
                END), 0) AS LegalDeductions,
              ISNULL(SUM(
                CASE 
                  WHEN dn.Tipo = 'Deduccion Empleado' AND dn.IdElementoAplicado IS NOT NULL THEN dn.Monto
                  ELSE 0
                END), 0) AS VoluntaryDeductions,
              ne.MontoNeto As NetSalary
            From Persona p
            Inner Join NominaEmpleado ne On ne.IdEmpleado = p.IdPersona
            Inner Join ComprobantePago cp On cp.IdNominaEmpleado = ne.IdNominaEmpleado
            Inner Join NominaEmpresa nem On nem.IdNominaEmpresa = ne.IdNominaEmpresa
            Inner Join Contrato c On c.IdPersona = p.IdPersona
            Inner Join DetalleNomina AS dn ON dn.IdNominaEmpleado = ne.IdNominaEmpleado
            Where p.IdPersona = @employeeId
              And nem.FechaInicio >= @startPayrollDate
              And nem.FechaFin <= @finalPayrollDate
            Group By c.Tipo, ne.PuestoEnMomento, cp.FechaPago, ne.MontoBruto, ne.MontoNeto
            Order By PaymentDate;";

        var header = await connection.QuerySingleOrDefaultAsync<EmployeePayrollHistoryReport>(
            headerQuery,
            new { employeeId });

        if (header == null) return new EmployeePayrollHistoryReport();

        var payrollsList = await connection.QueryAsync<EmployeeHistoryRow>(getEmpPayHistorySqlQuery,
            new { employeeId, startPayrollDate, finalPayrollDate});

        header.Rows = payrollsList.ToList();

        return header;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex,
            "GetEmployeePayrollHistoryInDateRange failed. Id del empleado: " +employeeId +
            ", fecha inicio: " + startPayrollDate +
            ", fecha final: " + finalPayrollDate);

        return new EmployeePayrollHistoryReport();
      }
    }
  }
}
