using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Planilla_Backend.CleanArchitecture.API.Http
{
  public class Error
  {
    public static ObjectResult FromException(ControllerBase ctrl, Exception ex, string instance)
    {
      int status = MapStatus(ex);
      return new ObjectResult(new { message = ex.Message }) { StatusCode = status };
    }

    public static ObjectResult FromMessage(ControllerBase ctrl, int status, string message, string instance)
    {
      return new ObjectResult(new { message = message }) { StatusCode = status };
    }

    private static int MapStatus(Exception ex)
    {
      if (ex is ArgumentException) return (int)HttpStatusCode.BadRequest;
      if (ex is InvalidOperationException) return (int)HttpStatusCode.Conflict;
      if (ex is KeyNotFoundException) return (int)HttpStatusCode.NotFound;
      if (ex is UnauthorizedAccessException) return (int)HttpStatusCode.Unauthorized;
      if (ex is OperationCanceledException || ex is TimeoutException) return (int)HttpStatusCode.ServiceUnavailable;
      return (int)HttpStatusCode.InternalServerError;
    }
  }
}
