using Planilla_Backend.Models;
using Planilla_Backend.Repositories;

namespace Planilla_Backend.Services
{
  public class PersonaUsuarioService
  {
    private readonly PersonaUsuarioRepository personaUsuarioRepository;
    private readonly DirectionsRepository directionsRepository;

    public PersonaUsuarioService()
    {
      personaUsuarioRepository = new PersonaUsuarioRepository();
      directionsRepository = new DirectionsRepository();
    }

    // Servicio para guardar una nueva Persona y su Usuario
    public int savePersonaUsuario(PersonaUsuario persona, string password)
    {
      try
      {
        int idUsuario = personaUsuarioRepository.savePersonaAndUsuario(persona, password);
        return idUsuario;
      } catch (Exception) {
        // Retornamos un valor de error (-1)
        return -1;
      }
    }

    // Servicio para obtener una Persona&Usuario por correo y contraseña
    public PersonaUsuario? GetPersonaUsuario(string email, string password)
    {
      try
      {
        PersonaUsuario? personaUsuario = personaUsuarioRepository.GetPersonaUsuarioByCredentials(email, password);
        return personaUsuario;
      } catch (Exception) {
        // Retornar null en caso de error
        return null;
      }
    }

    // Servicio para guardar la dirección de una persona
    public int savePersonaDirection(int idPersona, string zipCode , string otherSigns)
    {
      // Obtener el id de la división territorial
      int idDivision = directionsRepository.getDivisionByZipCode(zipCode);

      if (idDivision != -1)
      {
        try
        {
          // Obtener el id de direccion
          int idDirection = directionsRepository.saveDirection(idDivision, otherSigns, null, idPersona);
          return idDirection;
        } catch (Exception) {
          // Retornar -1 en caso de error al insertar
          return -1;
        }
      }

      // Retornar -1 si no se encontró el zip code
      return -1;
    }

    public int getUserIdByEmail(string email)
    {
      try
      {
        int idPersona = personaUsuarioRepository.getUserIdByEmail(email);
        return idPersona;
      } catch (Exception) {
        // Retornar -1 en caso de error
        return -1;
      }
    }
  }
}
