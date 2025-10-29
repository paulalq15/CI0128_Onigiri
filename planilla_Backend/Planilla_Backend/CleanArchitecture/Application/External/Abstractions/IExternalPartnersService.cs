using Planilla_Backend.CleanArchitecture.Application.External.Models;

namespace Planilla_Backend.CleanArchitecture.Application.External.Abstractions
{
  public interface IExternalPartnersService
  {
    Task<ApiResponse?> GetAsociacionSolidaristaAsync(string companyId, decimal gross, CancellationToken ct);
    Task<ApiResponse?> GetSeguroPrivadoAsync(int age, int dependents, CancellationToken ct);
    Task<ApiResponse?> GetPensionesVoluntariasAsync(string type, decimal gross, CancellationToken ct);
  }
}
