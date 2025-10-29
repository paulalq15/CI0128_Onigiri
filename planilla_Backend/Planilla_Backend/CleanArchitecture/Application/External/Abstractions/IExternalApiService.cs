using Planilla_Backend.CleanArchitecture.Application.External.Models;

namespace Planilla_Backend.CleanArchitecture.Application.External.Abstractions;

public interface IExternalApiService
{
  Task<ApiResponse?> GetDeductionsAsync(string baseUrl, string endpointPath, string bearerToken, IReadOnlyDictionary<string, string> queryParams, CancellationToken ct);
  Task<ApiResponse?> GetDeductionsAsyncRawAuth(string baseUrl, string endpointPath, string token, IReadOnlyDictionary<string, string> queryParams, CancellationToken ct);
}
