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

    public async Task<EmployeeModel?> GetEmployeeByPersonId(int personId)
    {
      return await _employeeRepo.GetByEmployeeId(personId);
    }


    public Task<bool> UpdateSelf(int employeeId, EmployeeModel employeeModel, CancellationToken ct = default)
    {
      // Aquí podrías validar IBAN, longitud de teléfono, etc.
      return _employeeRepo.UpdateByEmployee(employeeId, employeeModel, ct);
    }

    public async Task<bool> UpdateAsEmployer(int employerId, int employeeId, EmployeeModel employeeModel, CancellationToken ct = default)
    {
      return await _employeeRepo.UpdateByEmployer(employerId, employeeId, employeeModel, ct);
    }





  }
}
