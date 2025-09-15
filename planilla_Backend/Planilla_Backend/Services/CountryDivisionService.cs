using Planilla_Backend.Models;
using Planilla_Backend.Repositories;

namespace Planilla_Backend.Services
{
    public class CountryDivisionService
    {
        private readonly CountryDivisionRepository countryDivisionRepository;
        public CountryDivisionService()
        {
            countryDivisionRepository = new CountryDivisionRepository();
        }
        public List<ProvinciaModel> GetProvince()
        {
            return countryDivisionRepository.GetProvince();
        }
        public List<CantonModel> GetCounty(string provincia)
        {
            return countryDivisionRepository.GetCounty(provincia);
        }
        public List<DistritoModel> GetDistrict(string provincia, string canton)
        {
            return countryDivisionRepository.GetDistrict(provincia, canton);
        }
        public List<ZipCodeModel> GetZipCode(string provincia, string canton, string distrito)
        {
            return countryDivisionRepository.GetZipCode(provincia, canton, distrito);
        }
    }
}
