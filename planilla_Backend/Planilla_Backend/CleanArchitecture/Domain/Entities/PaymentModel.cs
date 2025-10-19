using System;

namespace Planilla_Backend.CleanArchitecture.Domain.Entities
{
  public class PaymentModel
  {
    public int Id { get; set; }
    public int EmployeePayrollId { get; set; }
    public int CreatedBy { get; set; }
    public decimal Amount { get; set; }
    required public string PaymentRef { get; set; }
    public DateTime PaymentDate { get; set; }
  }
}
