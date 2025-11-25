using Microsoft.Data.SqlClient;

namespace Planilla_Backend.CleanArchitecture.Application.Ports
{
  public interface ICompanyRepository
  {
    Task<int> IsPersonAdmin(int employeerPersonId);

    Task<int> IsPersonOwnerOfCompany(int companyUniqueId, int employeerPersonId);

    Task<int> DeleteCompanyByUniqueId(int companyUniqueId);
  }
}
