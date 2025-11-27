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

    public async Task<EmployeeModel?> GetByEmployeeId(int personId)
    {
      try
      {
        using var con = new SqlConnection(_connectionString);

        // 1) Persona
        const string qPersona = @"SELECT Cedula, Nombre1 AS Name1, Nombre2 AS Name2, Apellido1 AS Surname1, Apellido2 AS Surname2, Telefono AS Phone, FechaNacimiento AS BirthDate, TipoPersona AS PersonType
                                  FROM Persona
                                  WHERE IdPersona = @PersonId;";

        var person = await con.QueryFirstOrDefaultAsync(qPersona, new { PersonId = personId });

        if (person is null) return null;

        // 2) Contrato
        const string qContract = @"SELECT c.Puesto AS Position, c.Departamento AS Department, c.Salario AS Salary, c.Tipo AS EmployeeType, c.FechaInicio AS StartDate, c.FechaFin AS EndDate, c.CuentaPago AS BankAccount
                                   FROM Contrato c WHERE c.IdPersona = @PersonId";

        var contract = await con.QueryFirstOrDefaultAsync(qContract, new { PersonId = personId });

        // 3) Dirección
        const string qAddress = @"SELECT t.Provincia AS Province, t.Canton, t.Distrito AS District, t.CodigoPostal AS ZipCode, d.OtrasSenas AS OtherSigns
                                  FROM Direccion d
                                  JOIN DivisionTerritorialCR t ON t.IdDivision = d.IdDivision
                                  WHERE d.IdPersona = @PersonId;";

        var addr = await con.QueryFirstOrDefaultAsync(qAddress, new { PersonId = personId });

        // 4) IdEmpresa
        const string qCompanyId = @"SELECT upe.IdEmpresa
                                    FROM UsuariosPorEmpresa upe
                                    JOIN Usuario u ON u.IdUsuario = upe.IdUsuario
                                    WHERE u.IdPersona = @PersonId;";
        var companyId = await con.QueryFirstOrDefaultAsync<int>(qCompanyId, new { PersonId = personId });

        var model = new EmployeeModel
        {
          PersonData = new Planilla_Backend.LayeredArchitecture.Models.Person
          {
            Cedula = person.Cedula,
            Name1 = person.Name1,
            Name2 = person.Name2,
            Surname1 = person.Surname1,
            Surname2 = person.Surname2,
            Phone = person.Phone,
            BirthDate = person.BirthDate,
            PersonType = person.PersonType
          },
          ContractData = contract is null ? null : new Planilla_Backend.LayeredArchitecture.Models.Contract
          {
            Position = contract.Position,
            Department = contract.Department,
            Salary = contract.Salary,
            EmployeeType = contract.EmployeeType,
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
            BankAccount = contract.BankAccount
          },
          Direction = addr is null ? null : new Planilla_Backend.LayeredArchitecture.Models.DirectionsModel
          {
            Province = addr.Province,
            Canton = addr.Canton,
            District = addr.District,
            ZipCode = addr.ZipCode,
            OtherSigns = addr.OtherSigns,
          },
          CompanyUniqueId = companyId
        };
        return model;
      }
      catch (Exception ex)
      {
        Console.WriteLine("Error al obtner la información de Empleado. Detalle: \n" + ex.Message);
        return null;
      }
    }

    public async Task<bool> UpdateByEmployee(int personId, EmployeeModel employeeModel, CancellationToken ct = default)
    {
      await using var con = new SqlConnection(_connectionString);
      await con.OpenAsync(ct);
      await using var tx = await con.BeginTransactionAsync(ct);

      try
      {
        var person = employeeModel.PersonData;
        var dir = employeeModel.Direction;
        var contract = employeeModel.ContractData;

        // 1) Persona
        const string upPersona = @" UPDATE Persona SET
                                      Nombre1 = COALESCE(@Name1, Nombre1),
                                      Nombre2 = COALESCE(@Name2, Nombre2),
                                      Apellido1 = COALESCE(@Surname1, Apellido1),
                                      Apellido2 = COALESCE(@Surname2, Apellido2),
                                      Telefono = COALESCE(@Phone, Telefono)
                                    WHERE IdPersona = @PersonId;";

        await con.ExecuteAsync(upPersona, new
        {
          Name1 = person?.Name1,
          Name2 = person?.Name2,
          Surname1 = person?.Surname1,
          Surname2 = person?.Surname2,
          Phone = person?.Phone,
          PersonId = personId
        }, tx);

        // 2) Dirección
        if (dir is not null && (!string.IsNullOrWhiteSpace(dir.ZipCode) || !string.IsNullOrWhiteSpace(dir.OtherSigns)))
        {
          int? divisionId = null;
          if (!string.IsNullOrWhiteSpace(dir.ZipCode))
          {
            divisionId = await con.QuerySingleOrDefaultAsync<int?>("SELECT dbo.fn_GetDivisionIdByPostal(@ZipCode);", new { ZipCode = dir.ZipCode }, tx);
            if (divisionId is null)
              throw new InvalidOperationException($"El código postal '{dir.ZipCode}' no existe en DivisionTerritorialCR.");
          }

          const string upsertDireccion = @"UPDATE Direccion SET
                                            IdDivision = COALESCE(@DivisionId, IdDivision),
                                            OtrasSenas = COALESCE(@OtherSigns, OtrasSenas)
                                          WHERE IdPersona = @PersonId;";

          await con.ExecuteAsync(upsertDireccion, new
          {
            PersonId = personId,
            DivisionId = divisionId,
            OtherSigns = dir?.OtherSigns
          }, tx);
        }

        // 3) Contrato
        if (!string.IsNullOrWhiteSpace(contract?.BankAccount))
        {
          const string upContratoCuenta = @"UPDATE Contrato SET CuentaPago = @BankAccount
                                            WHERE IdPersona = @PersonId;";

          await con.ExecuteAsync(upContratoCuenta, new
          {
            BankAccount = contract!.BankAccount,
            PersonId = personId
          }, tx);
        }

        await tx.CommitAsync(ct);
        return true;
      }
      catch
      {
        await tx.RollbackAsync(ct);
        throw;
      }
    }

    public async Task<bool> UpdateByEmployer(int employerId, int personId, EmployeeModel employeeModel, CancellationToken ct = default)
    {
      await using var con = new SqlConnection(_connectionString);
      await con.OpenAsync(ct);
      await using var tx = await con.BeginTransactionAsync(ct);

      try
      {
        var person = employeeModel.PersonData;
        var dir = employeeModel.Direction;
        var contract = employeeModel.ContractData;

        // 1) Persona
        const string upPersona = @"UPDATE Persona SET
                                    Nombre1 = COALESCE(@Name1, Nombre1),
                                    Nombre2 = COALESCE(@Name2, Nombre2),
                                    Apellido1 = COALESCE(@Surname1,Apellido1),
                                    Apellido2 = COALESCE(@Surname2,Apellido2),
                                    Telefono = COALESCE(@Phone, Telefono)
                                   WHERE IdPersona = @PersonId;";

        await con.ExecuteAsync(upPersona, new
        {
          Name1 = person?.Name1,
          Name2 = person?.Name2,
          Surname1 = person?.Surname1,
          Surname2 = person?.Surname2,
          Phone = person?.Phone,
          PersonId = personId
        }, tx);

        // 2) Dirección
        if (dir is not null && (!string.IsNullOrWhiteSpace(dir.ZipCode) || !string.IsNullOrWhiteSpace(dir.OtherSigns)))
        {
          int? divisionId = null;
          if (!string.IsNullOrWhiteSpace(dir.ZipCode))
          {
            divisionId = await con.QuerySingleOrDefaultAsync<int?>("SELECT dbo.fn_GetDivisionIdByPostal(@ZipCode);", new { ZipCode = dir.ZipCode }, tx);
            if (divisionId is null)
              throw new InvalidOperationException($"El código postal '{dir.ZipCode}' no existe en DivisionTerritorialCR.");
          }

          const string upsertDireccion = @"UPDATE Direccion SET
                                            IdDivision = COALESCE(@DivisionId, IdDivision),
                                            OtrasSenas = COALESCE(@OtherSigns, OtrasSenas)
                                          WHERE IdPersona = @PersonId;";

          await con.ExecuteAsync(upsertDireccion, new
          {
            PersonId = personId,
            DivisionId = divisionId,
            OtherSigns = dir?.OtherSigns
          }, tx);
        }

        // 3) Contrato
        if (contract is not null)
        {
          if (contract.Salary < 0) throw new ArgumentOutOfRangeException(nameof(contract.Salary), "Salary must be >= 0.");

          await con.ExecuteAsync("EXEC sys.sp_set_session_context @key=N'IdModificador', @value=@val;", new { val = employerId }, transaction: tx);

          const string upContrato = @"UPDATE Contrato SET
                                        Puesto = COALESCE(@Position, Puesto),
                                        Departamento = COALESCE(@Department, Departamento),
                                        Salario = COALESCE(@Salary, Salario),
                                        CuentaPago = COALESCE(@BankAccount, CuentaPago)
                                      FROM Contrato
                                      WHERE IdPersona = @PersonId;";

          await con.ExecuteAsync(upContrato, new
          {
            Position = string.IsNullOrWhiteSpace(contract.Position) ? null : contract.Position,
            Department = string.IsNullOrWhiteSpace(contract.Department) ? null : contract.Department,
            Salary = contract.Salary == 0 ? (decimal?)null : contract.Salary,
            BankAccount = string.IsNullOrWhiteSpace(contract.BankAccount) ? null : contract.BankAccount,
            PersonId = personId
          }, tx);
        }

        await tx.CommitAsync(ct);
        return true;
      }
      catch
      {
        await tx.RollbackAsync(ct);
        throw;
      }
    }

    public async Task<bool> CheckIfEmployeeHasPayments(int userId)
    {
      using var connection = new SqlConnection(_connectionString);
      await connection.OpenAsync();

      const string query = @"
        SELECT CASE WHEN EXISTS(
            SELECT 1 FROM NominaEmpleado WHERE IdEmpleado = @userId
        ) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END";

       return await connection.QuerySingleAsync<bool>(query, new { userId });
    }

    public async Task SoftDeleteEmployee(int userId)
    {
      using var connection = new SqlConnection(_connectionString);
      await connection.OpenAsync();

      using var transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

      try
      {
        const string query = @"
          UPDATE Usuario
          SET IsDeleted = 1
          WHERE IdUsuario = @userId;

          UPDATE p
          SET p.IsDeleted = 1
          FROM Persona p
          INNER JOIN Usuario u ON u.IdPersona = p.IdPersona
          WHERE u.IdUsuario = @userId;

          UPDATE ElementoAplicado
          SET FechaFin = GETDATE()
          WHERE IdUsuario = @userId;

          UPDATE c
          SET c.FechaFin = GETDATE()
          FROM Contrato c
          INNER JOIN Persona p ON c.IdPersona = p.IdPersona
          INNER JOIN Usuario u ON u.IdPersona = p.IdPersona
          WHERE u.IdUsuario = @userId;
          ";

        await connection.ExecuteAsync(query, new { userId }, transaction);
        await transaction.CommitAsync();
      }

      catch
      {
        await transaction.RollbackAsync();
        throw;
      }
    }

    public async Task HardDeleteEmployee(int userId)
    {
      using var connection = new SqlConnection(_connectionString);
      await connection.OpenAsync();
      using var transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

      try
      {
        var idPersona = await connection.QuerySingleAsync<int>(
        "SELECT IdPersona FROM Usuario WHERE IdUsuario = @userId", new { userId }, transaction);

         await connection.ExecuteAsync("DELETE FROM ElementoAplicado WHERE IdUsuario = @userId", new { userId }, transaction);
         await connection.ExecuteAsync("DELETE FROM UsuariosPorEmpresa WHERE IdUsuario = @userId", new { userId }, transaction);
         await connection.ExecuteAsync("DELETE FROM HojaHoras WHERE IdEmpleado = @idPersona", new { idPersona }, transaction);
         await connection.ExecuteAsync("DELETE FROM HistorialLaboral WHERE IdEmpleado = @idPersona", new { idPersona }, transaction);
         await connection.ExecuteAsync("DELETE FROM Contrato WHERE IdPersona = @idPersona", new { idPersona }, transaction);
         await connection.ExecuteAsync("DELETE FROM Direccion WHERE IdPersona = @idPersona", new { idPersona }, transaction);
         await connection.ExecuteAsync("DELETE FROM Usuario WHERE IdUsuario = @userId", new { userId }, transaction);
         await connection.ExecuteAsync("DELETE FROM Persona WHERE IdPersona = @idPersona", new { idPersona }, transaction);

         await transaction.CommitAsync();
      }

      catch
      {
        await transaction.RollbackAsync();
        throw;
      }
    }
  }
}
