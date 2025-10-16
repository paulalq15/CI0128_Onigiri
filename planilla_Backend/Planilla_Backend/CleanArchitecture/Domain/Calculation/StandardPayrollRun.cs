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

    protected override List<EmployeeModel> SelectEmployees(int companyId, DateOnly dateFrom, DateOnly dateTo, PayrollContext ctx)
    {
      if (ctx == null) throw new ArgumentNullException(nameof(ctx));
      if (ctx.Employees == null) return new List<EmployeeModel>();

      var employeeList = new List<EmployeeModel>();
      var i = 0;
      while (i < ctx.Employees.Count)
      {
        var employee = ctx.Employees[i];
        if (employee != null) employeeList.Add(employee);
        i++;
      }
      return employeeList;
    }

    protected override ContractModel SelectContract(EmployeeModel emp, PayrollContext ctx)
    {
      if (emp == null) throw new ArgumentNullException(nameof(emp));
      if (ctx == null) throw new ArgumentNullException(nameof(ctx));
      if (ctx.Contracts == null || ctx.Contracts.Count == 0) return null;

      ContractModel activeContract = null;
      var i = 0;
      while (i < ctx.Contracts.Count)
      {
        var contract = ctx.Contracts[i];
        if (contract != null && contract.EmployeeId == emp.Id)
        {
          if (activeContract == null) activeContract = contract;
          else if (contract.StartDate > activeContract.StartDate) activeContract = contract;
        }
        i++;
      }
      return activeContract;
    }

    protected override PayrollDetailModel CreateEmployeeBaseLine(EmployeeModel employee, ContractModel contract, PayrollContext ctx)
    {
      if (employee == null) throw new ArgumentNullException(nameof(employee));
      if (contract == null) throw new ArgumentNullException(nameof(contract));
      if (ctx == null) throw new ArgumentNullException(nameof(ctx));

      EmployeePayrollModel employeePayroll = null;
      if (ctx.EmployeePayrollByEmployeeId != null)
      {
        ctx.EmployeePayrollByEmployeeId.TryGetValue(employee.Id, out employeePayroll);
      }

      if (employeePayroll == null)
      {
        employeePayroll = new EmployeePayrollModel();
        employeePayroll.Id = 0;
        employeePayroll.EmployeeId = employee.Id;
      }

      var strategy = _factory.CreateBaseStrategy(contract, ctx.Company);
      var line = strategy.CreateBaseLine(employeePayroll, contract, ctx);
      return line;
    }

    protected override List<PayrollDetailModel> ApplyConcepts(EmployeeModel employee, PayrollDetailModel baseLine, List<ElementModel> elements, PayrollContext ctx)
    {
      //TODO: Implement logic
      return new List<PayrollDetailModel>();
    }

  }
}
