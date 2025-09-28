using Dapper;
using Microsoft.Data.SqlClient;
using Planilla_Backend.Models;
using System.Data.SqlClient;

namespace Planilla_Backend.Repositories
{
  public class CompanyRepository
  {
    private readonly string _connectionString;
    public CompanyRepository()
    {
      var builder = WebApplication.CreateBuilder();
      _connectionString = builder.Configuration.GetConnectionString("OnigiriContext");
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

    public bool ValidateUserType(int userId)
    {
      using var connection = new SqlConnection(_connectionString);
      const string sql = @"SELECT p.TipoPersona
                          FROM dbo.Usuario u
                          INNER JOIN dbo.Persona p ON p.IdPersona = u.IdPersona
                          WHERE u.IdUsuario = @Id AND u.Estado = 'Activo'";
      var type = connection.ExecuteScalar<string?>(sql, new { Id = userId });
      return string.Equals(type, "Empleador", StringComparison.OrdinalIgnoreCase);
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

    public List<CompanyModel> getCompanies()
    {
      const string query = @"SELECT
                           IdEmpresa AS CompanyUniqueId,
                           CedulaJuridica AS CompanyId,
                           Nombre AS CompanyName,
                           Telefono AS Telephone,
                           CantidadBeneficios AS MaxBenefits,
                           FrecuenciaPago AS PaymentFrequency,
                           DiaPago1 AS PayDay1,
                           DiaPago2 AS PayDay2
                           FROM dbo.Empresa;";

      using var connection = new SqlConnection(_connectionString);

      return connection.Query<CompanyModel>(query).ToList();
    }
  }
}
