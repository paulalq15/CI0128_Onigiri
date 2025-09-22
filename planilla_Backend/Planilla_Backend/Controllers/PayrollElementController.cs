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
    }
}
