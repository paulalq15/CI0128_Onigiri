using Moq;
using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Application.Reports;

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
      ReportCode = ReportCodes.EmployeeDetailPayroll,
      CompanyId = 0,
      EmployeeId = 1,
      DateFrom = new DateTime(2025, 1, 1),
      DateTo = new DateTime(2025, 12, 31)
    };

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
