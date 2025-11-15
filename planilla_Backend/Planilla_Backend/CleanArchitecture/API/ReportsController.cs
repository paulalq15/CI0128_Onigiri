using Microsoft.AspNetCore.Mvc;
using Planilla_Backend.CleanArchitecture.API.Http;
using Planilla_Backend.CleanArchitecture.Application.Reports;

namespace Planilla_Backend.CleanArchitecture.API
{
  [Route("api/[controller]")]
  [ApiController]
  public class ReportsController : ControllerBase
  {
    private readonly IGenerateReportDataQuery _query;

    public ReportsController(IGenerateReportDataQuery query)
    {
      _query = query;
    }

    [HttpPost("data")]
    public async Task<IActionResult> GetReportData([FromBody] ReportRequestDto request, CancellationToken ct)
    {
      try
      {
        var result = await _query.ExecuteAsync(request, ct);
        return Ok(result);
      }
      catch (Exception ex)
      {
        return Error.FromException(this, ex, HttpContext.Request.Path);
      }
    }
  }
}
