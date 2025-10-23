using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Application.UseCases;

namespace Planilla_Backend.CleanArchitecture.Application.Services
{
  public class TimesheetService : ITimesheetService
  {
    private readonly ITimesheetRepository _repo;

    public TimesheetService(ITimesheetRepository repo)
    {
      _repo = repo;
    }

    public async Task InsertWeekAsync(int employeeId, DateTime weekStart, IReadOnlyList<DayEntryDto> entries, CancellationToken ct = default)
    {
      if (employeeId <= 0) throw new ArgumentOutOfRangeException(nameof(employeeId));
      if (entries is null || entries.Count == 0)
        throw new ArgumentException("Se requieren entradas de días.", nameof(entries));

      var weekStartMonday = WeekStartMonday(weekStart.Date);
      var weekEnd = weekStartMonday.AddDays(4);

      var existingRows = await _repo.GetWeekHoursAsync(employeeId, weekStartMonday, weekEnd, ct);
      var existingDates = new HashSet<DateTime>(existingRows
        .Where(e => e.Hours > 0)
        .Select(e => e.Date.Date));

      var seenDates = new HashSet<DateTime>();
      foreach (var e in entries)
      {
        if (e is null) throw new ArgumentException("Entrada nula.", nameof(entries));

        var d = e.Date.Date;

        // ✅ Solo validamos las fechas que el usuario está enviando realmente
        if (d < weekStartMonday || d > weekEnd)
          throw new ArgumentException($"La fecha {d:yyyy-MM-dd} no pertenece a la semana {weekStartMonday:yyyy-MM-dd}..{weekEnd:yyyy-MM-dd}.");

        if (d > DateTime.Today)
          throw new ArgumentException($"No se pueden registrar horas en fechas futuras: {d:yyyy-MM-dd}.");

        if (existingDates.Contains(d))
          throw new ArgumentException($"Las horas del día {d:yyyy-MM-dd} ya fueron registradas y no pueden modificarse.");

        if (!seenDates.Add(d))
          throw new ArgumentException($"Fecha duplicada en el payload: {d:yyyy-MM-dd}.");

        if (e.Hours > 9 || e.Hours < 0)
          throw new ArgumentOutOfRangeException(nameof(e.Hours), "Las horas deben estar entre 0 y 9.");
      }

      // Normalizamos y ordenamos
      var normalized = entries
        .Select(e => new DayEntryDto
        {
          Date = e.Date.Date,
          Hours = e.Hours,
          Description = string.IsNullOrWhiteSpace(e.Description) ? null : e.Description!.Trim()
        })
        .OrderBy(e => e.Date)
        .ToList()
        .AsReadOnly();

      await _repo.SaveWeekAsync(employeeId, weekStartMonday, normalized);
    }

    public async Task<WeekHoursDto> GetWeekHoursAsync(int employeeId, DateTime weekStart, DateTime weekEnd, CancellationToken ct = default)
    {

      var rows = await _repo.GetWeekHoursAsync(employeeId, weekStart, weekEnd, ct);

      return new WeekHoursDto
      {
        WeekStart = weekStart,
        WeekEnd = weekEnd,
        Entries = rows.ToList()
      };
    }

    private static DateTime WeekStartMonday(DateTime d)
    {
      // Alinea al lunes anterior (o mismo día si ya es lunes)
      int delta = ((int)d.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
      return d.AddDays(-delta).Date;
    }
  }
}
