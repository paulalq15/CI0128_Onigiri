using Microsoft.AspNetCore.Mvc;
using Planilla_Backend.CleanArchitecture.Application.UseCases;
using Planilla_Backend.CleanArchitecture.Domain.Entities;
using Planilla_Backend.CleanArchitecture.API.Http;

namespace Planilla_Backend.CleanArchitecture.API
{
  [Route("api/[controller]")]
  [ApiController]
  public class PayrollController : ControllerBase
  {
    private readonly ICreatePayrollCommand _create;
    private readonly IPayPayrollCommand _pay;
    private readonly IGetPayrollSummaryQuery _get;

    public PayrollController(ICreatePayrollCommand create, IPayPayrollCommand pay, IGetPayrollSummaryQuery get)
    {
      _create = create;
      _pay = pay;
      _get = get;
    }

    [HttpPost]
    public async Task<ActionResult<PayrollSummary>> Create([FromQuery] int companyId, [FromQuery] int personId, [FromQuery] DateTime dateFrom, [FromQuery] DateTime dateTo)
    {
      try
      {
        var summary = await _create.Execute(companyId, personId, dateFrom, dateTo);
        return Created("/api/Payroll", summary);
      }
      catch (Exception ex)
      {
        return Error.FromException(this, ex, "/api/Payroll");
      }
    }

    [HttpPost("payment")]
    public async Task<ActionResult<PayrollSummary>> Pay([FromQuery] int payrollId, [FromQuery] int personId)
    {
      try
      {
        var summary = await _pay.Execute(payrollId, personId);
        return Ok(summary);
      }
      catch (Exception ex)
      {
        return Error.FromException(this, ex, "/api/Payroll/payment");
      }
    }

    [HttpGet("summary")]
    public async Task<ActionResult<PayrollSummary>> GetSummary([FromQuery] int companyId)
    {
      try
      {
        var result = await _get.Execute(companyId);
        if (result == null) return NoContent();
        return Ok(result);
      }
      catch (Exception ex)
      {
        return Error.FromException(this, ex, "/api/Payroll/summary");
      }
    }
  }
}
