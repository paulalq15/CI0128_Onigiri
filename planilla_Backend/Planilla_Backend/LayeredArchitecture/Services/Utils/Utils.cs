using Microsoft.IdentityModel.Tokens;
using Planilla_Backend.LayeredArchitecture.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Planilla_Backend.LayeredArchitecture.Services.Utils
{
  public class Utils
  {
    private readonly IConfiguration configuration;
    public Utils(IConfiguration configuration)
    {
      this.configuration = configuration;
    }
    public string GenerateJWToken(int idPerson)
    {
      // Crear información del usuario para el token
      var userClaims = new[]
      {
        new Claim(ClaimTypes.NameIdentifier, idPerson.ToString()),
        new Claim("TokenType", "Activation")
      };

      // Obtenemos nuestra llave desde el appsettings (No es seguro, porque nuestro appsettings se sube al repo, pero xd)
      var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:KEY"]!));
      var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

      // Crear detalle del token
      var jwTokenConfig = new JwtSecurityToken(
        claims: userClaims,
        expires: DateTime.UtcNow.AddHours(5),
        signingCredentials: credentials
      );

      return new JwtSecurityTokenHandler().WriteToken(jwTokenConfig);
    }

    public string GenerateSetPasswordToken(int idUser, int minutes = 15)
    {
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:KEY"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, idUser.ToString()),
            new Claim("TokenType", "SetPassword")
        };

        var jwt = new JwtSecurityToken(
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(minutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
  }
}
