using Planilla_Backend.Models;
using Planilla_Backend.Repositories;
using System.Diagnostics.Metrics;

namespace Planilla_Backend.Services
{
    public class CreateCompanyService
    {
        private readonly CreateCompanyRepository createCompanyRepository;
        public CreateCompanyService()
        {
            createCompanyRepository = new CreateCompanyRepository();
        }
        public string CreateCompany(CreateCompanyModel company, out int companyId)
        {
            var result = string.Empty;
            companyId = 0;
            try
            {
                companyId = createCompanyRepository.CreateCompany(company);
                if (companyId <= 0)
                {
                    result = "Error al crear la empresa";
                }
            }
            catch (Exception ex)
            {
                result = $"Error creando empresa: {ex.Message}";
            }

            return result;
        }
    }
}
