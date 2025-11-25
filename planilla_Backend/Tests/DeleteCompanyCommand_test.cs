using Moq;
using Planilla_Backend.CleanArchitecture.Application.EmailsDTOs;
using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Application.Reports;
using Planilla_Backend.CleanArchitecture.Application.UseCases.company;
using Planilla_Backend.LayeredArchitecture.Services.EmailService;

namespace Tests;

public class DeleteCompanyCommand_test
{
  IDeleteCompanyCommand _deleteCompanyCommand;
  Mock<ICompanyRepository> _CompanyRepositoryMock;
  Mock<IEmailService> _emailServiceMock;

  [SetUp]
  public void Setup()
  {
    // Mock para la función interna que envía los correos electrónicos a los empleados
    DeleteCompanyEmployeeDataDTO deleteCompanyEmployeeDataDTO = new DeleteCompanyEmployeeDataDTO
    {
      UserName = "Test User",
      EmployeeEmail = "unitTest@gmail.com",
      CompanyName = "Test Company"
    };

    _CompanyRepositoryMock = new Mock<ICompanyRepository>();

    _CompanyRepositoryMock.Setup(r => r.GetEmployeesEmailsAndUserNameInCómpanyByIdCompany(It.IsAny<int>()))
        .ReturnsAsync(new List<DeleteCompanyEmployeeDataDTO> { deleteCompanyEmployeeDataDTO });

    _emailServiceMock = new Mock<IEmailService>();
    _deleteCompanyCommand = new DeleteCompanyCommand(_CompanyRepositoryMock.Object, _emailServiceMock.Object);
    _emailServiceMock
        .Setup(es => es.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .Returns(Task.CompletedTask);
  }

  [Test]
  public void Execute_ShouldThrow_ArgumentException_When_CompanyUniqueIdIsZero()
  {
    // Act & Assert
    var exCompanyZero = Assert.ThrowsAsync<ArgumentException>(
        async () => await _deleteCompanyCommand.Execute(0, 12));

    // Assert
    Assert.That(exCompanyZero.Message, Is.EqualTo("Id de la empresa inválido."));
  }

  [Test]
  public void Execute_ShouldReturn_ArgumentException_When_PersonIdIsZero()
  {
    // Act & Assert
    var exPersonZero = Assert.ThrowsAsync<ArgumentException>(
        async () =>await _deleteCompanyCommand.Execute(12, 0));

    // Assert
    Assert.That(exPersonZero.Message, Is.EqualTo("Id de persona inválido."));
  }

  [Test]
  public void Execute_ShouldThrow_KeyNotFoundException_When_UserIsNotAdminOrOwner()
  {
    // Arrange
    _CompanyRepositoryMock
        .Setup(r => r.IsPersonAdmin(It.IsAny<int>()))
        .ReturnsAsync(0);

    _CompanyRepositoryMock
        .Setup(r => r.IsPersonOwnerOfCompany(It.IsAny<int>(), It.IsAny<int>()))
        .ReturnsAsync(0);

    // Act
    var ex = Assert.ThrowsAsync<KeyNotFoundException>(
        async () => await _deleteCompanyCommand.Execute(10, 20));

    // Assert
    Assert.That(ex.Message, Is.EqualTo("No tiene la autorización para realizar esta acción."));
  }

  [Test]
  public void Execute_ShouldThrow_Exception_When_IsOwnerReturnsMinusOne()
  {
    // Arrange
    _CompanyRepositoryMock
        .Setup(r => r.IsPersonAdmin(It.IsAny<int>()))
        .ReturnsAsync(0);

    _CompanyRepositoryMock
        .Setup(r => r.IsPersonOwnerOfCompany(It.IsAny<int>(), It.IsAny<int>()))
        .ReturnsAsync(-1);

    // Act & Assert
    var ex = Assert.ThrowsAsync<Exception>(
        async () => await _deleteCompanyCommand.Execute(10, 20));

    // Assert
    Assert.That(ex.Message, Is.EqualTo("Error al verificar permisos de la persona."));
  }

  [Test]
  public void Execute_ShouldThrow_Exception_When_IsAdminReturnsMinusOne()
  {
    // Arrange
    _CompanyRepositoryMock
        .Setup(r => r.IsPersonAdmin(It.IsAny<int>()))
        .ReturnsAsync(-1);

    _CompanyRepositoryMock
        .Setup(r => r.IsPersonOwnerOfCompany(It.IsAny<int>(), It.IsAny<int>()))
        .ReturnsAsync(1);

    // Act & Assert
    var ex = Assert.ThrowsAsync<Exception>(
        async () => await _deleteCompanyCommand.Execute(10, 20)
    );

    // Assert
    Assert.That(ex.Message, Is.EqualTo("Error al verificar permisos de la persona."));
  }

  [Test]
  public void Execute_ShouldThrow_Exception_When_DeleteReturnsMinusOne()
  {
    // Arrange
    _CompanyRepositoryMock
        .Setup(r => r.IsPersonAdmin(It.IsAny<int>()))
        .ReturnsAsync(1);

    _CompanyRepositoryMock
        .Setup(r => r.DeleteCompanyByUniqueId(It.IsAny<int>()))
        .ReturnsAsync(-1);

    // Act 
    var ex = Assert.ThrowsAsync<Exception>(
        async () => await _deleteCompanyCommand.Execute(10, 20));

    // Assert
    Assert.That(ex.Message, Is.EqualTo("Error al intentar eliminar la empresa."));
  }

  [Test]
  public async Task Execute_ShouldReturn_RowsAffectedSuccesfuly_When_PersonIsAdmin()
  {
    // Arrange
    _CompanyRepositoryMock
        .Setup(r => r.IsPersonAdmin(It.IsAny<int>()))
        .ReturnsAsync(1);

    _CompanyRepositoryMock
        .Setup(r => r.DeleteCompanyByUniqueId(It.IsAny<int>()))
        .ReturnsAsync(5);

    // Act
    int result = await _deleteCompanyCommand.Execute(10, 20);

    // Assert
    Assert.That(result, Is.EqualTo(5));
  }

  [Test]
  public async Task Execute_ShouldReturn_RowsAffectedSuccesfuly_When_PersonIsOwner()
  {
    // Arrange
    _CompanyRepositoryMock
        .Setup(r => r.IsPersonAdmin(It.IsAny<int>()))
        .ReturnsAsync(0);

    _CompanyRepositoryMock
        .Setup(r => r.IsPersonOwnerOfCompany(It.IsAny<int>(), It.IsAny<int>()))
        .ReturnsAsync(1);

    _CompanyRepositoryMock
        .Setup(r => r.DeleteCompanyByUniqueId(It.IsAny<int>()))
        .ReturnsAsync(5);

    // Act
    int result = await _deleteCompanyCommand.Execute(10, 20);

    // Assert
    Assert.That(result, Is.EqualTo(5));
  }
}


