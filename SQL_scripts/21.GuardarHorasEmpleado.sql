ALTER TABLE dbo.HojaHoras
ADD CONSTRAINT UX_HojaHoras_Empleado_Fecha UNIQUE (IdEmpleado, Fecha);

CREATE TYPE HorasXSemana AS TABLE(
  Fecha date NOT NULL,
  Horas tinyint NOT NULL,
  Descripcion varchar(200) NULL
);

CREATE OR ALTER PROCEDURE InsertarHorasXSemana
  @IdEmpleado int,
  @Items HorasXSemana READONLY
AS
BEGIN
  SET NOCOUNT ON;

  INSERT INTO dbo.HojaHoras (Fecha, Horas, Descripcion, IdEmpleado)
  SELECT i.Fecha,
         i.Horas,
         i.Descripcion,
         @IdEmpleado
  FROM @Items AS i
  WHERE NOT EXISTS (
    SELECT 1
    FROM HojaHoras h
    WHERE h.IdEmpleado = @IdEmpleado
      AND h.Fecha = i.Fecha
  );
END


