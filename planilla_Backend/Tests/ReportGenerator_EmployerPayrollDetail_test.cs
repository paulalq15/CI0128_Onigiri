using Moq;
using NUnit.Framework;
using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Application.Reports;
using Planilla_Backend.CleanArchitecture.Domain.Entities;
using Planilla_Backend.CleanArchitecture.Domain.Reports;

namespace Tests
{
    public class ReportGenerator_EmployerPayrollDetail_test
    {
        private ReportGenerator_EmployerPayrollDetail _sut;
        private Mock<IReportRepository> _repository;
        private ReportRequestDto _request;

        [SetUp]
        public void Setup()
        {
            _sut = new ReportGenerator_EmployerPayrollDetail();
            _repository = new Mock<IReportRepository>();

            _request = new ReportRequestDto
            {
                ReportCode = ReportCodes.EmployerDetailPayroll,
                CompanyId = 1,
                EmployeeId = 10,
                PayrollId = 123
            };
        }

        private static EmployerPayrollReport BuildSampleReport() => new EmployerPayrollReport
        {
            CompanyName = "Empresa X",
            EmployerName = "Francisco Arroyo Mora",
            PaymentDate = new DateTime(2025, 10, 31),
            Cost = 1500m,
            Lines = new List<PayrollDetailLine>
            {
                new() { Description = "Salario base", Category = "Salario", Amount = 1000m },
                new() { Description = "CCSS SEM", Category = "Deduccion Empleador", Amount = 300m },
                new() { Description = "Seguro riesgos", Category = "Deduccion Empleador", Amount = 100m },
                new() { Description = "Bono gimnasio", Category = "Beneficio Empleado", Amount = 100m }
            }
        };

        [Test]
        public void GenerateAsync_WhenRequestIsNull_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() =>
              _sut.GenerateAsync(null!, _repository.Object));
        }

        [Test]
        public void GenerateAsync_WhenRepositoryIsNull_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() =>
              _sut.GenerateAsync(_request, null!));
        }

        [Test]
        public void GenerateAsync_WhenCompanyIdInvalid_ThrowsArgumentException()
        {
            _request.CompanyId = 0;

            var ex = Assert.ThrowsAsync<ArgumentException>(() =>
              _sut.GenerateAsync(_request, _repository.Object));

            Assert.That(ex!.Message, Does.Contain("CompanyId"));
        }

        [Test]
        public void GenerateAsync_WhenEmployeeIdInvalid_ThrowsArgumentException()
        {
            _request.EmployeeId = -1;

            var ex = Assert.ThrowsAsync<ArgumentException>(() =>
              _sut.GenerateAsync(_request, _repository.Object));

            Assert.That(ex!.Message, Does.Contain("EmployeeId"));
        }

        [Test]
        public void GenerateAsync_WhenPayrollIdInvalid_ThrowsArgumentException()
        {
            _request.PayrollId = null;

            var ex = Assert.ThrowsAsync<ArgumentException>(() =>
              _sut.GenerateAsync(_request, _repository.Object));

            Assert.That(ex!.Message, Does.Contain("PayrollId"));
        }

        [Test]
        public void GenerateAsync_WhenReportIsNull_ThrowsKeyNotFoundException()
        {
            _repository.Setup(r => r.GetEmployerPayrollReport(123, 10, default))
                       .ReturnsAsync((EmployerPayrollReport?)null);

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
              _sut.GenerateAsync(_request, _repository.Object));
        }

        [Test]
        public async Task GenerateAsync_ReturnsValidReportResult()
        {
            var sample = BuildSampleReport();

            _repository.Setup(r => r.GetEmployerPayrollReport(123, 10, default))
                       .ReturnsAsync(sample);

            var result = await _sut.GenerateAsync(_request, _repository.Object);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ReportCode, Is.EqualTo(ReportCodes.EmployerDetailPayroll));
            Assert.That(result.DisplayName, Is.EqualTo("Detalle de pago de planilla"));

            Assert.That(result.Columns, Has.Count.EqualTo(3));

            Assert.That(result.ReportInfo["CompanyName"], Is.EqualTo(sample.CompanyName));
            Assert.That(result.ReportInfo["EmployerName"], Is.EqualTo(sample.EmployerName));
            Assert.That(result.ReportInfo["PaymentDate"], Is.EqualTo(sample.PaymentDate));
        }


        [Test]
        public async Task GenerateAsync_BuildsSalaryAndTotalsCorrectly()
        {
            var data = BuildSampleReport();

            _repository.Setup(r => r.GetEmployerPayrollReport(123, 10, default))
                       .ReturnsAsync(data);

            var result = await _sut.GenerateAsync(_request, _repository.Object);

            var rows = result.Rows;

            Assert.That(rows.Count, Is.GreaterThan(5));

            var summary = rows.Last();
            Assert.That(summary["Descripción"], Is.EqualTo("Costo total empleador"));
            Assert.That(summary["Monto"], Is.EqualTo(data.Cost));
        }


        [Test]
        public async Task GenerateAsync_CallsRepositoryWithCorrectParams()
        {
            var sample = BuildSampleReport();

            _repository.Setup(r => r.GetEmployerPayrollReport(It.IsAny<int>(), It.IsAny<int>(), default))
                       .ReturnsAsync(sample);

            await _sut.GenerateAsync(_request, _repository.Object);

            _repository.Verify(r => r.GetEmployerPayrollReport(123, 10, default), Times.Once);
        }
    }
}
