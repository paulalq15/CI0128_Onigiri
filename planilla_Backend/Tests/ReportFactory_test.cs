using Moq;
using Planilla_Backend.CleanArchitecture.Application.Reports;

namespace Tests
{
  internal class ReportFactory_test
  {
    private Mock<IReportGenerator> _g1;
    private Mock<IReportGenerator> _g2;

    [SetUp]
    public void Setup()
    {
      _g1 = new Mock<IReportGenerator>();
      _g1.SetupGet(g => g.ReportCode).Returns("EmployeeDetailPayroll");

      _g2 = new Mock<IReportGenerator>();
      _g2.SetupGet(g => g.ReportCode).Returns("EmployerHistory");
    }

    [Test]
    public void Constructor_ShouldThrow_WhenGeneratorsIsNull()
    {
      Assert.Throws<ArgumentNullException>(() => new ReportFactory(null!));
    }

    [Test]
    public void Constructor_ShouldLoadGeneratorsIntoDictionary()
    {
      var list = new[] { _g1.Object, _g2.Object };

      var factory = new ReportFactory(list);

      Assert.Multiple(() =>
      {
        Assert.That(factory.GetGenerator("EmployeeDetailPayroll"), Is.SameAs(_g1.Object));
        Assert.That(factory.GetGenerator("EmployerHistory"), Is.SameAs(_g2.Object));
      });
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void GetGenerator_ShouldThrow_WhenReportCodeIsInvalid(string? code)
    {
      var list = new[] { _g1.Object };

      var factory = new ReportFactory(list);

      var ex = Assert.Throws<ArgumentException>(() => factory.GetGenerator(code!));
      StringAssert.Contains("código de reporte es requerido", ex!.Message);
    }

    [Test]
    public void GetGenerator_ShouldThrow_WhenGeneratorDoesNotExist()
    {
      var factory = new ReportFactory(new[] { _g1.Object });

      var ex = Assert.Throws<InvalidOperationException>(() => factory.GetGenerator("OtherReport"));
      StringAssert.Contains("No existe un generador configurado", ex!.Message);
    }

    [Test]
    public void GetGenerator_ShouldBeCaseInsensitive()
    {
      var factory = new ReportFactory(new[] { _g1.Object });

      // g1 tiene código: "EmployeeDetailPayroll"
      var result = factory.GetGenerator("employeedetailpayroll");

      Assert.That(result, Is.SameAs(_g1.Object));
    }
  }
}
