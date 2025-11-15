using Dapper;
using Microsoft.Data.SqlClient;
using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;
using Planilla_Backend.CleanArchitecture.Domain.Reports;

namespace Planilla_Backend.CleanArchitecture.Infrastructure
{
  public class ReportRepository: IReportRepository
  {
    private readonly string _connectionString;
    private readonly ILogger<PayrollRepository> _logger;
    public ReportRepository(IConfiguration config, ILogger<PayrollRepository> logger)
    {
      _connectionString = config.GetConnectionString("OnigiriContext");
      _logger = logger;
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
            cp.FechaPago AS PaymentDate
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
            dn.Tipo AS Category,
            dn.Monto AS Amount
          FROM DetalleNomina AS dn
          WHERE dn.IdNominaEmpleado = @payrollId
          ORDER BY dn.IdDetalleNomina;";

        var header = await connection.QuerySingleOrDefaultAsync<EmployeePayrollReport>(new CommandDefinition(headerQuery, new { payrollId }, cancellationToken: ct));
        if (header == null) throw new KeyNotFoundException("No se encontró información de la planilla seleccionada");
        var detailLines = await connection.QueryAsync<PayrollDetailLine>(new CommandDefinition(detailsQuery, new { payrollId }, cancellationToken: ct));

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
