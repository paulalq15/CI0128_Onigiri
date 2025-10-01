using Planilla_Backend.Models;
using Planilla_Backend.Repositories;

namespace Planilla_Backend.Services
{

  public class PayrollElementService
  {
    private const decimal maxElementValue = 99999999.99m;
    private const decimal maxPercentageValue = 100m;
    private const int maxElementNameLength = 40;
    private readonly PayrollElementRepository payrollElementRepository;
    public PayrollElementService(PayrollElementRepository payrollElementRepo)
    {
      payrollElementRepository = payrollElementRepo;
    }

    public string CreatePayrollElement(PayrollElementModel element)
    {
      try
      {
        element.ElementName = element.ElementName?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(element.ElementName) || element.ElementName.Length > maxElementNameLength)
          return "Nombre de elemento inválido. Debe tener entre 1 y " + maxElementNameLength.ToString() + "caracteres.";

        if (element.PaidBy != "Empleado" && element.PaidBy != "Empleador")
          return "El campo 'PagadoPor' debe ser 'Empleado' o 'Empleador'.";

        if (element.CalculationType != "Monto" && element.CalculationType != "Porcentaje" && element.CalculationType != "API")
          return "El campo 'Tipo' debe ser 'Monto', 'Porcentaje' o 'API'.";

        if (!payrollElementRepository.CheckUserType(element.UserId))
          return "El usuario actual no es un Empleador activo.";

        if (!payrollElementRepository.CheckCompanyStatus(element.CompanyId))
          return "La empresa no está activa.";

        if (element.CalculationValue < 0)
          return "El valor de cálculo no puede ser negativo.";
        else if (element.CalculationType == "Porcentaje" && element.CalculationValue > maxPercentageValue)
          return "Para tipo 'Porcentaje', el valor debe estar entre 0 y " + maxPercentageValue.ToString();
        else if (element.CalculationType == "Monto" && element.CalculationValue > maxElementValue)
          return "Para tipo 'Monto', el valor debe ser menor a " + maxElementValue.ToString();

        var isCreated = payrollElementRepository.CreatePayrollElement(element);
        if (!isCreated)
          return "Error al crear el elemento de planilla";
      }
      catch (Exception ex)
      {
        return $"Error creando elemento de planilla: {ex.Message}";
      }

      return string.Empty;
    }

    public async Task<List<PayrollElementModel>> GetPayrollElementsByIdCompany(int idCompany)
    {
      List<PayrollElementModel> payrollElementsList = await this.payrollElementRepository.GetPayrollElementsByIdCompany(idCompany)!;
      return payrollElementsList;
    }
  }
}
