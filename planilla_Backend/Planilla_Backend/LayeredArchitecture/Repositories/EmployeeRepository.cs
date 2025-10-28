using Dapper;
using Microsoft.Data.SqlClient;
using Planilla_Backend.LayeredArchitecture.Models;

namespace Planilla_Backend.LayeredArchitecture.Repositories
{
  public class EmployeeRepository
  {
    private readonly string _connectionString;
    public EmployeeRepository(IConfiguration config)
    {
      _connectionString = config.GetConnectionString("OnigiriContext");
    }

    // Método para guardar una nueva Persona, su Usuario y su Contrato en la base de datos
    public int saveEmployee(EmployeeModel employee)
    {
      using var connection = new SqlConnection(_connectionString);
      connection.Open();

      // Usamos una transacción para asegurarar que si falla la inserción de una tabla, no se inserte la otra
      using var transaction = connection.BeginTransaction();

      try
      {
        // Insertar Persona
        const string insertPersona = @"INSERT INTO Persona
            (Cedula, Nombre1, Nombre2, Apellido1, Apellido2, Telefono, FechaNacimiento, TipoPersona)
            VALUES
            (@IdCard, @Name1, @Name2, @Surname1, @Surname2, @Phone, @BirthDate, @TypePerson);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        int idPerson = connection.QuerySingle<int>(insertPersona, new
        {
          IdCard = employee.PersonData.Cedula,
          Name1 = employee.PersonData.Name1,
          Name2 = employee.PersonData.Name2,
          Surname1 = employee.PersonData.Surname1,
          Surname2 = employee.PersonData.Surname2,
          Phone = employee.PersonData.Phone,
          BirthDate = employee.PersonData.BirthDate,
          TypePerson = employee.PersonData.PersonType,
        }, transaction);

        // Insertar Usuario
        const string insertUser = @"INSERT INTO Usuario
            (Correo, Contrasena, Estado, IdPersona)
            VALUES
            (@Email, @Password, @Status, @IdPerson);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        int idUser = connection.QuerySingle<int>(insertUser, new
        {
          Email = employee.UserData.Email,
          Password = employee.UserData.Password,
          Status = employee.UserData.Status,
          IdPerson = idPerson,
        }, transaction);

        // Insertar UsuarioXEmpresa
        const string insertUserCompany = @"INSERT INTO UsuariosPorEmpresa (IdEmpresa, IdUsuario) VALUES (@CompanyUniqueId, @UserId);";

        connection.Execute(insertUserCompany, new
        {
          CompanyUniqueId = employee.CompanyUniqueId,
          UserId = idUser,
        }, transaction);

        // Insertar Contrato
        const string insertContract = @"INSERT INTO Contrato (Puesto, Departamento, Salario, Tipo, FechaInicio, FechaFin, CuentaPago, IdPersona)
          VALUES (@Position, @Department, @Salary, @EmployeeType, @StartDate, NULL, @BankAccount, @PersonID);";

        connection.Execute(insertContract, new
        {
          Position = employee.ContractData.Position,
          Department = employee.ContractData.Department,
          Salary = employee.ContractData.Salary,
          EmployeeType = employee.ContractData.EmployeeType,
          StartDate = employee.ContractData.StartDate,
          BankAccount = employee.ContractData.BankAccount,
          PersonID = idPerson,
        }, transaction);

        // Insertar Dirección
        const string insertAddress = @"INSERT INTO Direccion (IdDivision, OtrasSenas, IdPersona)
          VALUES (1,NULL, @PersonID)";

        connection.Execute(insertAddress, new
        {
          PersonID = idPerson,
        }, transaction);

        transaction.Commit();
        return idPerson;
      }
      catch (Exception ex)
      {
        transaction.Rollback();
        Console.WriteLine(ex.ToString());
        throw new Exception("Error al registrar empleado: " + ex.Message);
      }
    }



  }
}