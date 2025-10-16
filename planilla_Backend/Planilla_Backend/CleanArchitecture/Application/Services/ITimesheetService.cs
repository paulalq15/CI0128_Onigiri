﻿using Planilla_Backend.CleanArchitecture.Application.UseCases;

namespace Planilla_Backend.CleanArchitecture.Application.Services
{
  public interface ITimesheetService
  {
    Task InsertWeekAsync( int employeeId, DateTime weekStart, IReadOnlyList<DayEntryDto> entries, CancellationToken ct = default);
  }
}
