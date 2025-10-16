using Planilla_Backend.CleanArchitecture.Domain.Calculation;
using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  //Template Method Design Pattern - Concrete Implementation
  public class StandardPayrollRun : PayrollTemplate
  {
    private readonly CalculationFactory _factory;
    public StandardPayrollRun(CalculationFactory factory)
    {
      _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    // TODO: Implement the abstract methods with specific logic for standard payroll calculation
    protected override List<EmployeeModel> SelectEmployees(int companyId, DateOnly dateFrom, DateOnly dateTo, PayrollContext ctx)
    {
      throw new NotImplementedException();
    }

    protected override ContractModel SelectContract(EmployeeModel emp, PayrollContext ctx)
    {
      throw new NotImplementedException();
    }

    protected override PayrollDetailModel CreateBaseLine(EmployeeModel employee, ContractModel contract, PayrollContext ctx)
    {
      throw new NotImplementedException();
    }

    protected override List<PayrollDetailModel> ApplyConcepts(EmployeeModel employee, PayrollDetailModel baseLine, List<ElementModel> elements, PayrollContext ctx)
    {
      throw new NotImplementedException();
    }

  }
}
