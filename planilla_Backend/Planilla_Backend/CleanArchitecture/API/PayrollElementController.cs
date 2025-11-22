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
    private IPayrollElementQuery _payrollElementQuery;
    private IPayrollElementCommand _payrollElementCommand;

    public PayrollElementController(IPayrollElementQuery payrollElementQuery, IPayrollElementCommand payrollElementCommand)
    {
      _payrollElementQuery = payrollElementQuery;
      _payrollElementCommand = payrollElementCommand;
    }

    [HttpGet("{payrollElementId:int}")]
    public async Task<IActionResult> GetAPayrollElement(int payrollElementId)
    {
      try
      {
        PayrollElementEntity? element = await this._payrollElementQuery.GetPayrollElement(payrollElementId);

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
        int affectedRows = await this._payrollElementCommand.Update(payrollElement);

        if (affectedRows == 0) return NotFound(new { message = "Error al actualizar el elemento de planilla" });

        return Ok(new { message = "Elemento de planilla actualizado correctamente" });
      }
      catch (ArgumentException ex)
      {
        return BadRequest(new { message = ex.Message });
      }
    }

    [HttpPost("{payrollElementId:int}")]
    public async Task<IActionResult> DeletePayrollElement(int payrollElementId)
    {
      try
      {
        int affectedRows = await this._payrollElementCommand.Delete(payrollElementId);
        if (affectedRows == 0) return NotFound(new { message = "Error al eliminar el elemento de planilla" });
        return Ok(new { message = "Elemento de planilla eliminado correctamente" });
      }
      catch (ArgumentException ex)
      {
        return BadRequest(new { message = ex.Message });
      }
    }
  } // end class
}
