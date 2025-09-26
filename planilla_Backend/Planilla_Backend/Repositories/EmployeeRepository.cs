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
            _connectionString = builder.Configuration.GetConnectionString("PayrollContext");
        }

        public List<EmployeeModel> GetEmployees()
        {
            using var connection = new SqlConnection(_connectionString);
            string query = "SELECT * FROM dbo.UsuariosPorEmpresa;";

            return connection.Query<EmployeeModel>(query).ToList();
        }
    }
}
