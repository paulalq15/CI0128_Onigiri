using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Application.Dashboards
{
    public interface IEmployeeDashboardQuery
    {
        Task<EmployeeDashboardDto> GetDashboardAsync(int employeeId);
    }
}
