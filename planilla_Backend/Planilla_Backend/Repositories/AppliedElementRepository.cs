using Dapper;
using Microsoft.Data.SqlClient;
using Planilla_Backend.Models;
using System.Data.SqlClient;

namespace Planilla_Backend.Repositories
{
    public class AppliedElementRepository
    {
        private readonly string _connectionString;

        public AppliedElementRepository()
        {
            var builder = WebApplication.CreateBuilder();
            _connectionString = builder.Configuration.GetConnectionString("OnigiriContext");
        }

        public List<AppliedElement> getAppliedElements(int employeeId)
        {
            const string query = @"
            SELECT
              IdElementoAplicado AS ElementId,
              IdUsuario As UserId,
              FechaInicio AS StartDate,
              FechaFin AS EndDate,
              Estado AS Status
            FROM dbo.ElementoAplicado
            WHERE IdUsuario = @employeeId;";

            using var connection = new SqlConnection(_connectionString);

            return connection.Query<AppliedElement>(query, new { employeeId }).ToList();
        }
    }
}
