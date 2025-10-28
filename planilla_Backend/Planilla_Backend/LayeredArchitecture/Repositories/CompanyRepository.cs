using Dapper;
using Microsoft.Data.SqlClient;
using Planilla_Backend.LayeredArchitecture.Models;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SqlClient;

namespace Planilla_Backend.LayeredArchitecture.Repositories
{
  public class CompanyRepository
  {
    private readonly string _connectionString;
    public CompanyRepository(IConfiguration config)
    {
      _connectionString = config.GetConnectionString("OnigiriContext");
    }

    public bool ZipExists(string zipCode)
    {
      using var connection = new SqlConnection(_connectionString);
      const string sql = @"SELECT 1 FROM dbo.DivisionTerritorialCR WHERE CodigoPostal = @Zip";
      var r = connection.ExecuteScalar<int?>(sql, new { Zip = zipCode });
      return r.HasValue;
    }

    public bool ValidateUniqueCompanyNationalId(string cedula)
    {
      using var connection = new SqlConnection(_connectionString);
      const string sql = @"SELECT 1 FROM dbo.Empresa WHERE CedulaJuridica = @Ced";
      var r = connection.ExecuteScalar<int?>(sql, new { Ced = cedula });
      return r.HasValue;
    }

    public string? GetUserType(int userId)
    {
      using var connection = new SqlConnection(_connectionString);
      const string sql = @"SELECT p.TipoPersona
                          FROM dbo.Usuario u
                          INNER JOIN dbo.Persona p ON p.IdPersona = u.IdPersona
                          WHERE u.IdUsuario = @Id AND u.Estado = 'Activo'";
      return connection.ExecuteScalar<string?>(sql, new { Id = userId });
    }

    public int CreateCompany(CompanyModel company)
    {
      using var connection = new SqlConnection(_connectionString);
      connection.Open();
      using (var transaction = connection.BeginTransaction())
      {
        try
        {
          // Validate ZipCode and get IdDivision
          const string searchDivision =
            @"SELECT TOP (1) IdDivision FROM dbo.DivisionTerritorialCR WHERE CodigoPostal = @ZipCode;";
          int? idDivision = connection.ExecuteScalar<int?>(searchDivision, new
          {
            ZipCode = company.ZipCode
          }, transaction);
          if (idDivision is null)
          {
            throw new InvalidOperationException("El código postal no existe");
          }

          // Create company and get new Id
          const string insertCompany =
            @"INSERT INTO [dbo].[Empresa] ([CedulaJuridica], [Nombre], [Telefono], [CantidadBeneficios], [FrecuenciaPago], [DiaPago1], [DiaPago2], [IdCreadoPor])
            OUTPUT INSERTED.IdEmpresa
            VALUES (@CompanyId, @CompanyName, @Telephone, @MaxBenefits, @PaymentFrequency, @PayDay1, @PayDay2, @CreatedBy)";

          int id = connection.ExecuteScalar<int>(insertCompany, new
          {
            CompanyId = company.CompanyId,
            CompanyName = company.CompanyName,
            Telephone = company.Telephone,
            MaxBenefits = company.MaxBenefits,
            PaymentFrequency = company.PaymentFrequency,
            PayDay1 = company.PayDay1,
            PayDay2 = company.PayDay2,
            CreatedBy = company.CreatedBy,
          }, transaction);

          // Create address with the new company id and IdDivision
          const string insertAddress =
            @"INSERT INTO dbo.Direccion (IdDivision, OtrasSenas, IdEmpresa)
            VALUES (@IdDivision, @AddressDetails, @CompanyId)";
          connection.Execute(insertAddress, new
          {
            IdDivision = idDivision.Value,
            AddressDetails = company.AddressDetails,
            CompanyId = id
          }, transaction);

          // Link user with new company
          const string insertUserCompany =
            @"INSERT INTO [dbo].[UsuariosPorEmpresa] ([IdEmpresa], [IdUsuario])
            VALUES (@CompanyId, @UserId)";
          connection.Execute(insertUserCompany, new
          {
            CompanyId = id,
            UserId = company.CreatedBy,
          }, transaction);

          transaction.Commit();
          return id;
        }
        catch
        {
          transaction.Rollback();
          throw;
        }
      }
    }

    public IEnumerable<CompanySummaryModel> GetCompaniesByUser(int userId, bool onlyActive = true)
    {
      using var connection = new SqlConnection(_connectionString);
      var sql = @"
        SELECT e.IdEmpresa       AS CompanyUniqueId,
               e.CedulaJuridica  AS CompanyId,
               e.Nombre          AS CompanyName
        FROM dbo.UsuariosPorEmpresa ue
        INNER JOIN dbo.Empresa e ON e.IdEmpresa = ue.IdEmpresa
        WHERE ue.IdUsuario = @UserId
          " + (onlyActive ? "AND e.Estado = 'Activo'" : "") + @"
        ORDER BY e.Nombre;
      ";
      return connection.Query<CompanySummaryModel>(sql, new { UserId = userId });
    }

    public List<CompanyModel> GetCompaniesWithStats(int employerId, int viewerUserId)
    {
      using var connection = new SqlConnection(_connectionString);

      var viewerType = GetUserType(viewerUserId);
      bool isAdmin = string.Equals(viewerType, "Administrador", StringComparison.OrdinalIgnoreCase);

      const string sql = @"
        SELECT
          e.IdEmpresa AS CompanyUniqueId,
          e.CedulaJuridica AS CompanyId,
          e.Nombre AS CompanyName,
          e.Telefono AS Telephone,
          e.CantidadBeneficios AS MaxBenefits,
          e.FrecuenciaPago AS PaymentFrequency,
          e.DiaPago1 AS PayDay1,
          e.DiaPago2 AS PayDay2,
          e.IdCreadoPor AS CreatedBy,

          (SELECT COUNT(*) FROM dbo.UsuariosPorEmpresa upe WHERE upe.IdEmpresa = e.IdEmpresa) AS EmployeeCount,

          CASE WHEN @IsAdmin = 1 THEN
            (
              SELECT TOP 1
              CONCAT(p.Nombre1, ' ', ISNULL(p.Nombre2, ''), ' ', p.Apellido1, ' ', p.Apellido2)
              FROM dbo.Usuario u
              INNER JOIN dbo.Persona p ON p.IdPersona = u.IdPersona
              WHERE u.IdUsuario = e.IdCreadoPor
            )
          ELSE NULL END AS EmployerName

        FROM dbo.Empresa e
        WHERE (@IsAdmin = 1) OR (e.IdCreadoPor = @EmployerId)
        ORDER BY e.Nombre;";

      return connection.Query<CompanyModel>(sql, new { EmployerId = employerId, IsAdmin = isAdmin ? 1 : 0 }).ToList();
    }

    public List<CompanyModel> getCompanies(int employerId)
    {
      const string query = @"
        SELECT
          IdEmpresa AS CompanyUniqueId,
          CedulaJuridica AS CompanyId,
          Nombre AS CompanyName,
          Telefono AS Telephone,
          CantidadBeneficios AS MaxBenefits,
          FrecuenciaPago AS PaymentFrequency,
          DiaPago1 AS PayDay1,
          DiaPago2 AS PayDay2
        FROM dbo.Empresa
        WHERE IdCreadoPor = @EmployerId;";

      using var connection = new SqlConnection(_connectionString);

      return connection.Query<CompanyModel>(query, new { EmployerId = employerId }).ToList();
    }
    
    public async Task<List<CompanySummaryModel>> GetAllCompaniesSummary()
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);

        var sqlCompaniesSummary = @"
            SELECT
              IdEmpresa AS CompanyUniqueId,
              CedulaJuridica AS CompanyId,
              Nombre AS CompanyName
            FROM
              Empresa
        ";

        var companiesList = await connection.QueryAsync<CompanySummaryModel>(sqlCompaniesSummary);
        return companiesList.ToList();
      }
      catch (Exception ex)
      {
        Console.WriteLine("Error al obtener el resumen de todas las compañias: \n" + ex.Message);
        return new List<CompanySummaryModel>();
      }
    }

    public async Task<CompanyModel?> GetCompanyByUniqueId(int companyUniqueId)
    {
      try
      {
        using var connection = new SqlConnection(this._connectionString);

        var sqlCompanyByUniqueId = @"
            Select
              IdEmpresa As CompanyUniqueId,
              Estado As State,
              CedulaJuridica As CompanyId,
              Nombre As CompanyName,
              Telefono As Telephone,
              FechaCreacion As CreationDate,
              CantidadBeneficios As MaxBenefits,
              FrecuenciaPago As PaymentFrequency,
              DiaPago1 As PayDay1,
              DiaPago2 As PayDay2,
              IdCreadoPor As CreatedBy
            From Empresa
            Where IdEmpresa = @companyUniqueId";

        var company = await connection.QueryFirstOrDefaultAsync<CompanyModel>(sqlCompanyByUniqueId, new { companyUniqueId = companyUniqueId });
        return company;
      }
      catch (Exception ex)
      {
        Console.WriteLine("Error al obtener la compañia por su UniqueId. Detalle: \n" + ex.Message);
        return null;
      }
    }

    public async Task<int> GetMaxBenefitsTakenInCompany(int companyUniqueId)
    {
      int maxBenefitsTaken = 0;
      try
      {
        using var connection = new SqlConnection(this._connectionString);

        var sqlGetMaxBenTak = "SELECT dbo.GetMaxAmountBenefitsTakenInCompany(@companyId)";

        maxBenefitsTaken = await connection.QueryFirstOrDefaultAsync<int>(sqlGetMaxBenTak, new { companyId = companyUniqueId });
      } 
      catch (Exception ex)
      {
        Console.WriteLine("Error al obtener la mayor cantidad de beneficios tomados en una empresa. Detalle: \n" + ex.Message);
      }

      return maxBenefitsTaken;
    }

    public async Task<int> UpdateCompanyData(CompanyModel company)
    {
      int rowsAffected = 0;
      try
      {
        using var connection = new SqlConnection(this._connectionString);

        rowsAffected = await connection.ExecuteAsync("UpdateCompanyData",
            new
            {
              CompanyUniqueId = company.CompanyUniqueId,
              Name = company.CompanyName,
              Telephone = company.Telephone,
              MaxBenefits = company.MaxBenefits,
              ZipCode = company.Directions.ZipCode,
              OtherSigns = company.Directions.OtherSigns
            },
            commandType: CommandType.StoredProcedure);
      }
      catch (Exception ex)
      {
        Console.WriteLine("Error al actualizar la información de una empresa. Detalle: \n" + ex.Message);
      }
      return rowsAffected;
    }
    public async Task<CompanyModel> getCompanyByID(int companyId)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);

        var query =
          @"SELECT IdEmpresa AS CompanyUniqueId,
              CedulaJuridica AS CompanyId,
              Nombre AS CompanyName,
              Telefono AS Telephone,
              CantidadBeneficios AS MaxBenefits,
              FrecuenciaPago AS PaymentFrequency,
              DiaPago1 AS PayDay1,
              DiaPago2 AS PayDay2,
              IdCreadoPor AS CreatedBy
            FROM Empresa
            WHERE IdEmpresa = @companyId";

        var company = await connection.QuerySingleOrDefaultAsync<CompanyModel>(query, new { companyId });
        if (company == null) throw new KeyNotFoundException("La empresa no existe.");

        return company;
      }
      catch (Exception)
      {
        throw;
      }
    }
  }
}
