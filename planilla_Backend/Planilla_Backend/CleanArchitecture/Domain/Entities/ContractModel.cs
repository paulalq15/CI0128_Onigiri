namespace Planilla_Backend.CleanArchitecture.Domain.Entities
{
  public class ContractModel
  {
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public decimal Salary { get; set; }
    public string? PaymentAccount { get; set; }
    public string? Role { get; set; }
    public ContractType ContractType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
  }
}
