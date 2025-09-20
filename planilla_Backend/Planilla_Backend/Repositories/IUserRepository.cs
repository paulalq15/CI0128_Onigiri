using System.Threading.Tasks;
using Planilla_Backend.Models;

namespace Planilla_Backend.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetActiveEmailAsync(string correo);
    }
}
