using Dapper;
using Microsoft.Data.SqlClient;
using Planilla_Backend.Models;

namespace Planilla_Backend.Repositories
{
    public class CreateCompanyRepository
    {
        private readonly string _connectionString;
        public CreateCompanyRepository()
        {
            var builder = WebApplication.CreateBuilder();
            _connectionString = builder.Configuration.GetConnectionString("PayrollContext");
        }

        public bool CreateCompany(CreateCompanyModel company)
        {
            using var connection = new SqlConnection(_connectionString);
            string query = @"INSERT INTO [dbo].[Empresa] ([CedulaJuridica], [Nombre], [Telefono], [CantidadBeneficios], [FrecuenciaPago], [DiaPago1], [DiaPago2], [IdCreadoPor])
                             VALUES (@CompanyId, @CompanyName, @Telephone, @MaxBenefits, @PaymentFrequency, @PayDay1, @PayDay2, @CreatedBy)";
            int affectedRows = connection.Execute(query,new
            {
                CompanyId = company.CompanyId,
                CompanyName = company.CompanyName,
                Telephone = company.Telephone,
                MaxBenefits = company.MaxBenefits,
                PaymentFrequency = company.PaymentFrequency,
                PayDay1 = company.PayDay1,
                PayDay2 = company.PayDay2,
                CreatedBy = 1, //Temporal
            });
            return affectedRows >= 1;
        }
    }
}