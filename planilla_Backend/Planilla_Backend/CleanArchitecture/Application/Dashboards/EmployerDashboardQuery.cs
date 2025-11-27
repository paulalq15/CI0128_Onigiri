using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Application.Dashboards
{
  public class EmployerDashboardQuery : IEmployerDashboardQuery
  {
    private readonly IDashboardRepository _dashboardRepo;
    public EmployerDashboardQuery(IDashboardRepository dashboardRepo)
    {
      _dashboardRepo = dashboardRepo;
    }
    public async Task<EmployerDashboardDto> GetDashboardAsync(int companyId)
    {
      if (companyId <= 0) throw new ArgumentOutOfRangeException(nameof(companyId));

      var costByTypeTask = _dashboardRepo.GetEmployerDashboardCostByType(companyId);
      var costByMonthTask = _dashboardRepo.GetEmployerDashboardCostByMonth(companyId);
      var employeeCountTask = _dashboardRepo.GetEmployerDashboardEmployeeCountByType(companyId);

      await Task.WhenAll(costByTypeTask, costByMonthTask, employeeCountTask);

      return new EmployerDashboardDto
      {
        CostByTypes = costByTypeTask.Result,
        CostByMonth = costByMonthTask.Result,
        EmployeeCountByType = employeeCountTask.Result
      };
    }
  }
}
