namespace Planilla_Backend.CleanArchitecture.Domain.Entities
{
  public enum ContractType
  {
    FixedSalary,
    ProfessionalServices
  }

  public enum PaymentFrequency
  {
    Monthly,
    Biweekly
  }

  public enum PayrollItemType
  {
    Base,
    Tax,
    Benefit,
    EmployeeDeduction,
    EmployerContribution
  }

  public enum ElementCalculationType
  {
    FixedAmount,
    Percentage,
    ExternalAPI
  }
}