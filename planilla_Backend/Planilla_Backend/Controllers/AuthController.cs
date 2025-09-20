using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planilla_Backend.Services;
using Planilla_Backend.Models;

namespace Planilla_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var resp = await _auth.LoginAsync(req);
            if (!resp.Success) return Unauthorized(resp);
            return Ok(resp);
        }
    }
}
