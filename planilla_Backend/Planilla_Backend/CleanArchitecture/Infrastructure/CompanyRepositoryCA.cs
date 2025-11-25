using Dapper;
using Microsoft.Data.SqlClient;
using Planilla_Backend.CleanArchitecture.Application.EmailsDTOs;
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
                Inner Join Usuario u On u.IdPersona = @idPersona
                WHERE e.IdEmpresa = @companyUniqueId
                  AND e.IdCreadoPor = u.IdUsuario
              )
            THEN 1 ELSE 0 END
        ";

        int isOwner = await connection.QuerySingleAsync<int>(query, new { idPersona = employeerPersonId, companyUniqueId = companyUniqueId });

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

        const string query = "EXEC Sproc_delete_empresa @companyUniqueId";

        int rowsAffected = await connection.ExecuteAsync(query, new { companyUniqueId });

        return rowsAffected;
      }
      catch (Exception ex)
      {
        _logger.LogError("Error al eliminar la empresa: \n" + ex.Message);
        return -1;
      }
    }

    public async Task<List<DeleteCompanyEmployeeDataDTO>> GetEmployeesEmailsAndUserNameInCómpanyByIdCompany(int companyUniqueId)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);

        const string query = @"
            Select 
              CONCAT(p.Nombre1, ' ', p.Apellido1) As UserName,
              u.Correo As EmployeeEmail,
              e.Nombre AS CompanyName
            From Empresa e
            Inner Join UsuariosPorEmpresa upm On upm.IdEmpresa = e.IdEmpresa
            Inner Join Usuario u On u.IdUsuario = upm.IdUsuario
            Inner Join Persona p On p.IdPersona = u.IdPersona
            Where e.IdEmpresa = @companyUniqueId
              And e.IdCreadoPor <> u.IdUsuario;
        ";

        var employeesData = await connection.QueryAsync<DeleteCompanyEmployeeDataDTO>(query, new { companyUniqueId });

        return employeesData.ToList();
      }
      catch (Exception ex)
      {
        _logger.LogError("Error al obtener los correos electrónicos: \n" + ex.Message);
        return new List<DeleteCompanyEmployeeDataDTO>();
      }
    }
  }
}
