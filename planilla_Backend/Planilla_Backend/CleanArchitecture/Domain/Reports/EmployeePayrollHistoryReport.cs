namespace Planilla_Backend.CleanArchitecture.Domain.Reports
{
  public class EmployeePayrollHistoryReport
  {
    public string CompanyName { get; set; }
    public int CompanyId { get; set; }
    public string EmployeeName { get; set; }
    public int EmployeeId { get; set; }
    public DateTime initialDate { get; set; }
    public DateTime finalDate { get; set; }

    public List<EmployeeHistoryRow> Rows { get; set; } = new();
  }
}
