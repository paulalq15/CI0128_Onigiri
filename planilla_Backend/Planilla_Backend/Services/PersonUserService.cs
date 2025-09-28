using Planilla_Backend.Models;
using Planilla_Backend.Repositories;

namespace Planilla_Backend.Services
{
  public class PersonUserService
  {
    private readonly PersonUserRepository personUserRepository;

    public PersonUserService()
    {
      personUserRepository = new PersonUserRepository();
    }

    public List<Person> getEmployees()
    {
      return personUserRepository.getEmployees();
    }
  }
}
