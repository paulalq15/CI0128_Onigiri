using Microsoft.AspNetCore.Mvc;
using Planilla_Backend.CleanArchitecture.Application;
using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.API
{
  [Route("api/[controller]")]
  [ApiController]
  public class PayrollElementController : ControllerBase
  {
    private readonly IPayrollElementUseCase payrollElementUseCase;

    public PayrollElementController(IPayrollElementUseCase payrollElementService)
    {
      this.payrollElementUseCase = payrollElementService;
    }

    [HttpGet("{payrollElementId:int}")]
    public async Task<IActionResult> GetAPayrollElement(int payrollElementId)
    {
      PayrollElementEntity? element = await this.payrollElementUseCase.GetPayrollElementByElementId(payrollElementId);

      if (element == null) return NotFound(new { message = "Error al obtener el elemento de planilla" });

      return Ok(element);
    }

    [HttpPost("updPayrollElement")]
    public async Task<IActionResult> UpdatePayrollElement([FromBody] PayrollElementEntity payrollElement)
    {
      int rowsAffected = await this.payrollElementUseCase.UpdatePayrollElement(payrollElement);
      if (rowsAffected > 0)
      {
        return Ok(new { message = "Elemento de planilla actualizado correctamente" });
      }
      else if (rowsAffected == -1)
      {
        return BadRequest(new { message = "El nombre del elemento es obligatorio" });
      }
      else
      {
        return NotFound(new { message = "Error al actualizar el elemento de planilla" });
      }
    }
  }
}
