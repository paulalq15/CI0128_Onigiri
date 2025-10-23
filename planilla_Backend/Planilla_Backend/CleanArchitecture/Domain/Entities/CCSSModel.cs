using Planilla_Backend.CleanArchitecture.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Planilla_Backend.CleanArchitecture.Domain.Entities
{
  public class CCSSModel
  {
    public int Id { get; set; }
    required public string Category { get; set; }
    required public string Concept { get; set; }
    public decimal Rate { get; set; }
    public PayrollItemType ItemType { get; set; }
  }
}