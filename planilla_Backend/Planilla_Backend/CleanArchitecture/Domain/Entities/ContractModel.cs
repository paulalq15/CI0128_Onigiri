namespace Planilla_Backend.CleanArchitecture.Domain.Entities
{
  public class ContractModel
  {
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public decimal Salary { get; set; }
    public string? PaymentAccount { get; set; }
    public ContractType ContractType { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
  }
}
