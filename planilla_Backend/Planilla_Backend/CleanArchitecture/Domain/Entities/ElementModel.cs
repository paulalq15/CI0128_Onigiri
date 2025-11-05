using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Entities
{
  public class ElementModel
  {
    public int Id { get; set; }
    required public string Name { get; set; }
    public ElementCalculationType CalculationType { get; set; }
    public decimal Value { get; set; }
    public int EmployeeId { get; set; }
    public PayrollItemType ItemType { get; set; }
    public int? NumberOfDependents { get; set; }
    public string? PensionType { get; set; }
  }
}