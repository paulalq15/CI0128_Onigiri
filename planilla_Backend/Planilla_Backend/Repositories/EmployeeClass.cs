using Dapper;
using Microsoft.Data.SqlClient;
using Planilla_Backend.Models;
using System.Data.SqlClient;

namespace Planilla_Backend.Repositories 
{
    public class EmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository()
        {
            var builder = WebApplication.CreateBuilder();
            _connectionString = builder.Configuration.GetConnectionString("?");
        }

        public List<EmployeeModel> GetUsers()
        {
            using var connection = new SqlConnection(_connectionString);
            string query = "SELECT * FROM dbo.Usuario";
            return connection.Query<EmployeeModel>(query).ToList();
        }
    }
}