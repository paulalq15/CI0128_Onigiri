using Dapper;
using Microsoft.Data.SqlClient;
using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Application.Reports;
using Planilla_Backend.CleanArchitecture.Domain.Reports;

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
            JOIN Contrato AS c ON p.IdPersona = c.IdContrato
            LEFT JOIN ComprobantePago AS cp ON cp.IdNominaEmpleado = ne.IdNominaEmpleado
          WHERE ne.IdNominaEmpleado = @payrollId;";

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
          WHERE dn.IdNominaEmpleado = @payrollId AND dn.Tipo <> 'Deduccion Empleador'
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
  }
}
