using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planilla_Backend.Models;
using Planilla_Backend.Services;
using System.Text.Json;

namespace Planilla_Backend.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class PersonaUsuarioController : ControllerBase
  {
    private readonly PersonaUsuarioService personaUsuarioService;

    public PersonaUsuarioController()
    {
      personaUsuarioService = new PersonaUsuarioService();
    }

    [HttpPost("register")]
    public ActionResult<int> saveRegister(RegisterEmpleador registroData)
    {
      if (registroData.personaData == null || registroData.password == null || registroData.zipCode == null || registroData.otherSigns == null) {
        return BadRequest("Datos inválidos");
      }

      int idPersona = personaUsuarioService.savePersonaUsuario(registroData.personaData, registroData.password);

      if (idPersona != -1) {
        int idDirection = personaUsuarioService.savePersonaDirection(idPersona, registroData.zipCode, registroData.otherSigns);
        return Created(string.Empty, idPersona);
      } else {
        Console.WriteLine("Error al guardar");
        return BadRequest("Error al registrar la persona y usuario");
      }
    }

    [HttpGet("emailCheck")]
    public int getUserIdByEmail(string email)
    {
      if (email == null) {
        Console.WriteLine(email);
        return -1;
      }

      int idPersona = personaUsuarioService.getUserIdByEmail(email);

      return idPersona;
    }
  }
}
