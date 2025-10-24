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
        ContractType = ContractType.FixedSalary,
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
        ContractType = ContractType.ProfessionalServices,
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
        ContractType = ContractType.FixedSalary,
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
        ContractType = ContractType.FixedSalary,
        StartDate = DateTime.Now.AddMonths(-1),
        EndDate = null
      };
      Assert.Throws<ArgumentNullException>(() => _sut.CreateBaseLine(employeePayroll, contract, null));
    }

    [Test]
    public void CreateBaseLine_FixedSalary_ShouldThrow_InvalidOperationException_WhenContextHoursByEmployeeIsNull()
    {
      var employeeId = 1;
      var secondEmployeeId = 10;
      var hoursWorked = 30m;
      var monthlySalary = 500000;

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
        Salary = monthlySalary,
        PaymentAccount = "123456789",
        ContractType = ContractType.FixedSalary,
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