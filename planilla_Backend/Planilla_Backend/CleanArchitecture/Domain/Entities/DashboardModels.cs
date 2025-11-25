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
    List<EmployerDashboardCostByTypeModel> CostByTypes { get; set; } = new();
    List<EmployerDashboardCostByMonthModel> CostByMonth { get; set; } = new();
    List<EmployerDashboardEmployeeCountByTypeModel> EmployeeCountByType { get; set; } = new();
  }
}
