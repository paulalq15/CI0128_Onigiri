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

      if (companyId <= 0) throw new ArgumentException("companyId must be positive");
      if (personId <= 0) throw new ArgumentException("personId must be positive");
      if (dateFrom > dateTo) throw new ArgumentException("dateFrom must be <= dateTo");

      // Kick off all repository calls in parallel
      var companyTask = _repo.GetCompany(companyId);
      var employeesTask = _repo.GetEmployees(companyId, dateFrom, dateTo);
      var contractsTask = _repo.GetContracts(companyId, dateFrom, dateTo);
      var taxesTask = _repo.GetTaxes(dateFrom, dateTo);
      var ccssTask = _repo.GetCCSS(dateFrom, dateTo);
      var hoursTask = _repo.GetEmployeeTimesheets(companyId, dateFrom, dateTo);

      await Task.WhenAll(companyTask, employeesTask, contractsTask, taxesTask, ccssTask, hoursTask);

      var company = await companyTask;
      var employees = (await employeesTask).ToList();
      var contracts = (await contractsTask).ToList();
      var taxes = (await taxesTask).ToList();
      var ccss = (await ccssTask).ToList();
      var hoursByEmployee = await hoursTask;

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
      var companyPayroll = new CompanyPayrollModel();
      companyPayroll.CompanyId = company.Id;
      companyPayroll.DateFrom = dateFrom;
      companyPayroll.DateTo = dateTo;
      companyPayroll.PayrollStatus = "Creado";
      companyPayroll.Gross = 0m;
      companyPayroll.EmployeeDeductions = 0m;
      companyPayroll.EmployerDeductions = 0m;
      companyPayroll.Benefits = 0m;
      companyPayroll.Net = 0m;
      companyPayroll.Cost = 0m;
      companyPayroll.CreatedBy = personId;

      var companyPayrollId = await _repo.SaveCompanyPayroll(companyPayroll);
      companyPayroll.Id = companyPayrollId;

      // Employee payroll with zero totals for each employee
      ctx.EmployeePayrollByEmployeeId = new Dictionary<int, EmployeePayrollModel>();

      var i = 0;
      while (i < employees.Count)
      {
        var employee = employees[i];

        var employeePayroll = new EmployeePayrollModel();
        employeePayroll.CompanyPayrollId = companyPayrollId;
        employeePayroll.EmployeeId = employee.Id;
        employeePayroll.Gross = 0m;
        employeePayroll.EmployeeDeductions = 0m;
        employeePayroll.EmployerDeductions = 0m;
        employeePayroll.Benefits = 0m;
        employeePayroll.Net = 0m;
        employeePayroll.Cost = 0m;

        var employeePayrollId = await _repo.SaveEmployeePayroll(employeePayroll);
        employeePayroll.Id = employeePayrollId;

        ctx.EmployeePayrollByEmployeeId[employee.Id] = employeePayroll;

        i++;
      }

      // Payroll Details for each employee payroll
      var calculatedLines = _template.RunCalculation(companyId, dateFrom, dateTo, ctx);
      if (calculatedLines != null && calculatedLines.Count > 0)
      {
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

        foreach (var group in detailsByPayroll)
        {
          var employeePayrollId = group.Key;
          var employeeDetails = group.Value;
          await _repo.SavePayrollDetails(employeePayrollId, employeeDetails);
        }
      }

      // TODO: update totals for each employee payroll and company payroll

      // Employee Totals using dictionary ctx.EmployeePayrollByEmployeeId
      //...

      // Company Totals using companyPayroll
      //...

      // Payroll summary to return
      var summary = new PayrollSummary();
      summary.CompanyPayrollId = companyPayroll.Id;
      summary.TotalGrossSalaries = companyPayroll.Gross;
      summary.TotalEmployerDeductions = companyPayroll.EmployerDeductions;
      summary.TotalEmployeeDeductions = companyPayroll.EmployeeDeductions;
      summary.TotalBenefits = companyPayroll.Benefits;
      summary.TotalNetEmployee = companyPayroll.Net;
      summary.TotalEmployerCost = companyPayroll.Cost;
      summary.DateFrom = companyPayroll.DateFrom;
      summary.DateTo = companyPayroll.DateTo;

      return summary;
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
