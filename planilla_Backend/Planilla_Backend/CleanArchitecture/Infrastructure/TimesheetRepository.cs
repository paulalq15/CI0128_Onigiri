using Dapper;
using Microsoft.Data.SqlClient;
using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Application.UseCases;
using System.Data;

namespace Planilla_Backend.CleanArchitecture.Infrastructure
{
  public class TimesheetRepository : ITimesheetRepository
  {
    private readonly string _connectionString;
    public TimesheetRepository(IConfiguration cfg) => _connectionString = cfg.GetConnectionString("OnigiriContext")!;

    public async Task SaveWeekAsync(int employeeId, DateTime weekStart, IEnumerable<DayEntryDto> entries, CancellationToken ct = default)
    {
      var tvp = new DataTable();
      tvp.Columns.Add("Fecha", typeof(DateTime));
      tvp.Columns.Add("Horas", typeof(byte));
      tvp.Columns.Add("Descripcion", typeof(string));

      foreach (var e in entries)
      {
        if (e.Hours > 9) throw new ArgumentOutOfRangeException(nameof(e.Hours), "Máximo 9 horas.");
        tvp.Rows.Add(e.Date.Date, e.Hours, (object?)e.Description ?? DBNull.Value);
      }

      await using var connection = new SqlConnection(_connectionString);
      await connection.OpenAsync(ct);
      var p = new DynamicParameters();
      p.Add("@IdEmpleado", employeeId, DbType.Int32);
      p.Add("@Items", tvp.AsTableValuedParameter("dbo.HorasXSemana"));

      var cmd = new CommandDefinition("dbo.InsertarHorasXSemana", parameters: p, commandType: CommandType.StoredProcedure, cancellationToken: ct);

      await connection.ExecuteAsync(cmd);
    }

    public async Task<IReadOnlyList<DayEntryDto>> GetWeekHoursAsync(int employeeId, DateTime weekStart, DateTime weekEnd, CancellationToken ct = default)
    {
      if (employeeId <= 0) throw new ArgumentOutOfRangeException(nameof(employeeId));

      await using var connection = new SqlConnection(_connectionString);
      await connection.OpenAsync(ct);

      var p = new DynamicParameters();
      p.Add("@IdEmpleado", employeeId, DbType.Int32);
      p.Add("@InicioSemana", weekStart.Date, DbType.Date);
      p.Add("@FinSemana", weekEnd.Date, DbType.Date);

      var cmd = new CommandDefinition(
        commandText: "dbo.ObtenerHorasXSemana",
        parameters: p,
        commandType: CommandType.StoredProcedure,
        cancellationToken: ct
      );

      var rows = await connection.QueryAsync<DayEntryDto>(cmd);

      return rows.ToList().AsReadOnly();
    }
  }
}
