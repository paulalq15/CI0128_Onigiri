using Planilla_Backend.CleanArchitecture.Application.Ports;
using Planilla_Backend.CleanArchitecture.Domain.Calculation;
using Planilla_Backend.CleanArchitecture.Domain.Entities;

namespace Planilla_Backend.CleanArchitecture.Application.UseCases
{
  public class CreatePayrollCommand : ICreatePayrollCommand
  {
    private readonly IPayrollRepository _repo;
    private readonly PayrollTemplate _template;

    public CreatePayrollCommand(IPayrollRepository repo, PayrollTemplate template)
    {
      _repo = repo;
      _template = template;
    }

    public async Task<PayrollSummary> Execute(int companyId, int personId, DateTime dateFrom, DateTime dateTo)
    {

      if (companyId <= 0) throw new ArgumentException("El parámetro companyId debe ser mayor que cero");
      if (personId <= 0) throw new ArgumentException("El parámetro personId debe ser mayor que cero");
      if (dateFrom > dateTo) throw new ArgumentException("Rango de fechas inválido");

      var payrollExists = await _repo.ExistsPayrollForPeriod(companyId, dateFrom, dateTo);
      if (payrollExists) throw new InvalidOperationException("Ya existe una planilla para el periodo seleccionado");

      // Kick off all repository calls in parallel
      var companyTask = _repo.GetCompany(companyId);
      var employeesTask = _repo.GetEmployees(companyId, dateFrom, dateTo);
      var contractsTask = _repo.GetContracts(companyId, dateFrom, dateTo);
      var taxesTask = _repo.GetTaxes(dateFrom, dateTo);
      var ccssTask = _repo.GetCCSS(dateFrom, dateTo);
      var hoursTask = _repo.GetEmployeeTimesheets(companyId, dateFrom, dateTo);

      await Task.WhenAll(companyTask, employeesTask, contractsTask, taxesTask, ccssTask, hoursTask);

      var company = await companyTask;
      if (company == null || company.Id <= 0) throw new KeyNotFoundException("La empresa no existe");

      var employees = (await employeesTask).ToList();
      var contracts = (await contractsTask).ToList();
      var taxes = (await taxesTask).ToList();
      var ccss = (await ccssTask).ToList();
      var hoursByEmployee = await hoursTask;

      if (employees == null || employees.Count == 0) throw new InvalidOperationException("No hay empleados con contrato vigente en el periodo seleccionado");

      var elementsByEmployee = await BuildElementsMapAsync(companyId, employees, dateFrom, dateTo);

      // Context object to pass around, created using repository results
      var ctx = new PayrollContext
      {
        Company = company,
        DateFrom = dateFrom,
        DateTo = dateTo,
        Employees = employees,
        Contracts = contracts,
        CCSSRates = ccss,
        TaxBrackets = taxes,
        ElementsByEmployee = elementsByEmployee,
        HoursByEmployee = hoursByEmployee
      };

      // Company payroll with zero totals
      var companyPayroll = new CompanyPayrollModel
      {
        CompanyId = company.Id,
        DateFrom = dateFrom,
        DateTo = dateTo,
        PayrollStatus = "Creado",
        Gross = 0m,
        EmployeeDeductions = 0m,
        EmployerDeductions = 0m,
        Benefits = 0m,
        Net = 0m,
        Cost = 0m,
        CreatedBy = personId,
      };

      var companyPayrollId = await _repo.SaveCompanyPayroll(companyPayroll);
      companyPayroll.Id = companyPayrollId;

      // Employee payroll with zero totals for each employee
      ctx.EmployeePayrollByEmployeeId = new Dictionary<int, EmployeePayrollModel>();

      var i = 0;
      while (i < employees.Count)
      {
        var employee = employees[i];
        
        var employeePayroll = new EmployeePayrollModel
        {
          CompanyPayrollId = companyPayrollId,
          EmployeeId = employee.Id,
          Gross = 0m,
          EmployeeDeductions = 0m,
          EmployerDeductions = 0m,
          Benefits = 0m,
          Net = 0m,
          Cost = 0m,
          BaseSalaryForPeriod = 0m,
        };

        var employeePayrollId = await _repo.SaveEmployeePayroll(employeePayroll);
        employeePayroll.Id = employeePayrollId;

        ctx.EmployeePayrollByEmployeeId[employee.Id] = employeePayroll;

        i++;
      }

      // Payroll Details for each employee payroll
      var calculatedLines = _template.RunCalculation(companyId, dateFrom, dateTo, ctx);
      if (calculatedLines != null && calculatedLines.Count > 0)
      {
        // Group lines by EmployeePayrollId
        var detailsByPayroll = new Dictionary<int, List<PayrollDetailModel>>();
        var lineIndex = 0;

        while (lineIndex < calculatedLines.Count)
        {
          var line = calculatedLines[lineIndex];

          if (line != null && line.EmployeePayrollId > 0)
          {
            List<PayrollDetailModel> employeeDetails;
            if (!detailsByPayroll.TryGetValue(line.EmployeePayrollId, out employeeDetails))
            {
              employeeDetails = new List<PayrollDetailModel>();
              detailsByPayroll[line.EmployeePayrollId] = employeeDetails;
            }
            employeeDetails.Add(line);
          }

          lineIndex++;
        }

        // Save details per employee payroll to database
        foreach (var group in detailsByPayroll)
        {
          var employeePayrollId = group.Key;
          var employeeDetails = group.Value;
          await _repo.SavePayrollDetails(employeePayrollId, employeeDetails);
        }
      }

      var totalsByPayroll = new Dictionary<int, (decimal Gross, decimal EmpDed, decimal EmprDed, decimal Benefits, decimal BaseSalary)>();

      foreach (var line in calculatedLines)
      {
        if (line is null || line.EmployeePayrollId <= 0) continue;

        if (!totalsByPayroll.TryGetValue(line.EmployeePayrollId, out var t))
          t = (0m, 0m, 0m, 0m, 0m);

        switch (line.Type)
        {
          case PayrollItemType.Base:
            t.Gross += line.Amount;
            break;

          case PayrollItemType.Tax:
            t.EmpDed += line.Amount;
            break;

          case PayrollItemType.EmployeeDeduction:
            t.EmpDed += line.Amount;
            break;

          case PayrollItemType.EmployerContribution:
            t.EmprDed += line.Amount;
            break;

          case PayrollItemType.Benefit:
            t.Benefits += line.Amount;
            break;

          default:
            break;
        }

        totalsByPayroll[line.EmployeePayrollId] = t;
      }

      decimal companyGross = 0m, companyEmpDed = 0m, companyEmprDed = 0m, companyBenefits = 0m, companyNet = 0m, companyCost = 0m;

      foreach (var kvp in totalsByPayroll)
      {
        var employeePayrollId = kvp.Key;
        var t = kvp.Value;

        var net = t.Gross - t.EmpDed;
        var cost = t.Gross + t.EmprDed + t.Benefits;

        var empModel = ctx.EmployeePayrollByEmployeeId.Values.FirstOrDefault(e => e.Id == employeePayrollId);
        if (empModel is null) continue;

        empModel.Gross = t.Gross;
        empModel.EmployeeDeductions = t.EmpDed;
        empModel.EmployerDeductions = t.EmprDed;
        empModel.Benefits = t.Benefits;
        empModel.Net = net;
        empModel.Cost = cost;
        empModel.BaseSalaryForPeriod = t.BaseSalary;

        await _repo.UpdateEmployeePayrollTotals(employeePayrollId, empModel);

        companyGross += empModel.Gross;
        companyEmpDed += empModel.EmployeeDeductions;
        companyEmprDed += empModel.EmployerDeductions;
        companyBenefits += empModel.Benefits;
        companyNet += empModel.Net;
        companyCost += empModel.Cost;
      }

      companyPayroll.Gross = companyGross;
      companyPayroll.EmployeeDeductions = companyEmpDed;
      companyPayroll.EmployerDeductions = companyEmprDed;
      companyPayroll.Benefits = companyBenefits;
      companyPayroll.Net = companyNet;
      companyPayroll.Cost = companyCost;

      await _repo.UpdateCompanyPayrollTotals(companyPayrollId, companyPayroll);

      // Payroll summary to return
      return new PayrollSummary
      {
        CompanyPayrollId = companyPayroll.Id,
        TotalGrossSalaries = companyPayroll.Gross,
        TotalEmployerDeductions = companyPayroll.EmployerDeductions,
        TotalEmployeeDeductions = companyPayroll.EmployeeDeductions,
        TotalBenefits = companyPayroll.Benefits,
        TotalNetEmployee = companyPayroll.Net,
        TotalEmployerCost = companyPayroll.Cost,
        DateFrom = companyPayroll.DateFrom,
        DateTo = companyPayroll.DateTo,
      };
    }

    private async Task<IDictionary<int, IList<ElementModel>>> BuildElementsMapAsync(int companyId, IList<EmployeeModel> employees, DateTime dateFrom, DateTime dateTo)
    {
      var tasks = employees.Select(async e =>
      {
        var list = await _repo.GetElementsForEmployee(companyId, e.Id, dateFrom, dateTo);
        return (EmployeeId: e.Id, Elements: (IList<ElementModel>)list.ToList());
      });

      var results = await Task.WhenAll(tasks);

      var dict = new Dictionary<int, IList<ElementModel>>();
      foreach (var r in results)
        dict[r.EmployeeId] = r.Elements;

      return dict;
    }
  }
}
