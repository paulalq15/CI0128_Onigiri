using Planilla_Backend.Models;
using Planilla_Backend.Repositories;

namespace Planilla_Backend.Services
{
  public class EmployeeService
  {
    private readonly EmployeeRepository _employeeRepo;

    public EmployeeService(EmployeeRepository employeeRepo)
    {
      _employeeRepo = employeeRepo;
    }

    public int RegisterEmployee(RegisterEmployee employeeModel)
    {
      try
      {
        int personId = _employeeRepo.saveEmployee(employeeModel);
        return personId;
      } catch (Exception) {
        return -1;
      }
    }
  }
}
