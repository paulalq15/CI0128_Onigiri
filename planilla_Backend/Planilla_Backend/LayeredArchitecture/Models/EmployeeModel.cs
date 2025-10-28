using Planilla_Backend.Models;

namespace Planilla_Backend.LayeredArchitecture.Models
{
  public class EmployeeModel
  {
    public Person? PersonData { get; set; }
    public User? UserData { get; set; }
    public Contract? ContractData { get; set; }
    public int CompanyUniqueId { get; set; }

    //Address
    public DirectionsModel? Direction { get; set; }
  }
}
