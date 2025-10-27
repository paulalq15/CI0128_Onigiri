using Planilla_Backend.LayeredArchitecture.Models;
using Planilla_Backend.LayeredArchitecture.Repositories;

namespace Planilla_Backend.LayeredArchitecture.Services
{
  public class EmployeeService
  {
    private readonly EmployeeRepository _employeeRepo;

    public EmployeeService(EmployeeRepository employeeRepo)
    {
      _employeeRepo = employeeRepo;
    }

    public int RegisterEmployee(EmployeeModel employeeModel)
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
