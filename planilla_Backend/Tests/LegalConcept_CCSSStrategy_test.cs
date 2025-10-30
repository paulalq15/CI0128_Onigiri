using Planilla_Backend.CleanArchitecture.Domain.Calculation;
using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Tests;

public class LegalConcept_CCSSStrategy_test
{
  private ILegalConceptStrategy legalConceptStrategy;
  private EmployeePayrollModel employeePayroll;
  private PayrollContext ctx;
  
  private EmployeePayrollModel employeePayrollnull;
  private PayrollContext ctxnull;

  [SetUp]
  public void Setup()
  {
    // Arrange
    this.legalConceptStrategy = new LegalConcept_CCSSStrategy();
    this.employeePayroll = new EmployeePayrollModel { 
      Id = 1,
      Gross = 400000
    };

    this.ctx = new PayrollContext();
  }

  [Test]
  public void Applicable_ShouldReturnTrue_WhenContractTypeIsFixedSalary()
  {
    // Arrange
    var contract = new ContractModel { ContractType = ContractType.FixedSalary };

    // Act
    var result = this.legalConceptStrategy.Applicable(contract);

    // Assert
    Assert.IsTrue(result);
  }

  [Test]
  public void Applicable_ShouldReturnFalse_WhenContractIsNullOrNotFixedSalary()
  {
    // Arrange
    var contract = new ContractModel { ContractType = ContractType.ProfessionalServices };

    // Act
    var resultNull = this.legalConceptStrategy.Applicable(null);
    var resultProfServices = this.legalConceptStrategy.Applicable(contract);

    // Assert
    Assert.IsFalse(resultNull);
    Assert.IsFalse(resultProfServices);
  }


  [Test]
  public void ApplyShoulReturnArgumentException_employeePayroll_IsNull()
  {
    // Act & Assert
    var ex = Assert.Throws<ArgumentNullException>(() => this.legalConceptStrategy.Apply(this.employeePayrollnull, this.ctx));

    // Assert
    Assert.IsNull(this.employeePayrollnull);
    Assert.That(ex.ParamName, Is.EqualTo("La planilla del empleado es requerida"));
  }

  [Test]
  public void ApplyShoulReturnArgumentException_ctx_IsNull()
  {
    // Act & Assert
    var ex = Assert.Throws<ArgumentNullException>(() => this.legalConceptStrategy.Apply(this.employeePayroll, this.ctxnull));

    // Assert
    Assert.IsNull(this.ctxnull);
    Assert.That(ex.ParamName, Is.EqualTo("El contexto de planilla es requerido"));
  }

  [Test]
  public void ApplyShoulReturnInvalidOperationException_ctx_CCSSRates_IsNull_Or_ctx_CCSSRates_Count_IsZero()
  {
    // Act & Assert
    var exNullOrZero = Assert.Throws<InvalidOperationException>(() => this.legalConceptStrategy.Apply(this.employeePayroll, this.ctx));

    // Assert
    Assert.That(this.ctx.CCSSRates, Is.Null.Or.Empty);
    Assert.That(exNullOrZero.Message, Does.Contain("Las tasas de la CCSS son requeridas"));
  }
}
