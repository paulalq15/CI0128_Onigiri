using Planilla_Backend.Models;
using Planilla_Backend.Repositories;

namespace Planilla_Backend.Services
{
  public class PersonUserService
  {
    private readonly PersonUserRepository personUserRepository;
    private readonly DirectionsRepository directionsRepository;

    public PersonUserService()
    {
      personUserRepository = new PersonUserRepository();
      directionsRepository = new DirectionsRepository();
    }

    // Servicio para guardar una nueva Persona y su Usuario
    public int savePersonUser(PersonUser person, string password)
    {
      try
      {
        int idUser = personUserRepository.savePersonAndUser(person, password);
        return idUser;
      } catch (Exception) {
        return -1;
      }
    }

    // Servicio para obtener una Persona&Usuario por correo y contraseña
    public PersonUser? getPersonUser(string email, string password)
    {
      try
      {
        PersonUser? personUser = personUserRepository.getPersonUserByCredentials(email, password);
        return personUser;
      } catch (Exception) {
        return null;
      }
    }

    // Servicio para guardar la dirección de una persona
    public int savePersonDirection(int idPerson, string zipCode , string otherSigns)
    {
      int idDivision = directionsRepository.getDivisionByZipCode(zipCode);

      if (idDivision != -1)
      {
        try
        {
          int idDirection = directionsRepository.saveDirection(idDivision, otherSigns, null, idPerson);
          return idDirection;
        } catch (Exception) {
          return -1;
        }
      }

      return -1;
    }

    public int getUserIdByEmail(string email)
    {
      try
      {
        int idPerson = personUserRepository.getUserIdByEmail(email);
        return idPerson;
      } catch (Exception) {
        return -1;
      }
    }

    public List<Person> getEmployees()
    {
      return personUserRepository.getEmployees();
    }
  }
}
