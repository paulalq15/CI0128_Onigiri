using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Application.Dashboards
{
    public class EmployeeDashboardQuery : IEmployeeDashboardQuery
    {
        private readonly IDashboardRepository _dashboardRepo;

        public EmployeeDashboardQuery(IDashboardRepository dashboardRepo)
        {
            _dashboardRepo = dashboardRepo;
        }

        public async Task<EmployeeDashboardDto> GetDashboardAsync(int employeeId)
        {
            if (employeeId <= 0) throw new ArgumentOutOfRangeException(nameof(employeeId));

            var employeeFiguresPerMonth = this._dashboardRepo.GetEmployeeFiguresPerMonth(employeeId);

            await Task.WhenAll(employeeFiguresPerMonth);

            return new EmployeeDashboardDto
            {
                EmployeeFiguresPerMonth = employeeFiguresPerMonth.Result,
            };
        }
    }
}
