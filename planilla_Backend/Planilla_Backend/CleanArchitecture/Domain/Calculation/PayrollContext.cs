using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public class PayrollContext
  {
    public CompanyModel Company { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public IList<ContractModel> Contracts { get; set; }
    public IList<CCSSModel> CCSSRates { get; set; }
    public IList<TaxModel> TaxBrackets { get; set; }

    // Per-employee assigned elements. Key: EmployeeId, Value: list of elements for that employee
    public IDictionary<int, IList<ElementModel>> ElementsByEmployee { get; set; }

    // Only for hourly contracts. Key: EmployeeId, Value: hours in the period
    public IDictionary<int, decimal> HoursByEmployee { get; set; }

    public IList<EmployeeModel> Employees { get; set; }
    public IDictionary<int, EmployeePayrollModel> EmployeePayrollByEmployeeId { get; set; }

    public PayrollContext()
    {
      Contracts = new List<ContractModel>();
      ElementsByEmployee = new Dictionary<int, IList<ElementModel>>();
      CCSSRates = new List<CCSSModel>();
      TaxBrackets = new List<TaxModel>();
      HoursByEmployee = new Dictionary<int, decimal>();
      Employees = new List<EmployeeModel>();
      EmployeePayrollByEmployeeId = new Dictionary<int, EmployeePayrollModel>();
    }

  }
}
