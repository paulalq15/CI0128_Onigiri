using Planilla_Backend.CleanArchitecture.Application.UseCases;

namespace Planilla_Backend.CleanArchitecture.Application.Ports
{
  public interface ITimesheetRepository
  {
    Task SaveWeekAsync(int employeeId, DateTime weekStart, IEnumerable<DayEntryDto> entries, CancellationToken ct = default);
    Task<IReadOnlyList<DayEntryDto>> GetWeekHoursAsync(int employeeId, DateTime weekStart, DateTime weekEnd, CancellationToken ct = default);
  }
}
