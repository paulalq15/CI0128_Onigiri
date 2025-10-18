using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planilla_Backend.LayeredArchitecture.Models;
using Planilla_Backend.LayeredArchitecture.Services;
using System.Diagnostics.Metrics;

namespace Planilla_Backend.LayeredArchitecture.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class CountryDivisionController : ControllerBase
  {
    private readonly CountryDivisionService countryDivisionService;

    public CountryDivisionController(CountryDivisionService countryDivisionServ)
    {
      countryDivisionService = countryDivisionServ;
    }

    [HttpGet("Provinces")]
    public List<DivisionModel> GetProvince()
    {
      var provinces = countryDivisionService.GetProvince();
      return provinces;
    }

    [HttpGet("Counties")]
    public List<DivisionModel> GetCounty(string province)
    {
      var counties = countryDivisionService.GetCounty(province);
      return counties;
    }

    [HttpGet("Districts")]
    public List<DivisionModel> GetDistrict(string province, string county)
    {
      var districts = countryDivisionService.GetDistrict(province, county);
      return districts;
    }

    [HttpGet("ZipCode")]
    public ActionResult<DivisionModel> GetZipCode(string province, string county, string district)
    {
      var zipCode = countryDivisionService.GetZipCode(province, county, district);
      if (zipCode is null) return NotFound();
      return zipCode;
    }
  }
}
