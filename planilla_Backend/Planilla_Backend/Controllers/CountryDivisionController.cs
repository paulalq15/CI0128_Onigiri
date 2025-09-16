using Planilla_Backend.Models;
using Planilla_Backend.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Planilla_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryDivisionController : ControllerBase
    {
        private readonly CountryDivisionService countryDivisionService;

        public CountryDivisionController()
        {
            countryDivisionService = new CountryDivisionService();
        }

        [HttpGet("Provinces")]
        public List<DivisionModel> GetProvince()
        {
            var provincias = countryDivisionService.GetProvince();
            return provincias;
        }

        [HttpGet("Counties")]
        public List<DivisionModel> GetCounty(string provincia)
        {
            var cantones = countryDivisionService.GetCounty(provincia);
            return cantones;
        }

        [HttpGet("Districts")]
        public List<DivisionModel> GetDistrict(string provincia, string canton)
        {
            var distritos = countryDivisionService.GetDistrict(provincia, canton);
            return distritos;
        }

        [HttpGet("ZipCode")]
        public List<DivisionModel> GetZipCode(string provincia, string canton, string distrito)
        {
            var zipCode = countryDivisionService.GetZipCode(provincia, canton, distrito);
            return zipCode;
        }
    }
}
