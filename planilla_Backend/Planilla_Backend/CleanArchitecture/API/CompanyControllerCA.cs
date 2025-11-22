using Microsoft.AspNetCore.Mvc;
using Planilla_Backend.CleanArchitecture.Application.UseCases.company;

namespace Planilla_Backend.CleanArchitecture.API
{
  [ApiController]
  [Route("api/external")]
  public class CompanyControllerCA : ControllerBase
  {
    private readonly IDeleteCompanyCommand deleteCompanyCommand;

    public CompanyControllerCA(IDeleteCompanyCommand deleteCompanyCommand)
    {
      this.deleteCompanyCommand = deleteCompanyCommand;
    }

    [HttpDelete("deleteCompany")]
    public async Task<ActionResult<int>> deleteCompany(int companyId, int employeerPersonId)
    {
      int rowsAffected = await this.deleteCompanyCommand.Execute(companyId, employeerPersonId);
      return Ok(rowsAffected);
    }
  }
}
