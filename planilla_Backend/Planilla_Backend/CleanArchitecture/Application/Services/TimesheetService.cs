﻿using Planilla_Backend.CleanArchitecture.Application.Ports;
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

      // Normaliza: semana inicia en lunes (puedes alinear con tu función SQL)
      var weekStartMonday = WeekStartMonday(weekStart.Date);
      var weekEnd = weekStartMonday.AddDays(6);

      // Validaciones de negocio del caso de uso (insert-only)
      //  - Horas en 0..9 (tu repo ya valida >9, aquí anticipamos errores)
      //  - Fechas dentro de la semana objetivo
      //  - No permitir duplicados en el payload para el mismo día
      var seenDates = new HashSet<DateTime>();
      foreach (var e in entries)
      {
        if (e is null) throw new ArgumentException("Entrada nula.", nameof(entries));

        var d = e.Date.Date;
        if (e.Hours > 9 || e.Hours < 0)
          throw new ArgumentOutOfRangeException(nameof(e.Hours), "Las horas deben estar entre 0 y 9.");

        if (d < weekStartMonday || d > weekEnd)
          throw new ArgumentException($"La fecha {d:yyyy-MM-dd} no pertenece a la semana {weekStartMonday:yyyy-MM-dd}..{weekEnd:yyyy-MM-dd}.");

        if (!seenDates.Add(d))
          throw new ArgumentException($"Fecha duplicada en el payload: {d:yyyy-MM-dd}.");
      }

      // (Opcional) orden estable para trazabilidad
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

      // Delegar a infraestructura (SP: dbo.InsertarHorasXSemana, UDTT: dbo.HorasXSemana)
      await _repo.SaveWeekAsync(employeeId, weekStartMonday, normalized);
    }

    private static DateTime WeekStartMonday(DateTime d)
    {
      // Alinea al lunes anterior (o mismo día si ya es lunes)
      int delta = ((int)d.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
      return d.AddDays(-delta).Date;
    }
  }
}
