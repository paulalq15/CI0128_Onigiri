namespace Planilla_Backend.CleanArchitecture.Domain.Reports
{
  public class PayrollDetailLine
  {
    public string Description { get; set; } = null!;
    public string Category { get; set; } = null!;
    public decimal Amount { get; set; }
    public int Order { get; set; }
  }
}
