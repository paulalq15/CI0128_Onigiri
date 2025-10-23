CREATE OR ALTER PROCEDURE ObtenerHorasXSemana
  @IdEmpleado int,
  @InicioSemana date,
  @FinSemana date
AS
BEGIN
  SET NOCOUNT ON;

  SELECT 
      Fecha AS [Date],
      Horas AS [Hours],
      Descripcion AS [Description]
  FROM dbo.HojaHoras
  WHERE IdEmpleado = @IdEmpleado
    AND Fecha BETWEEN @InicioSemana AND @FinSemana
  ORDER BY Fecha;
END
