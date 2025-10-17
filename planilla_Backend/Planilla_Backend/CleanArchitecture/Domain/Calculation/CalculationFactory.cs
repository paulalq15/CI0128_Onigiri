using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public class CalculationFactory
  {
    private readonly IEnumerable<ISalaryBaseStrategy> _baseStrategies;
    private readonly IEnumerable<IConceptStrategy> _conceptStrategies;
    private readonly IEnumerable<ILegalConceptStrategy> _legalConceptStrategies;
    private readonly Dictionary<ElementCalculationType, IConceptStrategy> _conceptMap = new Dictionary<ElementCalculationType, IConceptStrategy>();

    public CalculationFactory(IEnumerable<ISalaryBaseStrategy> baseStrategies, IEnumerable<IConceptStrategy> conceptStrategies, IEnumerable<ILegalConceptStrategy> legalConceptStrategies)
    {
      _baseStrategies = baseStrategies ?? throw new ArgumentNullException(nameof(baseStrategies));
      _conceptStrategies = conceptStrategies ?? Array.Empty<IConceptStrategy>();
      _legalConceptStrategies = legalConceptStrategies ?? Array.Empty<ILegalConceptStrategy>();

      foreach (var strategy in _conceptStrategies)
      {
        if (strategy == null) continue;
        if (strategy is Concept_FixedAmountStrategy) _conceptMap[ElementCalculationType.FixedAmount] = strategy;
        else if (strategy is Concept_PercentageStrategy) _conceptMap[ElementCalculationType.Percentage] = strategy;
        else if (strategy is Concept_ApiStrategy) _conceptMap[ElementCalculationType.ExternalAPI] = strategy;
      }
    }

    public ISalaryBaseStrategy CreateBaseStrategy(ContractModel contract)
    {
      if (contract == null) throw new ArgumentNullException(nameof(contract));

      foreach (var strategy in _baseStrategies)
      {
        if (strategy != null && strategy.Applicable(contract)) return strategy;
      }

      throw new InvalidOperationException("No base salary strategy found for the contract type.");

    }

    public IConceptStrategy CreateConceptStrategies(ElementModel element)
    {
      if (element == null) throw new ArgumentNullException(nameof(element));
      IConceptStrategy strategy;

      if (_conceptMap.TryGetValue(element.CalculationType, out strategy)) return strategy;

      return null;
    }

    public IList<ILegalConceptStrategy> CreateLegalConceptStrategies(ContractModel contract)
    {
      if (contract == null) throw new ArgumentNullException(nameof(contract));
      var result = new List<ILegalConceptStrategy>();

      foreach (var strategy in _legalConceptStrategies)
      {
        if (strategy == null) continue;
        if (strategy.Applicable(contract)) result.Add(strategy);
      }

      return result;
    }
  }
}
