namespace Planilla_Backend.LayeredArchitecture.Models
{
  public class PayrollElementModel
  {
    public string ElementName { get; set; }

    public string PaidBy { get; set; }

    public string CalculationType { get; set; }

    public decimal CalculationValue { get; set; }

    public int CompanyId { get; set; }

    public int UserId { get; set; }
    public string Status { get; set; }
  }
}
