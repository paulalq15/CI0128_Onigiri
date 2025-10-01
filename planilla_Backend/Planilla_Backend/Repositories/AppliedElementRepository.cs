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
              ea.IdElementoAplicado AS AppliedElementId,
              ea.IdElemento AS ElementId,
              ep.Nombre AS ElementName,
              ea.IdUsuario As UserId,
              ea.FechaInicio AS StartDate,
              ea.FechaFin AS EndDate,
              ea.Estado AS Status
            FROM dbo.ElementoAplicado ea
            JOIN dbo.ElementoPlanilla ep ON ea.IdElemento = ep.IdElemento
            WHERE ea.IdUsuario = @employeeId;";

            using var connection = new SqlConnection(_connectionString);

            return connection.Query<AppliedElement>(query, new { employeeId }).ToList();
        }
    }
}
