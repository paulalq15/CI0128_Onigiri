using Dapper;
using Microsoft.Data.SqlClient;
using Planilla_Backend.Models;

namespace Planilla_Backend.Repositories
{
  public class PersonaUsuarioRepository
  {
    private readonly string _connectionString;
    public PersonaUsuarioRepository()
    {
      var builder = WebApplication.CreateBuilder();
      _connectionString = builder.Configuration.GetConnectionString("OnigiriContext");
    }

    // Método para guardar una nueva Persona y su Usuario en la base de datos
    public int savePersonaAndUsuario(PersonaUsuario persona, string password)
    {
      using var connection = new SqlConnection(_connectionString);
      connection.Open();

      // Usamos una transacción para asegurarar que si falla la inserción de una tabla, no se inserte la otra
      using var transaction = connection.BeginTransaction();

      try
      {
        // Insertar Persona
        var sqlPersona = @"INSERT INTO Persona
            (Cedula, Nombre1, Nombre2, Apellido1, Apellido2, Telefono, FechaNacimiento, TipoPersona)
            VALUES
            (@Cedula, @Nombre1, @Nombre2, @Apellido1, @Apellido2, @Telefono, @FechaNacimiento, @TipoPersona);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        var idPersona = connection.QuerySingle<int>(sqlPersona, persona, transaction);
        // Insertar Usuario
        var sqlUsuario = @"INSERT INTO Usuario
            (Correo, Contrasena, Estado, IdPersona)
            VALUES
            (@Correo, @password, @Estado, @IdPersona);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        var idUsuario = connection.QuerySingle<int>(sqlUsuario, new { persona.Correo, password, persona.Estado, IdPersona = idPersona }, transaction);

        transaction.Commit();
        return idUsuario;
      }
      catch (Exception ex)
      {
        // Si falla, revertimos la transacción y retornamos un error
        transaction.Rollback();
        Console.WriteLine(ex.ToString());
        throw new Exception("Error al guardar Persona y Usuario: " + ex.Message);
      }
    }

    // Método para obtener una Persona&Usuario por correo y contraseña. ? permite retornar null
    public PersonaUsuario? GetPersonaUsuarioByCredentials(string correo, string password)
    {
      using var connection = new SqlConnection(_connectionString);

      PersonaUsuario? resultPersona = null;

      try
      {
        // Obtener Usuario y Persona asociada
        var sql = "SELECT u.IdUsuario, u.Correo, u.Estado, u.IdPersona, " +
              "p.* " +
              "FROM Usuario u " +
              "INNER JOIN Persona p ON u.IdPersona = p.IdPersona " +
              "WHERE u.Correo = @Correo AND u.Contrasena = @Password";


        resultPersona = connection.QueryFirstOrDefault<PersonaUsuario>(sql, new { Correo = correo, Password = password });
      }
      catch (Exception ex) {
        throw new Exception("Error al obtener usuario" + ex.Message);
      }

      return resultPersona;
    }

    // Método para obtener el IdPersona por correo
    public int getUserIdByEmail(string email)
    {
      using var connection = new SqlConnection(_connectionString);

      int resultUserdID = 0;

      try
      {
        // Obtener Usuario
        var sql = @"SELECT u.IdUsuario
              FROM Usuario u
              WHERE u.Correo = @Correo";

        resultUserdID = connection.QueryFirstOrDefault<int>(sql, new { Correo = email });
      }
      catch (Exception ex)
      {
        throw new Exception("Error al obtener usuario" + ex.Message);
      }

      // Si no existe el correo, devuelve 0
      return resultUserdID;
    }
  }
}