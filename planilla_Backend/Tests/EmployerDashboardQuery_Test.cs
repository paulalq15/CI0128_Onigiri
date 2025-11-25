using Moq;
using Planilla_Backend.CleanArchitecture.Application.Dashboards;
using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Tests
{
  public class EmployerDashboardQuery_Test
  {
    private Mock<IDashboardRepository> _dashboardRepoMock = null!;
    private EmployerDashboardQuery _sut = null!;

    [SetUp]
    public void Setup()
    {
      _dashboardRepoMock = new Mock<IDashboardRepository>(MockBehavior.Strict);
      _sut = new EmployerDashboardQuery(_dashboardRepoMock.Object);
    }

    [Test]
    public void GetDashboardAsync_WhenCompanyIdIsZero_ThrowsArgumentOutOfRangeException()
    {
      // Arrange
      int invalidCompanyId = 0;

      // Act
      var ex = Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _sut.GetDashboardAsync(invalidCompanyId));

      // Assert
      Assert.That(ex!.ParamName, Is.EqualTo("companyId"));
    }

    [Test]
    public void GetDashboardAsync_WhenCompanyIdIsNegative_ThrowsArgumentOutOfRangeException()
    {
      // Arrange
      int invalidCompanyId = -1;

      // Act
      var ex = Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _sut.GetDashboardAsync(invalidCompanyId));

      // Assert
      Assert.That(ex!.ParamName, Is.EqualTo("companyId"));
    }

    [Test]
    public async Task GetDashboardAsync_WhenCompanyIdIsValid_CallsAllRepositoryMethodsOnce()
    {
      // Arrange
      int companyId = 10;

      var costByType = new List<EmployerDashboardCostByTypeModel>();
      var costByMonth = new List<EmployerDashboardCostByMonthModel>();
      var employeeCount = new List<EmployerDashboardEmployeeCountByTypeModel>();

      _dashboardRepoMock.Setup(r => r.GetEmployerDashboardCostByType(companyId)).ReturnsAsync(costByType);
      _dashboardRepoMock.Setup(r => r.GetEmployerDashboardCostByMonth(companyId)).ReturnsAsync(costByMonth);
      _dashboardRepoMock.Setup(r => r.GetEmployerDashboardEmployeeCountByType(companyId)).ReturnsAsync(employeeCount);

      // Act
      var result = await _sut.GetDashboardAsync(companyId);

      // Assert
      Assert.That(result, Is.Not.Null);

      _dashboardRepoMock.Verify(r => r.GetEmployerDashboardCostByType(companyId), Times.Once);
      _dashboardRepoMock.Verify(r => r.GetEmployerDashboardCostByMonth(companyId), Times.Once);
      _dashboardRepoMock.Verify(r => r.GetEmployerDashboardEmployeeCountByType(companyId), Times.Once);
      _dashboardRepoMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetDashboardAsync_WhenRepositoryReturnsData_ReturnsAggregatedDto()
    {
      // Arrange
      int companyId = 14;

      var expectedCostByType = new List<EmployerDashboardCostByTypeModel>
      {
        new EmployerDashboardCostByTypeModel { CostType = "Salarios", TotalCost = 1000m },
        new EmployerDashboardCostByTypeModel { CostType = "Cargas Sociales", TotalCost = 500m }
      };

      var expectedCostByMonth = new List<EmployerDashboardCostByMonthModel>
      {
        new EmployerDashboardCostByMonthModel { Month = "10-2025", TotalCost = 2000m },
        new EmployerDashboardCostByMonthModel { Month = "11-2025", TotalCost = 2500m }
      };

      var expectedEmployeeCountByType = new List<EmployerDashboardEmployeeCountByTypeModel>
      {
        new EmployerDashboardEmployeeCountByTypeModel { EmployeeType = "Tiempo Completo", EmployeeCount = 3 },
        new EmployerDashboardEmployeeCountByTypeModel { EmployeeType = "Medio Tiempo", EmployeeCount = 1 }
      };

      _dashboardRepoMock.Setup(r => r.GetEmployerDashboardCostByType(companyId)).ReturnsAsync(expectedCostByType);
      _dashboardRepoMock.Setup(r => r.GetEmployerDashboardCostByMonth(companyId)).ReturnsAsync(expectedCostByMonth);
      _dashboardRepoMock.Setup(r => r.GetEmployerDashboardEmployeeCountByType(companyId)).ReturnsAsync(expectedEmployeeCountByType);

      // Act
      var result = await _sut.GetDashboardAsync(companyId);

      // Assert
      Assert.That(result, Is.Not.Null);
      Assert.That(result.CostByTypes, Is.EqualTo(expectedCostByType));
      Assert.That(result.CostByMonth, Is.EqualTo(expectedCostByMonth));
      Assert.That(result.EmployeeCountByType, Is.EqualTo(expectedEmployeeCountByType));
    }
  } // end class
}
