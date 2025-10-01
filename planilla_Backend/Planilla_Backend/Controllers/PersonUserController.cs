using Microsoft.AspNetCore.Mvc;
using Planilla_Backend.Models;
using Planilla_Backend.Services;
using Planilla_Backend.Services.Email.EmailModels;
using Planilla_Backend.Services.Email.EmailTypes;
using Planilla_Backend.Services.EmailService;
using Planilla_Backend.Services.Utils;

namespace Planilla_Backend.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class PersonUserController : ControllerBase
  {
    private readonly PersonUserService personUserService;
    private readonly IEmailService emailService;
    private readonly Utils utils;

    public PersonUserController(PersonUserService personUserServ, IEmailService emailService, Utils utils)
    {
      personUserService = personUserServ;
      this.emailService = emailService;
      this.utils = utils;
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

          // Enviar correo de activación
          ActivationAccountModel activationModel = new ActivationAccountModel();
          ActivationAccountEmail activationAccountEmail = new ActivationAccountEmail();
          activationModel.userName = registerData.personData.Name1 + " " + registerData.personData.Surname1;

          //Generar token de activación
          string token = this.utils.GenerateJWToken(idPerson);

          activationModel.activationLink = $"https://localhost:7071/api/Tokens/ActivateAccount?token={token}";

          // Enviar correo
          try
          {
            emailService.SendEmail(registerData.personData.Email, activationAccountEmail.GetSubject(), activationAccountEmail.GetBody(activationModel));
          } catch (Exception)
          {
            return BadRequest("Error al enviar el correo de activación");
          }
          return Ok(idPerson);
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

    [HttpGet("getEmployeesByCompanyId")]
    public List<PersonUser> getEmployeesByCompanyId(int companyId)
    {
      return personUserService.getEmployeesByCompanyId(companyId);
    }
  }
}
