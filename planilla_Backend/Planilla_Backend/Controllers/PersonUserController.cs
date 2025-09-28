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

    [HttpGet("getEmployees")]
    public List<Person> getEmployees()
    {
      var employees = personUserService.getEmployees();
      return employees;
    }
  }
}
