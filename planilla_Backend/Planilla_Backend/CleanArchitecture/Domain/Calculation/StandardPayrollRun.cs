using Planilla_Backend.CleanArchitecture.Domain.Calculation;
using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  //Template Method Design Pattern - Concrete Implementation
  public class StandardPayrollRun : PayrollTemplate
  {
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

    protected override List<PayrollDetailModel> ApplyConcepts(EmployeeModel employee, PayrollDetailModel @base, List<ElementModel> elements, PayrollContext ctx)
{
      throw new NotImplementedException();
    }

    protected override CompanyPayrollModel ConsolidateCompanyTotals(List<PayrollDetailModel> lines, PayrollContext ctx)
{
      throw new NotImplementedException();
    }

    protected override CompanyPayrollModel BuildCompanyPayrollModel(CompanyModel company, List<PayrollDetailModel> details, PayrollContext ctx)
{
      throw new NotImplementedException();
    }

    protected override List<PayrollDetailModel> PersistResults(CompanyPayrollModel header, List<PayrollDetailModel> details, PayrollContext ctx)
{
      throw new NotImplementedException();
    }
  }
}
