using Planilla_Backend.Models;
using Planilla_Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Planilla_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreateCompanyController : ControllerBase
    {
        private readonly CreateCompanyService createCompanyService;
        public CreateCompanyController()
        {
            createCompanyService = new CreateCompanyService();
        }

        [HttpPost]
        public async Task<ActionResult<int>> CreateCompany(CreateCompanyModel company)
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
            else
            {
                return BadRequest(result);
            }
        }
    }


}
