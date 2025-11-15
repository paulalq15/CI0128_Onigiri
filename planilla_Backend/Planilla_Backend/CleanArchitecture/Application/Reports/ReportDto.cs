using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Application.Reports
{
  public static class ReportCodes
  {
    public const string EmployeeDetailPayroll = "EmployeeDetailPayroll";
    public const string EmployeeHistoryPayroll = "EmployeeHistoryPayroll";
    public const string EmployerHistoryPayroll = "EmployerHistoryPayroll";
    public const string EmployerDetailPayroll = "EmployerDetailPayroll";
    public const string EmployerEmployeePayroll = "EmployerEmployeePayroll";
  }

  public class ReportRequestDto
  {
    public string ReportCode { get; set; } = null!;
    public int? CompanyId { get; set; }
    public int? EmployeeId { get; set; }
    public string? EmployeeNationalId { get; set; }
    public int? PayrollId { get; set; }
    public EmployeeType? EmployeeType { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
  }

  public class ReportResultDto
  {
    public string ReportCode { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public IReadOnlyList<string> Columns { get; set; } = Array.Empty<string>();
    public IReadOnlyList<Dictionary<string, object?>> Rows { get; set; } = Array.Empty<Dictionary<string, object?>>();
    public Dictionary<string, object?> ReportInfo { get; set; } = new Dictionary<string, object?>();
  }
}
