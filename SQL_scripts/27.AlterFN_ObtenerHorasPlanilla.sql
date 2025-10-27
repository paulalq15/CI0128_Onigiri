CREATE OR ALTER FUNCTION dbo.fn_GetPayrollTimesheets(@companyId INT, @dateFrom DATE, @dateTo DATE)
RETURNS @Result TABLE(EmployeeId INT, TotalHours DECIMAL(10,2))
AS
BEGIN
	INSERT INTO @Result(EmployeeId, TotalHours)
	SELECT
		h.IdEmpleado AS EmployeeId,
		SUM(CAST(h.Horas AS DECIMAL(10,2))) AS TotalHours
	FROM HojaHoras AS h
		JOIN Persona AS p ON p.IdPersona = h.IdEmpleado
		JOIN Usuario AS u ON u.IdPersona = p.IdPersona
		JOIN UsuariosPorEmpresa AS ue ON ue.IdUsuario = u.IdUsuario
		JOIN Empresa AS e ON e.IdEmpresa = ue.IdEmpresa
		JOIN Contrato AS c ON c.IdPersona = h.IdEmpleado
	CROSS APPLY (
		SELECT
		OverlapStart = CASE WHEN c.FechaInicio > @dateFrom THEN c.FechaInicio ELSE @dateFrom END,
		OverlapEnd = CASE WHEN ISNULL(c.FechaFin, '9999-12-31') < @dateTo THEN c.FechaFin ELSE @dateTo END
		) AS payrollContract
	WHERE
		e.IdEmpresa = @companyId
		AND payrollContract.OverlapEnd >= payrollContract.OverlapStart
		AND h.Fecha >= payrollContract.OverlapStart
		AND h.Fecha <= payrollContract.OverlapEnd
	GROUP BY
		h.IdEmpleado, c.Tipo, payrollContract.OverlapStart, payrollContract.OverlapEnd
	HAVING SUM(CAST(h.Horas AS DECIMAL(10,2))) >=
		CASE
			WHEN c.Tipo IN ('Tiempo Completo', 'Servicios Profesionales') THEN dbo.fn_GetTotalWorkingDays(payrollContract.OverlapStart, payrollContract.OverlapEnd) * 9
			WHEN c.Tipo IN ('Medio Tiempo') THEN dbo.fn_GetTotalWorkingDays(payrollContract.OverlapStart, payrollContract.OverlapEnd) * 4
			ELSE 0
		END;

	RETURN;
END;
GO