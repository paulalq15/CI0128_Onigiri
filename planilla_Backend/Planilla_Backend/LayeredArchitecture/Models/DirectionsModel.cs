namespace Planilla_Backend.Models
{
  public class DirectionsModel
  {
    public int IdDirection { get; set; }

    public int IdDivision { get; set; }

    public string? Province { get; set; }

    public string? Canton { get; set; }

    public string? District { get; set; }

    public string? ZipCode { get; set; }

    public string? OtherSigns { get; set; }

    public int? IdCompany { get; set; }

    public int? IntPerson { get; set; }
  }
}