using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planilla_Backend.Models;
using Planilla_Backend.Services;
using System.Reflection.PortableExecutable;
using System.Text.Json;

namespace Planilla_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppliedElementController : Controller
    {
        private readonly AppliedElementService appliedElementService;

        public AppliedElementController()
        {
            appliedElementService = new AppliedElementService();
        }

        [HttpGet("getAppliedElements")]
        public List<AppliedElement> getAppliedElements([FromQuery] int employeeId)
        {
            return this.appliedElementService.getAppliedElements(employeeId);
        }

        [HttpPost("addAppliedElement")]
        public async Task<IActionResult> AddAppliedElement([FromBody] AppliedElement appliedElement)
        {
            if (appliedElement == null || !appliedElement.UserId.HasValue || !appliedElement.ElementId.HasValue)
            {
                return BadRequest("Invalid applied element data.");
            }

            this.appliedElementService.addAppliedElement(appliedElement);

            return CreatedAtAction(nameof(AddAppliedElement), new { id = appliedElement.ElementId }, appliedElement);
        }

        [HttpPost("deactivateAppliedElement")]
        public void deactivateAppliedElement([FromBody] AppliedElement appliedElement) {
            this.appliedElementService.deactivateAppliedElement(appliedElement);
        }
    }
}
