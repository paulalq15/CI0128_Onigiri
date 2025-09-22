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

    }
}
