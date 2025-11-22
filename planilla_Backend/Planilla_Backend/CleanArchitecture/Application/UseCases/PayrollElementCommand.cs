using Microsoft.IdentityModel.Tokens;
using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;
using Planilla_Backend.LayeredArchitecture.Services.Email.EmailTypes;
using Planilla_Backend.LayeredArchitecture.Services.EmailService;

namespace Planilla_Backend.CleanArchitecture.Application.UseCases
{
  public class PayrollElementCommand : IPayrollElementCommand
  {
    private readonly IPayrollElementRepository _payrollElementRepository;
    private readonly IPayrollElementQuery _payrollElementQuery;
    private readonly IEmailService _emailService;

    public PayrollElementCommand(IPayrollElementRepository payrollElementRepository, IPayrollElementQuery payrollElementQuery, IEmailService emailService)
    {
      _payrollElementRepository = payrollElementRepository;
      _payrollElementQuery = payrollElementQuery;
      _emailService = emailService;
    }

    public async Task<int> Update(PayrollElementEntity payrollElement)
    {
      if (payrollElement.ElementName.IsNullOrEmpty()) throw new ArgumentException("El nombre del elemento es obligatorio");

      int affectedRows = await _payrollElementRepository.UpdatePayrollElement(payrollElement);
      
      return affectedRows;
    }

    public async Task<int> Delete(int elementId)
    {
      if (elementId <= 0) throw new ArgumentException("El Id del elemento es obligatorio");
      if (await _payrollElementRepository.GetPayrollElementByElementId(elementId) is null) throw new ArgumentException("El elemento no existe");
      int result = await _payrollElementRepository.DeletePayrollElement(elementId);

      var affectedEmployees = await _payrollElementQuery.GetEmployeeEmails(elementId);

      foreach (var employee in affectedEmployees.Entries)
      {
        BenefitEliminationEmail benefitEliminationEmail = new BenefitEliminationEmail(employee);
        await _emailService.SendEmail(employee.EmployeeEmail, benefitEliminationEmail.GetSubject(), benefitEliminationEmail.GetBody(employee));
      }

      return result;
    }

  } // end class
}
