using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Planilla_Backend.Services;
using Planilla_Backend.Models;
using Planilla_Backend.Services.Utils;
using Planilla_Backend.Services.Email.EmailModels;
using Planilla_Backend.Services.Email.EmailTypes;
using Planilla_Backend.Services.EmailService;

namespace Planilla_Backend.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class TokensController : ControllerBase
  {
    private PersonUserService personUserService;
    private readonly IConfiguration configuration;
    private readonly Utils utils;
    private readonly IEmailService emailService;
    public TokensController(PersonUserService personUserServ, IConfiguration configuration, Utils utils, IEmailService emailService)
    {
      this.personUserService = personUserServ;
      this.configuration = configuration;
      this.utils = utils;
      this.emailService = emailService;
    }

    [HttpGet("ActivateAccount")]
    public IActionResult ActivateAccount(string token)
    {
      string redirectUrl = "http://localhost:8080/auth/ActivateAccount?status="; // URL base del frontend

      try
      {
        var handler = new JwtSecurityTokenHandler();
        var claimsPrincipal = handler.ValidateToken(token, new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["JWT:KEY"]!)),
          ValidateIssuer = false,
          ValidateAudience = false,
          ValidateLifetime = true,
          ClockSkew = TimeSpan.Zero
        }, out var validatedToken);

        var tokenType = claimsPrincipal.FindFirstValue("TokenType");
        var idPersonString = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (tokenType != "Activation")
        {
          redirectUrl += "invalid-token";
        }
        else if (string.IsNullOrEmpty(idPersonString))
        {
          redirectUrl += "invalid-user";
        }
        else
        {
          int idPerson = int.Parse(idPersonString);

          bool isAlreadyActive = this.personUserService.IsUserPersonActive(idPerson);
          if (isAlreadyActive)
          {
            redirectUrl += "already-active";
          }
          else
          {
            int updateResult = this.personUserService.UpdateUserPesonStatusToActivate(idPerson);
            redirectUrl += updateResult < 1 ? "failed" : "success";
          }
        }
      }
      catch (Exception)
      {
        redirectUrl += "expired";
      }

      return Redirect(redirectUrl);
    }

    [HttpPost("ResendActivation")]
    public IActionResult ResendActivationAccount([FromBody] string email)
    {
      int idUser = this.personUserService.getUserIdByEmail(email);
      if (idUser < 1) return NotFound(new { message = "Usuario no encontrado" });

      PersonUser? personUser = this.personUserService.GetPersonUserbyIdUser(idUser);
      if (personUser == null) return NotFound(new { message = "NULL, Usuario no encontrado" });

      if (personUser.Status == "Activo") return BadRequest(new { message = "La cuenta ya está activa" });

      // Nuevo token
      string token = utils.GenerateJWToken(personUser.IdPerson);

      ActivationAccountModel activationModel = new ActivationAccountModel
      {
        userName = $"{personUser.Name1} {personUser.Surname1}",
        activationLink = $"https://localhost:7071/api/Tokens/ActivateAccount?token={token}"
      };

      ActivationAccountEmail activationAccountEmail = new ActivationAccountEmail();

      try
      {
        emailService.SendEmail(personUser.Email, activationAccountEmail.GetSubject(), activationAccountEmail.GetBody(activationModel));
      }
      catch (Exception)
      {
        return StatusCode(500, new { message = "No se pudo envíar el correo" });
      }

      return Ok(new { message = "¡Correo de activación reenviado!" });
    }

    [HttpGet("ActivateEmployee")]
    public IActionResult ActivateEmployee(string token)
    {
      const string frontBaseUrl = "http://localhost:8080/auth/EmployeeActivation";
      string status;
      string? setPwdToken = null;

      try
      {
        if (string.IsNullOrWhiteSpace(token))
        {
          status = "invalid-token";
        }
        else
        {
          var handler = new JwtSecurityTokenHandler();
          var principal = handler.ValidateToken(token, new TokenValidationParameters
          {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["JWT:KEY"]!)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
          }, out _);

          var tokenType = principal.FindFirstValue("TokenType");
          var idPersonString = principal.FindFirstValue(ClaimTypes.NameIdentifier);

          if (!string.Equals(tokenType, "Activation", StringComparison.Ordinal))
          {
            status = "invalid-token";
          }
          else if (string.IsNullOrWhiteSpace(idPersonString))
          {
            status = "invalid-user";
          }
          else
          {
            int idPerson = int.Parse(idPersonString);

            if (personUserService.IsUserPersonActive(idPerson))
            {
              status = "already-active";
            }
            else
            {

              var updated = personUserService.UpdateUserPesonStatusToActivate(idPerson);
              status = updated < 1 ? "failed" : "success";

              if (status == "success")
              {
                var user = personUserService.GetPersonUserById(idPerson);
                if (user is null)
                {
                  status = "invalid-user";
                }
                else
                {
                  setPwdToken = utils.GenerateSetPasswordToken(user.IdUser, minutes: 15);
                }
              }
            }
          }
        }
      }
      catch (SecurityTokenExpiredException)
      {
        status = "expired";
      }
      catch
      {
        status = "failed";
      }
      // Construye la URL de redirección al Front
      var query = new Dictionary<string, string?> { ["status"] = status };
      if (status == "success" && !string.IsNullOrEmpty(setPwdToken))
        query["token"] = setPwdToken;

      var redirectUrl = QueryHelpers.AddQueryString(frontBaseUrl, query);
      return Redirect(redirectUrl);
    }
    
    [HttpPost("SetPassword")]
    public IActionResult SetPassword(
      [FromBody] string password,
      [FromHeader(Name = "Authorization")] string? authHeader = null,
      [FromQuery] string? token = null)
    {

      string? bearer = null;
      if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        bearer = authHeader.Substring("Bearer ".Length).Trim();

      var tokenToValidate = !string.IsNullOrWhiteSpace(bearer) ? bearer : token;
      if (string.IsNullOrWhiteSpace(tokenToValidate))
        return Unauthorized("Token requerido.");

      ClaimsPrincipal claimsPrincipal;
      try
      {
        var handler = new JwtSecurityTokenHandler();
        claimsPrincipal = handler.ValidateToken(tokenToValidate, new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["JWT:KEY"]!)),
          ValidateIssuer = false,
          ValidateAudience = false,
          ValidateLifetime = true,
          ClockSkew = TimeSpan.Zero
        }, out var _);
      }
      catch (SecurityTokenExpiredException)
      {
        return Unauthorized("Token expirado.");
      }
      catch
      {
        return Unauthorized("Token inválido.");
      }

      var tokenType = claimsPrincipal.FindFirstValue("TokenType");
      if (!string.Equals(tokenType, "SetPassword", StringComparison.Ordinal))
        return Unauthorized("Token inválido para cambio de contraseña.");

      var nameId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
      if (string.IsNullOrWhiteSpace(nameId))
        return Unauthorized("Token sin identificador.");

      if (!int.TryParse(nameId, out var idFromClaim))
        return Unauthorized("Identificador inválido.");

      var user = personUserService.GetPersonUserbyIdUser(idFromClaim);
      if (user is null)
      {
        return NotFound("Usuario no encontrado.");
      }

        // llemar service de update pwd
        personUserService.SetUserPassword(user.IdUser, password);

        return Ok(new { Success = true, Message = "Contraseña actualizada." });
    }
  }
}
