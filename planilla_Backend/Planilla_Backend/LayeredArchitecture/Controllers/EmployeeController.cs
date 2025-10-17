using Microsoft.AspNetCore.Mvc;
using Planilla_Backend.LayeredArchitecture.Models;
using Planilla_Backend.LayeredArchitecture.Services;
using Planilla_Backend.LayeredArchitecture.Services.Email.EmailModels;
using Planilla_Backend.LayeredArchitecture.Services.Email.EmailTypes;
using Planilla_Backend.LayeredArchitecture.Services.EmailService;
using Planilla_Backend.LayeredArchitecture.Services.Utils;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
  private readonly EmployeeService _employeeService;
  private readonly IEmailService _emailService;
  private readonly Utils _tokenUtil;

  public EmployeeController(EmployeeService employeeService, IEmailService emailService, Utils tokenUtil)
  {
    _employeeService = employeeService;
    _emailService = emailService;
    _tokenUtil = tokenUtil;
  }

  [HttpPost]
  public ActionResult RegisterEmployee([FromBody] RegisterEmployee employee)
  {
    // Front-end already validated; here we just call the service
    int personId = _employeeService.RegisterEmployee(employee);


    if (personId != -1)
    {
      ActivationAccountModel activationModel = new ActivationAccountModel();
      activationModel.userName = employee.PersonData.Name1 + " " + employee.PersonData.Surname1;
      EmployeeActivationEmail activationAccountEmail = new EmployeeActivationEmail(activationModel);

      //Generar token de activación
      string token = this._tokenUtil.GenerateJWToken(personId);

      activationModel.activationLink = $"https://localhost:7071/api/Tokens/ActivateEmployee?token={token}";

      // Enviar correo
      try
      {
        _emailService.SendEmail(employee.UserData.Email, activationAccountEmail.GetSubject(), activationAccountEmail.GetBody(activationModel));
      } catch (Exception)
      {
        return BadRequest("Error al enviar el correo de activación");
      }
      return Ok(new { success = true, personId });
    }
    return BadRequest("Error al registrar al empleado");
  }
}
