using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  //Template Method Design Pattern
  public abstract class PayrollTemplate
  {
    public List<PayrollDetailModel> RunCalculation(int companyId, DateTime dateFrom, DateTime dateTo, PayrollContext ctx)
    {
      var payrollDetails = new List<PayrollDetailModel>();
      var employees = SelectEmployees(companyId, ctx);
      var i = 0;

      while (i < employees.Count)
      {
        var employee = employees[i];
        var contract = SelectContract(employee, dateFrom, dateTo, ctx);
        if (contract != null)
        {
          var baseLine = CreateEmployeeBaseLine(employee, contract, ctx);
          if (baseLine != null) payrollDetails.Add(baseLine);

          var legalLines = ApplyLegalConcepts(employee, contract, ctx);
          if (legalLines != null)
          {
            var lineIndex = 0;
            while (lineIndex < legalLines.Count)
            {
              var line = legalLines[lineIndex];
              if (line != null) payrollDetails.Add(line);
              lineIndex++;
            }
          }

          var conceptLines = ApplyConcepts(employee, ctx);
          if (conceptLines != null)
          {
            var lineIndex = 0;
            while (lineIndex < conceptLines.Count)
            {
              var line = conceptLines[lineIndex];
              if (line != null) payrollDetails.Add(line);
              lineIndex++;
            }
          }
        }

        i++;
      }

      return payrollDetails;
    }

    protected abstract List<EmployeeModel> SelectEmployees(int companyId, PayrollContext ctx);
    protected abstract ContractModel SelectContract(EmployeeModel employee, DateTime dateFrom, DateTime dateTo, PayrollContext ctx);
    protected abstract PayrollDetailModel CreateEmployeeBaseLine(EmployeeModel employee, ContractModel contract, PayrollContext ctx);
    protected abstract List<PayrollDetailModel> ApplyLegalConcepts(EmployeeModel employee, ContractModel contract, PayrollContext ctx);
    protected abstract List<PayrollDetailModel> ApplyConcepts(EmployeeModel employee, PayrollContext ctx);
  }
}