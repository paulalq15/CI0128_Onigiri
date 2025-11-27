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
        var result = await _query.GenerateReportAsync(request, ct);
        return Ok(result);
      }

      catch (Exception ex)
      {
        return Error.FromException(this, ex, HttpContext.Request.Path);
      }
    }

    [HttpGet("employee/payroll-periods")]
    public async Task<ActionResult<IEnumerable<ReportPayrollPeriodDto>>> GetEmployeePayrollPeriods([FromQuery] int companyId, [FromQuery] int employeeId, [FromQuery] int top = 12)
    {
      var periods = await _query.GetEmployeePayrollPeriodsAsync(companyId, employeeId, top);
      return Ok(periods);
    }

    [HttpGet("employer/payroll-periods")]
    public async Task<ActionResult<IEnumerable<ReportPayrollPeriodDto>>> GetEmployerPayrollPeriods([FromQuery] int companyId, [FromQuery] int top = 10)
    {
      var periods = await _query.GetEmployerPayrollPeriodsAsync(companyId, top);
      return Ok(periods);
    }
  }
}
