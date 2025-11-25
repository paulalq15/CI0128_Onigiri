using System.Data;
using Microsoft.Data.SqlClient;
using Planilla_Backend.CleanArchitecture.Application.Ports;

namespace Planilla_Backend.CleanArchitecture.Infrastructure
{
  public sealed class PayrollDbSession : IPayrollDbSession
  {
    private readonly string _connectionString;

    public SqlConnection Connection { get; }
    public SqlTransaction? Transaction { get; private set; }

    public PayrollDbSession(IConfiguration config)
    {
      _connectionString = config.GetConnectionString("OnigiriContext");
      Connection = new SqlConnection(_connectionString);
    }

    public async Task BeginAsync(IsolationLevel isolationLevel)
    {
      if (Connection.State != ConnectionState.Open)
        await Connection.OpenAsync();

      Transaction = (SqlTransaction)await Connection.BeginTransactionAsync(isolationLevel);
    }

    public async Task CommitAsync()
    {
      if (Transaction == null) return;

      await Transaction.CommitAsync();
      await Connection.CloseAsync();
      Transaction = null;
    }

    public async Task RollbackAsync()
    {
      if (Transaction == null) return;

      await Transaction.RollbackAsync();
      await Connection.CloseAsync();
      Transaction = null;
    }

    public async ValueTask DisposeAsync()
    {
      if (Transaction != null)
        await RollbackAsync();

      await Connection.DisposeAsync();
    }
  }
}