using Planilla_Backend.CleanArchitecture.Application.UseCases;

namespace Planilla_Backend.CleanArchitecture.Application.Ports
{
  public interface ITimesheetRepository
  {
    Task SaveWeekAsync(int employeeId, DateTime weekStart, IEnumerable<DayEntryDto> entries, CancellationToken ct = default);
  }
}
