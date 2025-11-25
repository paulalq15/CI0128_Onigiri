using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Application.UseCases.company
{
  public interface IDeleteCompanyCommand
  {
    Task<int> Execute(int companyUniqueId, int personId);
  }
}
