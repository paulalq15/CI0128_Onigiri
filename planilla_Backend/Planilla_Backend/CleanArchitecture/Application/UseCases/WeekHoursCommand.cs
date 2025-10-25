namespace Planilla_Backend.CleanArchitecture.Application.UseCases
{
  public class WeekHoursCommand
  {
    public DateTime WeekStart { get; set; }
    public List<DayEntryDto> Entries { get; set; } = new();
  }
}
