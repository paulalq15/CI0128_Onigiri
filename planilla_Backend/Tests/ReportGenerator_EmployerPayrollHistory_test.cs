using Moq;
using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Application.Reports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;
using Planilla_Backend.CleanArchitecture.Domain.Reports;

namespace Tests
{
  public class ReportGenerator_EmployerPayrollHistory_test
  {
    ReportGenerator_EmployerPayrollHistory _sut;
    ReportRequestDto _request;
    Mock<IReportRepository> _reportRepositoryMock;

    [SetUp]
    public void Setup()
    {
      _sut = new ReportGenerator_EmployerPayrollHistory();
      _reportRepositoryMock = new Mock<IReportRepository>();
      _request = BuildValidRequest();
    }

    private static ReportRequestDto BuildValidRequest() => new ReportRequestDto
    {
      ReportCode = ReportCodes.EmployerHistoryPayroll,
      CompanyId = 0,
      EmployeeId = 1,
      DateFrom = new DateTime(2025, 1, 1),
      DateTo = new DateTime(2025, 12, 31)
    };

    private static List<EmployerHistoryRow> BuildSampleRows() => new()
    {
      new EmployerHistoryRow
      {
        CompanyName = "Empresa PI",
        PaymentFrequency = PaymentFrequency.Monthly,
        DateFrom = new DateTime(2025, 2, 1),
        DateTo   = new DateTime(2025, 2, 28),
        PaymentDate = new DateTime(2025, 2, 28, 12, 0, 0),
        GrossSalary = 5500000m,
        EmployerContributions = 1466850m,
        EmployeeBenefits = 623200m,
        EmployerCost = 7590050m
      },
      new EmployerHistoryRow
      {
        CompanyName = "Empresa PI Quincenal",
        PaymentFrequency = PaymentFrequency.Biweekly,
        DateFrom = new DateTime(2025, 3, 1),
        DateTo   = new DateTime(2025, 3, 15),
        PaymentDate = new DateTime(2025, 3, 15, 12, 0, 0),
        GrossSalary = 1000000m,
        EmployerContributions = 200000m,
        EmployeeBenefits = 50000m,
        EmployerCost = 1250000m
      }
    };

    [Test]
    public async Task GenerateAsync_ShouldBuildRowsAndMapFieldsCorrectly()
    {
      // Arrange
      var rows = BuildSampleRows();

      _reportRepositoryMock
        .Setup(r => r.GetEmployerHistoryCompaniesAsync(
          It.IsAny<int?>(),
          It.IsAny<int>(),
          It.IsAny<DateTime>(),
          It.IsAny<DateTime>(),
          It.IsAny<CancellationToken>()))
        .ReturnsAsync(rows);

      // Act
      var result = await _sut.GenerateAsync(_request, _reportRepositoryMock.Object, CancellationToken.None);

      // Assert básicos
      Assert.That(result.ReportCode, Is.EqualTo(ReportCodes.EmployerHistoryPayroll));
      Assert.That(result.DisplayName, Is.EqualTo("Histórico pago de planilla empresas"));
      Assert.That(result.Columns, Is.EquivalentTo(new[]
      {
        "CompanyName",
        "PaymentFrequency",
        "Period",
        "PaymentDate",
        "GrossSalary",
        "EmployerContributions",
        "EmployeeBenefits",
        "EmployerCost"
      }));

      Assert.That(result.Rows.Count, Is.EqualTo(2));

      Dictionary<string, object?> Row(int i) => result.Rows[i];

      var r0 = Row(0);
      Assert.That(r0["CompanyName"], Is.EqualTo("Empresa PI"));
      Assert.That(r0["PaymentFrequency"], Is.EqualTo("Mensual"));
      Assert.That(r0["Period"], Is.EqualTo("Del 01/02/2025 al 28/02/2025"));
      Assert.That(r0["PaymentDate"], Is.EqualTo(new DateTime(2025, 2, 28, 12, 0, 0)));
      Assert.That(r0["GrossSalary"], Is.EqualTo(5500000m));
      Assert.That(r0["EmployerContributions"], Is.EqualTo(1466850m));
      Assert.That(r0["EmployeeBenefits"], Is.EqualTo(623200m));
      Assert.That(r0["EmployerCost"], Is.EqualTo(7590050m));

      var r1 = Row(1);
      Assert.That(r1["PaymentFrequency"], Is.EqualTo("Quincenal"));
      Assert.That(r1["Period"], Is.EqualTo("Del 01/03/2025 al 15/03/2025"));
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
    public void GenerateAsync_ShouldThrow_WhenEmployeeIdIsMissingOrInvalid()
    {
      var invalidRequest = new ReportRequestDto
      {
        ReportCode = ReportCodes.EmployeeDetailPayroll,
        CompanyId = 1,
        EmployeeId = 0,
        DateFrom = new DateTime(2025, 1, 1),
        DateTo = new DateTime(2025, 12, 31)
      };

      var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.GenerateAsync(invalidRequest, _reportRepositoryMock.Object));
      StringAssert.Contains("EmployeeId", ex!.Message);
    }

    [Test]
    public void GenerateAsync_ShouldThrow_WhenDateFromIsMissing()
    {
      var invalidRequest = new ReportRequestDto
      {
        ReportCode = ReportCodes.EmployerHistoryPayroll,
        CompanyId = 1,
        EmployeeId = 1,
        DateFrom = null,
        DateTo = new DateTime(2025, 12, 31)
      };

      var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.GenerateAsync(invalidRequest, _reportRepositoryMock.Object));
      StringAssert.Contains("fecha de inicio", ex!.Message);
    }

    [Test]
    public void GenerateAsync_ShouldThrow_WhenDateToIsMissing()
    {
      var invalidRequest = new ReportRequestDto
      {
        ReportCode = ReportCodes.EmployerHistoryPayroll,
        CompanyId = 1,
        EmployeeId = 1,
        DateFrom = new DateTime(2025, 1, 1),
        DateTo = null
      };

      var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.GenerateAsync(invalidRequest, _reportRepositoryMock.Object));
      StringAssert.Contains("fecha fin", ex!.Message);
    }
  }
}
