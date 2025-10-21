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
      _baseStrategies = baseStrategies ?? throw new ArgumentNullException("Las estrategias base son requeridas");
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
      if (contract == null) throw new ArgumentNullException("El contrato es requerido");

      foreach (var strategy in _baseStrategies)
      {
        if (strategy != null && strategy.Applicable(contract)) return strategy;
      }

      throw new InvalidOperationException("No hay estrategia base para el tipo de contrato");

    }

    public IConceptStrategy CreateConceptStrategies(ElementModel element)
    {
      if (element == null) throw new ArgumentNullException("El elemento es requerido");
      IConceptStrategy strategy;

      if (_conceptMap.TryGetValue(element.CalculationType, out strategy)) return strategy;

      throw new InvalidOperationException("No existe una estrategia para el tipo de cálculo del elemento");
    }

    public IList<ILegalConceptStrategy> CreateLegalConceptStrategies(ContractModel contract)
    {
      if (contract == null) throw new ArgumentNullException("El contrato es requerido");
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
