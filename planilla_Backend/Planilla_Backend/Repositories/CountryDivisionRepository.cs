using Dapper;
using Microsoft.Data.SqlClient;
using Planilla_Backend.Models;
using System.Diagnostics.Metrics;

namespace Planilla_Backend.Repositories
{
    public class CountryDivisionRepository
    {
        private readonly string _connectionString;
        public CountryDivisionRepository()
        {
            var builder = WebApplication.CreateBuilder();
            _connectionString = builder.Configuration.GetConnectionString("OnigiriContext");
        }

        public List<DivisionModel> GetProvince()
        {
            using var connection = new SqlConnection(_connectionString);
            string query = "SELECT DISTINCT Provincia AS Value FROM dbo.DivisionTerritorialCR";
            return connection.Query<DivisionModel>(query).ToList();
        }
        public List<DivisionModel> GetCounty(string province)
        {
            using var connection = new SqlConnection(_connectionString);
            string query = "SELECT DISTINCT Canton AS Value FROM dbo.DivisionTerritorialCR WHERE Provincia = @province";
            return connection.Query<DivisionModel>(query, new { province }).ToList();
        }
        public List<DivisionModel> GetDistrict(string province, string county)
        {
            using var connection = new SqlConnection(_connectionString);
            string query = "SELECT Distrito AS Value FROM dbo.DivisionTerritorialCR WHERE Provincia = @province AND Canton = @county";
            return connection.Query<DivisionModel>(query, new { province, county }).ToList();
        }
        public DivisionModel? GetZipCode(string province, string county, string district)
        {
            using var connection = new SqlConnection(_connectionString);
            string query = "SELECT CodigoPostal AS Value FROM dbo.DivisionTerritorialCR WHERE Provincia = @province AND Canton = @county AND Distrito = @district";
            return connection.QueryFirstOrDefault<DivisionModel>(query, new { province, county, district });
        }
    }
}
