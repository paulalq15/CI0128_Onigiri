using System.Text.Json.Serialization;
namespace Planilla_Backend.CleanArchitecture.Application.External.Models
{
  public class ApiItem
  {
    [JsonPropertyName("type")]
    public string Type { get; set; } = default!; // "EE" = empleado, "ER" = empleador
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
  }

  public class ApiResponse
  {
    [JsonPropertyName("deductions")]
    public List<ApiItem> Deductions { get; set; } = new();
  }
}
