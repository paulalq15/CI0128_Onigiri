using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public CompanyController(CompanyService createCompanyServ)
    {
      createCompanyService = createCompanyServ;
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

    [HttpGet("by-user/{userId:int}")]
    public ActionResult<IEnumerable<CompanySummaryModel>> GetMyCompanies([FromRoute] int userId, [FromQuery] bool onlyActive = true)
    {
      if (userId <= 0) return BadRequest("El ID de usuario es inválido.");

      var (error, companies) = createCompanyService.GetCompaniesForUser(userId, onlyActive);

      if (!string.IsNullOrEmpty(error))
      {
        var msg = error.ToLowerInvariant();
        if (msg.Contains("no es un empleador")) return Forbid(error);
        return BadRequest(error);
      }

      // 200 con la lista (vacía o con datos)
      return Ok(companies);
    }

    [HttpGet("getCompaniesWithStats")]
    public ActionResult<List<CompanyModel>> GetCompaniesWithStats([FromQuery] int employerId, [FromQuery] int viewerUserId)
    {
      if (employerId <= 0 || viewerUserId <= 0) return BadRequest("Invalid parameters.");

      var rows = createCompanyService.GetCompaniesWithStats(employerId, viewerUserId);
      return Ok(rows);
    }

    [HttpGet("getCompanies")]
    public List<CompanyModel> getCompanies([FromQuery] int employerId)
    {
      return this.createCompanyService.getCompanies(employerId);
    }

    [HttpGet("GetAllCompaniesSummary")]
    public async Task<ActionResult<List<CompanySummaryModel>>> GetAllCompaniesSummary()
    {
      List<CompanySummaryModel> companySummaryModelsList = await this.createCompanyService.GetAllCompaniesSummary();

      return Ok(companySummaryModelsList);
    }

    // GetCompanyByCompanyUniqueId
    [HttpGet("GCBCUI")]
    public async Task<ActionResult<CompanyModel>> GetCompanyByCompanyUniqueId(int companyUniqueId)
    {
      CompanyModel? company = await this.createCompanyService.GetCompanyByUniqueId(companyUniqueId);

      if (company == null) return NotFound(new { message = "No se encontró la empresa" });

      return Ok(company);
    }

    // GetCompanyByCompanyUniqueId
    [HttpGet("MaxBenTak")]
    public async Task<ActionResult<int>> GetMaxAmountBenefitsTakenByCompanyUniqueId(int companyUniqueId)
    {
      int maxBenefitsAmount = await this.createCompanyService.GetMaxBenefitsTakenInCompany(companyUniqueId);

      return Ok(maxBenefitsAmount);
    }

    // Update company data
    [HttpPost("UpdComp")]
    public async Task<ActionResult<int>> UpdateCompanyData([FromBody] CompanyModel company)
    {
      int rowsAffected = await this.createCompanyService.updateCompanyData(company);

      return Ok(rowsAffected);
    }
  }
}
