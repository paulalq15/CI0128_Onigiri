using Planilla_Backend.CleanArchitecture.Domain.Reports;

namespace Planilla_Backend.CleanArchitecture.Application.Ports
{
  public interface IReportRepository
  {
    Task<EmployeePayrollReport> GetEmployeePayrollReport(int payrollId, CancellationToken ct = default);
  }
}
