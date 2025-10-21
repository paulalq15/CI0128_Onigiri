using Microsoft.AspNetCore.Mvc;
using Planilla_Backend.CleanArchitecture.Application.UseCases;
using Planilla_Backend.CleanArchitecture.Domain.Entities;
using Planilla_Backend.CleanArchitecture.Infrastructure;

namespace Planilla_Backend.CleanArchitecture.API
{
  [Route("api/[controller]")]
  [ApiController]
  public class PayrollElementController : ControllerBase
  {
    private readonly IPayrollElementRepository payrollElementRepository;

    public PayrollElementController(IPayrollElementRepository payrollElementRepository)
    {
      this.payrollElementRepository = payrollElementRepository;
    }

    [HttpGet("{payrollElementId:int}")]
    public async Task<IActionResult> GetAPayrollElement(int payrollElementId)
    {
      try
      {
        IGetPayrollElement getPayrollElementCommand = new GetPayrollElementById(this.payrollElementRepository, payrollElementId);
        PayrollElementEntity? element = await getPayrollElementCommand.Execute();

        return Ok(element);
      }
      catch (ArgumentException ex)
      {
        return BadRequest(new { message = ex.Message });
      }
    }

    [HttpPost("updPayrollElement")]
    public async Task<IActionResult> UpdatePayrollElement([FromBody] PayrollElementEntity payrollElement)
    {
      try
      {
        IUpdatePayrollElement updatePayrollElementCommand = new UpdatePayrollElement(this.payrollElementRepository, payrollElement);

        int affectedRows = await updatePayrollElementCommand.Execute();

        if (affectedRows == 0) return NotFound(new { message = "Error al actualizar el elemento de planilla" });

        return Ok(new { message = "Elemento de planilla actualizado correctamente" });
      }
      catch (ArgumentException ex)
      {
        return BadRequest(new { message = ex.Message });
      }
    }
  }
}
