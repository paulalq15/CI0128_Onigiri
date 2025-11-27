namespace Planilla_Backend.CleanArchitecture.Domain.Entities
{
  public class EmployerDashboardCostByTypeModel
  {
    public string CostType { get; set; } = string.Empty;
    public decimal TotalCost { get; set; }
  }

  public class EmployerCostRow
  {
    public decimal? Salaries { get; set; }
    public decimal? SocialCosts { get; set; }
    public decimal? Benefits { get; set; }
  }

  public class EmployerDashboardCostByMonthModel
  {
    public string Month { get; set; } = string.Empty;
    public decimal TotalCost { get; set; }
  }

  public class EmployerDashboardEmployeeCountByTypeModel
  {
    public string EmployeeType { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
  }

  public class EmployerDashboardDto
  {
    public List<EmployerDashboardCostByTypeModel> CostByTypes { get; set; } = new();
    public List<EmployerDashboardCostByMonthModel> CostByMonth { get; set; } = new();
    public List<EmployerDashboardEmployeeCountByTypeModel> EmployeeCountByType { get; set; } = new();
  }

  public class EmployeeDashboardEmployeeFiguresPerMonth
  {
    public int GrossSalary { get; set; } = new();
    public int NetSalary { get; set; } = new();
  }

  public class EmployeeDashboardDto
  {
    public List<EmployeeDashboardEmployeeFiguresPerMonth> EmployeeFiguresPerMonth { get; set; } = new();
  }
}
