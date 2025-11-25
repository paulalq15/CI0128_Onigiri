using Moq;
using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Application.UseCases;
using Planilla_Backend.CleanArchitecture.Domain.Entities;
using Planilla_Backend.LayeredArchitecture.Services.Email.EmailModels;
using Planilla_Backend.LayeredArchitecture.Services.EmailService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
  internal class PayrollElementCommand_Test
  {
    private Mock<IPayrollElementRepository> _repositoryMock = null!;
    private Mock<IPayrollElementQuery> _queryMock = null!;
    private Mock<IEmailService> _emailServiceMock = null!;
    private PayrollElementCommand _sut = null!; // System Under Test

    [SetUp]
    public void Setup()
    {
      _repositoryMock = new Mock<IPayrollElementRepository>();
      _queryMock = new Mock<IPayrollElementQuery>();
      _emailServiceMock = new Mock<IEmailService>();
      _sut = new PayrollElementCommand(_repositoryMock.Object, _queryMock.Object, _emailServiceMock.Object);
    }

    [Test]
    public void Delete_WhenElementIdIsZero_ThrowsArgumentException()
    {
      // Arrange
      int elementId = 0;

      // Act
      var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.Delete(elementId));

      // Assert
      Assert.That(ex!.Message, Does.Contain("El Id del elemento es obligatorio"));

      _repositoryMock.VerifyNoOtherCalls();
      _queryMock.VerifyNoOtherCalls();
      _emailServiceMock.VerifyNoOtherCalls();
    }

    [Test]
    public void Delete_WhenElementDoesNotExist_ThrowsArgumentException()
    {
      // Arrange
      int elementId = 10;

      _repositoryMock.Setup(r => r.GetPayrollElementByElementId(elementId)).ReturnsAsync((PayrollElementEntity?)null);

      // Act
      var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.Delete(elementId));

      // Assert
      Assert.That(ex!.Message, Does.Contain("El elemento no existe"));

      _repositoryMock.Verify(r => r.GetPayrollElementByElementId(elementId), Times.Once);

      _repositoryMock.Verify(r => r.DeletePayrollElement(It.IsAny<int>()),Times.Never);

      _queryMock.VerifyNoOtherCalls();
      _emailServiceMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task Delete_WhenElementExistsAndNoEmployeesAffected_DeletesElementAndDoesNotSendEmails()
    {
      // Arrange
      int elementId = 10;

      var element = new PayrollElementEntity
      {
        IdElement = elementId,
        ElementName = "Bono"
      };

      _repositoryMock.Setup(r => r.GetPayrollElementByElementId(elementId)).ReturnsAsync(element);

      _repositoryMock.Setup(r => r.DeletePayrollElement(elementId)).ReturnsAsync(1);

      var emptyResult = new DeletePayrollElementEmailListDto
      {
        Entries = new List<DeletePayrollElementEmailDto>()
      };

      _queryMock.Setup(q => q.GetEmployeeEmails(elementId)).ReturnsAsync(emptyResult);

      // Act
      var result = await _sut.Delete(elementId);

      // Assert
      Assert.That(result, Is.EqualTo(1));

      _repositoryMock.Verify(r => r.GetPayrollElementByElementId(elementId), Times.Once);

      _repositoryMock.Verify(r => r.DeletePayrollElement(elementId), Times.Once);

      _queryMock.Verify(q => q.GetEmployeeEmails(elementId), Times.Once);

      // No se deben enviar correos si no hay empleados
      _emailServiceMock.Verify(e => e.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);

      _repositoryMock.VerifyNoOtherCalls();
      _queryMock.VerifyNoOtherCalls();
      _emailServiceMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task Delete_WhenElementExistsAndEmployeesAffected_DeletesElementAndSendsEmails()
    {
      // Arrange
      int elementId = 10;

      var element = new PayrollElementEntity
      {
        IdElement = elementId,
        ElementName = "Bono"
      };

      _repositoryMock.Setup(r => r.GetPayrollElementByElementId(elementId)).ReturnsAsync(element);

      _repositoryMock.Setup(r => r.DeletePayrollElement(elementId)).ReturnsAsync(1);

      var affectedEmployees = new DeletePayrollElementEmailListDto
      {
        Entries = new List<DeletePayrollElementEmailDto>
        {
          new DeletePayrollElementEmailDto { EmployeeEmail = "empleado1@empresa.com" },
          new DeletePayrollElementEmailDto { EmployeeEmail = "empleado2@empresa.com" }
        }
      };

      _queryMock.Setup(q => q.GetEmployeeEmails(elementId)).ReturnsAsync(affectedEmployees);

      _emailServiceMock.Setup(e => e.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

      // Act
      var result = await _sut.Delete(elementId);

      // Assert
      Assert.That(result, Is.EqualTo(1));

      _repositoryMock.Verify(r => r.GetPayrollElementByElementId(elementId), Times.Once);

      _repositoryMock.Verify(r => r.DeletePayrollElement(elementId), Times.Once);

      _queryMock.Verify(q => q.GetEmployeeEmails(elementId), Times.Once);

      _emailServiceMock.Verify(e => e.SendEmail("empleado1@empresa.com", It.IsAny<string>(), It.IsAny<string>()), Times.Once);

      _emailServiceMock.Verify(e => e.SendEmail("empleado2@empresa.com", It.IsAny<string>(), It.IsAny<string>()), Times.Once);

      _emailServiceMock.Verify(e => e.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));

      _repositoryMock.VerifyNoOtherCalls();
      _queryMock.VerifyNoOtherCalls();
      _emailServiceMock.VerifyNoOtherCalls();
    }
  } // end class
}
