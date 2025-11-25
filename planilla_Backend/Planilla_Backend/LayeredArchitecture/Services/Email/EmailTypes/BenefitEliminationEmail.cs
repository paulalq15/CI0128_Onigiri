using Planilla_Backend.LayeredArchitecture.Services.Email.EmailModels;

namespace Planilla_Backend.LayeredArchitecture.Services.Email.EmailTypes
{
  public class BenefitEliminationEmail : IEmailType
  {
    public readonly DeletePayrollElementEmailDto _deletePayrollElementEmailDto;
    public BenefitEliminationEmail(DeletePayrollElementEmailDto deletePayrollElementEmailDto)
    {
      _deletePayrollElementEmailDto = deletePayrollElementEmailDto;
    }
    public string GetSubject() => "Notificación de eliminación de beneficio";
    public string GetBody(object model)
    {
      return $@"
        <div style='font-family: Arial, sans-serif; background-color: #f5f5f5; padding:20px;'>
          <div style='max-width:600px; margin:0 auto; background-color:#ffffff; border-radius:10px; box-shadow:0 0 10px rgba(0,0,0,0.1); padding:40px; text-align:center;'>
            <h2 style='color:#333333;'>Hola {_deletePayrollElementEmailDto.EmployeeName},</h2>
            <p style='color:#555555; font-size:16px; line-height:1.5;'>
              Le informamos que el siguiente beneficio ha sido eliminado de su planilla:
            </p>
            <ul style='text-align:left; color:#555555; font-size:16px; line-height:1.5;'>
              <li><strong>Nombre del Beneficio:</strong> {_deletePayrollElementEmailDto.Benefit}</li>
              <li><strong>Fecha de Eliminación:</strong> {_deletePayrollElementEmailDto.EffectiveDate:dd/MM/yyyy}</li>
            </ul>
            <p style='color:#555555; font-size:14px; line-height:1.5;'>
              Si tiene alguna pregunta o necesita más información, no dude en ponerse en contacto con el departamento de recursos humanos.
            </p>
            <p style='color:#888888; font-size:14px; margin-top:30px;'>
              Atentamente,<br/>
              El equipo de ONIGIRI INTELLIGENT SOLUTIONS
            </p>
          </div>
        </div>";
    }
  }
}
