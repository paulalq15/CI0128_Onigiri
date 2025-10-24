using Planilla_Backend.CleanArchitecture.Domain.Calculation;
using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Tests
{
  public class LegalConcept_TaxStrategyTest
  {
    LegalConcept_TaxStrategy _sut;
    PayrollContext _ctx;

    [SetUp]
    public void Setup()
    {
      _sut = new LegalConcept_TaxStrategy();
      _ctx = BuildContextWith2025TaxBrackets();
    }

    private static PayrollContext BuildContextWith2025TaxBrackets() => new PayrollContext
    {
      TaxBrackets = new List<TaxModel>
      {
        new() {Id = 1, Min = 0m, Max = 922000m, Rate = 0.00m, ItemType = PayrollItemType.EmployeeDeduction},
        new() {Id = 2, Min = 922000m, Max = 1352000m, Rate = 0.10m, ItemType = PayrollItemType.EmployeeDeduction},
        new() {Id = 3, Min = 1352000m, Max = 2373000m, Rate = 0.15m, ItemType = PayrollItemType.EmployeeDeduction},
        new() {Id = 4, Min = 2373000m, Max = 4745000m, Rate = 0.20m, ItemType = PayrollItemType.EmployeeDeduction},
        new() {Id = 5, Min = 4745000m, Max = null, Rate = 0.25m, ItemType = PayrollItemType.EmployeeDeduction},
      }
    };

    [TestCase(0, 0, 1)]
    [TestCase(400000, 0, 1)]
    [TestCase(922000, 0, 1)]
    [TestCase(1200000, 27800, 2)]
    [TestCase(1352000, 43000, 2)]
    [TestCase(1352001, 43000.15, 3)]
    [TestCase(1893560.85, 124234.13, 3)]
    [TestCase(2373000, 196150, 3)]
    [TestCase(3560000, 433550, 4)]
    [TestCase(4745000, 670550, 4)]
    [TestCase(4800000, 684300, 5)]
    [TestCase(6200890.59, 1034522.65, 5)]
    public void Apply_ShouldReturnTheSameExpectedValueAndTaxId(decimal baseSalary, decimal expected, int taxId)
    {
      var employeeId = 1;

      var employeePayroll = new EmployeePayrollModel
      {
        Id = 1,
        CompanyPayrollId = 1,
        EmployeeId = employeeId,
        BaseSalaryForPeriod = baseSalary
      };

      var detail = _sut.Apply(employeePayroll, _ctx).First();

      Assert.That(detail.Amount, Is.EqualTo(expected));
      Assert.That(detail.IdTax, Is.EqualTo(taxId));
    }
  }
}