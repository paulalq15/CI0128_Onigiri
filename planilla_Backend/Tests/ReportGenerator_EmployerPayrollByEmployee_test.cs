using Moq;
using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Application.Reports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;
using Planilla_Backend.CleanArchitecture.Domain.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
  internal class ReportGenerator_EmployerPayrollByEmployee_test
  {
    ReportGenerator_EmployerPayrollByEmployee _sut;
    ReportRequestDto _request;
    Mock<IReportRepository> _reportRepositoryMock;

    [SetUp]
    public void Setup()
    {
      _sut = new ReportGenerator_EmployerPayrollByEmployee();
      _reportRepositoryMock = new Mock<IReportRepository>();
      _request = BuildValidRequest();
    }

    private static ReportRequestDto BuildValidRequest() => new ReportRequestDto
    {
      ReportCode = ReportCodes.EmployerEmployeePayroll,
      CompanyId = 1,
      DateFrom = new DateTime(2025, 1, 1),
      DateTo = new DateTime(2025, 12, 31)
    };

    private static List<EmployerReportByPersonRow> BuildSampleRows() => new()
    {
      new EmployerReportByPersonRow
      {
        EmployeeName = "Ana Vargas Vargas",
        NationalId = "1-1111-1111",
        EmployeeType = EmployeeType.FullTime.ToString(),
        PaymentPeriod = "2025-10-01 al 2025-10-31",
        PaymentDate = new DateTime(2025, 11, 01, 12, 0, 0),
        GrossSalary = 1000000,
        EmployerContributions = 266700,
        EmployeeBenefits = 25000,
        EmployerCost = 1291700
      },
      new EmployerReportByPersonRow
      {
        EmployeeName = "Jorge Rojas Rojas",
        NationalId = "2-2222-2222",
        EmployeeType = EmployeeType.FullTime.ToString(),
        PaymentPeriod = "2025-10-01 al 2025-10-31",
        PaymentDate = new DateTime(2025, 11, 01, 12, 0, 0),
        GrossSalary = 1000000,
        EmployerContributions = 266700,
        EmployeeBenefits = 25000,
        EmployerCost = 1291700
      }
    };
    
    [Test]
    public async Task GenerateAsync_CalculateTotalsCorrectly()
    {
      // Arrange
      var rows = BuildSampleRows();

      _reportRepositoryMock
        .Setup(r => r.GetEmployerPayrollByPersonAsync(
          It.IsAny<int>(),
          It.IsAny<DateTime>(),
          It.IsAny<DateTime>(),
          It.IsAny<string?>(),
          It.IsAny<string?>(),
          It.IsAny<CancellationToken>()))
        .ReturnsAsync(rows);

      // Act
      var result = await _sut.GenerateAsync(_request, _reportRepositoryMock.Object, CancellationToken.None);

      // Assert
      Assert.That(result.ReportCode, Is.EqualTo(ReportCodes.EmployerEmployeePayroll));
      Assert.That(result.DisplayName, Is.EqualTo("Detalle de Planilla Por Empleado"));
      Assert.That(result.Columns, Is.EquivalentTo(new[]
      {
        "EmployeeName",
        "NationalId",
        "EmployeeType",
        "PaymentPeriod",
        "PaymentDate",
        "GrossSalary",
        "EmployerContributions",
        "EmployeeBenefits",
        "EmployerCost"
      }));

      Assert.That(result.Rows.Count, Is.EqualTo(3));

      Dictionary<string, object?> Row(int i) => result.Rows[i];

      var r0 = Row(0);
      Assert.That(r0["EmployeeName"], Is.EqualTo("Ana Vargas Vargas"));
      Assert.That(r0["NationalId"], Is.EqualTo("1-1111-1111"));
      Assert.That(r0["EmployeeType"], Is.EqualTo("FullTime"));
      Assert.That(r0["PaymentPeriod"], Is.EqualTo("2025-10-01 al 2025-10-31"));
      Assert.That(r0["PaymentDate"], Is.EqualTo("01-11-2025"));
      Assert.That(r0["GrossSalary"], Is.EqualTo(1000000));
      Assert.That(r0["EmployerContributions"], Is.EqualTo(266700));
      Assert.That(r0["EmployeeBenefits"], Is.EqualTo(25000));
      Assert.That(r0["EmployerCost"], Is.EqualTo(1291700));

      var r1 = Row(1);
      Assert.That(r1["EmployeeName"], Is.EqualTo("Jorge Rojas Rojas"));
      Assert.That(r1["NationalId"], Is.EqualTo("2-2222-2222"));
      Assert.That(r1["EmployeeType"], Is.EqualTo("FullTime"));
      Assert.That(r1["PaymentPeriod"], Is.EqualTo("2025-10-01 al 2025-10-31"));
      Assert.That(r1["PaymentDate"], Is.EqualTo("01-11-2025"));
      Assert.That(r1["GrossSalary"], Is.EqualTo(1000000));
      Assert.That(r1["EmployerContributions"], Is.EqualTo(266700));
      Assert.That(r1["EmployeeBenefits"], Is.EqualTo(25000));
      Assert.That(r1["EmployerCost"], Is.EqualTo(1291700));

      var r2 = Row(2);
      Assert.That(r2["EmployeeName"], Is.EqualTo("Total"));
      Assert.That(r2["GrossSalary"], Is.EqualTo(2000000));
      Assert.That(r2["EmployerContributions"], Is.EqualTo(533400));
      Assert.That(r2["EmployeeBenefits"], Is.EqualTo(50000));
      Assert.That(r2["EmployerCost"], Is.EqualTo(2583400));
    }

  } // end class
}
