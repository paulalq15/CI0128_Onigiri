namespace Planilla_Backend.CleanArchitecture.Application.UseCases
{
  public class WeekHoursDto
  {
    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd { get; set; }
    //public List<DateTime> Days { get; set; } = new();
    public List<DayEntryDto> Entries { get; set; } = new();
  }
}
