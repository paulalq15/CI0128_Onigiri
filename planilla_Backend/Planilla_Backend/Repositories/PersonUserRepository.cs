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

    // Método para guardar una nueva Persona y su Usuario en la base de datos
    public int savePersonAndUser(PersonUser person, string password)
    {
      using var connection = new SqlConnection(_connectionString);
      connection.Open();

      // Usamos una transacción para asegurarar que si falla la inserción de una tabla, no se inserte la otra
      using var transaction = connection.BeginTransaction();

      try
      {
        // Insertar Persona
        var sqlPerson = @"INSERT INTO Persona
            (Cedula, Nombre1, Nombre2, Apellido1, Apellido2, Telefono, FechaNacimiento, TipoPersona)
            VALUES
            (@IdCard, @Name1, @Name2, @Surname1, @Surname2, @Number, @BirthdayDate, @TypePerson);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        var idPerson = connection.QuerySingle<int>(sqlPerson, person, transaction);
        // Insertar Usuario
        var sqlUser = @"INSERT INTO Usuario
            (Correo, Contrasena, Estado, IdPersona)
            VALUES
            (@Email, @Password, @Status, @IdPerson);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        var idUser = connection.QuerySingle<int>(sqlUser, new { person.Email, Password = password, person.Status, IdPerson = idPerson }, transaction);

        transaction.Commit();
        return idPerson;
      }
      catch (Exception ex)
      {
        transaction.Rollback();
        Console.WriteLine(ex.ToString());
        throw new Exception("Error al guardar Persona y Usuario: " + ex.Message);
      }
    }

    // Método para obtener una Persona&Usuario por correo y contraseña. ? permite retornar null
    public PersonUser? getPersonUserByCredentials(string email, string password)
    {
      using var connection = new SqlConnection(_connectionString);

      PersonUser? resultPerson = null;

      try
      {
        // Obtener Usuario y Persona asociada
        var sql = "SELECT u.IdUsuario, u.Correo, u.Estado, u.IdPersona, " +
              "p.* " +
              "FROM Usuario u " +
              "INNER JOIN Persona p ON u.IdPersona = p.IdPersona " +
              "WHERE u.Correo = @Email AND u.Contrasena = @Password";


        resultPerson = connection.QueryFirstOrDefault<PersonUser>(sql, new { Email = email, Password = password });
      }
      catch (Exception ex) {
        throw new Exception("Error al obtener usuario" + ex.Message);
      }

      return resultPerson;
    }

    public int getUserIdByEmail(string email)
    {
      using var connection = new SqlConnection(_connectionString);

      int resultUserdID = 0;

      try
      {
        // Obtener idUsuario
        var sql = @"SELECT u.IdUsuario
              FROM Usuario u
              WHERE u.Correo = @Email";

        resultUserdID = connection.QueryFirstOrDefault<int>(sql, new { Email = email });
      }
      catch (Exception ex)
      {
        throw new Exception("Error al obtener usuario" + ex.Message);
      }

      // Si no existe el correo, devuelve 0
      return resultUserdID;
    }

    public List<PersonUser> getEmployeesByCompanyId(int companyId)
    {
            const string query = @"
              SELECT
                p.Cedula AS IdCard,
                p.Nombre1 AS Name1,
                p.Nombre2 AS Name2,
                p.Apellido1 AS Surname1,
                p.Apellido2 AS Surname2,
                p.FechaNacimiento AS BirthDate,
                u.Correo AS Email,
                c.Tipo AS ContractType,
                c.Puesto AS JobPosition,
                c.Departamento AS Department
              FROM UsuariosPorEmpresa upe
                JOIN Usuario u ON upe.IdUsuario = u.IdUsuario
                JOIN Persona p ON u.IdPersona = p.IdPersona
                JOIN Contrato c ON p.IdPersona = c.IdPersona
              WHERE upe.IdEmpresa = @companyId;
             ";

      using var connection = new SqlConnection(_connectionString);
      return connection.Query<PersonUser>(query, new { companyId }).ToList();
    }
  }
}
