using Planilla_Backend.Models;
using Planilla_Backend.Repositories;
using System.Diagnostics.Metrics;

namespace Planilla_Backend.Services
{
    public class CountryDivisionService
    {
        private readonly CountryDivisionRepository countryDivisionRepository;
        public CountryDivisionService()
        {
            countryDivisionRepository = new CountryDivisionRepository();
        }
        public List<DivisionModel> GetProvince()
        {
            return countryDivisionRepository.GetProvince();
        }
        public List<DivisionModel> GetCounty(string province)
        {
            return countryDivisionRepository.GetCounty(province);
        }
        public List<DivisionModel> GetDistrict(string province, string county)
        {
            return countryDivisionRepository.GetDistrict(province, county);
        }
        public DivisionModel? GetZipCode(string province, string county, string district)
        {
            return countryDivisionRepository.GetZipCode(province, county, district);
        }
    }
}
