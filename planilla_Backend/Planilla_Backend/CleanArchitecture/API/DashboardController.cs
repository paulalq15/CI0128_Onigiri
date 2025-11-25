using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planilla_Backend.CleanArchitecture.Application.Dashboards;

namespace Planilla_Backend.CleanArchitecture.API
{
  [Route("api/[controller]")]
  [ApiController]
  public class DashboardController : ControllerBase
  {
    private readonly IEmployerDashboardQuery _employerDashboardQuery;
    public DashboardController(IEmployerDashboardQuery employerDashboardQuery)
    {
      _employerDashboardQuery = employerDashboardQuery;
    }
    [HttpGet("employer/{companyId:int}")]
    public async Task<IActionResult> GetEmployerDashboard([FromRoute] int companyId)
    {
      try
      {
        var dashboard = await _employerDashboardQuery.GetDashboardAsync(companyId);
        return Ok(dashboard);
      }
      catch (Exception ex)
      {
        return BadRequest(new { message = ex.Message });
      }
    }
  }
}
