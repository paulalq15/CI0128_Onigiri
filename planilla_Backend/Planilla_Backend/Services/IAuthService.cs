using Planilla_Backend.Models;

namespace Planilla_Backend.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest req);
    }
}
