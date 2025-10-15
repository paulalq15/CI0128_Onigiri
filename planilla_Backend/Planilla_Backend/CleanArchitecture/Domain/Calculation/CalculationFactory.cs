using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public class CalculationFactory
  {
    private readonly IEnumerable<ISalaryBaseStrategy> _baseStrategies;
    private readonly IEnumerable<IConceptStrategy> _conceptStrategies;

    public CalculationFactory(IEnumerable<ISalaryBaseStrategy> baseStrategies, IEnumerable<IConceptStrategy> conceptStrategies)
    {
      _baseStrategies = baseStrategies ?? throw new ArgumentNullException(nameof(baseStrategies));
      _conceptStrategies = conceptStrategies ?? Array.Empty<IConceptStrategy>();
    }
    public ISalaryBaseStrategy CreateBaseStrategy(ContractModel contract, CompanyModel company)
    {
      if (contract == null) throw new ArgumentNullException(nameof(contract));

      ISalaryBaseStrategy selected = null;
      foreach (var s in _baseStrategies)
      {
        if (s != null && s.Applicable(contract))
        {
          selected = s;
          break;
        }
      }

      if (selected == null) throw new InvalidOperationException("No base salary strategy found for the contract type.");

      return selected;

    }

    public IList<IConceptStrategy> CreateConceptStrategies(CompanyModel company, string period)
    {
      // TODO: create and return the list of concept strategies to apply
      return new List<IConceptStrategy>();
    }
  }
}
