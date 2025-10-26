using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Application.UseCases
{
  public class GetPayrollElementById : IGetPayrollElement
  {
    private readonly IPayrollElementRepository payrollElementRepository;

    public GetPayrollElementById(IPayrollElementRepository payrollElementRepository)
    {
      this.payrollElementRepository = payrollElementRepository;
    }

    public async Task<PayrollElementEntity?> Execute(int payElementId)
    {
      if (payElementId <= 0) throw new ArgumentException("El parámetro elementId debe ser mayor que cero");

      PayrollElementEntity? payrollElementEntity = await this.payrollElementRepository.GetPayrollElementByElementId(payElementId);

      return payrollElementEntity;
    }
  }
}
