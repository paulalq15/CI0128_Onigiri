using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planilla_Backend.Models;
using Planilla_Backend.Services;
using System.Text.Json;

namespace Planilla_Backend.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class PersonUserController : ControllerBase
  {
    private readonly PersonUserService personUserService;

    public PersonUserController()
    {
      personUserService = new PersonUserService();
    }

    [HttpPost("register")]
    public ActionResult<int> saveRegister(RegisterEmpleador registerData)
    {
      if (registerData.personData == null || registerData.password == null || registerData.zipCode == null || registerData.otherSigns == null) {
        return BadRequest("Datos inválidos");
      }

      int idPerson = personUserService.savePersonUser(registerData.personData, registerData.password);

      if (idPerson != -1) {
        int idDirection = personUserService.savePersonDirection(idPerson, registerData.zipCode, registerData.otherSigns);
        if (idDirection != -1) {
          return Created(string.Empty, idPerson);
        }
      }

      Console.WriteLine("Error al guardar");
      return BadRequest("Error al registrar la persona y usuario");
    }

    [HttpGet("emailCheck")]
    public int getUserIdByEmail(string email)
    {
      if (email == null) {
        Console.WriteLine(email);
        return -1;
      }

      int idPersona = personUserService.getUserIdByEmail(email);

      return idPersona;
    }
  }
}
