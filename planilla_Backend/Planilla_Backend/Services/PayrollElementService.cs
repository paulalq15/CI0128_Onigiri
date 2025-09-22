using Planilla_Backend.Models;
using Planilla_Backend.Repositories;

namespace Planilla_Backend.Services
{

    public class PayrollElementService
    {
        private readonly PayrollElementRepository payrollElementRepository;
        public PayrollElementService()
        {
            payrollElementRepository = new PayrollElementRepository();
        }

        public string CreatePayrollElement(PayrollElementModel element)
        {
            var result = string.Empty;
            try
            {
                var isCreated = payrollElementRepository.CreatePayrollElement(element);
                if (!isCreated)
                {
                    result = "Error al crear el elemento de planilla";
                }
            }
            catch (Exception ex)
            {
                result = $"Error creando elemento de planilla: {ex.Message}";
            }

            return result;
        }
    }
}
