using Moq;
using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Application.UseCases;
using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Tests;

public class GetPayrollElementById_test
{
  [SetUp]
  public void Setup()
  {
    // Arrange
  }

  [Test]
  public async Task GetPayrollElementByElementId_ReturnsArgumentException_WhenIdIsZeroOrNegative()
  {
    // Arrange
    int payrollElementIdZero = 0;
    int payrollElementIdNegative = -1;

    var mockRepository = new Mock<IPayrollElementRepository>();
    IGetPayrollElement getPayrollElement = new GetPayrollElementById(mockRepository.Object);

    // Act & Assert
    var exZero = Assert.ThrowsAsync<ArgumentException>(async () => await getPayrollElement.Execute(payrollElementIdZero));

    var exNegative = Assert.ThrowsAsync<ArgumentException>(async () => await getPayrollElement.Execute(payrollElementIdNegative));

    // Assert
    Assert.That(exZero.Message, Does.Contain("El parámetro elementId debe ser mayor que cero"));
    Assert.That(exNegative.Message, Does.Contain("El parámetro elementId debe ser mayor que cero"));
  }

  [Test]
  public async Task GetPayrollElementByElementId_ReturnsExpectedElement()
  {
    // Arrange
    int payrollElementId = 1;

    PayrollElementEntity elementEntity = new PayrollElementEntity();

    var mockRepository = new Mock<IPayrollElementRepository>();
    mockRepository.Setup(r => r.GetPayrollElementByElementId(payrollElementId))
        .ReturnsAsync(elementEntity);

    IGetPayrollElement getPayrollElement = new GetPayrollElementById(mockRepository.Object);

    // Act
    PayrollElementEntity? element = await getPayrollElement.Execute(payrollElementId);

    // Assert
    Assert.IsTrue(element != null);
  }
}
