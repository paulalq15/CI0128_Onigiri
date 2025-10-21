using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planilla_Backend.LayeredArchitecture.Services;
using Planilla_Backend.LayeredArchitecture.Models;

namespace Planilla_Backend.LayeredArchitecture.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private readonly AuthService _auth;

    public AuthController(AuthService auth)
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
