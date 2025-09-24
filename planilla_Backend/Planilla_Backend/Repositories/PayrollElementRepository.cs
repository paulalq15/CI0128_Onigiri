using Dapper;
using Microsoft.Data.SqlClient;
using Planilla_Backend.Models;
using System.Data.SqlClient;

namespace Planilla_Backend.Repositories
{
    public class PayrollElementRepository
    {
        private readonly string _connectionString;
        public PayrollElementRepository()
        {
            var builder = WebApplication.CreateBuilder();
            _connectionString = builder.Configuration.GetConnectionString("PayrollContext");
        }

        public bool CreatePayrollElement(PayrollElementModel element)
        {
            using var connection = new SqlConnection(_connectionString);
            const string query = @"INSERT INTO dbo.ElementoPlanilla (Nombre, PagadoPor, Tipo, Valor, IdEmpresa)
                                 VALUES (@ElementName, @PaidBy, @CalculationType, @CalculationValue, @CompanyId)";
            int rowsAffected = connection.Execute(query, new 
            {
                ElementName = element.ElementName,
                PaidBy = element.PaidBy,
                CalculationType = element.CalculationType,
                CalculationValue = element.CalculationValue,
                CompanyId = element.CompanyId
            });
            return rowsAffected > 0;
        }

    }
}
