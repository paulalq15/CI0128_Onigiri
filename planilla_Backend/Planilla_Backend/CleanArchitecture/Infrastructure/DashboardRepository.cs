using Dapper;
using Microsoft.Data.SqlClient;
using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Infrastructure
{
  public class DashboardRepository : IDashboardRepository
  {
    private readonly string _connectionString;
    private readonly ILogger<ReportRepository> _logger;

    public DashboardRepository(IConfiguration config, ILogger<ReportRepository> logger)
    {
      _connectionString = config.GetConnectionString("OnigiriContext");
      _logger = logger;
    }

    public async Task<List<EmployerDashboardCostByTypeModel>> GetEmployerDashboardCostByType(int companyId)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);
        const string query = @"
          select sum(MontoNeto) as Salaries, sum(DeduccionesEmpleador) as SocialCosts, sum(Beneficios) as Benefits
          from NominaEmpresa
          where IdEmpresa = @companyId;";
        
        var row = await connection.QueryFirstOrDefaultAsync<EmployerCostRow>(query, new { CompanyId = companyId });

        if (row == null) return new List<EmployerDashboardCostByTypeModel>();

        var result = new List<EmployerDashboardCostByTypeModel>
        {
          new EmployerDashboardCostByTypeModel
          {
            CostType = "Salarios",
            TotalCost = row.Salaries ?? 0
          },
          new EmployerDashboardCostByTypeModel
          {
            CostType = "Cargas Sociales",
            TotalCost = row.SocialCosts ?? 0
          },
          new EmployerDashboardCostByTypeModel
          {
            CostType = "Beneficios",
            TotalCost = row.Benefits ?? 0
          }
        };

        return result;

      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error obteniendo datos de costo de planilla: {CompanyId}", companyId);
        throw;
      }
    }

    public async Task<List<EmployerDashboardCostByMonthModel>> GetEmployerDashboardCostByMonth(int companyId)
    {
      try
      {
        var connection = new SqlConnection(_connectionString);
        const string query = @"
          select FORMAT(FechaInicio, 'MM-yyyy') as [Month], Costo as TotalCost
          from NominaEmpresa
          where IdEmpresa = @companyId
          order by [Month]";

        var rows = await connection.QueryAsync<EmployerDashboardCostByMonthModel>(query, new { CompanyId = companyId });
        var result = rows.ToList();
        return result;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error obteniendo datos de costo por mes: {CompanyId}", companyId);
        throw;
      }
    }

    public async Task<List<EmployerDashboardEmployeeCountByTypeModel>> GetEmployerDashboardEmployeeCountByType(int companyId)
    {
      try
      {
        var connection = new SqlConnection(_connectionString);
        const string query = @"
          select c.Tipo as EmployeeType, count(c.Tipo) as EmployeeCount
          from Contrato c
            join Usuario u on u.IdPersona = c.IdPersona
            join UsuariosPorEmpresa ue on ue.IdUsuario = u.IdUsuario
          where ue.IdEmpresa = @companyId
          group by Tipo";
        var rows = await connection.QueryAsync<EmployerDashboardEmployeeCountByTypeModel>(query, new { CompanyId = companyId });
        var result = rows.ToList();
        return result;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error obteniendo conteo de empleados por tipo: {CompanyId}", companyId);
        throw;
      }
    }

    public async Task<List<EmployeeDashboardEmployeeFiguresPerMonth>> GetEmployeeFiguresPerMonth(int employeeId)
    {
      try
      {
        var connection = new SqlConnection(_connectionString);

        const string query = @"
          SELECT TOP 1
            ne.MontoBruto AS GrossSalary,
            ne.MontoNeto AS NetSalary
          FROM NominaEmpleado ne
          WHERE IdEmpleado = @employeeId
          ORDER BY ne.IdNominaEmpresa DESC;";

        var rows = await connection.QueryAsync<EmployeeDashboardEmployeeFiguresPerMonth>(query, new { EmployeeId = employeeId });
        var result = rows.ToList();

        return result;
      }

      catch (Exception ex)
      {
        _logger.LogError(ex, "Error obteniendo conteo de empleados por tipo: {CompanyId}", employeeId);
        throw;
      }
    }
  }
}
