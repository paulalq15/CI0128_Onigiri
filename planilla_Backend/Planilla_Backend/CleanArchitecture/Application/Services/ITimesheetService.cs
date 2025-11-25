using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Application.Services
{
  public interface ITimesheetService
  {
    Task InsertWeekAsync( int employeeId, DateTime weekStart, IReadOnlyList<DayEntryDto> entries, CancellationToken ct = default);
    Task<WeekHoursQuery> GetWeekHoursAsync(int employeeId, DateTime weekStart, DateTime weekEnd, CancellationToken ct = default);
  }
}
