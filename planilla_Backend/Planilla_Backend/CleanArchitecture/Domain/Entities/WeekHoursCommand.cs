namespace Planilla_Backend.CleanArchitecture.Domain.Entities
{
  public class WeekHoursCommand
  {
    public DateTime WeekStart { get; set; }
    public List<DayEntryDto> Entries { get; set; } = new();
  }
}
