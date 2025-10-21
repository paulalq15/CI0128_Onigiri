using Planilla_Backend.LayeredArchitecture.Models;
using Planilla_Backend.LayeredArchitecture.Repositories;
using System.Text.RegularExpressions;

namespace Planilla_Backend.LayeredArchitecture.Services
{
  public class CompanyService
  {
    private readonly CompanyRepository createCompanyRepository;
    private readonly DirectionsRepository directionsRepository;
    private const int maxBenefitNumber = 255;
    private const int maxPaymentDay = 31;
    private const int maxNameLength = 150;
    private const int maxAddressLength = 250;

    public CompanyService(CompanyRepository createCompanyRepo, DirectionsRepository directionsRepository)
    {
      this.createCompanyRepository = createCompanyRepo;
      this.directionsRepository = directionsRepository;

    }
    public string CreateCompany(CompanyModel company, out int companyId)
    {
      var result = string.Empty;
      companyId = 0;
      try
      {
        //Validations and trimming
        company.CompanyName = company.CompanyName?.Trim() ?? string.Empty;
        company.AddressDetails = company.AddressDetails?.Trim() ?? string.Empty;
        company.Telephone = string.IsNullOrWhiteSpace(company.Telephone) ? null : company.Telephone.Trim();

        //REGEX #-###-######
        if (string.IsNullOrWhiteSpace(company.CompanyId) || !Regex.IsMatch(company.CompanyId, @"^\d-\d{3}-\d{6}$"))
          return "Cédula jurídica inválida. Formato esperado: X-XXX-XXXXXX";

        if (string.IsNullOrWhiteSpace(company.CompanyName) || company.CompanyName.Length > maxNameLength)
          return "Nombre de empresa inválido. Debe tener entre 1 y " + maxNameLength.ToString() + " caracteres.";

        //REGEX ####-####
        if (company.Telephone != null && !Regex.IsMatch(company.Telephone, @"^\d{4}-\d{4}$"))
          return "Teléfono inválido. Formato esperado: XXXX-XXXX";

        if (company.MaxBenefits < 0 || company.MaxBenefits > maxBenefitNumber)
          return "Cantidad de beneficios inválida. Debe estar entre 0 y " + maxBenefitNumber.ToString();

        if (string.IsNullOrWhiteSpace(company.PaymentFrequency) || (company.PaymentFrequency != "Quincenal"
          && company.PaymentFrequency != "Mensual"))
          return "Frecuencia de pago inválida. Debe ser 'Quincenal' o 'Mensual'.";

        if (company.PaymentFrequency == "Quincenal")
        {
          if (company.PayDay2 is null)
            return "Para pagos quincenales, el segundo día de pago es requerido.";
          if (company.PayDay1 < 1 || company.PayDay1 > maxPaymentDay)
            return "El día de pago debe estar entre 1 y " + maxPaymentDay.ToString();
          if (company.PayDay2 <= company.PayDay1)
            return "El segundo día de pago debe ser mayor que el primero.";
        }
        else
        {
          if (company.PayDay1 < 1 || company.PayDay1 > maxPaymentDay)
            return "Día de pago inválido, debe estar entre 1 y " + maxPaymentDay.ToString();
          if (company.PayDay2 is not null)
            return "Para pagos mensuales no se permite un segundo día de pago.";
        }

        if (string.IsNullOrWhiteSpace(company.AddressDetails) || company.AddressDetails.Length > maxAddressLength)
          return "Las otras señas deben tener entre 1 y " + maxAddressLength.ToString() + " caracteres.";

        //REGEX #####
        if (string.IsNullOrWhiteSpace(company.ZipCode) || !Regex.IsMatch(company.ZipCode, @"^\d{5}$"))
          return "Código postal inválido. Debe ser un número de 5 dígitos.";

        if (company.CreatedBy <= 0)
          return "El ID del usuario actual es inválido.";

        var userType = createCompanyRepository.GetUserType(company.CreatedBy);
        if (!string.Equals(userType?.Trim(), "Empleador", StringComparison.OrdinalIgnoreCase))
          return "El usuario actual no es un Empleador activo.";

        if (!createCompanyRepository.ZipExists(company.ZipCode))
          return "El código postal no existe.";

        if (createCompanyRepository.ValidateUniqueCompanyNationalId(company.CompanyId))
          return "Ya existe una empresa con la cédula jurídica ingresada.";

        companyId = createCompanyRepository.CreateCompany(company);
        if (companyId <= 0)
          return "Error al crear la empresa";
      }
      catch (Exception ex)
      {
        return $"Error creando empresa: {ex.Message}";
      }

      return string.Empty; ;
    }

    public (string error, IEnumerable<CompanySummaryModel> companies) GetCompaniesForUser(int userId, bool onlyActive = true)
    {
      try
      {
        if (userId <= 0)
          return ("El ID de usuario es inválido.", Enumerable.Empty<CompanySummaryModel>());

        var userType = createCompanyRepository.GetUserType(userId);
        if (!string.Equals(userType?.Trim(), "Empleador", StringComparison.OrdinalIgnoreCase))
          return ("El usuario actual no es un Empleador activo.", Enumerable.Empty<CompanySummaryModel>());

        var rows = createCompanyRepository.GetCompaniesByUser(userId, onlyActive);
        return (string.Empty, rows);
      }
      catch (Exception ex)
      {
        return ($"Error obteniendo empresas: {ex.Message}", Enumerable.Empty<CompanySummaryModel>());
      }
    }

    public List<CompanyModel> GetCompaniesWithStats(int employerId, int viewerUserId)
    {
      return createCompanyRepository.GetCompaniesWithStats(employerId, viewerUserId);
    }

    public List<CompanyModel> getCompanies(int employerId)
    {
      return this.createCompanyRepository.getCompanies(employerId);
    }

    public async Task<List<CompanySummaryModel>> GetAllCompaniesSummary()
    {
      return await this.createCompanyRepository.GetAllCompaniesSummary();
    }

    public async Task<CompanyModel?> GetCompanyByUniqueId(int companyUniqueId)
    {
      CompanyModel? company = await this.createCompanyRepository.GetCompanyByUniqueId(companyUniqueId);

      if (company == null) return null;

      company.Directions = await this.directionsRepository.GetCompanyDirectionsByCompanyUniqueId(companyUniqueId);

      return company;
    }

    public async Task<int> GetMaxBenefitsTakenInCompany(int companyUniqueId)
    {
      int maxBenefitsAmount = await this.createCompanyRepository.GetMaxBenefitsTakenInCompany(companyUniqueId);

      return maxBenefitsAmount;
    }

    public async Task<int> updateCompanyData(CompanyModel company)
    {
      int rowsAffected = await this.createCompanyRepository.UpdateCompanyData(company);

      return rowsAffected;
    }

    public async Task<CompanyModel> GetCompanyByID(int companyId)
    {
      return await this.createCompanyRepository.getCompanyByID(companyId);
    }
  }
}
