using Microsoft.AspNetCore.Mvc;
using Planilla_Backend.CleanArchitecture.Application.External.Abstractions;
using Planilla_Backend.CleanArchitecture.Application.External.Models;

[ApiController]
[Route("api/external")]
public class ExternalPreviewController : ControllerBase
{
  private readonly IExternalPartnersService _partners;

  public ExternalPreviewController(IExternalPartnersService partners) => _partners = partners;

  [HttpGet("asociacionsolidarista")]
  [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
  public async Task<IActionResult> Aso([FromQuery] string cedulaEmpresa, [FromQuery] decimal salarioBruto, CancellationToken ct)
    => Ok(await _partners.GetAsociacionSolidaristaAsync(cedulaEmpresa, salarioBruto, ct));

  [HttpGet("seguroprivado")]
  [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
  public async Task<IActionResult> Seg([FromQuery] int edad, [FromQuery] int dependientes, CancellationToken ct)
    => Ok(await _partners.GetSeguroPrivadoAsync(edad, dependientes, ct));

  [HttpGet("pensiones")]
  [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
  public async Task<IActionResult> Pen([FromQuery] string tipo, [FromQuery] decimal salarioBruto, CancellationToken ct)
    => Ok(await _partners.GetPensionesVoluntariasAsync(tipo, salarioBruto, ct));
}
