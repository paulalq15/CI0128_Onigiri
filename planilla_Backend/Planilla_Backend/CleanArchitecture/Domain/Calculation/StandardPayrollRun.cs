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
      _factory = factory ?? throw new ArgumentNullException("La fábrica de cálculo es requerida");
    }

    protected override List<EmployeeModel> SelectEmployees(int companyId, PayrollContext ctx)
    {
      if (ctx == null) throw new ArgumentNullException("El contexto de planilla es requerido");
      if (ctx.Employees == null) return new List<EmployeeModel>();

      var employeeList = new List<EmployeeModel>();
      var i = 0;
      while (i < ctx.Employees.Count)
      {
        var employee = ctx.Employees[i];
        if (employee != null && employee.CompanyId == companyId) employeeList.Add(employee);
        i++;
      }
      return employeeList;
    }

    protected override ContractModel SelectContract(EmployeeModel employee, DateTime dateFrom, DateTime dateTo, PayrollContext ctx)
    {
      if (employee == null) throw new ArgumentNullException("El empleado es requerido");
      if (ctx == null) throw new ArgumentNullException("El contexto de planilla es requerido");
      if (ctx.Contracts == null || ctx.Contracts.Count == 0) return null;

      ContractModel activeContract = null;
      var i = 0;

      // pick the overlapping contract with the latest StartDate
      while (i < ctx.Contracts.Count)
      {
        var contract = ctx.Contracts[i];
        if (contract != null && contract.EmployeeId == employee.Id)
        {
          var contractStart = contract.StartDate;
          var contractEnd = contract.EndDate.HasValue ? contract.EndDate.Value : DateTime.MaxValue;

          if ((contractStart <= dateTo) && (contractEnd >= dateFrom))
          {
            if (activeContract == null) activeContract = contract;
            else if (contractStart > activeContract.StartDate) activeContract = contract;
          }
        }
        i++;
      }
      if (activeContract == null) return null;
      return activeContract;
    }

    protected override PayrollDetailModel CreateEmployeeBaseLine(EmployeeModel employee, ContractModel contract, PayrollContext ctx)
    {
      if (employee == null) throw new ArgumentNullException("El empleado es requerido");
      if (contract == null) throw new ArgumentNullException("El contrato es requerido");
      if (ctx == null) throw new ArgumentNullException("El contexto de planilla es requerido");
      if (!ctx.EmployeePayrollByEmployeeId.TryGetValue(employee.Id, out var employeePayroll)) throw new ArgumentNullException("No se encontró la planilla del empleado");

      var strategy = _factory.CreateBaseStrategy(contract);
      var line = strategy.CreateBaseLine(employeePayroll, contract, ctx);
      return line;
    }

    protected override List<PayrollDetailModel> ApplyLegalConcepts(EmployeeModel employee, ContractModel contract, PayrollContext ctx)
    {
      if (employee == null) throw new ArgumentNullException("El empleado es requerido");
      if (ctx == null) throw new ArgumentNullException("El contexto de planilla es requerido");
      if (!ctx.EmployeePayrollByEmployeeId.TryGetValue(employee.Id, out var employeePayroll)) throw new ArgumentNullException("No se encontró la planilla del empleado");

      var payrollDetailLines = new List<PayrollDetailModel>();

      // get all applicable legal strategies based on the contract
      var legalStrategies = _factory.CreateLegalConceptStrategies(contract);
      foreach (var strategy in legalStrategies)
      {
        if (strategy == null) continue;
        var resultLines = strategy.Apply(employeePayroll, ctx);
        if (resultLines == null) continue;

        foreach (var line in resultLines)
        {
          if (line != null) payrollDetailLines.Add(line);
        }
      }

      return payrollDetailLines;
    }

    protected override List<PayrollDetailModel> ApplyConcepts(EmployeeModel employee, PayrollContext ctx)
    {
      if (employee == null) throw new ArgumentNullException("El empleado es requerido");
      if (ctx == null) throw new ArgumentNullException("El contexto de planilla es requerido");
      if (!ctx.EmployeePayrollByEmployeeId.TryGetValue(employee.Id, out var employeePayroll)) throw new ArgumentNullException("No se encontró la planilla del empleado");

      var payrollDetailLines = new List<PayrollDetailModel>();

      if (ctx.ElementsByEmployee == null) return payrollDetailLines;
      if (!ctx.ElementsByEmployee.TryGetValue(employee.Id, out var employeeElements)) return payrollDetailLines;

      foreach (var element in employeeElements)
      {
        if (element == null) continue;
        var strategy = _factory.CreateConceptStrategies(element);
        if (strategy == null) continue;
        var resultLines = strategy.Apply(employeePayroll, element, ctx, employee);
        if (resultLines == null) continue;
        foreach (var line in resultLines)
        {
          if (line != null) payrollDetailLines.Add(line);
        }
      }

      return payrollDetailLines;
    }

  }
}
