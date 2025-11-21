using System.Data;
using Microsoft.Data.SqlClient;

namespace Planilla_Backend.CleanArchitecture.Application.Ports
{
  public interface IPayrollDbSession : IAsyncDisposable
  {
    SqlConnection Connection { get; }
    SqlTransaction? Transaction { get; }

    Task BeginAsync(IsolationLevel isolationLevel);
    Task CommitAsync();
    Task RollbackAsync();
  }
}