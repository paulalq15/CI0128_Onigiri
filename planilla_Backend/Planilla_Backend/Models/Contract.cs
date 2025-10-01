namespace Planilla_Backend.Models
{
  public class Contract
  {
    public int IdContract { get; set; }
    public string Position { get; set; }
    public string Department { get; set; }
    public decimal Salary { get; set; }
    public string EmployeeType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string BankAccount { get; set; }
    public int PersonID { get; set; }
  }
}