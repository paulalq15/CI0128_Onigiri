using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Domain.Calculation
{
  public interface IConceptStrategy
  {
    PayrollDetailModel Apply(PayrollDetailModel @base, ElementModel concept, PayrollContext ctx);
  }
}
