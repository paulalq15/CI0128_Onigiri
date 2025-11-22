using Dapper;
using Microsoft.Data.SqlClient;
using Planilla_Backend.CleanArchitecture.Application.Ports;

namespace Planilla_Backend.CleanArchitecture.Infrastructure
{
  public class CompanyRepositoryCA : ICompanyRepository
  {
    private readonly string _connectionString;
    private readonly ILogger<CompanyRepositoryCA> _logger;

    public CompanyRepositoryCA(IConfiguration config, ILogger<CompanyRepositoryCA> logger)
    {
      _connectionString = config.GetConnectionString("OnigiriContext");
      _logger = logger;
    }

    public async Task<int> IsPersonAdmin(int employeerPersonId)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);

        const string query = @"
            SELECT 
              CASE 
                WHEN EXISTS (
                  SELECT 1
                  FROM Persona p
                  WHERE p.IdPersona = @idPersona
                    AND p.TipoPersona = 'Administrador'
                )
              THEN 1 ELSE 0 END
        ";

        int isAdmin = await connection.QuerySingleAsync<int>(query, new { idPersona = employeerPersonId });

        return isAdmin;
      }
      catch (Exception ex)
      {
        _logger.LogError("Error al verificar si la persona es administrador: \n" + ex.Message);
        return -1;
      }
    }

    public async Task<int> IsPersonOwnerOfCompany(int companyUniqueId, int employeerPersonId)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);

        const string query = @"
          SELECT 
            CASE 
              WHEN EXISTS (
                SELECT 1
                FROM Empresa e
                WHERE e.IdEmpresa = @companyUniqueId
                  AND e.IdCreadoPor = @idPersona
              )
            THEN 1 ELSE 0 END
        ";

        int isOwner = await connection.QuerySingleAsync<int>(query, new { companyUniqueId = companyUniqueId, idPersona = employeerPersonId });

        return isOwner;
      }
      catch (Exception ex)
      {
        _logger.LogError("Error al verificar si la persona es dueña de la empresa: \n" + ex.Message);
        return -1;
      }
    }

    public async Task<int> DeleteCompanyByUniqueId(int companyUniqueId)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);
        
        // No se elimina directamente, la base de datos debe manejar con un trigger
        const string query = @"
          DELETE FROM Empresa
          WHERE IdEmpresa = @companyUniqueId
        ";

        int rowsAffected = await connection.ExecuteAsync(query, new { companyUniqueId = companyUniqueId });

        return rowsAffected;
      }
      catch (Exception ex)
      {
        _logger.LogError("Error al eliminar la empresa: \n" + ex.Message);
        return -1;
      }
    }
}
