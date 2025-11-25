namespace Planilla_Backend.CleanArchitecture.Domain.Entities
{
  public class WeekHoursQuery
  {
    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd { get; set; }
    public List<DayEntryDto> Entries { get; set; } = new();
  }
}
