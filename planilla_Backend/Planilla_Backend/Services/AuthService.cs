using Planilla_Backend.Models;
using Planilla_Backend.Repositories;

namespace Planilla_Backend.Services
{
    public class AuthService
    {
        private readonly UserRepository _repo;

        public AuthService(UserRepository repo)
        {
            _repo = repo;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
            {
                return new LoginResponse { Success = false, Message = "Correo y contraseña son requeridos." };
            }

            var user = await _repo.GetActiveEmailAsync(req.Email.Trim());
            if (user is null)
            {
                return new LoginResponse { Success = false, Message = "Usuario no encontrado o inactivo." };
            }

            // VALIDACIÓN EN TEXTO PLANO - Cambiar***
            if (!string.Equals(user.Password, req.Password))
            {
                return new LoginResponse { Success = false, Message = "Contraseña incorrecta." };
            }

            var p = user.Person;
            var name = p is null
                ? null
                : $"{p.Name1} {(string.IsNullOrWhiteSpace(p.Name2) ? "" : p.Name2 + " ")}{p.Surname1} {(string.IsNullOrWhiteSpace(p.Surname2) ? "" : p.Surname2)}".Replace("  ", " ").Trim();

            return new LoginResponse
            {
                Success = true,
                Message = "Login exitoso.",
                UserId = user.UserId,
                PersonID = user.PersonID,
                FullName = name,
                PersonType = p?.PersonType,
                Email = user.Email
            };
        }
    }
}
