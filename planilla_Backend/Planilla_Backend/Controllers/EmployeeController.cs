using Microsoft.AspNetCore.Mvc;

namespace Planilla_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        [HttpGet]
        public string Get() {
            return "Hola Mundo";
        }
    }
}
