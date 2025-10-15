using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planilla_Backend.CleanArchitecture.Application.UseCases;
using Planilla_Backend.CleanArchitecture.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

    [HttpPost("create")]
    public async Task<ActionResult<PayrollSummary>> Create([FromQuery] int companyId, [FromQuery] DateOnly DateFrom, [FromQuery] DateOnly DateTo)
    {
      var result = await _create.Execute(companyId, DateFrom, DateTo);
      return Ok(result);
    }

    [HttpPost("pay")]
    public async Task<IActionResult> Pay([FromQuery] int payrollId)
    {
      await _pay.Execute(payrollId);
      return Ok();
    }

    [HttpGet("{id}/summary")]
    public async Task<ActionResult<PayrollSummary>> GetSummary([FromRoute] int id)
    {
      var result = await _get.Execute(id);
      return Ok(result);
    }
  }
}
