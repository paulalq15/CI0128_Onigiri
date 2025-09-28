using Dapper;
using Microsoft.Data.SqlClient;
using Planilla_Backend.Models;

namespace Planilla_Backend.Repositories
{
  public class PersonUserRepository
  {
    private readonly string _connectionString;
    public PersonUserRepository()
    {
      var builder = WebApplication.CreateBuilder();
      _connectionString = builder.Configuration.GetConnectionString("OnigiriContext");
    }

    public List<Person> getEmployees()
    {
      const string query = @"SELECT
                           IdPersona AS PersonID,
                           Cedula,
                           Nombre1 AS Name1,
                           Nombre2 AS Name2,
                           Apellido1 AS Surname1,
                           Apellido2 AS Surname2,
                           Telefono AS Phone,
                           FechaNacimiento AS BirthDate,
                           TipoPersona AS PersonType
                           FROM dbo.Persona;";

      using var connection = new SqlConnection(_connectionString);

      return connection.Query<Person>(query).ToList();
    }
  }
}
