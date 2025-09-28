using Microsoft.AspNetCore.Mvc;
using Planilla_Backend.Models;
using Planilla_Backend.Services;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
  private readonly EmployeeService _employeeService;

  public EmployeeController(EmployeeService employeeService)
  {
    _employeeService = employeeService;
  }

  [HttpPost]
  public ActionResult RegisterEmployee([FromBody] RegisterEmployee employee)
  {
    // Front-end already validated; here we just call the service
    int personId = _employeeService.RegisterEmployee(employee);
    return Ok(new { success = true, personId });
  }
}
