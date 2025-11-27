using Moq;
using Planilla_Backend.CleanArchitecture.Application.Dashboards;
using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Tests
{
    public class EmployeeDashboardQuery_Test
    {
        private Mock<IDashboardRepository> _dashboardRepoMock = null!;
        private EmployeeDashboardQuery _sut = null!;

        [SetUp]
        public void Setup()
        {
            _dashboardRepoMock = new Mock<IDashboardRepository>(MockBehavior.Strict);
            _sut = new EmployeeDashboardQuery(_dashboardRepoMock.Object);
        }

        [Test]
        public void GetDashboardAsync_WhenEmployeeIdIsZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange:
            int invalidEmployeeId = 0;

            // Act:
            var ex = Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _sut.GetDashboardAsync(invalidEmployeeId));

            // Assert:
            Assert.That(ex!.ParamName, Is.EqualTo("employeeId"));
        }

        [Test]
        public void GetDashboardAsync_WhenCompanyIdIsNegative_ThrowsArgumentOutOfRangeException()
        {
            // Arrange:
            int employeeId = -1;

            // Act:
            var ex = Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _sut.GetDashboardAsync(employeeId));

            // Assert:
            Assert.That(ex!.ParamName, Is.EqualTo("employeeId"));
        }

        [Test]
        public async Task GetDashboardAsync_WhenCompanyIdIsValid_CallsAllRepositoryMethodsOnce()
        {
            // Arrange:
            int employeeId = 15;

            var employeeFiguresPerMonth = new List<EmployeeDashboardEmployeeFiguresPerMonth>();

            _dashboardRepoMock.Setup(r => r.GetEmployeeFiguresPerMonth(employeeId)).ReturnsAsync(employeeFiguresPerMonth);


            // Act:
            var result = await _sut.GetDashboardAsync(employeeId);

            // Assert:
            Assert.That(result, Is.Not.Null);

            _dashboardRepoMock.Verify(r => r.GetEmployeeFiguresPerMonth(employeeId), Times.Once);
            _dashboardRepoMock.VerifyNoOtherCalls();
        }
    }
}
