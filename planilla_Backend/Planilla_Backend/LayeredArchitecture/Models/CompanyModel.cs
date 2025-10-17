namespace Planilla_Backend.LayeredArchitecture.Models
{
  public class CompanyModel
  {
    //Company details
    public int CompanyUniqueId { get; set; } // For internal use only
    public string CompanyId { get; set; } // cedula juridica
    public string CompanyName { get; set; }
    public string? Telephone { get; set; }
    public int MaxBenefits { get; set; }
    public string PaymentFrequency { get; set; }
    public int PayDay1 { get; set; }
    public int? PayDay2 { get; set; }
    public int CreatedBy { get; set; }

    //Address details
    public string AddressDetails { get; set; }
    public string ZipCode { get; set; }

    public int? EmployeeCount { get; set; }
    public string? EmployerName { get; set; }
  }
}
