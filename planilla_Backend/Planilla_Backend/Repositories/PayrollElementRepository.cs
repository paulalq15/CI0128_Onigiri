using Dapper;
using Microsoft.Data.SqlClient;
using Planilla_Backend.Models;
using System.Data.SqlClient;

namespace Planilla_Backend.Repositories
{
  public class PayrollElementRepository
  {
    private readonly string _connectionString;
    public PayrollElementRepository()
    {
      var builder = WebApplication.CreateBuilder();
      _connectionString = builder.Configuration.GetConnectionString("OnigiriContext");
    }

    public bool CheckCompanyStatus(int companyId)
    {
      using var connection = new SqlConnection(_connectionString);
      const string query = @"SELECT 1 FROM dbo.Empresa WHERE IdEmpresa = @Id AND Estado = 'Activo'";
      var company = connection.ExecuteScalar<int?>(query, new { Id = companyId });
      return company.HasValue;
    }

    public bool CheckUserType(int userId)
    {
      using var connection = new SqlConnection(_connectionString);
      const string query = @"SELECT p.TipoPersona
                                FROM dbo.Usuario u
                                INNER JOIN dbo.Persona p ON p.IdPersona = u.IdPersona
                                WHERE u.IdUsuario = @Id AND u.Estado = 'Activo'";
      var personType = connection.ExecuteScalar<string?>(query, new { Id = userId });
      return string.Equals(personType, "Empleador", StringComparison.OrdinalIgnoreCase);
    }

    public bool CreatePayrollElement(PayrollElementModel element)
    {
      using var connection = new SqlConnection(_connectionString);
      const string query = @"INSERT INTO dbo.ElementoPlanilla (Nombre, PagadoPor, Tipo, Valor, IdEmpresa)
                                 VALUES (@ElementName, @PaidBy, @CalculationType, @CalculationValue, @CompanyId)";
      int rowsAffected = connection.Execute(query, new
      {
        elementName = element.ElementName,
        paidBy = element.PaidBy,
        calculationType = element.CalculationType,
        calculationValue = element.CalculationValue,
        companyId = element.CompanyId
      });
      return rowsAffected > 0;
    }

    public async Task<List<PayrollElementModel>> GetPayrollElementsByIdCompany(int idCompany)
    {
      try
      {
        using var connection = new SqlConnection(_connectionString);

        const string sqlGetPayrollElements = @"
          SELECT
          Nombre AS ElementName,
          Tipo AS CalculationType,
          Valor AS CalculationValue,
          PagadoPor AS PaidBy,
          Estado AS Status,
          IdEmpresa AS CompanyId
          FROM
          ElementoPlanilla
          WHERE
          IdEmpresa = @idCompany";

        var elementsList = await connection.QueryAsync<PayrollElementModel>(sqlGetPayrollElements, new { idCompany });
        return elementsList.ToList();
      } catch (Exception ex) {
        Console.WriteLine("Error al obtener elementos de planilla: " + ex.Message);
        return new List<PayrollElementModel>();
      }
    }
  }
}
