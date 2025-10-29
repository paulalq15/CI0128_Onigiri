using Microsoft.Extensions.Configuration;
using Planilla_Backend.CleanArchitecture.Application.External.Abstractions;
using Planilla_Backend.CleanArchitecture.Application.External.Models;

namespace Planilla_Backend.CleanArchitecture.Infrastructure.External;

public class ExternalPartnersService : IExternalPartnersService
{
  private readonly IExternalApiService _client;
  private readonly IConfiguration _cfg;

  public ExternalPartnersService(IExternalApiService client, IConfiguration cfg)
  {
    _client = client;
    _cfg = cfg;
  }

  public Task<ApiResponse?> GetAsociacionSolidaristaAsync(string companyId, decimal gross, CancellationToken ct)
  {
    var baseUrl = _cfg["ExternalApis:AsociacionSolidarista:BaseUrl"]!;
    var token = _cfg["ExternalApis:AsociacionSolidarista:Token"]!;
    var endpointPath = "api/asociacionsolidarista/aporte-empleado";

    return _client.GetDeductionsAsync(baseUrl, endpointPath, token,
      new Dictionary<string, string>
      {
        ["cedulaEmpresa"] = companyId,
        ["salarioBruto"] = gross.ToString(System.Globalization.CultureInfo.InvariantCulture)
      },
      ct);
  }

  public Task<ApiResponse?> GetSeguroPrivadoAsync(int age, int dependents, CancellationToken ct)
  {
    var baseUrl = _cfg["ExternalApis:SeguroPrivado:BaseUrl"]!;
    var token = _cfg["ExternalApis:SeguroPrivado:Token"]!;
    var endpointPath = "SeguroPrivado/seguro-privado";

    return _client.GetDeductionsAsyncRawAuth(baseUrl, endpointPath, token,
      new Dictionary<string, string>
      {
        ["edad"] = age.ToString(),
        ["dependientes"] = dependents.ToString(),
      },
      ct);
  }

  public Task<ApiResponse?> GetPensionesVoluntariasAsync(string type, decimal gross, CancellationToken ct)
  {
    var baseUrl = _cfg["ExternalApis:PensionesVoluntarias:BaseUrl"]!;
    var endpointPath = "";

    return _client.GetDeductionsAsync(baseUrl, endpointPath, string.Empty,
      new Dictionary<string, string>
      {
        ["planType"] = type, // "A" o "B"
        ["grossSalary"] = gross.ToString(System.Globalization.CultureInfo.InvariantCulture)
      },
      ct);
  }
}
