// Models/CompanySummaryModel.cs
namespace Planilla_Backend.LayeredArchitecture.Models
{
  public class CompanySummaryModel
  {
    public int CompanyUniqueId { get; set; }   // IdEmpresa
    public string CompanyId { get; set; }      // CedulaJuridica
    public string CompanyName { get; set; }    // Nombre
  }
}
