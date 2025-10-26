using Dapper;
using Microsoft.Data.SqlClient;
using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Infrastructure
{
  public class PayrollElementRepositoryCA : IPayrollElementRepository
  {
    private readonly string connectionString;

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
  }
}
