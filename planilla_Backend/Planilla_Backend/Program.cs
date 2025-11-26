using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Application.Reports;
using Planilla_Backend.CleanArchitecture.Application.Services;
using Planilla_Backend.CleanArchitecture.Application.UseCases;
using Planilla_Backend.CleanArchitecture.Application.UseCases.company;
using Planilla_Backend.CleanArchitecture.Domain.Calculation;
using Planilla_Backend.CleanArchitecture.Infrastructure;
using Planilla_Backend.CleanArchitecture.Infrastructure.External;
using Planilla_Backend.CleanArchitecture.Application.Dashboards;
using Planilla_Backend.LayeredArchitecture.Repositories;
using Planilla_Backend.LayeredArchitecture.Services;
using Planilla_Backend.LayeredArchitecture.Services.EmailService;
using Planilla_Backend.LayeredArchitecture.Services.Utils;
using System.Text.Json.Serialization;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
  options.AddPolicy(name: MyAllowSpecificOrigins,
                    policy =>
                    {
                      policy.WithOrigins("http://localhost:8080", "https://planillaonigiri.netlify.app")
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
});

// Add services to the container.
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmployeeRepository>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<CompanyRepository>();
builder.Services.AddScoped<CompanyService>();
builder.Services.AddScoped<CountryDivisionRepository>();
builder.Services.AddScoped<CountryDivisionService>();
builder.Services.AddScoped<DirectionsRepository>();
builder.Services.AddScoped<PayrollElementRepository>();
builder.Services.AddScoped<PayrollElementService>();
builder.Services.AddScoped<PersonUserRepository>();
builder.Services.AddScoped<PersonUserService>();

builder.Services.AddScoped<ICreatePayrollCommand, CreatePayrollCommand>();
builder.Services.AddScoped<IPayrollRepository, PayrollRepository>();
builder.Services.AddScoped<IPayrollDbSession, PayrollDbSession>();
builder.Services.AddScoped<PayrollTemplate, StandardPayrollRun>();
builder.Services.AddScoped<CalculationFactory>();

builder.Services.AddScoped<ISalaryBaseStrategy, Salary_FixedStrategy>();
builder.Services.AddScoped<ISalaryBaseStrategy, Salary_HourlyStrategy>();

builder.Services.AddScoped<IConceptStrategy, Concept_ApiStrategy>();
builder.Services.AddScoped<IConceptStrategy, Concept_FixedAmountStrategy>();
builder.Services.AddScoped<IConceptStrategy, Concept_PercentageStrategy>();

builder.Services.AddScoped<ILegalConceptStrategy, LegalConcept_CCSSStrategy>();
builder.Services.AddScoped<ILegalConceptStrategy, LegalConcept_TaxStrategy>();

// Payroll Element
builder.Services.AddScoped<IPayrollElementRepository, PayrollElementRepositoryCA>();
builder.Services.AddScoped<IPayrollElementQuery, PayrollElementQuery>();
builder.Services.AddScoped<IPayrollElementCommand, PayrollElementCommand>();

// Timesheet
builder.Services.AddScoped<ITimesheetRepository, TimesheetRepository>();
builder.Services.AddScoped<ITimesheetService, TimesheetService>();

// External APIs (Asociación, Seguro, Pensiones)
builder.Services.AddScoped<ExternalPartnersService>();
builder.Services.AddExternalApis(builder.Configuration);

// Reports
builder.Services.AddScoped<IGenerateReportDataQuery, GenerateReportDataQuery>();
builder.Services.AddScoped<IReportFactory, ReportFactory>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportGenerator, ReportGenerator_EmployeePayrollDetail>();
builder.Services.AddScoped<IReportGenerator, ReportGenerator_EmployeePayrollHistory>();
builder.Services.AddScoped<IReportGenerator, ReportGenerator_EmployerPayrollDetail>();
builder.Services.AddScoped<IReportGenerator, ReportGenerator_EmployerPayrollHistory>();
builder.Services.AddScoped<IReportGenerator, ReportGenerator_EmployerPayrollByEmployee>();

// Company
builder.Services.AddScoped<ICompanyRepository, CompanyRepositoryCA>();
builder.Services.AddScoped<IDeleteCompanyCommand, DeleteCompanyCommand>();

// Dashboard
builder.Services.AddScoped<IEmployerDashboardQuery, EmployerDashboardQuery>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();

builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(options =>
{
  options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Service for Email
builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddSingleton<Utils>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(MyAllowSpecificOrigins);

app.MapControllers();

app.Run();