using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planilla_Backend.CleanArchitecture.Application.Services;
using Planilla_Backend.CleanArchitecture.Application.UseCases;
using Planilla_Backend.LayeredArchitecture.Models;
using System.Security.Claims;

namespace Planilla_Backend.CleanArchitecture.API
{
  [Route("api/[controller]")]
  [ApiController]
  public class TimesheetController : ControllerBase
  {
    private readonly ITimesheetService _svc;
    public TimesheetController(ITimesheetService svc) => _svc = svc;

    [HttpPost("week/{personId:int}")]
    public async Task<IActionResult> SaveWeek([FromRoute] int personId, [FromBody] SaveWeekHoursRequest req, CancellationToken ct)
    {
      var employeeId = personId;
      await _svc.InsertWeekAsync(employeeId, req.WeekStart, req.Entries, ct);
      return NoContent();
    }
  }
}
