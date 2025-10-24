//using Moq;
using Planilla_Backend.CleanArchitecture.Domain.Calculation;
using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Tests
{
  public class Salary_HourlyStrategyTest
  {

    Salary_HourlyStrategy _sut;

    [SetUp]
    public void Setup()
    {
      _sut = new Salary_HourlyStrategy();
    }

    [Test]
    public void Applicable_ShouldReturnTrue_WhenContractTypeIsProfessionalServices()
    {
      var contract = new ContractModel
      {
        Id = 1,
        EmployeeId = 1,
        Salary = 1000m,
        PaymentAccount = "123456789",
        ContractType = ContractType.ProfessionalServices,
        StartDate = DateTime.Now.AddMonths(-1),
        EndDate = null
      };

      var result = _sut.Applicable(contract);

      Assert.IsTrue(result);
    }

    [Test]
    public void Applicable_ShouldReturnFalse_WhenContractTypeIsNotProfessionalServices()
    {
      var contract = new ContractModel
      {
        Id = 1,
        EmployeeId = 1,
        Salary = 1000m,
        PaymentAccount = "123456789",
        ContractType = ContractType.FixedSalary,
        StartDate = DateTime.Now.AddMonths(-1),
        EndDate = null
      };

      var result = _sut.Applicable(contract);

      Assert.IsFalse(result);
    }

    [TestCase(50, 0)]
    [TestCase(0, 100)]
    [TestCase(560.29, 99)]
    [TestCase(1500, 100)]
    public void CreateBaseLine_ShouldCreatePayrollLineWithCorrectAmountAndSaveGrossSalary(decimal hourlyRate, decimal hoursWorked)
    {
      var employeeId = 1;
      var employeePayroll = new EmployeePayrollModel
      {
        Id = 1,
        CompanyPayrollId = 10,
        EmployeeId = employeeId,
        Gross = 0m,
        EmployeeDeductions = 0m,
        EmployerDeductions = 0m,
        Benefits = 0m,
        Net = 0m,
        Cost = 0m,
        BaseSalaryForPeriod = 0m
      };

      var contract = new ContractModel
      {
        Id = 1,
        EmployeeId = 1,
        Salary = hourlyRate,
        PaymentAccount = "123456789",
        ContractType = ContractType.ProfessionalServices,
        StartDate = new DateTime(2025, 10, 01),
        EndDate = null
      };

      var context = new PayrollContext
      {
        DateFrom = new DateTime(2025, 10, 01),
        DateTo = new DateTime(2025, 10, 15),
        HoursByEmployee = new Dictionary<int, decimal>
        {
          { employeeId, hoursWorked }
        }
      };

      var expectedSalary = Math.Round(hourlyRate * hoursWorked, 2);
      var detail = _sut.CreateBaseLine(employeePayroll, contract, context);

      Assert.AreEqual(expectedSalary, employeePayroll.Gross);
      Assert.IsNotNull(detail);
      Assert.AreEqual(employeePayroll.Id, detail.EmployeePayrollId);
      Assert.AreEqual("Servicios Profesionales", detail.Description);
      Assert.AreEqual(PayrollItemType.Base, detail.Type);
      Assert.AreEqual(expectedSalary, detail.Amount);
      Assert.IsNull(detail.IdCCSS);
      Assert.IsNull(detail.IdTax);
      Assert.IsNull(detail.IdElement);
    }

    [Test]
    public void CreateBaseLine_ShouldThrow_ArgumentNullException_WhenEmployeePayrollIsNull()
    {
      var hoursWorked = 30m;
      var hourlyRate = 100m;

      var contract = new ContractModel
      {
        Id = 1,
        EmployeeId = 1,
        Salary = hourlyRate,
        PaymentAccount = "123456789",
        ContractType = ContractType.ProfessionalServices,
        StartDate = DateTime.Now.AddMonths(-1),
        EndDate = null
      };
      var context = new PayrollContext
      {
        DateFrom = DateTime.Today.AddDays(-15),
        DateTo = DateTime.Today,
        HoursByEmployee = new Dictionary<int, decimal>
        {
          { 1, hoursWorked }
        }
      };
      Assert.Throws<ArgumentNullException>(() => _sut.CreateBaseLine(null, contract, context));
    }

    [Test]
    public void CreateBaseLine_ShouldThrow_ArgumentNullException_WhenContractIsNull()
    {
      var hoursWorked = 30m;
      var hourlyRate = 100m;

      var employeePayroll = new EmployeePayrollModel
      {
        Id = 1,
        CompanyPayrollId = 10,
        EmployeeId = 1,
      };
      var context = new PayrollContext
      {
        DateFrom = DateTime.Today.AddDays(-15),
        DateTo = DateTime.Today,
        HoursByEmployee = new Dictionary<int, decimal>
        {
          { 1, hoursWorked }
        }
      };
      Assert.Throws<ArgumentNullException>(() => _sut.CreateBaseLine(employeePayroll, null, context));
    }

    [Test]
    public void CreateBaseLine_ShouldThrow_ArgumentNullException_WhenContextIsNull()
    {
      var hoursWorked = 30m;
      var hourlyRate = 100m;
      var employeePayroll = new EmployeePayrollModel
      {
        Id = 1,
        CompanyPayrollId = 10,
        EmployeeId = 1,
      };
      var contract = new ContractModel
      {
        Id = 1,
        EmployeeId = 1,
        Salary = hourlyRate,
        PaymentAccount = "123456789",
        ContractType = ContractType.ProfessionalServices,
        StartDate = DateTime.Now.AddMonths(-1),
        EndDate = null
      };
      Assert.Throws<ArgumentNullException>(() => _sut.CreateBaseLine(employeePayroll, contract, null));
    }

    [Test]
    public void CreateBaseLine_ShouldThrow_InvalidOperationException_WhenContextHoursByEmployeeIsNull()
    {
      var employeeId = 1;
      var secondEmployeeId = 10;
      var hoursWorked = 30m;
      var hourlyRate = 100m;

      var employeePayroll = new EmployeePayrollModel
      {
        Id = 1,
        CompanyPayrollId = 10,
        EmployeeId = employeeId,
      };

      var contract = new ContractModel
      {
        Id = 1,
        EmployeeId = 1,
        Salary = hourlyRate,
        PaymentAccount = "123456789",
        ContractType = ContractType.ProfessionalServices,
        StartDate = DateTime.Now.AddMonths(-1),
        EndDate = null
      };

      var context = new PayrollContext
      {
        DateFrom = DateTime.Today.AddDays(-15),
        DateTo = DateTime.Today,
        HoursByEmployee = new Dictionary<int, decimal>
        {
          { secondEmployeeId, hoursWorked }
        }
      };

      Assert.Throws<InvalidOperationException>(() => _sut.CreateBaseLine(employeePayroll, contract, context));
    }
  }
}