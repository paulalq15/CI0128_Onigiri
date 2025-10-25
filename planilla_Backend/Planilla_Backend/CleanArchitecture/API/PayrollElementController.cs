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
    private IGetPayrollElement getPayrollElementCommand;
    private IUpdatePayrollElement updatePayrollElementCommand;

    public PayrollElementController(IGetPayrollElement getPayrollElement, IUpdatePayrollElement updatePayrollElement)
    {
      this.getPayrollElementCommand = getPayrollElement;
      this.updatePayrollElementCommand = updatePayrollElement;
    }

    [HttpGet("{payrollElementId:int}")]
    public async Task<IActionResult> GetAPayrollElement(int payrollElementId)
    {
      try
      {
        PayrollElementEntity? element = await this.getPayrollElementCommand.Execute(payrollElementId);

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
        int affectedRows = await this.updatePayrollElementCommand.Execute(payrollElement);

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
