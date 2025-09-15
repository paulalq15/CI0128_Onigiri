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

        [HttpGet("provincias")]
        public List<ProvinciaModel> GetProvincias()
        {
            var provincias = countryDivisionService.GetProvince();
            return provincias;
        }

        [HttpGet("cantones")]
        public List<CantonModel> GetCantones(string provincia)
        {
            var cantones = countryDivisionService.GetCounty(provincia);
            return cantones;
        }

        [HttpGet("distritos")]
        public List<DistritoModel> GetDistritos(string provincia, string canton)
        {
            var distritos = countryDivisionService.GetDistrict(provincia, canton);
            return distritos;
        }

        [HttpGet("zipCode")]
        public List<ZipCodeModel> GetZipCode(string provincia, string canton, string distrito)
        {
            var zipCode = countryDivisionService.GetZipCode(provincia, canton, distrito);
            return zipCode;
        }
    }
}
