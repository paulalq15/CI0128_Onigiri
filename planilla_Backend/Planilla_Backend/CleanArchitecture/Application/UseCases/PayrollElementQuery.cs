using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;
using Planilla_Backend.LayeredArchitecture.Services.Email.EmailModels;

namespace Planilla_Backend.CleanArchitecture.Application.UseCases
{
  public class PayrollElementQuery : IPayrollElementQuery
  {
    private readonly IPayrollElementRepository payrollElementRepository;

    public PayrollElementQuery(IPayrollElementRepository payrollElementRepository)
    {
      this.payrollElementRepository = payrollElementRepository;
    }

    public async Task<PayrollElementEntity?> GetPayrollElement(int payElementId)
    {
      if (payElementId <= 0) throw new ArgumentException("El parámetro elementId debe ser mayor que cero");

      PayrollElementEntity? payrollElementEntity = await this.payrollElementRepository.GetPayrollElementByElementId(payElementId);

      return payrollElementEntity;
    }

    public async Task<DeletePayrollElementEmailListDto> GetEmployeeEmails(int elementId)
    {
      if (elementId <= 0) throw new ArgumentException("El parámetro elementId debe ser mayor que cero");

      var items = await this.payrollElementRepository.GetEmployeeEmailsByAssignedElement(elementId);

      DeletePayrollElementEmailListDto emailList = new DeletePayrollElementEmailListDto
      {
        Entries = items.ToList()
      };

      return emailList;
    }

  } // end class
}
