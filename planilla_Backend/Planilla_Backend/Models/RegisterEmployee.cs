namespace Planilla_Backend.Models
{
  public class RegisterEmployee
  {
    public Person PersonData { get; set; }
    public User UserData { get; set; }
    public Contract ContractData { get; set; }
    public int CompanyUniqueId { get; set; }
  }
}
