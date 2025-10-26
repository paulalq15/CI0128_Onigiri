namespace Planilla_Backend.CleanArchitecture.Domain.Entities
{
  public class PayrollElementEntity
  {
    public int IdElement {  get; set; }

    public string ElementName { get; set; }
    
    public string CalculationType { get; set; }

    public decimal CalculationValue { get; set; }
    
    public string PaidBy { get; set; }

    public string Status { get; set; }

    public int CompanyId { get; set; }
  }
}
