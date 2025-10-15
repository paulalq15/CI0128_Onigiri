using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public class CalculationFactory
  {
    public ISalaryBaseStrategy CreateBaseStrategy(ContractModel contract, CompanyModel company)
    {
      // TODO: create instance and return the correct base salary strategy
      throw new NotImplementedException();
    }

    public IList<IConceptStrategy> CreateConceptStrategies(CompanyModel company, string period)
    {
      // TODO: create and return the list of concept strategies to apply
      throw new NotImplementedException();
    }
  }
}
