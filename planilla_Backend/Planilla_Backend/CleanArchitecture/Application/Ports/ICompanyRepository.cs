using Microsoft.Data.SqlClient;
using Planilla_Backend.CleanArchitecture.Application.EmailsDTOs;

namespace Planilla_Backend.CleanArchitecture.Application.Ports
{
  public interface ICompanyRepository
  {
    Task<int> IsPersonAdmin(int employeerPersonId);

    Task<int> IsPersonOwnerOfCompany(int companyUniqueId, int employeerPersonId);

    Task<int> DeleteCompanyByUniqueId(int companyUniqueId);

    Task<List<DeleteCompanyEmployeeDataDTO>> GetEmployeesEmailsAndUserNameInCómpanyByIdCompany(int companyUniqueId);
  }
}
