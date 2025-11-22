using Dapper;
using Microsoft.Data.SqlClient;
using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;
using System.ComponentModel.Design;

namespace Planilla_Backend.CleanArchitecture.Infrastructure
{
  public class PayrollElementRepositoryCA : IPayrollElementRepository
  {
    private readonly string connectionString;
    private readonly ILogger<ReportRepository> _logger;

    public PayrollElementRepositoryCA(IConfiguration config)
    {
      this.connectionString = config.GetConnectionString("OnigiriContext");
    }

    public async Task<PayrollElementEntity?> GetPayrollElementByElementId(int elementId)
    {
      try
      {
        using var connection = new SqlConnection(this.connectionString);

        const string sqlGetPayrollElement = @"
          Select
            IdElemento As IdElement,
            Nombre As ElementName,
            Tipo As CalculationType,
            Valor As CalculationValue,
            PagadoPor As PaidBy,
            Estado As Status,
            IdEmpresa As CompanyId
          From ElementoPlanilla
          Where IdElemento = @elementId";

        var payrollElement = await connection.QueryFirstOrDefaultAsync<PayrollElementEntity>(sqlGetPayrollElement, new { elementId });
        return payrollElement;
      }
      catch (Exception ex)
      {
        Console.WriteLine("Error al obtener un elemento de planilla" + ex.Message);
        return null;
      }
    }

    public async Task<int> UpdatePayrollElement(PayrollElementEntity payrollElement)
    {
      int affectedRows = 0;
      try
      {
        using var connection = new SqlConnection(this.connectionString);

        const string sqlUpdPayrollElement = @"
            Update ElementoPlanilla
            Set 
                Nombre = @ElementName,
                Tipo = @CalculationType,
                Valor = @CalculationValue,
                PagadoPor = @PaidBy,
                Estado = @Status
            Where IdElemento = @IdElement
            ";

        affectedRows = await connection.ExecuteAsync(sqlUpdPayrollElement, payrollElement);
      }
      catch (Exception ex)
      {
        Console.WriteLine("Error al actualizar un elemento de planilla. Detalle: " + ex.Message);
      }
      
      return affectedRows;
    }

    public async Task<int> DeletePayrollElement(int elementId)
    {
      int affectedRows = 0;
      try
      {
        using var connection = new SqlConnection(this.connectionString);
        const string storedProc = "dbo.sp_DeleteElementoPlanilla";
        var parameters = new { elementId };
        affectedRows = await connection.ExecuteAsync(storedProc, parameters, commandType: System.Data.CommandType.StoredProcedure);
      }
      catch (Exception ex)
      {
        _logger.LogError("Error al eliminar un elemento de planilla. Detalle: {Message}", ex.Message);
      }
      return affectedRows;
    }
    public async Task<IEnumerable<DeletePayrollElementEmailDto>> EmailPayrollElementAssignedEmployees(int elementId)
    {
      try
      {
        using var connection = new SqlConnection(this.connectionString);
        const string sqlGetEmails = @"
          select
	          p.Nombre1 + ' ' + p.Apellido1 + ' ' + p.Apellido2 AS EmployeeName,
	          u.Correo as EmployeeEmail, 
	          e.Nombre as Benefit
          from ElementoPlanilla e
          join ElementoAplicado ea on e.IdElemento = ea.IdElemento
          join Usuario u on ea.IdUsuario = u.IdUsuario
          join Persona p on p.IdPersona = u.IdPersona
          where e.IdElemento = @elementId";
        var emailList = await connection.QueryAsync<DeletePayrollElementEmailDto>(sqlGetEmails, new { elementId });
        return emailList;
      }
      catch (Exception ex)
      {
        _logger.LogError("Error al obtener los correos de los empleados asignados al elemento de planilla. Detalle: {Message}", ex.Message);
        return Enumerable.Empty<DeletePayrollElementEmailDto>();
      }
    }
  } // end class
}
