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

    public PayrollController(ICreatePayrollCommand create)
    {
      _create = create;
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
  }
}
