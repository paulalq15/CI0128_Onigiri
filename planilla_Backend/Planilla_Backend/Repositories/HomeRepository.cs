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

        public int CreateCompany(CreateCompanyModel company)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // Validar ZipCode y obtener IdDivision
                    const string searchDivision =
                        @"SELECT TOP (1) IdDivision FROM dbo.DivisionTerritorialCR WHERE CodigoPostal = @ZipCode;";
                    int? idDivision = connection.ExecuteScalar<int?>(searchDivision, new 
                    { 
                        ZipCode = company.ZipCode 
                    }, transaction);
                    if (idDivision is null)
                        throw new InvalidOperationException("El código postal no existe");

                    // Crear Empresa y obtener Id
                    const string insertCompany = 
                        @"INSERT INTO [dbo].[Empresa] ([CedulaJuridica], [Nombre], [Telefono], [CantidadBeneficios], [FrecuenciaPago], [DiaPago1], [DiaPago2], [IdCreadoPor])
                        OUTPUT INSERTED.IdEmpresa
                        VALUES (@CompanyId, @CompanyName, @Telephone, @MaxBenefits, @PaymentFrequency, @PayDay1, @PayDay2, @CreatedBy)";

                    int id = connection.ExecuteScalar<int>(insertCompany, new
                    {
                        CompanyId = company.CompanyId,
                        CompanyName = company.CompanyName,
                        Telephone = company.Telephone,
                        MaxBenefits = company.MaxBenefits,
                        PaymentFrequency = company.PaymentFrequency,
                        PayDay1 = company.PayDay1,
                        PayDay2 = company.PayDay2,
                        CreatedBy = company.CreatedBy > 0 ? company.CreatedBy : 1, // temporal
                    }, transaction);

                    // Crear dirección con el id de empresa y con id de división
                    const string insertAddress =
                        @"INSERT INTO dbo.Direccion (IdDivision, OtrasSenas, IdEmpresa)
                        VALUES (@IdDivision, @AddressDetails, @CompanyId)";
                    connection.Execute(insertAddress, new
                    {
                        IdDivision = idDivision.Value,
                        AddressDetails = company.AddressDetails,
                        CompanyId = id
                    }, transaction);

                    // Link usuario con empresa creada
                    const string insertUserCompany = 
                        @"INSERT INTO [dbo].[UsuariosPorEmpresa] ([IdEmpresa], [IdUsuario])
                        VALUES (@CompanyId, @UserId)";
                    connection.Execute(insertUserCompany, new
                    {
                        CompanyId = id,
                        UserId = company.CreatedBy > 0 ? company.CreatedBy : 1, //temporal
                    }, transaction);

                    transaction.Commit();
                    return id;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}