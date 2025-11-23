using Moq;
using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Application.Reports;

namespace Tests
{
  internal class GenerateReportDataQuery_test
  {
    private GenerateReportDataQuery _sut;
    private Mock<IReportFactory> _reportFactoryMock;
    private Mock<IReportRepository> _reportRepositoryMock;
    private Mock<IReportGenerator> _reportGeneratorMock;

    [SetUp]
    public void Setup()
    {
      _reportFactoryMock = new Mock<IReportFactory>();
      _reportRepositoryMock = new Mock<IReportRepository>();
      _reportGeneratorMock = new Mock<IReportGenerator>();

      _sut = new GenerateReportDataQuery(_reportFactoryMock.Object, _reportRepositoryMock.Object);
    }

    private static ReportRequestDto BuildValidRequest() => new ReportRequestDto
    {
      ReportCode = ReportCodes.EmployeeDetailPayroll,
      CompanyId = 1,
      EmployeeId = 10,
      PayrollId = 123
    };

    [Test]
    public void GenerateReportAsync_ShouldThrow_WhenRequestIsNull()
    {
      Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.GenerateReportAsync(null!, CancellationToken.None));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void GenerateReportAsync_ShouldThrow_WhenReportCodeIsInvalid(string? reportCode)
    {
      var request = new ReportRequestDto
      {
        ReportCode = reportCode,
        CompanyId = 1
      };

      var ex = Assert.ThrowsAsync<ArgumentException>(
        async () => await _sut.GenerateReportAsync(request, CancellationToken.None));

      Assert.That(ex, Is.Not.Null);
      StringAssert.Contains("El código de reporte es requerido", ex!.Message);
    }

    [Test]
    public void GenerateReportAsync_ShouldThrow_WhenGeneratorNotFound()
    {
      var request = BuildValidRequest();

      _reportFactoryMock
        .Setup(f => f.GetGenerator(request.ReportCode))
        .Returns((IReportGenerator)null!);

      var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await _sut.GenerateReportAsync(request, CancellationToken.None));

      Assert.That(ex, Is.Not.Null);
      StringAssert.Contains("No existe un generador configurado para el reporte", ex!.Message);
    }

    [Test]
    public async Task GenerateReportAsync_ShouldCallGeneratorAndReturnResult()
    {
      var request = BuildValidRequest();
      var ct = CancellationToken.None;

      var expectedResult = new ReportResultDto
      {
        ReportCode = request.ReportCode,
        DisplayName = "Reporte X",
        Columns = new[] { "Col1", "Col2" },
        Rows = new List<Dictionary<string, object?>>()
      };

      _reportFactoryMock
        .Setup(f => f.GetGenerator(request.ReportCode))
        .Returns(_reportGeneratorMock.Object);

      _reportGeneratorMock
        .Setup(g => g.GenerateAsync(request, _reportRepositoryMock.Object, ct))
        .ReturnsAsync(expectedResult);

      var result = await _sut.GenerateReportAsync(request, ct);

      Assert.That(result, Is.SameAs(expectedResult));

      _reportFactoryMock.Verify(
        f => f.GetGenerator(request.ReportCode),
        Times.Once);

      _reportGeneratorMock.Verify(
        g => g.GenerateAsync(request, _reportRepositoryMock.Object, ct),
        Times.Once);
    }

    [Test]
    public void GetEmployeePayrollPeriodsAsync_ShouldThrow_WhenCompanyIdIsInvalid()
    {
      var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.GetEmployeePayrollPeriodsAsync(0, 10, 5));

      Assert.That(ex, Is.Not.Null);
      StringAssert.Contains("El parámetro CompanyId es requerido y debe ser mayor que cero", ex!.Message);
    }

    [Test]
    public void GetEmployeePayrollPeriodsAsync_ShouldThrow_WhenEmployeeIdIsInvalid()
    {
      var ex = Assert.ThrowsAsync<ArgumentException>(
        async () => await _sut.GetEmployeePayrollPeriodsAsync(1, 0, 5));

      Assert.That(ex, Is.Not.Null);
      StringAssert.Contains("El parámetro EmployeeId es requerido y debe ser mayor que cero", ex!.Message);
    }

    [Test]
    public void GetEmployeePayrollPeriodsAsync_ShouldThrow_WhenTopIsInvalid()
    {
      var ex = Assert.ThrowsAsync<ArgumentException>(
        async () => await _sut.GetEmployeePayrollPeriodsAsync(1, 10, 0));

      Assert.That(ex, Is.Not.Null);
      StringAssert.Contains("El parámetro top es requerido y debe ser mayor que cero", ex!.Message);
    }

    [Test]
    public async Task GetEmployeePayrollPeriodsAsync_ShouldCallRepositoryAndReturnResult()
    {
      var companyId = 1;
      var employeeId = 10;
      var top = 5;

      var expected = new List<ReportPayrollPeriodDto>
      {
        new ReportPayrollPeriodDto
        {
          PayrollId = 123,
          DateFrom = new DateTime(2025, 10, 1),
          DateTo = new DateTime(2025, 10, 31),
          PeriodLabel = "Del 01/10/2025 al 31/10/2025",
        }
      };

      _reportRepositoryMock
        .Setup(r => r.GetEmployeePayrollPeriodsAsync(companyId, employeeId, top))
        .ReturnsAsync(expected);

      var result = await _sut.GetEmployeePayrollPeriodsAsync(companyId, employeeId, top);

      Assert.That(result, Is.EqualTo(expected));
      _reportRepositoryMock.Verify(
        r => r.GetEmployeePayrollPeriodsAsync(companyId, employeeId, top),
        Times.Once);
    }

  }
}
