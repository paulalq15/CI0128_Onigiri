using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Application.Dashboards
{
  public interface IEmployerDashboardQuery
  {
    Task<EmployerDashboardDto> GetDashboardAsync(int companyId);
  }
}
