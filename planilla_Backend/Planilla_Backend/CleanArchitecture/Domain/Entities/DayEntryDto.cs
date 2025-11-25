namespace Planilla_Backend.CleanArchitecture.Domain.Entities
{
  public class DayEntryDto
  {
    public DateTime Date { get; set; }
    public byte Hours { get; set; }
    public string? Description { get; set; }
  }
}
