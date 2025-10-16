using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planilla_Backend.CleanArchitecture.Application.UseCases;
using Planilla_Backend.CleanArchitecture.Domain.Entities;
using Planilla_Backend.LayeredArchitecture.Models;

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
    public async Task<ActionResult<PayrollSummary>> Create([FromQuery] int companyId, [FromQuery] int personId, [FromQuery] DateTime DateFrom, [FromQuery] DateTime DateTo)
    {
      var result = await _create.Execute(companyId, personId, DateFrom, DateTo);
      return Ok(result);
    }

    [HttpPost("payment")]
    public async Task<IActionResult> Pay([FromQuery] int payrollId, [FromQuery] int personId)
    {
      await _pay.Execute(payrollId, personId);
      return Ok();
    }

    [HttpGet("summary")]
    public async Task<ActionResult<PayrollSummary>> GetSummary([FromQuery] int companyId)
    {
      var result = await _get.Execute(companyId);
      if (result == null) return NoContent();
      return Ok(result);
    }
  }
}
