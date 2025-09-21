using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PensionAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]  // URL: https://localhost:7019/api/controller

    public class API_Controller : ControllerBase {
        [HttpGet]
        public IActionResult Get(char? planType, double? grossSalary) {
            try {
                // Validate the sent parameters:
                if (planType == null || grossSalary == null) {
                    return BadRequest(new { error = "Bad Request: Missing parameters" });  // 400
                }

                if (double.IsNaN(grossSalary.Value)) {
                    return BadRequest(new { error = "Bad Request: 'parameter grossSalary' must be a number" });  // 400
                }

                if (grossSalary <= 0) {
                    return BadRequest(new { error = "Bad Request: 'parameter grossSalary' must be a positive decimal" });  // 400
                }

                double salary = grossSalary.Value;
                double employerContribution = 0;
                double employeeContribution = 0;

                switch (planType) {
                    case 'A':
                        employerContribution = salary * 0.035;
                        employeeContribution = salary * 0.03;
                        break;

                    case 'B':
                        employerContribution = salary * 0.055;
                        employeeContribution = salary * 0.05;
                        break;

                    case 'C':
                        employerContribution = salary * 0;
                        employeeContribution = salary * 0.01;
                        break;

                    default:
                        return BadRequest(new { error = "Bad Request: Parameter 'planType' can only be A, B or C" });  // 400
                }

                // 200:
                return Ok(new
                {
                    PlanType = planType,
                    GrossSalary = grossSalary,
                    EmployerContribution = employerContribution,
                    EmployeeContribution = employeeContribution,
                });
            }
            
            catch (Exception ex) {
                return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
            }
        }
    }
}
