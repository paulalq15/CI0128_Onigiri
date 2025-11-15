using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Domain.Reports;

namespace Planilla_Backend.CleanArchitecture.Application.Reports
{
  public class ReportGenerator_EmployeePayrollDetail : IReportGenerator
  {
    public string ReportCode
    {
      get { return ReportCodes.EmployeeDetailPayroll; }
    }
    public async Task<ReportResultDto> GenerateAsync(ReportRequestDto request, IReportRepository repository, CancellationToken ct = default)
    {
      if (request is null) throw new ArgumentNullException(nameof(request));
      if (repository is null) throw new ArgumentNullException(nameof(repository));
      if (!request.CompanyId.HasValue || request.CompanyId.Value <= 0) throw new ArgumentException("El parámetro CompanyId es requerido y debe ser mayor que cero", nameof(request));
      if (!request.EmployeeId.HasValue || request.EmployeeId.Value <= 0) throw new ArgumentException("El parámetro EmployeeId es requerido y debe ser mayor que cero", nameof(request));
      if (!request.PayrollId.HasValue || request.PayrollId.Value <= 0) throw new ArgumentException("El parámetro PayrollId es requerido y debe ser mayor que cero", nameof(request));

      var report = await repository.GetEmployeePayrollReport(request.PayrollId.Value, ct);
      if (report is null) throw new KeyNotFoundException("No se encontró información de pago para el empleado en la planilla seleccionada");
      if (report.CompanyId != request.CompanyId.Value) throw new KeyNotFoundException("La empresa solicitante no coincide con la empresa registrada en la planilla");
      if (report.EmployeeId != request.EmployeeId.Value) throw new KeyNotFoundException("El empleado solicitante no coincide con el empleado registrado en la planilla");

      var rows = BuildRows(report);

      var result = new ReportResultDto
      {
        ReportCode = ReportCodes.EmployeeDetailPayroll,
        DisplayName = "Detalle de pago de planilla",
        Columns = new List<string> { "Descripción", "Categoría", "Monto" },
        Rows = rows,
        ReportInfo = new Dictionary<string, object?>
        {
          ["CompanyName"] = report.CompanyName,
          ["EmployeeName"] = report.EmployeeName,
          ["EmployeeType"] = report.EmployeeType.ToString(),
          ["PaymentDate"] = report.PaymentDate
        }
      };

      return result;
    }

    private static List<Dictionary<string, object?>> BuildRows(EmployeePayrollReport report)
    {
      var rows = new List<Dictionary<string, object?>>();

      var salaryLines = report.Lines.Where(l => l.Category == "Salario").ToList();
      var mandatoryLines = report.Lines.Where(l => l.Category == "Deduccion obligatoria").ToList();
      var voluntaryLines = report.Lines.Where(l => l.Category == "Deduccion voluntaria").ToList();
      var benefitLines = report.Lines.Where(l => l.Category == "Beneficio").ToList();

      AddLines(rows, salaryLines);

      if (mandatoryLines.Any())
      {
        AddLines(rows, mandatoryLines);
        AddTotalRow(rows, "Total deducciones obligatorias", "Deduccion obligatoria", mandatoryLines);
      }

      if (voluntaryLines.Any())
      {
        AddLines(rows, voluntaryLines);
        AddTotalRow(rows, "Total deducciones voluntarias", "Deduccion voluntaria", voluntaryLines);
      }

      if (benefitLines.Any())
      {
        AddLines(rows, benefitLines);
        AddTotalRow(rows, "Total beneficios", "Beneficio", benefitLines);
      }

      rows.Add(new Dictionary<string, object?>
      {
        ["Descripción"] = "Pago Neto",
        ["Categoría"] = "Resumen",
        ["Monto"] = report.NetAmount
      });

      return rows;
    }

    private static void AddLines(
      List<Dictionary<string, object?>> rows,
      IEnumerable<PayrollDetailLine> lines)
    {
      foreach (var line in lines)
      {
        rows.Add(new Dictionary<string, object?>
        {
          ["Descripción"] = line.Description,
          ["Categoría"] = line.Category,
          ["Monto"] = line.Amount
        });
      }
    }

    private static void AddTotalRow(
      List<Dictionary<string, object?>> rows,
      string description,
      string category,
      IEnumerable<PayrollDetailLine> lines)
    {
      rows.Add(new Dictionary<string, object?>
      {
        ["Descripción"] = description,
        ["Categoría"] = category,
        ["Monto"] = lines.Sum(l => l.Amount)
      });
    }
  }
}
