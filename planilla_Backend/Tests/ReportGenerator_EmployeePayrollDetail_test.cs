using Moq;
using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Application.Reports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;
using Planilla_Backend.CleanArchitecture.Domain.Reports;

namespace Tests
{
  public class ReportGenerator_EmployeePayrollDetail_test
  {
    ReportGenerator_EmployeePayrollDetail _sut;
    ReportRequestDto _request;
    Mock<IReportRepository> _reportRepositoryMock;

    [SetUp]
    public void Setup()
    {
      _sut = new ReportGenerator_EmployeePayrollDetail();
      _reportRepositoryMock = new Mock<IReportRepository>();
      _request = BuildValidRequest();
    }

    private static ReportRequestDto BuildValidRequest() => new ReportRequestDto
    {
      ReportCode = ReportCodes.EmployeeDetailPayroll,
      CompanyId = 1,
      EmployeeId = 10,
      PayrollId = 123
    };

    private static EmployeePayrollReport BuildSampleReport() => new EmployeePayrollReport
    {
      CompanyId = 1,
      CompanyName = "Empresa PI",
      EmployeeId = 10,
      EmployeeName = "Juan Solano",
      EmployeeType = EmployeeType.FullTime,
      PaymentDate = new DateTime(2025, 10, 31),
      NetAmount = 860m,

      Lines = new List<PayrollDetailLine>
      {
        new() { Description = "Salario bruto", Category = "Salario", Amount = 1000m },
        new() { Description = "CCSS - SEM", Category = "Deduccion obligatoria", Amount = -100m },
        new() { Description = "CCSS - IVM", Category = "Deduccion obligatoria", Amount = -50m },
        new() { Description = "Pensión voluntaria", Category = "Deduccion voluntaria", Amount = -20m },
        new() { Description = "Gimnasio", Category = "Beneficio", Amount = 30m }
      }
    };

    [Test]
    public async Task GenerateAsync_ShouldBuildRowsWithGroupsTotalsAndNet()
    {
      // Arrange
      var report = BuildSampleReport();

      _reportRepositoryMock
        .Setup(r => r.GetEmployeePayrollReport(It.IsAny<int>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(report);

      // Act
      var result = await _sut.GenerateAsync(_request, _reportRepositoryMock.Object, CancellationToken.None);

      // Assert - Basic Info
      Assert.That(result.ReportCode, Is.EqualTo(ReportCodes.EmployeeDetailPayroll));
      Assert.That(result.DisplayName, Is.EqualTo("Detalle de pago de planilla"));
      Assert.That(result.Columns, Is.EquivalentTo(new[] { "Descripción", "Categoría", "Monto" }));

      // Assert - Rows
      Assert.That(result.Rows.Count, Is.EqualTo(7));

      Dictionary<string, object?> Row(int i) => result.Rows[i];

      Assert.That(Row(0)["Descripción"], Is.EqualTo("Salario bruto"));
      Assert.That(Row(0)["Monto"], Is.EqualTo(1000m));

      Assert.That(Row(1)["Descripción"], Is.EqualTo("CCSS - SEM"));
      Assert.That(Row(2)["Descripción"], Is.EqualTo("CCSS - IVM"));
      Assert.That(Row(3)["Descripción"], Is.EqualTo("Total deducciones obligatorias"));
      Assert.That(Row(3)["Monto"], Is.EqualTo(-150m));

      Assert.That(Row(4)["Descripción"], Is.EqualTo("Pensión voluntaria"));
      Assert.That(Row(5)["Descripción"], Is.EqualTo("Total deducciones voluntarias"));
      Assert.That(Row(5)["Monto"], Is.EqualTo(-20m));

      Assert.That(Row(6)["Descripción"], Is.EqualTo("Pago Neto"));
      Assert.That(Row(6)["Categoría"], Is.EqualTo("Resumen"));
      Assert.That(Row(6)["Monto"], Is.EqualTo(860m));
    }

    [Test]
    public void GenerateAsync_ShouldThrow_WhenRequestIsNull()
    {
      Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.GenerateAsync(null!, _reportRepositoryMock.Object));
    }

    [Test]
    public void GenerateAsync_ShouldThrow_WhenRepositoryIsNull()
    {
      Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.GenerateAsync(_request, null!));
    }

    [Test]
    public void GenerateAsync_ShouldThrow_WhenCompanyIdIsInvalid()
    {
      var invalidRequest = new ReportRequestDto
      {
        ReportCode = ReportCodes.EmployeeDetailPayroll,
        CompanyId = 0,
        EmployeeId = 10,
        PayrollId = 123
      };

      var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.GenerateAsync(invalidRequest, _reportRepositoryMock.Object));
      StringAssert.Contains("CompanyId", ex!.Message);
    }

    [Test]
    public void GenerateAsync_ShouldThrow_WhenCompanyDoesNotMatch()
    {
      var report = BuildSampleReport();
      report.CompanyId = 999;

      _reportRepositoryMock
        .Setup(r => r.GetEmployeePayrollReport(123, It.IsAny<CancellationToken>()))
        .ReturnsAsync(report);

      Assert.ThrowsAsync<KeyNotFoundException>(async () => await _sut.GenerateAsync(_request, _reportRepositoryMock.Object));
    }

    [Test]
    public void GenerateAsync_ShouldThrow_WhenEmployeeDoesNotMatch()
    {
      var report = BuildSampleReport();
      report.EmployeeId = 999;

      _reportRepositoryMock
        .Setup(r => r.GetEmployeePayrollReport(123, It.IsAny<CancellationToken>()))
        .ReturnsAsync(report);

      Assert.ThrowsAsync<KeyNotFoundException>(async () => await _sut.GenerateAsync(_request, _reportRepositoryMock.Object));
    }

  }
}
