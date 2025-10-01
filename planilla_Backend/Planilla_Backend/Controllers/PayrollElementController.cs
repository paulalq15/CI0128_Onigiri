using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planilla_Backend.Models;
using Planilla_Backend.Services;

namespace Planilla_Backend.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class PayrollElementController : ControllerBase
  {
    private readonly PayrollElementService payrollElementService;
    public PayrollElementController()
    {
      payrollElementService = new PayrollElementService();
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

    [HttpGet("getPayrollElements")]
    public List<PayrollElementModel> getPayrollElements([FromQuery] string paidBy)
    {
      return this.payrollElementService.getPayrollElements(paidBy);
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
