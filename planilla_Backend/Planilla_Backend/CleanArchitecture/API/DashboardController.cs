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
    private readonly IEmployerDashboardQuery _employeeDashboardQuery;

    public DashboardController(IEmployerDashboardQuery employerDashboardQuery, IEmployerDashboardQuery employeeDashboardQuery)
    {
      _employerDashboardQuery = employerDashboardQuery;
      _employeeDashboardQuery = employeeDashboardQuery;
    }

    /*
    public DashboardController(IEmployerDashboardQuery employeeDashboardQuery)
    {
      _employeeDashboardQuery = employeeDashboardQuery;
    }*/

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

    [HttpGet("employee/{employeeId:int}")]
    public async Task<IActionResult> GetEmployeeDashboard([FromRoute] int employeeId)
    {
      try
      {
        var dashboard = await _employeeDashboardQuery.GetDashboardAsync(employeeId);
        return Ok(dashboard);
      }

      catch (Exception ex)
      {
        return BadRequest(new { message = ex.Message });
      }
    }
  }
}
