using Planilla_Backend.CleanArchitecture.Application.Ports;

namespace Planilla_Backend.CleanArchitecture.Application.UseCases.company
{
  public class DeleteCompanyCommand : IDeleteCompanyCommand
  {
    private readonly ICompanyRepository companyRepository;

    public DeleteCompanyCommand(ICompanyRepository createCompanyRepository)
    {
      this.companyRepository = createCompanyRepository;
    }

    public async Task<int> Execute(int companyUniqueId, int personId)
    {
      if (personId < 0) throw new ArgumentException("Id de persona inválido");
      if (companyUniqueId < 0) throw new ArgumentException("Id de la empresa inválido");

      int isAdmin = await this.companyRepository.IsPersonAdmin(personId);

      if (isAdmin != 1)
      {
        int isOwner = await this.companyRepository.IsPersonOwnerOfCompany(companyUniqueId, personId);

        if (isOwner != 1) throw new KeyNotFoundException("No tiene la autorización para realizar esta acción");
      }

      int rowsAffected = await this.companyRepository.DeleteCompanyByUniqueId(companyUniqueId);

      return rowsAffected;
    }
  }
}
