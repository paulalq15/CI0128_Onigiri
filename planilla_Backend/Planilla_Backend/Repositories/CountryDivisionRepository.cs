using Planilla_Backend.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Planilla_Backend.Repositories
{
    public class CountryDivisionRepository
    {
        private readonly string _connectionString;
        public CountryDivisionRepository()
        {
            var builder = WebApplication.CreateBuilder();
            _connectionString = builder.Configuration.GetConnectionString("PayrollContext");
        }

        public List<ProvinciaModel> GetProvince()
        {
            using var connection = new SqlConnection(_connectionString);
            string query = "SELECT DISTINCT Provincia FROM dbo.DivisionTerritorialCR";
            return connection.Query<ProvinciaModel>(query).ToList();
        }
        public List<CantonModel> GetCounty(string provincia)
        {
            using var connection = new SqlConnection(_connectionString);
            string query = "SELECT DISTINCT Canton FROM dbo.DivisionTerritorialCR WHERE Provincia = @provincia";
            return connection.Query<CantonModel>(query, new { provincia }).ToList();
        }
        public List<DistritoModel> GetDistrict(string provincia, string canton)
        {
            using var connection = new SqlConnection(_connectionString);
            string query = "SELECT Distrito FROM dbo.DivisionTerritorialCR WHERE Provincia = @provincia AND Canton = @canton";
            return connection.Query<DistritoModel>(query, new { provincia, canton }).ToList();
        }
        public List<ZipCodeModel> GetZipCode(string provincia, string canton, string distrito)
        {
            using var connection = new SqlConnection(_connectionString);
            string query = "SELECT CodigoPostal FROM dbo.DivisionTerritorialCR WHERE Provincia = @provincia AND Canton = @canton AND Distrito = @distrito";
            return connection.Query<ZipCodeModel>(query, new { provincia, canton, distrito }).ToList();
        }
    }
}
