using Dapper;
using Microsoft.Data.SqlClient;
using Planilla_Backend.Models;

namespace Planilla_Backend.Repositories
{
    public class UserRepository
    {
        private readonly string _connectionString;
        public UserRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("PayrollContext");
        }

        public async Task<User?> GetActiveEmailAsync(string email)
        {
            const string sql = @"
                SELECT 
                    -- USER (incluye la empresa ANTES del splitOn)
                    u.IdUsuario       AS UserId, 
                    u.Correo          AS Email, 
                    u.Contrasena      AS Password, 
                    u.Estado          AS Status, 
                    u.IdPersona       AS PersonID,

                    (
                        SELECT TOP (1) e.IdEmpresa
                        FROM dbo.UsuariosPorEmpresa ue
                        JOIN dbo.Empresa e  ON e.IdEmpresa = ue.IdEmpresa
                        WHERE ue.IdUsuario = u.IdUsuario
                          AND e.Estado = 'Activo'
                        ORDER BY ue.IdUsEmp DESC
                    )                  AS CompanyUniqueId,

                    -- PERSON (primera col para splitOn)
                    p.Cedula          AS Cedula,
                    p.IdPersona       AS PersonID, 
                    p.Nombre1         AS Name1, 
                    p.Nombre2         AS Name2, 
                    p.Apellido1       AS Surname1, 
                    p.Apellido2       AS Surname2, 
                    p.Telefono        AS Phone, 
                    p.FechaNacimiento AS BirthDate,
                    p.TipoPersona     AS PersonType
                FROM dbo.Usuario u
                JOIN dbo.Persona p 
                  ON p.IdPersona = u.IdPersona
                WHERE u.Correo = @Email
                  AND u.Estado = 'Activo';
            ";

            using var conn = new SqlConnection(_connectionString);

            User? user = null;
            await conn.QueryAsync<User, Person, User>(
                sql,
                (u, p) =>
                {
                    if (user == null) user = u;
                    user.Person = p;
                    return user;
                },
                new { Email = email },
                splitOn: "Cedula"
            );

            return user;
        }
    }
}
