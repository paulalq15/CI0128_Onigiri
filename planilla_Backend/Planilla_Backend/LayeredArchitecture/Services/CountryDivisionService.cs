using Planilla_Backend.LayeredArchitecture.Models;
using Planilla_Backend.LayeredArchitecture.Repositories;
using System.Diagnostics.Metrics;

namespace Planilla_Backend.LayeredArchitecture.Services
{
  public class CountryDivisionService
  {
    private readonly CountryDivisionRepository countryDivisionRepository;
    public CountryDivisionService(CountryDivisionRepository countryDivisionRepo)
    {
      countryDivisionRepository = countryDivisionRepo;
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
