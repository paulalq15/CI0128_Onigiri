using Planilla_Backend.CleanArchitecture.Infrastructure;
using Planilla_Backend.CleanArchitecture.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Planilla_Backend.CleanArchitecture.Application
{
  public class PayrollElementUseCase : IPayrollElementUseCase
  {
    private readonly IPayrollElementRepository payrollElementRepository;
    
    public PayrollElementUseCase(IPayrollElementRepository payrollElementRepository)
    {
      this.payrollElementRepository = payrollElementRepository;
    }

    public async Task<PayrollElementEntity?> GetPayrollElementByElementId(int elementId)
    {
      return await this.payrollElementRepository.GetPayrollElementByElementId(elementId);
    }

    public async Task<int> UpdatePayrollElement(PayrollElementEntity payrollElement)
    {
      if (payrollElement.ElementName.IsNullOrEmpty()) return -1;

      return await this.payrollElementRepository.UpdatePayrollElement(payrollElement);
    }
  }
}
