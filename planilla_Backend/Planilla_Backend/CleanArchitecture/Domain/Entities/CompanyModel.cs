namespace Planilla_Backend.CleanArchitecture.Domain.Entities
{
  public class CompanyModel
  {
    public int Id { get; set; }
    public PaymentFrequency PaymentFrequency { get; set; }
    public int PayDay1 { get; set; }
    public int? PayDay2 { get; set; }
  }
}
