using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planilla_Backend.Models;
using Planilla_Backend.Services;

namespace Planilla_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly CompanyService createCompanyService;
        public CompanyController()
        {
            createCompanyService = new CompanyService();
        }

        [HttpPost]
        public ActionResult<int> CreateCompany([FromBody] CompanyModel company)
        {
            if (company == null)
            {
                return BadRequest("Datos inválidos");
            }

            var result = createCompanyService.CreateCompany(company, out int companyId);
            if (string.IsNullOrEmpty(result))
            {
                return Created(string.Empty, companyId);
            }

            var msg = result.ToLowerInvariant();

            // Determinar el código de estado HTTP basado en el mensaje de error
            if (msg.Contains("cédula"))
                return Conflict(result); // 409 duplicado

            if (msg.Contains("postal") || msg.Contains("usuario"))
                return NotFound(result); // 404 dependencia faltante

            return BadRequest(result); // 400 error de validación u otro error del cliente
        }
    }


}
