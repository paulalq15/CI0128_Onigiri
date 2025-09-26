using Planilla_Backend.Models;
using Planilla_Backend.Repositories;

namespace Planilla_Backend.Services
{
    public class EmployeeService
    {
        private readonly EmployeeRepository employeeRepository;

        public EmployeeService()
        {
            employeeRepository = new EmployeeRepository();
        }

        public List<EmployeeModel> GetEmployees()
        {
            // Add any missing business logic when it is neccesary.
            return employeeRepository.GetEmployees();
        }
    }
}
