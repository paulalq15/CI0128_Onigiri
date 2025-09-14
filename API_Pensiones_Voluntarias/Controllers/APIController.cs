using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Pensiones_Voluntarias.Controllers {
    [Route("api/[controller]")]      // https://localhost:7019/api/controller
    [ApiController]

    public class APIController : ControllerBase {
        [HttpGet]
        public string Get() {
            return "---API---";
        }
    }
}
