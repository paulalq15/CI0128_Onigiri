using Planilla_Backend.CleanArchitecture.Application.Reports;
using Planilla_Backend.CleanArchitecture.Domain.Reports;

namespace Planilla_Backend.CleanArchitecture.Application.Ports
{
  public interface IReportRepository
  {
    Task<EmployeePayrollReport> GetEmployeePayrollReport(int payrollId, CancellationToken ct = default);
    Task<IEnumerable<ReportPayrollPeriodDto>> GetEmployeePayrollPeriodsAsync(int companyId, int employeeId, int top);
    Task<IEnumerable<EmployerHistoryRow>> GetEmployerHistoryCompaniesAsync(int? companyId, int employeeId, DateTime dateFrom, DateTime dateTo, CancellationToken ct = default);
    Task<EmployeePayrollHistoryReport> GetEmployeePayrollHistoryInDateRange(int employeeId, DateTime startPayrollDate, DateTime finalPayrollDate);
  }
}
