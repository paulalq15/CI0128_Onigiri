namespace Planilla_Backend.CleanArchitecture.Application.UseCases
{
  public class DayEntryDto
  {
    public DateTime Date { get; set; }
    public byte Hours { get; set; }
    public string? Description { get; set; }
  }
}
