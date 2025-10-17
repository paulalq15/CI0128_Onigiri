using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public class CalculationFactory
  {
    private readonly IEnumerable<ISalaryBaseStrategy> _baseStrategies;
    private readonly IEnumerable<IConceptStrategy> _conceptStrategies;
    private readonly IEnumerable<ILegalConceptStrategy> _legalConceptStrategies;

    public CalculationFactory(IEnumerable<ISalaryBaseStrategy> baseStrategies, IEnumerable<IConceptStrategy> conceptStrategies, IEnumerable<ILegalConceptStrategy> legalConceptStrategies)
    {
      _baseStrategies = baseStrategies ?? throw new ArgumentNullException(nameof(baseStrategies));
      _conceptStrategies = conceptStrategies ?? Array.Empty<IConceptStrategy>();
      _legalConceptStrategies = legalConceptStrategies ?? Array.Empty<ILegalConceptStrategy>();
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

    public IList<IConceptStrategy> CreateConceptStrategies(CompanyModel company, DateTime dateFrom, DateTime dateTo)
    {
      // TODO: create and return the list of concept strategies to apply
      return new List<IConceptStrategy>();
    }

    public IList<ILegalConceptStrategy> CreateLegalConceptStrategies(CompanyModel company, DateTime dateFrom, DateTime dateTo)
    {
      // TODO: create and return the list of concept strategies to apply
      return new List<ILegalConceptStrategy>();
    }
  }
}
