namespace Planilla_Backend.CleanArchitecture.Domain.Entities
{
  public class TaxModel
  {
    public int Id { get; set; }
    public decimal Min { get; set; }
    public decimal? Max { get; set; }
    public decimal Rate { get; set; }
  }
}