using Planilla_Backend.CleanArchitecture.Domain.Calculation;
using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Tests;

public class Concept_FixedAmountStrategy_test
{
  private IConceptStrategy conceptStrategy;
  private EmployeePayrollModel employeePayroll;
  private ElementModel concept;
  
  private EmployeePayrollModel employeePayrollnull;
  private ElementModel conceptnull;

  [SetUp]
  public void Setup()
  {
    // Arrange
    this.conceptStrategy = new Concept_FixedAmountStrategy();
    this.employeePayroll = new EmployeePayrollModel { Id = 1 };

    this.concept = new ElementModel
    {
      Name = "Elemento de prueba",
      ItemType = PayrollItemType.Benefit,
      Value = 50,
      Id = 1
    };
  }

  [Test]
  public void ApplyShoulReturnArgumentException_employeePayroll_IsNull()
  {
    // Act & Assert
    var ex = Assert.Throws<ArgumentNullException>(() => this.conceptStrategy.Apply(this.employeePayrollnull, this.concept, null, null));

    // Assert
    Assert.IsNull(this.employeePayrollnull);
    Assert.That(ex.ParamName, Is.EqualTo("La planilla del empleado es requerida"));
  }

  [Test]
  public void ApplyShoulReturnArgumentException_concept_IsNull()
  {
    // Act & Assert
    var ex = Assert.Throws<ArgumentNullException>(() => this.conceptStrategy.Apply(this.employeePayroll, this.conceptnull, null, null));

    // Assert
    Assert.IsNull(this.conceptnull);
    Assert.That(ex.ParamName, Is.EqualTo("El elemento de planilla es requerido"));
  }
}
