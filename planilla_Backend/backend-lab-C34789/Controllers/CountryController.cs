using backend_lab_C34789.Models;
using backend_lab_C34789.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend_lab_C34789.Controllers {
  [Route("api/[controller]")]
  [ApiController]
  public class CountryController : ControllerBase {
    private readonly CountryService countryService;

    public CountryController()
    {
      countryService = new CountryService();
    }

    [HttpGet]
    public List<CountryModel> Get() {
      var paises = countryService.GetCountries();
      return paises;
    }

    [HttpPost]
    public async Task<ActionResult<bool>> CreateCountry(CountryModel country)
    {
      if (country == null)
      {
        return BadRequest();
      }

      var result = countryService.CreateCountry(country);
      if (string.IsNullOrEmpty(result))
      {
        return Ok(result);
      } else
      {
        return BadRequest(result);
      }
    }
  }
}
