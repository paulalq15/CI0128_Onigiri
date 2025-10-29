using System.Net.Http.Json;
using System.Web;
using Planilla_Backend.CleanArchitecture.Application.External.Abstractions;
using Planilla_Backend.CleanArchitecture.Application.External.Models;
namespace Planilla_Backend.CleanArchitecture.Infrastructure.External;

public class ExternalApiService : IExternalApiService
{
  private readonly HttpClient _http;

  public ExternalApiService(HttpClient http) => _http = http;

  public async Task<ApiResponse?> GetDeductionsAsync(string baseUrl, string endpointPath, string bearerToken, IReadOnlyDictionary<string, string> queryParams, CancellationToken ct)
  {
    var ub = new UriBuilder($"{baseUrl.TrimEnd('/')}/{endpointPath.TrimStart('/')}");
    var qp = HttpUtility.ParseQueryString(string.Empty);
    foreach (var kv in queryParams)
      qp[kv.Key] = kv.Value;

    ub.Query = qp.ToString() ?? string.Empty;

    using var req = new HttpRequestMessage(HttpMethod.Get, ub.Uri);
    req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

    var resp = await _http.SendAsync(req, ct);
    if (!resp.IsSuccessStatusCode)
    {
      var body = await resp.Content.ReadAsStringAsync(ct);
      var code = (int)resp.StatusCode;
      throw code switch
      {
        400 => new InvalidOperationException($"[400] Bad Request del proveedor. {body}"),
        403 => new UnauthorizedAccessException($"[403] Forbidden (token faltante/ inválido). {body}"),
        500 => new Exception($"[500] Error interno del proveedor. {body}"),
        _ => new HttpRequestException($"Error HTTP externo {(int)resp.StatusCode} {resp.StatusCode}. {body}")
      };
    }

    return await resp.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken: ct);
  }

  public async Task<ApiResponse?> GetDeductionsAsyncRawAuth(string baseUrl, string endpointPath, string token, IReadOnlyDictionary<string, string> queryParams, CancellationToken ct)
  {
    using var client = new HttpClient();
    var ub = new UriBuilder(baseUrl.TrimEnd('/') + "/" + endpointPath.TrimStart('/'));
    var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
    foreach (var kv in queryParams)
      query[kv.Key] = kv.Value;
    ub.Query = query.ToString();

    var req = new HttpRequestMessage(HttpMethod.Get, ub.Uri);
    req.Headers.Add("Authorization", token);

    var resp = await client.SendAsync(req, ct);
    var body = await resp.Content.ReadAsStringAsync(ct);

    if (!resp.IsSuccessStatusCode)
      throw new InvalidOperationException($"[{(int)resp.StatusCode}] {resp.ReasonPhrase}. {body}");

    return System.Text.Json.JsonSerializer.Deserialize<ApiResponse>(body);
  }
}
