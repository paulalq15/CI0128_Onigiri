using Planilla_Backend.Models;
using Planilla_Backend.Repositories;

namespace Planilla_Backend.Services
{

    public class PayrollElementService
    {
        private const decimal MaxElementValue = 99999999.99m;
        private const decimal MaxPercentageValue = 100m;
        private const int MaxElementNameLength = 40;
        private readonly PayrollElementRepository payrollElementRepository;
        public PayrollElementService()
        {
            payrollElementRepository = new PayrollElementRepository();
        }

        public string CreatePayrollElement(PayrollElementModel element)
        {
            try
            {
                //Validaciones
                element.ElementName = element.ElementName?.Trim() ?? string.Empty;
                
                if (string.IsNullOrWhiteSpace(element.ElementName) || element.ElementName.Length > MaxElementNameLength)
                {
                    return "Nombre de elemento inválido. Debe tener entre 1 y " + MaxElementNameLength.ToString() + "caracteres.";
                }

                if (element.PaidBy != "Empleado" && element.PaidBy != "Empleador")
                {
                    return "El campo 'PagadoPor' debe ser 'Empleado' o 'Empleador'.";
                }

                if (element.CalculationType != "Monto" && element.CalculationType != "Porcentaje" && element.CalculationType != "API")
                {
                    return "El campo 'Tipo' debe ser 'Monto', 'Porcentaje' o 'API'.";
                }

                if (!payrollElementRepository.UserIsActiveEmployer(element.UserId))
                {
                    return "El usuario actual no es un Empleador activo.";
                }

                if (!payrollElementRepository.CompanyIsActive(element.CompanyId)) {
                    return "La empresa no está activa.";
                }

                //Agregar regla del API cuando se defina el ID de los otros grupos
                if (element.CalculationValue < 0)
                {
                    return "El valor de cálculo no puede ser negativo.";
                } 
                else if (element.CalculationType == "Porcentaje" && element.CalculationValue > MaxPercentageValue)
                {
                    return "Para tipo 'Porcentaje', el valor debe estar entre 0 y " + MaxPercentageValue.ToString();
                } 
                else if (element.CalculationType == "Monto" && element.CalculationValue > MaxElementValue)
                {
                    return "Para tipo 'Monto', el valor debe ser menor a " + MaxElementValue.ToString();
                }

                var isCreated = payrollElementRepository.CreatePayrollElement(element);
                if (!isCreated)
                {
                    return "Error al crear el elemento de planilla";
                }
            }
            catch (Exception ex)
            {
                return $"Error creando elemento de planilla: {ex.Message}";
            }

            return string.Empty;
        }
    }
}
