using Planilla_Backend.CleanArchitecture.Domain.Entities;
using Planilla_Backend.CleanArchitecture.Infrastructure;

namespace Planilla_Backend.CleanArchitecture.Application.UseCases
{
  public class GetPayrollElementById : IGetPayrollElement
  {
    private readonly IPayrollElementRepository payrollElementRepository;
    private readonly int elementId;

    public GetPayrollElementById(IPayrollElementRepository payrollElementRepository, int elementId)
    {
      this.payrollElementRepository = payrollElementRepository;
      this.elementId = elementId;
    }

    public async Task<PayrollElementEntity?> Execute()
    {
      if (this.elementId <= 0) throw new ArgumentException("El parámetro elementId debe ser mayor que cero");

      PayrollElementEntity? payrollElementEntity = await this.payrollElementRepository.GetPayrollElementByElementId(this.elementId);

      return payrollElementEntity;
    }
  }
}
