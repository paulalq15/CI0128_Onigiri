namespace Planilla_Backend.Models
{
  public class AppliedElement
  {
    public int AppliedElementId { get; set; }
    public int ElementId { get; set; }
    public string ElementName { get; set; }
    public int UserId { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public string Status { get; set; }
  }
}
