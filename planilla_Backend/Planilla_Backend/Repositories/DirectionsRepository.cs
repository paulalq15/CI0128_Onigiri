using Dapper;
using Microsoft.Data.SqlClient;
using Planilla_Backend.Models;

namespace Planilla_Backend.Repositories
{
  public class DirectionsRepository
  {
    private readonly string _connectionString;
    public DirectionsRepository(IConfiguration config) {
      _connectionString = config.GetConnectionString("OnigiriContext");
    }

    // Comprobar si el zip code existe y devolver el id de la división territorial
    public int getDivisionByZipCode(string zipCode)
    {
      using var connection = new SqlConnection(_connectionString);
      const string query = @"SELECT TOP (1) IdDivision FROM dbo.DivisionTerritorialCR WHERE CodigoPostal = @ZipCode;";

      try
      {
        int? idDivision = connection.ExecuteScalar<int?>(query, new { ZipCode = zipCode });
        // Si no eciste el zip code, devolver -1
        if (idDivision is null) return -1;

        return idDivision.Value;
      } catch (Exception ex) {
        Console.WriteLine(ex.ToString());
        throw new Exception("Error al obtener la división territorial: " + ex.Message);
      }
    }

    // Guardar nueva dirección y devolver el id
    public int saveDirection(int idDivision, string otherSigns, int? idEmpresa = null, int? idPerson = null)
    {
      using var connection = new SqlConnection(_connectionString);
      
      const string insertDirection = @"INSERT INTO dbo.Direccion
          (IdDivision, OtrasSenas, IdEmpresa, IdPersona)
          OUTPUT INSERTED.IdDireccion
          VALUES
          (@IdDivision, @OtherSigns, @IdEmpresa, @IdPersona);";
      
      // Realizar la inserción en try, por si falla
      try
      {
        int idDirection = connection.ExecuteScalar<int>(insertDirection, new
        {
          IdDivision = idDivision,
          OtherSigns = otherSigns,
          IdEmpresa = idEmpresa,
          IdPersona = idPerson
        });

        return idDirection;
      } catch (Exception ex) {
        throw new Exception("Error al guardar la dirección: " + ex.Message);
      }
    }

    public async Task<DirectionsModel?> GetCompanyDirectionsByCompanyUniqueId(int companyId)
    {
      try
      {
        using var connection = new SqlConnection(this._connectionString);

        var sqlCompanyDirections = @"
            Select
              d.IdDireccion As IdDirection,
              d.IdDivision As IdDivision,
              d.OtrasSenas As OtherSigns,
              d.IdEmpresa As IdCompany,
              dt.Provincia As Province,
              dt.Canton As Canton,
              dt.Distrito As District,
              dt.CodigoPostal As ZipCode
            From Direccion d
            Inner Join DivisionTerritorialCR dt On dt.IdDivision = d.IdDivision
            Where d.IdEmpresa = @companyId";

        var directions = await connection.QueryFirstOrDefaultAsync<DirectionsModel>(sqlCompanyDirections, new { companyId });
        return directions;
      }
      catch (Exception ex)
      {
        Console.WriteLine("Error al devolver la dirección de la empresa. Detalle: \n" + ex.Message);
        return null;
      }
    }
  }
}
