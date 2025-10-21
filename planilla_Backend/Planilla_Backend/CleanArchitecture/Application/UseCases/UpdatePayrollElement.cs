using Microsoft.IdentityModel.Tokens;
using Planilla_Backend.CleanArchitecture.Domain.Entities;
using Planilla_Backend.CleanArchitecture.Infrastructure;

namespace Planilla_Backend.CleanArchitecture.Application.UseCases
{
  public class UpdatePayrollElement : IUpdatePayrollElement
  {
    private readonly IPayrollElementRepository payrollElementRepository;

    private readonly PayrollElementEntity payrollElement;

    public UpdatePayrollElement(IPayrollElementRepository payrollElementRepository, PayrollElementEntity payrollElement)
    {
      this.payrollElementRepository = payrollElementRepository;
      this.payrollElement = payrollElement;
    }

    public async Task<int> Execute()
    {
      if (this.payrollElement.ElementName.IsNullOrEmpty()) throw new ArgumentException("El nombre del elemento es obligatorio");

      int affectedRows = await this.payrollElementRepository.UpdatePayrollElement(this.payrollElement);
      
      return affectedRows;
    }

  }
}
