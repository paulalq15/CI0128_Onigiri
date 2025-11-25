CREATE OR ALTER PROCEDURE sp_GetPayrollReport
  @companyId INT,
  @start DATE,
  @end DATE,
  @nationalId VARCHAR(11) = NULL,
  @employeeType VARCHAR(25) = NULL 
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.Nombre1 + ' ' + p.Apellido1 + ' ' + p.Apellido2 AS EmployeeName,
        p.Cedula AS NationalId,
        c.Tipo AS EmployeeType,
        'Del ' + FORMAT(n.FechaInicio, 'dd/MM/yyyy') + ' al ' + FORMAT(n.FechaFin, 'dd/MM/yyyy') AS PaymentPeriod,
        n.FechaCreacion AS PaymentDate,
        ne.MontoBruto AS GrossSalary,
        ne.DeduccionesEmpleador AS EmployerContributions,
        ne.Beneficios AS EmployeeBenefits,
        ne.Costo AS EmployerCost
    FROM NominaEmpleado ne
    JOIN Persona       p ON ne.IdEmpleado      = p.IdPersona
    JOIN Contrato      c ON p.IdPersona        = c.IdPersona
    JOIN NominaEmpresa n ON ne.IdNominaEmpresa = n.IdNominaEmpresa
    WHERE
        n.FechaInicio BETWEEN @start AND @end
        AND n.IdEmpresa = @companyId
        AND (@nationalId IS NULL OR p.Cedula = @nationalId)
        AND (@employeeType IS NULL OR c.Tipo = @employeeType)
    ORDER BY PaymentDate;
END;
GO