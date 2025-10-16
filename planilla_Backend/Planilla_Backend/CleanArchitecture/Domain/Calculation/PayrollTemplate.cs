using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  //Template Method Design Pattern
  public abstract class PayrollTemplate
  {
    public List<PayrollDetailModel> RunCalculation(int companyId, DateTime dateFrom, DateTime dateTo, PayrollContext ctx)
    {
      var payrollDetails = new List<PayrollDetailModel>();

      List<EmployeeModel> employees = SelectEmployees(companyId, dateFrom, dateTo, ctx);

      int i = 0;
      while (i < employees.Count)
      {
        EmployeeModel employee = employees[i];

        ContractModel contract = SelectContract(employee, ctx);
        if (contract != null)
        {
          PayrollDetailModel baseLine = CreateEmployeeBaseLine(employee, contract, ctx);
          
          var emptyElements = new List<ElementModel>();
          List<PayrollDetailModel> conceptLines = ApplyConcepts(employee, baseLine, emptyElements, ctx);

          payrollDetails.Add(baseLine);

          int j = 0;
          while (j < conceptLines.Count)
          {
            payrollDetails.Add(conceptLines[j]);
            j++;
          }
        }

        i++;
      }

      return payrollDetails;
    }

    protected abstract List<EmployeeModel> SelectEmployees(int companyId, DateTime dateFrom, DateTime dateTo, PayrollContext ctx);
    protected abstract ContractModel SelectContract(EmployeeModel emp, PayrollContext ctx);
    protected abstract PayrollDetailModel CreateEmployeeBaseLine(EmployeeModel employee, ContractModel contract, PayrollContext ctx);
    protected abstract List<PayrollDetailModel> ApplyConcepts(EmployeeModel employee, PayrollDetailModel baseLine, List<ElementModel> elements, PayrollContext ctx);
  }
}
