using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  //Template Method Design Pattern
  public abstract class PayrollTemplate
  {
    public PayrollSummary Run(int companyId, DateOnly dateFrom, DateOnly dateTo, PayrollContext ctx)
    {
      // TODO: flow orchestration
      throw new NotImplementedException();
    }
    protected abstract List<EmployeeModel> SelectEmployees(int companyId, DateOnly dateFrom, DateOnly dateTo, PayrollContext ctx);

    protected abstract ContractModel SelectContract(EmployeeModel emp, PayrollContext ctx);

    protected abstract PayrollDetailModel CreateBaseLine(EmployeeModel employee, ContractModel contract, PayrollContext ctx);

    protected abstract List<PayrollDetailModel> ApplyConcepts(EmployeeModel employee, PayrollDetailModel @base, List<ElementModel> elements, PayrollContext ctx);

    protected abstract CompanyPayrollModel ConsolidateCompanyTotals(List<PayrollDetailModel> lines, PayrollContext ctx);

    protected abstract CompanyPayrollModel BuildCompanyPayrollModel(CompanyModel company, List<PayrollDetailModel> details, PayrollContext ctx);

    protected abstract List<PayrollDetailModel> PersistResults(CompanyPayrollModel header, List<PayrollDetailModel> details, PayrollContext ctx);
  }
}
