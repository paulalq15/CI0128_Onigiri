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

    public async Task<int> Execute(PayrollElementEntity payrollElement)
    {
      if (payrollElement.ElementName.IsNullOrEmpty()) throw new ArgumentException("El nombre del elemento es obligatorio");

      int affectedRows = await this.payrollElementRepository.UpdatePayrollElement(payrollElement);
      
      return affectedRows;
    }
  }
}
