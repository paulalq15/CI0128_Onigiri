using Planilla_Backend.Models;
using Planilla_Backend.Repositories;

namespace Planilla_Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repo;

        public AuthService(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Correo) || string.IsNullOrWhiteSpace(req.Contrasena))
            {
                return new LoginResponse { Success = false, Message = "Correo y contraseña son requeridos." };
            }

            var user = await _repo.GetActiveEmailAsync(req.Correo.Trim());
            if (user is null)
            {
                return new LoginResponse { Success = false, Message = "Usuario no encontrado o inactivo." };
            }

            // VALIDACIÓN EN TEXTO PLANO - Cambiar***
            if (!string.Equals(user.Contrasena, req.Contrasena))
            {
                return new LoginResponse { Success = false, Message = "Contraseña incorrecta." };
            }

            var p = user.Persona;
            var nombre = p is null
                ? null
                : $"{p.Nombre1} {(string.IsNullOrWhiteSpace(p.Nombre2) ? "" : p.Nombre2 + " ")}{p.Apellido1} {(string.IsNullOrWhiteSpace(p.Apellido2) ? "" : p.Apellido2)}".Replace("  ", " ").Trim();

            return new LoginResponse
            {
                Success = true,
                Message = "Login exitoso.",
                IdUsuario = user.IdUsuario,
                IdPersona = user.IdPersona,
                NombreCompleto = nombre,
                TipoPersona = p?.TipoPersona,
                Correo = user.Correo
            };
        }
    }
}
