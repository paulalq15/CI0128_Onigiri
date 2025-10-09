using Planilla_Backend.Services.Email.EmailModels;

namespace Planilla_Backend.Services.Email.EmailTypes
{
  public class ActivationAccountEmail : IEmailType
  {
    public string GetSubject() => "Activación de cuenta";
    public string GetBody(object model)
    {
      var activationModel = (ActivationAccountModel)model;

      return $@"
          <div style='font-family: Arial, sans-serif; background-color: #f5f5f5; padding:20px;'>
            <div style='max-width:600px; margin:0 auto; background-color:#ffffff; border-radius:10px; box-shadow:0 0 10px rgba(0,0,0,0.1); padding:40px; text-align:center;'>
              <h2 style='color:#333333;'>Hola {activationModel.userName}, bienvenido al sistema ONIGIRI INTELLIGENT SOLUTIONS</h2>
              <p style='color:#555555; font-size:16px; line-height:1.5;'>
                Por favor, haga clic en el siguiente botón para activar su cuenta:
              </p>
              <a href='{activationModel.activationLink}' style='display:inline-block; padding:15px 25px; background-color:#28a745; color:#ffffff; text-decoration:none; font-size:16px; border-radius:5px; margin:20px 0;'>Activar Cuenta</a>
              <p style='color:#555555; font-size:14px; line-height:1.5;'>
                El enlace expira en 5 horas.<br/>
                Si usted no se ha registrado en nuestro sistema, por favor ignore este correo.
              </p>
              <p style='color:#888888; font-size:14px; margin-top:30px;'>
                Con mucha gratitud,<br/>
                El equipo de ONIGIRI INTELLIGENT SOLUTIONS
              </p>
            </div>
          </div>";
    }
  }
}
