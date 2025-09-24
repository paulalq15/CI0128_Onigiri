using Dapper;
using Microsoft.Data.SqlClient;
using Planilla_Backend.Models;

namespace Planilla_Backend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        public UserRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("PayrollContext");
        }

        public async Task<User?> GetActiveEmailAsync(string correo)
        {
            const string sql = @" SELECT u.IdUsuario, u.Correo, u.Contrasena, u.Estado, u.IdPersona,
                                  p.Cedula, p.IdPersona, p.Nombre1, p.Nombre2, p.Apellido1, p.Apellido2, p.Telefono, p.FechaNacimiento, p.TipoPersona
                                  FROM dbo.Usuario u
                                  JOIN dbo.Persona p ON p.IdPersona = u.IdPersona
                                  WHERE u.Correo = @Correo AND u.Estado = 'Activo';";

            using var conn = new SqlConnection(_connectionString);

            User? usuario = null;
            await conn.QueryAsync<User, Person, User>(
                sql,
                (u, p) =>
                {
                    if (usuario == null)
                    {
                        usuario = u;
                    }
                    usuario.Persona = p;
                    return usuario;
                },
                new { Correo = correo },
                splitOn: "Cedula"
            );

            return usuario;
        }
    }
}
