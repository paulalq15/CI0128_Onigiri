using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Application.Ports
{
  public interface IDashboardRepository
  {
    Task<List<EmployerDashboardCostByTypeModel>> GetEmployerDashboardCostByType(int companyId);
    Task<List<EmployerDashboardCostByMonthModel>> GetEmployerDashboardCostByMonth(int companyId);
    Task<List<EmployerDashboardEmployeeCountByTypeModel>> GetEmployerDashboardEmployeeCountByType(int companyId);
    Task<List<EmployeeDashboardEmployeeFiguresPerMonth>> GetEmployeeFiguresPerMonth(int employeeId);
  }
}
