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
    }
}
