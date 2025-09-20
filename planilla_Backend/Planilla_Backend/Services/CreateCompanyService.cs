using Planilla_Backend.Models;
using Planilla_Backend.Repositories;
using System.Text.RegularExpressions;

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
                //Validaciones
                company.CompanyName = company.CompanyName?.Trim() ?? string.Empty;
                company.AddressDetails = company.AddressDetails?.Trim() ?? string.Empty;
                company.Telephone = string.IsNullOrWhiteSpace(company.Telephone) ? null : company.Telephone.Trim();

                if (string.IsNullOrWhiteSpace(company.CompanyId) || !Regex.IsMatch(company.CompanyId, @"^\d-\d{3}-\d{6}$"))
                {
                    return "Cédula jurídica inválida. Formato esperado: X-XXX-XXXXXX";
                }

                if (string.IsNullOrWhiteSpace(company.CompanyName) || company.CompanyName.Length > 150)
                {
                    return "Nombre de empresa inválido. Debe tener entre 1 y 150 caracteres.";
                }

                if (company.Telephone != null && !Regex.IsMatch(company.Telephone, @"^\d{4}-\d{4}$"))
                {
                    return "Teléfono inválido. Formato esperado: XXXX-XXXX";
                }

                if (company.MaxBenefits < 0 || company.MaxBenefits > 255)
                {
                    return "Cantidad de beneficios inválida. Debe estar entre 0 y 255.";
                }

                if (string.IsNullOrWhiteSpace(company.PaymentFrequency) || 
                    (company.PaymentFrequency != "Quincenal" && 
                     company.PaymentFrequency != "Mensual"))
                {
                    return "Frecuencia de pago inválida. Debe ser 'Quincenal' o 'Mensual'.";
                }

                if (company.PaymentFrequency == "Quincenal")
                {
                    if (company.PayDay2 is null)
                    {
                        return "Para pagos quincenales, el segundo día de pago es requerido.";
                    }
                    if (company.PayDay1 < 1 || company.PayDay1 > 31)
                    {
                        return "El día de pago debe estar entre 1 y 31.";
                    }
                    if (company.PayDay2 <= company.PayDay1)
                    {
                        return "El segundo día de pago debe ser mayor que el primero.";
                    }
                }
                else
                {
                    if (company.PayDay1 < 1 || company.PayDay1 > 31)
                    {
                        return "Día de pago inválido, debe estar entre 1 y 31.";
                    }
                    if (company.PayDay2 is not null)
                    {
                        return "Para pagos mensuales no se permite un segundo día de pago.";
                    }
                }

                if (string.IsNullOrWhiteSpace(company.AddressDetails) || company.AddressDetails.Length > 250)
                {
                    return "Las otras señas deben tener entre 1 y 250 caracteres.";
                }

                if (string.IsNullOrWhiteSpace(company.ZipCode) || !Regex.IsMatch(company.ZipCode, @"^\d{5}$"))
                {
                    return "Código postal inválido. Debe ser un número de 5 dígitos.";
                }

                if (company.CreatedBy <= 0)
                {
                    return "El ID del usuario actual es inválido.";
                }

                if (!createCompanyRepository.UserIsActiveEmployer(company.CreatedBy))
                {
                    return "El usuario actual no es un Empleador activo.";
                }

                if (!createCompanyRepository.ZipExists(company.ZipCode))
                {
                    return "El código postal no existe.";
                }

                if (createCompanyRepository.CompanyExistsByCedula(company.CompanyId))
                {
                    return "Ya existe una empresa con la cédula jurídica ingresada.";
                }

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
