using Microsoft.AspNetCore.Mvc;
using Planilla_Backend.CleanArchitecture.API.Http;
using Planilla_Backend.CleanArchitecture.Application.UseCases.company;

namespace Planilla_Backend.CleanArchitecture.API
{
  [ApiController]
  [Route("api/Company")]
  public class CompanyControllerCA : ControllerBase
  {
    private readonly IDeleteCompanyCommand deleteCompanyCommand;

    public CompanyControllerCA(IDeleteCompanyCommand deleteCompanyCommand)
    {
      this.deleteCompanyCommand = deleteCompanyCommand;
    }

    [HttpDelete("company")]
    public async Task<ActionResult<int>> deleteCompany(int companyId, int employeerPersonId)
    {
      try
      {
        int rowsAffected = await this.deleteCompanyCommand.Execute(companyId, employeerPersonId);
        return Ok(rowsAffected);
      }
      catch (ArgumentException ex)
      {
        return BadRequest(new { message = ex.Message });
      }
      catch (KeyNotFoundException ex)
      {
        return NotFound(new { message = ex.Message });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "Error interno del servidor. " + ex.Message });
      }
    }
  }
}
