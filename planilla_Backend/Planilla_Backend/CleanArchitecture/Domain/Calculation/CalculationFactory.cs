using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public class CalculationFactory
  {
    private readonly Salary_FixedStrategy _fixedStrategy;
    private readonly Salary_HourlyStrategy _hourlyStrategy;

    public CalculationFactory(Salary_FixedStrategy fixedStrategy, Salary_HourlyStrategy hourlyStrategy)
    {
      _fixedStrategy = fixedStrategy ?? throw new ArgumentNullException(nameof(fixedStrategy));
      _hourlyStrategy = hourlyStrategy ?? throw new ArgumentNullException(nameof(hourlyStrategy));
    }
    public ISalaryBaseStrategy CreateBaseStrategy(ContractModel contract, CompanyModel company)
    {
      if (contract == null) throw new ArgumentNullException(nameof(contract));

      if (_hourlyStrategy.Applicable(contract)) return (ISalaryBaseStrategy)_hourlyStrategy;
      if (_fixedStrategy.Applicable(contract)) return (ISalaryBaseStrategy)_fixedStrategy;

      throw new InvalidOperationException("No base salary strategy found for the contract type.");

    }

    public IList<IConceptStrategy> CreateConceptStrategies(CompanyModel company, string period)
    {
      // TODO: create and return the list of concept strategies to apply
      return new List<IConceptStrategy>();
    }
  }
}
