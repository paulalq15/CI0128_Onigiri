using Planilla_Backend.CleanArchitecture.Domain.Calculation;
using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Tests
{
  public class Salary_FixedStrategyTest
  {

    Salary_FixedStrategy _sut;

    [SetUp]
    public void Setup()
    {
      _sut = new Salary_FixedStrategy();
    }

    [Test]
    public void Applicable_ShouldReturnTrue_WhenContractTypeIsFixedSalary()
    {
      var contract = new ContractModel
      {
        Id = 1,
        EmployeeId = 1,
        Salary = 1000m,
        PaymentAccount = "123456789",
        ContractType = EmployeeType.FullTime,
        StartDate = DateTime.Now.AddMonths(-1),
        EndDate = null
      };
      var result = _sut.Applicable(contract);
      Assert.IsTrue(result);
    }

    [Test]
    public void Applicable_ShouldReturnFalse_WhenContractTypeIsNotFixedSalary()
    {
      var contract = new ContractModel
      {
        Id = 1,
        EmployeeId = 1,
        Salary = 1000m,
        PaymentAccount = "123456789",
        ContractType = EmployeeType.ProfessionalServices,
        StartDate = DateTime.Now.AddMonths(-1),
        EndDate = null
      };
      var result = _sut.Applicable(contract);
      Assert.IsFalse(result);
    }

    [Test]
    public void CreateBaseLine_FixedSalary_ShouldThrow_ArgumentNullException_WhenEmployeePayrollIsNull()
    {
      var monthlySalary = 500000;
      var hoursWorked = 100m;

      var contract = new ContractModel
      {
        Id = 1,
        EmployeeId = 1,
        Salary = monthlySalary,
        PaymentAccount = "123456789",
        ContractType = EmployeeType.FullTime,
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
    public void CreateBaseLine_FixedSalary_ShouldThrow_ArgumentNullException_WhenContractIsNull()
    {
      var hoursWorked = 30m;
      var monthlySalary = 500000;

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
    public void CreateBaseLine_FixedSalary_ShouldThrow_ArgumentNullException_WhenContextIsNull()
    {
      var hoursWorked = 30m;
      var monthlySalary = 500000;
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
        Salary = monthlySalary,
        PaymentAccount = "123456789",
        ContractType = EmployeeType.FullTime,
        StartDate = DateTime.Now.AddMonths(-1),
        EndDate = null
      };
      Assert.Throws<ArgumentNullException>(() => _sut.CreateBaseLine(employeePayroll, contract, null));
    }

    [Test]
    public void CreateBaseLine_FixedSalary_ShouldCreatePayrollLineForTheWholePeriod()
    {
      var employeeId = 1;
      var monthlySalary = 1000000m;
      var hoursWorked = 100m;
      var employeePayroll = new EmployeePayrollModel
      {
        Id = 1,
        CompanyPayrollId = 10,
        EmployeeId = employeeId,
        Gross = 0m,
        BaseSalaryForPeriod = 0m
      };

      var contract = new ContractModel
      {
        Id = 1,
        EmployeeId = 1,
        Salary = monthlySalary,
        PaymentAccount = "123456789",
        ContractType = EmployeeType.FullTime,
        StartDate = new DateTime(2025, 10, 01),
        EndDate = null
      };

      var context = new PayrollContext
      {
        DateFrom = new DateTime(2025, 10, 01),
        DateTo = new DateTime(2025, 10, 15),
        Company = new CompanyModel
        {
          PaymentFrequency = PaymentFrequency.Biweekly
        }
      };

      var expectedSalary = Math.Round(monthlySalary / 2, 2);
      var detail = _sut.CreateBaseLine(employeePayroll, contract, context);

      Assert.AreEqual(expectedSalary, employeePayroll.Gross);
      Assert.AreEqual(monthlySalary, employeePayroll.BaseSalaryForPeriod);
      Assert.IsNotNull(detail);
      Assert.AreEqual(employeePayroll.Id, detail.EmployeePayrollId);
      Assert.AreEqual("Salario Tiempo Completo", detail.Description);
      Assert.AreEqual(PayrollItemType.Base, detail.Type);
      Assert.AreEqual(expectedSalary, detail.Amount);
      Assert.IsNull(detail.IdCCSS);
      Assert.IsNull(detail.IdTax);
      Assert.IsNull(detail.IdElement);
    }

    [Test]
    public void CreateBaseLine_FixedSalary_ShouldCreatePayrollLineForAfterTheContractStarted()
    {
      var employeeId = 1;
      var monthlySalary = 1000000m;
      var hoursWorked = 100m;
      var employeePayroll = new EmployeePayrollModel
      {
        Id = 1,
        CompanyPayrollId = 10,
        EmployeeId = employeeId,
        Gross = 0m,
        BaseSalaryForPeriod = 0m
      };

      var contract = new ContractModel
      {
        Id = 1,
        EmployeeId = 1,
        Salary = monthlySalary,
        PaymentAccount = "123456789",
        ContractType = EmployeeType.ProfessionalServices,
        StartDate = new DateTime(2025, 10, 6),
        EndDate = null
      };

      var context = new PayrollContext
      {
        DateFrom = new DateTime(2025, 10, 01),
        DateTo = new DateTime(2025, 10, 15),
        Company = new CompanyModel
        {
          PaymentFrequency = PaymentFrequency.Biweekly
        }
      };

      var expectedSalary = Math.Round(monthlySalary / 30 * (15 - 6 + 1), 2);
      var expectedMonthlySalary = Math.Round(monthlySalary / 30 * (30 - 6 + 1), 2);
      var detail = _sut.CreateBaseLine(employeePayroll, contract, context);

      Assert.AreEqual(expectedSalary, employeePayroll.Gross);
      Assert.AreEqual(expectedMonthlySalary, employeePayroll.BaseSalaryForPeriod);
      Assert.IsNotNull(detail);
      Assert.AreEqual(employeePayroll.Id, detail.EmployeePayrollId);
      Assert.AreEqual("Salario Servicios Profesionales", detail.Description);
      Assert.AreEqual(PayrollItemType.Base, detail.Type);
      Assert.AreEqual(expectedSalary, detail.Amount);
      Assert.IsNull(detail.IdCCSS);
      Assert.IsNull(detail.IdTax);
      Assert.IsNull(detail.IdElement);
    }

    [Test]
    public void CreateBaseLine_FixedSalary_ShouldCreatePayrollLineForBeforeTheContractEnds()
    {
      var employeeId = 1;
      var monthlySalary = 1000000m;
      var hoursWorked = 100m;
      var employeePayroll = new EmployeePayrollModel
      {
        Id = 1,
        CompanyPayrollId = 10,
        EmployeeId = employeeId,
        Gross = 0m,
        BaseSalaryForPeriod = 0m
      };

      var contract = new ContractModel
      {
        Id = 1,
        EmployeeId = 1,
        Salary = monthlySalary,
        PaymentAccount = "123456789",
        ContractType = EmployeeType.ProfessionalServices,
        StartDate = new DateTime(2025, 10, 6),
        EndDate = new DateTime(2025, 10, 20)
      };

      var context = new PayrollContext
      {
        DateFrom = new DateTime(2025, 10, 01),
        DateTo = new DateTime(2025, 10, 15),
        Company = new CompanyModel
        {
          PaymentFrequency = PaymentFrequency.Biweekly
        }
      };

      var expectedSalary = Math.Round(monthlySalary / 30 * (15 - 6 + 1), 2);
      var expectedMonthlySalary = Math.Round(monthlySalary / 30 * (20 - 6 + 1), 2);
      var detail = _sut.CreateBaseLine(employeePayroll, contract, context);

      Assert.AreEqual(expectedSalary, employeePayroll.Gross);
      Assert.AreEqual(expectedMonthlySalary, employeePayroll.BaseSalaryForPeriod);
      Assert.IsNotNull(detail);
      Assert.AreEqual(employeePayroll.Id, detail.EmployeePayrollId);
      Assert.AreEqual("Salario Servicios Profesionales", detail.Description);
      Assert.AreEqual(PayrollItemType.Base, detail.Type);
      Assert.AreEqual(expectedSalary, detail.Amount);
      Assert.IsNull(detail.IdCCSS);
      Assert.IsNull(detail.IdTax);
      Assert.IsNull(detail.IdElement);
    }
  }
}