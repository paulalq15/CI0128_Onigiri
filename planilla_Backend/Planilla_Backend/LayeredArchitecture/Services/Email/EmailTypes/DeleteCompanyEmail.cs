namespace Planilla_Backend.LayeredArchitecture.Services.Email.EmailTypes
{
  public class DeleteCompanyEmail : IEmailType
  { 
    private string userName;
    private string companyName;

    public DeleteCompanyEmail(string userName, string companyName)
    {
      this.userName = userName;
      this.companyName = companyName;
    }

    public string GetSubject() 
    {
      return "Notificación de eliminación de empresa";
    }

    public string GetBody(object model)
    {
      return $@"
        <div style='font-family: Arial, sans-serif; background-color: #f5f5f5; padding: 20px;'>
          <div style='max-width: 600px; margin: 0 auto; background-color:#ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.1);'>

            <!-- Encabezado -->
            <div style='background-color: #1C4532; padding: 25px; text-align: center;'>
              <h2 style='color: white; margin: 0; font-size: 22px;'>
                Aviso Importante
              </h2>
            </div>

            <!-- Cuerpo -->
            <div style='padding: 35px; text-align: left; color: #333;'>
              <p style='font-size: 16px; line-height: 1.6;'>
                Estimado/a <strong>{this.userName}</strong>,
              </p>

              <p style='font-size: 16px; line-height: 1.6; color:#444;'>
                Le informamos que la empresa <strong>{this.companyName}</strong> ha sido
                eliminada de nuestro sistema.
              </p>

              <p style='font-size: 16px; line-height: 1.6; color:#555;'>
                Como consecuencia, ya no tendrá acceso a los servicios relacionados con dicha empresa
                y su cuenta asociada ha sido desactivada.
              </p>

              <p style='font-size: 15px; line-height: 1.6; color:#555;'>
                Si necesita más información o asistencia adicional, nuestro equipo de soporte está a su disposición.
              </p>

              <!-- Línea divisoria -->
              <div style='margin: 30px 0; border-top: 1px solid #7A9E70;'></div>

              <p style='font-size: 14px; color: #1C4532; text-align: center; margin-top: 20px;'>
                Atentamente,<br/>
                <strong style='color:#1C4532;'>ONIGIRI INTELLIGENT SOLUTIONS</strong>
              </p>
            </div>

          </div>
        </div>";
    }
  }
}
