using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planilla_Backend.Models;
using Planilla_Backend.Services;

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
    }
}
