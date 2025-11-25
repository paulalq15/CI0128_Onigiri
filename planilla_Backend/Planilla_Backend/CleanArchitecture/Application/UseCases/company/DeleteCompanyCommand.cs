using Planilla_Backend.CleanArchitecture.Application.EmailsDTOs;
using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.LayeredArchitecture.Services.Email.EmailTypes;
using Planilla_Backend.LayeredArchitecture.Services.EmailService;

namespace Planilla_Backend.CleanArchitecture.Application.UseCases.company
{
  public class DeleteCompanyCommand : IDeleteCompanyCommand
  {
    private readonly ICompanyRepository companyRepository;
    IEmailService emailService;

    public DeleteCompanyCommand(ICompanyRepository createCompanyRepository, IEmailService emailService)
    {
      this.companyRepository = createCompanyRepository;
      this.emailService = emailService;
    }

    public async Task<int> Execute(int companyUniqueId, int personId)
    {
      if (personId <= 0) throw new ArgumentException("Id de persona inválido.");
      if (companyUniqueId <= 0) throw new ArgumentException("Id de la empresa inválido.");

      int isAdmin = await this.companyRepository.IsPersonAdmin(personId);

      if (isAdmin == 0)
      {
        int isOwner = await this.companyRepository.IsPersonOwnerOfCompany(companyUniqueId, personId);

        if (isOwner == 0) throw new KeyNotFoundException("No tiene la autorización para realizar esta acción.");

        if (isOwner == -1) throw new Exception("Error al verificar permisos de la persona.");
      }

      if (isAdmin == -1) throw new Exception("Error al verificar permisos de la persona.");

      // Obtenemos la información de los empleados antes de eliminar la empresa
      List<DeleteCompanyEmployeeDataDTO> deleteCompanyEmployeesData = await this.companyRepository.GetEmployeesEmailsAndUserNameInCómpanyByIdCompany(companyUniqueId);

      int rowsAffected = await this.companyRepository.DeleteCompanyByUniqueId(companyUniqueId);

      if (rowsAffected == -1) throw new Exception("Error al intentar eliminar la empresa.");

      // Enviamos los correos electrónicos a los empleados afectados
      await SendEmailsToEmployeesAsync(deleteCompanyEmployeesData);

      return rowsAffected;
    }

    private async Task SendEmailsToEmployeesAsync(List<DeleteCompanyEmployeeDataDTO> employeesData)
    {
      foreach (var employee in employeesData)
      {
        IEmailType email = new DeleteCompanyEmail(employee.UserName, employee.CompanyName);

        await emailService.SendEmail(employee.EmployeeEmail, email.GetSubject(), email.GetBody(null));
      }
    }
  }
}
