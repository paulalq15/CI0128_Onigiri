using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
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

        public async Task addAppliedElement(AppliedElement newAppliedElement)
        {
            if (newAppliedElement == null)
                throw new ArgumentNullException(nameof(newAppliedElement));

            if (!newAppliedElement.UserId.HasValue)
                throw new ArgumentException("UserId no puede ser nulo.");

            if (!newAppliedElement.ElementId.HasValue)
                throw new ArgumentException("ElementId no puede ser nulo.");

            const string query = @"
              INSERT INTO dbo.ElementoAplicado (IdUsuario, IdElemento, FechaInicio, FechaFin)
              VALUES (@IdUsuario, @IdElemento, GETDATE(), NULL);
            ";

            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(query, connection);

                command.Parameters.Add(new SqlParameter("@IdUsuario", newAppliedElement.UserId.Value));
                command.Parameters.Add(new SqlParameter("@IdElemento", newAppliedElement.ElementId.Value));

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }

            catch (SqlException ex)
            {
                throw new ApplicationException("Error al insertar el elemento aplicado en la base de datos.", ex);
            }
        }

        public void deactivateAppliedElement(AppliedElement appliedElement)
        {
            const string query = @"
              UPDATE dbo.ElementoAplicado
              SET FechaFin = GETDATE()
              WHERE IdElemento = @IdElemento;
            ";

            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(query, connection);

                command.Parameters.Add(new SqlParameter("@IdElemento", appliedElement.ElementId));

                connection.Open();
                command.ExecuteNonQuery();
            }

            catch (SqlException ex)
            {
                throw new ApplicationException("Error al intentar modificar el elemento aplicado en la base de datos.", ex);
            }
        }
    }
}
