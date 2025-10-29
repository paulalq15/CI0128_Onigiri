using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Planilla_Backend.CleanArchitecture.Application.External.Abstractions;

namespace Planilla_Backend.CleanArchitecture.Infrastructure.External;

public static class ExternalApisRegistration
{
  public static IServiceCollection AddExternalApis(this IServiceCollection services, IConfiguration cfg)
  {
    var timeoutSeconds = cfg.GetValue<int>("ExternalApis:TimeoutSeconds", 10);

    services.AddHttpClient<IExternalApiService, ExternalApiService>()
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .ConfigureHttpClient(c => c.Timeout = TimeSpan.FromSeconds(timeoutSeconds));

    services.AddScoped<IExternalPartnersService, ExternalPartnersService>();
    return services;
  }
}
