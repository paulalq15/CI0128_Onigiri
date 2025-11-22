using Microsoft.IdentityModel.Tokens;
using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Application.UseCases
{
  public class PayrollElementCommand : IPayrollElementCommand
  {
    private readonly IPayrollElementRepository payrollElementRepository;

    public PayrollElementCommand(IPayrollElementRepository payrollElementRepository)
    {
      this.payrollElementRepository = payrollElementRepository;
    }

    public async Task<int> Update(PayrollElementEntity payrollElement)
    {
      if (payrollElement.ElementName.IsNullOrEmpty()) throw new ArgumentException("El nombre del elemento es obligatorio");

      int affectedRows = await this.payrollElementRepository.UpdatePayrollElement(payrollElement);
      
      return affectedRows;
    }

    public async Task<int> Delete(int elementId)
    {
      if (elementId == 0) throw new ArgumentException("El Id del elemento es obligatorio");
      if (await this.payrollElementRepository.GetPayrollElementByElementId(elementId) is null) throw new ArgumentException("El elemento no existe");
      int result = await this.payrollElementRepository.DeletePayrollElement(elementId);
      return result;
    }

  } // end class
}
