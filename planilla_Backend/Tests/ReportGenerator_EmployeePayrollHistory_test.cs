using Moq;
using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Application.Reports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;
using Planilla_Backend.CleanArchitecture.Domain.Reports;

namespace Tests;

public class ReportGenerator_EmployeePayrollHistory_test
{
  ReportGenerator_EmployeePayrollHistory _sut;
  ReportRequestDto _request;
  Mock<IReportRepository> _reportRepositoryMock;

  [SetUp]
  public void Setup()
  {
    _sut = new ReportGenerator_EmployeePayrollHistory();
    _reportRepositoryMock = new Mock<IReportRepository>();
    _request = BuildValidRequest();
  }

  private ReportRequestDto BuildValidRequest()
  {
    return new ReportRequestDto
    {
      ReportCode = ReportCodes.EmployeeDetailPayroll,
      CompanyId = 1,
      EmployeeId = 10,
      DateFrom = new DateTime(2025, 09, 1),
      DateTo = new DateTime(2025, 10, 31),
    };
  }

  private static EmployeePayrollHistoryReport BuildSampleReport()
  {
    return new EmployeePayrollHistoryReport
    {
      CompanyName = "Empresa PI",
      CompanyId = 1,
      EmployeeName = "Juan Solano",
      EmployeeId = 10,
      initialDate = new DateTime(2025, 09, 1),
      finalDate = new DateTime(2025, 10, 31),
      Rows = new List<EmployeeHistoryRow> 
      {
        new() {
          ContractType = EmployeeType.FullTime,
          Role = "Contador",
          PaymentDate = new DateTime(2025, 09, 29),
          GrossSalary = 535000,
          LegalDeductions = 48565,
          VoluntaryDeductions = 20000,
          NetSalary = 466435,
        },

        new() {
          ContractType = EmployeeType.FullTime,
          Role = "Auditor",
          PaymentDate = new DateTime(2025, 10, 29),
          GrossSalary = 800000,
          LegalDeductions = 75000,
          VoluntaryDeductions = 20000,
          NetSalary = 705000,
        },
      },
    };
  }

  [Test]
  public async Task GenerateAsync_ShouldBuildRowsWithGroupsTotalsAndNet()
  {
    // Arrange
    var report = BuildSampleReport();
    _reportRepositoryMock.Setup(r =>
        r.GetEmployeePayrollHistoryInDateRange(10, new DateTime(2025, 09, 1), new DateTime(2025, 10, 31)))
        .ReturnsAsync(report);

    // Act
    var result = await _sut.GenerateAsync(_request, _reportRepositoryMock.Object);

    //Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result.DisplayName, Is.EqualTo("Histórico pago de planilla"));
    Assert.That(result.Columns, Is.EquivalentTo(new[] {
        "Tipo de contrato",
        "Puesto",
        "Fecha de pago",
        "Salario bruto",
        "Deducciones obligatorias",
        "Deducciones voluntarias",
        "Salario neto"
    }));

    Assert.That(result.Rows.Count, Is.EqualTo(3));

    // Fila 1
    var row0 = result.Rows[0];
    Assert.That(row0["contractType"], Is.EqualTo("FullTime"));
    Assert.That(row0["position"], Is.EqualTo("Contador"));
    Assert.That(row0["paymentDate"], Is.EqualTo("29-09-2025"));
    Assert.That(row0["grossSalary"], Is.EqualTo(535000));
    Assert.That(row0["mandatoryDeductions"], Is.EqualTo(48565));
    Assert.That(row0["voluntaryDeductions"], Is.EqualTo(20000));
    Assert.That(row0["netSalary"], Is.EqualTo(466435));

    // Fila 2
    var row1 = result.Rows[1];
    Assert.That(row1["contractType"], Is.EqualTo("FullTime"));
    Assert.That(row1["position"], Is.EqualTo("Auditor"));
    Assert.That(row1["paymentDate"], Is.EqualTo("29-10-2025"));
    Assert.That(row1["grossSalary"], Is.EqualTo(800000));
    Assert.That(row1["mandatoryDeductions"], Is.EqualTo(75000));
    Assert.That(row1["voluntaryDeductions"], Is.EqualTo(20000));
    Assert.That(row1["netSalary"], Is.EqualTo(705000));

    // Totales
    var totals = result.Rows[2];
    Assert.That(totals["totalGrossSalary"], Is.EqualTo(1335000));
    Assert.That(totals["totalLegalDeductions"], Is.EqualTo(123565));
    Assert.That(totals["totalVoluntaryDeductions"], Is.EqualTo(40000));
    Assert.That(totals["totalNetSalary"], Is.EqualTo(1171435));

    Assert.That(result.ReportInfo["EmployeeName"], Is.EqualTo("Juan Solano"));
    Assert.That(result.ReportInfo["CompanyName"], Is.EqualTo("Empresa PI"));
    Assert.That(result.ReportInfo["DateFrom"], Is.EqualTo(new DateTime(2025, 09, 01)));
    Assert.That(result.ReportInfo["DateTo"], Is.EqualTo(new DateTime(2025, 10, 31)));
  }

  [Test]
  public void GenerateAsync_ShouldThrow_WhenRequestIsNull()
  {
    // Act & Assert
    Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.GenerateAsync(null!, _reportRepositoryMock.Object));
  }

  [Test]
  public void GenerateAsync_ShouldThrow_WhenRepositoryIsNull()
  {
    // Act & Assert
    Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.GenerateAsync(_request, null!));
  }

  [Test]
  public void GenerateAsync_ShouldThrow_WhenCompanyIdIsInvalid()
  {
    // Arrange
    var invalid = new ReportRequestDto
    {
      ReportCode = _request.ReportCode,
      CompanyId = 0,
      EmployeeId = _request.EmployeeId,
      DateFrom = _request.DateFrom,
      DateTo = _request.DateTo
    };

    // Act
    var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
        await _sut.GenerateAsync(invalid, _reportRepositoryMock.Object)
    );

    // Assert
    Assert.That(ex!.Message, Does.Contain("CompanyId"));
  }

  [Test]
  public void GenerateAsync_ShouldThrow_WhenEmployeeIdIsInvalid()
  {
    // Arrange
    var invalid = new ReportRequestDto
    {
      ReportCode = _request.ReportCode,
      CompanyId = _request.CompanyId,
      EmployeeId = null,
      DateFrom = _request.DateFrom,
      DateTo = _request.DateTo
    };

    // Act
    var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
        await _sut.GenerateAsync(invalid, _reportRepositoryMock.Object)
    );

    // Assert
    Assert.That(ex!.Message, Does.Contain("EmployeeId"));
  }
}
