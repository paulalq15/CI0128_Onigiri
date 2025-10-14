using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Entities
{
  public class CCSSModel
  {
    public int Id { get; set; }
    public string? Category { get; set; }
    public string? Concept { get; set; }
    public decimal Rate { get; set; }
    public PayrollItemType ItemType { get; set; }
  }
}