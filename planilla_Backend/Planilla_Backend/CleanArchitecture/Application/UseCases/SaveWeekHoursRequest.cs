namespace Planilla_Backend.CleanArchitecture.Application.UseCases
{
  public class SaveWeekHoursRequest
  {
    public DateTime WeekStart { get; set; }
    public List<DayEntryDto> Entries { get; set; } = new();
  }
}
