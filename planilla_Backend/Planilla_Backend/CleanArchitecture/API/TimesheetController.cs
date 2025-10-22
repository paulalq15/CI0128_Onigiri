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
    public async Task<IActionResult> SaveWeek([FromRoute] int personId, [FromBody] WeekHoursCommand req, CancellationToken ct)
    {
      var employeeId = personId;
      await _svc.InsertWeekAsync(employeeId, req.WeekStart, req.Entries, ct);
      return NoContent();
    }

    [HttpGet("week/{personId:int}")]
    public async Task<ActionResult<WeekHoursDto>> GetWeekHours([FromRoute] int personId, [FromQuery] DateTime? day, [FromQuery] DateTime? weekStart, [FromQuery] DateTime? weekEnd, CancellationToken ct)
    {
      if (personId <= 0)
        return BadRequest("IdEmpleado inválido.");

      DateTime start;
      DateTime end;

      if (weekStart.HasValue && weekEnd.HasValue)
      {
        start = weekStart.Value.Date;
        end = weekEnd.Value.Date;
      }
      else if (day.HasValue)
      {
        var d = day.Value.Date;
        int delta = ((int)d.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7; // alinea a lunes
        start = d.AddDays(-delta);
        end = start.AddDays(6);
      }
      else
      {
        return BadRequest("Debes enviar 'day' o bien 'weekStart' y 'weekEnd'.");
      }

      if (end < start) return BadRequest("'weekEnd' debe ser >= 'weekStart'.");
      if ((end - start).TotalDays != 6) return BadRequest("El rango debe cubrir exactamente una semana (7 días).");

      var result = await _svc.GetWeekHoursAsync(personId, start, end, ct);
      return Ok(result);
    }
  }
}
