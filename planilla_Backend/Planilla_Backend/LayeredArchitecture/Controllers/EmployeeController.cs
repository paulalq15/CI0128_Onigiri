using Microsoft.AspNetCore.Hosting.Server;
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
  private readonly string _activationBaseUrl;

  public EmployeeController(EmployeeService employeeService, IEmailService emailService, Utils tokenUtil, IConfiguration configuration)
  {
    _employeeService = employeeService;
    _emailService = emailService;
    _tokenUtil = tokenUtil;
    _activationBaseUrl = configuration["APP_URLS:ActivationBackendBase"];
  }

  [HttpPost]
  public ActionResult RegisterEmployee([FromBody] EmployeeModel employee)
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

      activationModel.activationLink = $"{_activationBaseUrl}/api/Tokens/ActivateEmployee?token={token}";

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

  [HttpGet("{personId:int}")]
  public async Task<ActionResult<EmployeeModel>> GetEmployee(int personId)
  {
    var data = await _employeeService.GetEmployeeByPersonId(personId);
    if (data is null) return NotFound();
    return Ok(data);
  }

  [HttpPut("me/{personId:int}")]
  public async Task<IActionResult> UpdateMyProfile(int personId, [FromBody] EmployeeModel body, CancellationToken ct)
  {
    var ok = await _employeeService.UpdateSelf(personId, body, ct);
    return ok ? NoContent() : BadRequest("No se pudo actualizar.");
  }

  [HttpPut("emp/{employerId:int}/employee/{personId:int}")]
  public async Task<IActionResult> UpdateEmployeeByEmployer(int employerId, int personId, [FromBody] EmployeeModel body, CancellationToken ct)
  {
    var ok = await _employeeService.UpdateAsEmployer(employerId, personId, body, ct);
    return ok ? NoContent() : BadRequest("No se pudo actualizar.");
  }

  [HttpDelete("{userId:int}")]
  public async Task<IActionResult> DeleteEmployee(int userId) {
    await _employeeService.DeleteEmployee(userId);
    return Ok();
  }
}
