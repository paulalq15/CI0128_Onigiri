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

      await using var con = new SqlConnection(_connectionString);
      await con.OpenAsync(ct);
      var p = new DynamicParameters();
      p.Add("@IdEmpleado", employeeId, DbType.Int32);
      p.Add("@Items", tvp.AsTableValuedParameter("dbo.HorasXSemana"));

      var cmd = new CommandDefinition("dbo.InsertarHorasXSemana", parameters: p, commandType: CommandType.StoredProcedure, cancellationToken: ct);

      await con.ExecuteAsync(cmd);
    }
  }
}
