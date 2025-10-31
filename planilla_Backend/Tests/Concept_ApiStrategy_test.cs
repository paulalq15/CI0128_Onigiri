using Microsoft.Extensions.Configuration;
using Moq;
using Planilla_Backend.CleanArchitecture.Application.External.Abstractions;
using Planilla_Backend.CleanArchitecture.Application.External.Models;
using Planilla_Backend.CleanArchitecture.Domain.Calculation;
using Planilla_Backend.CleanArchitecture.Domain.Entities;
using Planilla_Backend.CleanArchitecture.Infrastructure.External;

namespace Tests
{
  public class Concept_ApiStrategy_test
  {
    private Concept_ApiStrategy _sut;
    private PayrollContext _payrollContext;
    private EmployeePayrollModel _employeePayrollModel;
    private EmployeeModel _employeeModel;
    private ElementModel _conceptModel;

    private Mock<IExternalApiService> _apiClientMock;
    private Mock<IConfiguration> _configMock;
    private ExternalPartnersService _partnersService;

    [SetUp]
    public void Setup()
    {
      _configMock = new Mock<IConfiguration>();

      _configMock.Setup(c => c["ExternalApis:AsociacionSolidarista:BaseUrl"])
        .Returns("https://fake-asociacion/api/");
      _configMock.Setup(c => c["ExternalApis:AsociacionSolidarista:Token"])
        .Returns("token-asoc");

      _configMock.Setup(c => c["ExternalApis:SeguroPrivado:BaseUrl"])
        .Returns("https://fake-seguro/api/");
      _configMock.Setup(c => c["ExternalApis:SeguroPrivado:Token"])
        .Returns("token-seguro");

      _configMock.Setup(c => c["ExternalApis:PensionesVoluntarias:BaseUrl"])
        .Returns("https://fake-pensiones/api/");

      _apiClientMock = new Mock<IExternalApiService>(MockBehavior.Strict);
      _partnersService = new ExternalPartnersService(_apiClientMock.Object, _configMock.Object);
      _payrollContext = new PayrollContext {Company = new CompanyModel {LegalID = "3101123456"}};
      _employeePayrollModel = new EmployeePayrollModel {Id = 123, Gross = 1_000_000m};
      _employeeModel = new EmployeeModel {PersonType = "Empleado",Age = 30};
      _conceptModel = new ElementModel {Name = ""};
      _sut = new Concept_ApiStrategy(_partnersService);
    }

    [Test]
    public void Apply_WhenEmployeePayrollIsNull_Throws()
    {
      Assert.Throws<ArgumentNullException>(() => _sut.Apply(null!, _conceptModel, _payrollContext, _employeeModel));
    }

    [Test]
    public void Apply_WhenConceptIsNull_Throws()
    {
      Assert.Throws<ArgumentNullException>(() => _sut.Apply(_employeePayrollModel, null!, _payrollContext, _employeeModel));
    }

    [Test]
    public void Apply_WhenContextIsNull_Throws()
    {
      Assert.Throws<ArgumentNullException>(() => _sut.Apply(_employeePayrollModel, _conceptModel, null!, _employeeModel));
    }

    [Test]
    public void Apply_SeguroPrivado_WhenEmployeeHasNoAge_Throws()
    {
      var concept = new ElementModel {Name = "", Value = 2, NumberOfDependents = 1};
      var employee = new EmployeeModel {PersonType = "Empleado", Age = null};

      Assert.Throws<InvalidOperationException>(() => _sut.Apply(_employeePayrollModel, concept, _payrollContext, employee));
    }

    [Test]
    public void Apply_SeguroPrivado_WhenNoDependents_Throws()
    {
      var concept = new ElementModel {Name = "", Value = 2, NumberOfDependents = null};
      var employee = new EmployeeModel {PersonType = "Empleado", Age = 35};

      Assert.Throws<InvalidOperationException>(() => _sut.Apply(_employeePayrollModel, concept, _payrollContext, employee));
    }

    [Test]
    public void Apply_AsociacionSolidarista_ReturnsTwoDetails_WhenAPIHasEEandER()
    {
      var apiResponse = new ApiResponse {Deductions = new List<ApiItem> {new ApiItem {Type = "EE", Amount = 1500m}, new ApiItem {Type = "ER", Amount = 2500m}}};

      _apiClientMock
        .Setup(c => c.GetDeductionsAsync(
          "https://fake-asociacion/api/",
          "api/asociacionsolidarista/aporte-empleado",
          "token-asoc",
          It.IsAny<Dictionary<string, string>>(),
          It.IsAny<CancellationToken>()))
        .ReturnsAsync(apiResponse);

      var concept = new ElementModel {Id = 10, Name = "Asociación Solidarista", Value = 1};

      // Act
      var result = _sut.Apply(_employeePayrollModel, concept, _payrollContext, _employeeModel);

      // Assert
      var list = new List<PayrollDetailModel>(result);
      Assert.That(list.Count, Is.EqualTo(2));

      var ee = list.Find(d => d.Description.Contains("(Empleado)"));
      var er = list.Find(d => d.Description.Contains("(Empleador)"));

      Assert.NotNull(ee);
      Assert.That(ee!.Amount, Is.EqualTo(1500m));
      Assert.That(ee.Type, Is.EqualTo(PayrollItemType.EmployeeDeduction));

      Assert.NotNull(er);
      Assert.That(er!.Amount, Is.EqualTo(2500m));
      Assert.That(er.Type, Is.EqualTo(PayrollItemType.Benefit));

      _apiClientMock.VerifyAll();
    }

    [Test]
    public void Apply_AsociacionSolidarista_WhenApiReturnsNull_ReturnsEmpty()
    {
      _apiClientMock
        .Setup(c => c.GetDeductionsAsync(
          "https://fake-asociacion/api/",
          "api/asociacionsolidarista/aporte-empleado",
          "token-asoc",
          It.IsAny<Dictionary<string, string>>(),
          It.IsAny<CancellationToken>()))
        .ReturnsAsync((ApiResponse?)null);

      var concept = new ElementModel {Id = 10, Name = "Asociación Solidarista", Value = 1};

      var result = _sut.Apply(_employeePayrollModel, concept, _payrollContext, _employeeModel);

      Assert.IsEmpty(result);
      _apiClientMock.VerifyAll();
    }

    [Test]
    public void Apply_SeguroPrivado_ReturnsEmployeeDeduction()
    {
      var apiResponse = new ApiResponse {Deductions = new List<ApiItem>{new ApiItem {Type = "EE", Amount = 500m}}};

      _apiClientMock
        .Setup(c => c.GetDeductionsAsyncRawAuth(
          "https://fake-seguro/api/",
          "SeguroPrivado/seguro-privado",
          "token-seguro",
          It.IsAny<Dictionary<string, string>>(),
          It.IsAny<CancellationToken>()))
        .ReturnsAsync(apiResponse);

      var concept = new ElementModel {Id = 20, Name = "Seguro Privado", Value = 2, NumberOfDependents = 2};

      var result = _sut.Apply(_employeePayrollModel, concept, _payrollContext, _employeeModel);

      var list = new List<PayrollDetailModel>(result);
      Assert.That(list.Count, Is.EqualTo(1));
      Assert.That(list[0].Type, Is.EqualTo(PayrollItemType.EmployeeDeduction));
      Assert.That(list[0].Amount, Is.EqualTo(500m));
      Assert.That(list[0].IdElement, Is.EqualTo(20));

      _apiClientMock.VerifyAll();
    }

    [Test]
    public void Apply_PensionVoluntaria_ReturnsEmployeeDeduction()
    {
      var apiResponse = new ApiResponse {Deductions = new List<ApiItem> {new ApiItem {Type = "EE", Amount = 10_000m}}};

      _apiClientMock
        .Setup(c => c.GetDeductionsAsync(
          "https://fake-pensiones/api/",
          "", // tu servicio usa endpointPath = ""
          string.Empty,
          It.IsAny<Dictionary<string, string>>(),
          It.IsAny<CancellationToken>()))
        .ReturnsAsync(apiResponse);

      var concept = new ElementModel {Id = 30, Name = "Pensión Voluntaria", Value = 3, PensionType = "A"};

      var result = _sut.Apply(_employeePayrollModel, concept, _payrollContext, _employeeModel);

      var list = new List<PayrollDetailModel>(result);
      Assert.That(list.Count, Is.EqualTo(1));
      Assert.That(list[0].Amount, Is.EqualTo(10_000m));
      Assert.That(list[0].Type, Is.EqualTo(PayrollItemType.EmployeeDeduction));

      _apiClientMock.VerifyAll();
    }

    [Test]
    public void Apply_WhenConceptValueIsUnknown_Throws()
    {
      var concept = new ElementModel {Id = 99, Name = "Desconocido", Value = 999};

      Assert.Throws<InvalidOperationException>(() =>
        _sut.Apply(_employeePayrollModel, concept, _payrollContext, _employeeModel));
    }
  }
}
