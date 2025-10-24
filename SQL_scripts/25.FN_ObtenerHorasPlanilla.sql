CREATE FUNCTION dbo.fn_GetTotalWorkingDays(@dateFrom DATE, @dateTo DATE)
RETURNS INT
AS
     BEGIN
         DECLARE @businessDays INT= 0;
         WHILE @dateFrom <= @dateTo
             BEGIN
                 IF DATENAME(WEEKDAY, @dateFrom) IN('Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday')
                     BEGIN
                         SET @businessDays = @businessDays + 1;
                 END;
                 SET @dateFrom = DATEADD(DAY, 1, @dateFrom);
             END;
         RETURN @businessDays;
     END;
GO

CREATE OR ALTER FUNCTION dbo.fn_GetPayrollTimesheets(@companyId INT, @dateFrom DATE, @dateTo DATE)
RETURNS @Result TABLE(EmployeeId INT, TotalHours DECIMAL(10,2))
AS
BEGIN
    DECLARE @businessDays INT = dbo.fn_GetTotalWorkingDays(@dateFrom, @dateTo);

    INSERT INTO @Result(EmployeeId, TotalHours)
    SELECT h.IdEmpleado AS EmployeeId, SUM(CAST(h.Horas AS decimal(10,2))) AS TotalHours
    FROM HojaHoras AS h
        JOIN Persona AS p ON p.IdPersona = h.IdEmpleado
        JOIN Usuario AS u ON u.IdPersona = p.IdPersona
        JOIN UsuariosPorEmpresa AS ue ON ue.IdUsuario = u.IdUsuario
        JOIN Empresa AS e ON ue.IdEmpresa = e.IdEmpresa
        JOIN Contrato AS c ON c.IdPersona = h.IdEmpleado
    WHERE e.IdEmpresa = @companyId AND h.Fecha >= @dateFrom AND h.Fecha <= @dateTo
    GROUP BY h.IdEmpleado, c.Tipo
    HAVING SUM(CAST(h.Horas AS decimal(10,2))) >=
        CASE
            WHEN c.Tipo IN ('Tiempo Completo', 'Servicios Profesionales') THEN @businessDays * 9
            WHEN c.Tipo IN ('Medio Tiempo') THEN @businessDays * 4
        END;

    RETURN
END
GO

--SELECT *
--FROM dbo.fn_GetPayrollTimesheets(13, '2025-10-01', '2025-10-15')