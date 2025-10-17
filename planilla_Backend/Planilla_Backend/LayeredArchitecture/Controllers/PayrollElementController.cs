using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planilla_Backend.LayeredArchitecture.Models;
using Planilla_Backend.LayeredArchitecture.Services;

namespace Planilla_Backend.LayeredArchitecture.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class PayrollElementController : ControllerBase
  {
    private readonly PayrollElementService payrollElementService;
    public PayrollElementController(PayrollElementService payrollElementServ)
    {
      payrollElementService = payrollElementServ;
    }

    [HttpPost]
    public async Task<ActionResult<bool>> CreatePayrollElement(PayrollElementModel element)
    {
      if (element == null)
      {
        return BadRequest();
      }

      var result = payrollElementService.CreatePayrollElement(element);
      if (string.IsNullOrEmpty(result))
      {
        return Ok(true);
      }
      else
      {
        return BadRequest(result);
      }
    }

    [HttpGet("GetPayRollElements")]
    public async Task<ActionResult<List<PayrollElementModel>>> GetPayrollElements(int idCompany)
    {
      var payrollElements = await this.payrollElementService.GetPayrollElementsByIdCompany(idCompany);

      if (payrollElements == null || !payrollElements.Any()) return NotFound("No se encontraron elementos de planilla");

      return Ok(payrollElements);
    }
  }
}
