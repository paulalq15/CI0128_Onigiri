CREATE OR ALTER TRIGGER tr_Contrato_Historial
ON Contrato
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO HistorialLaboral
        (FechaModificacion, AtributoAfectado, ValorAnterior, ValorNuevo, IdModificadoPor, IdEmpleado, IdContrato)
    SELECT
        SYSDATETIME() AS FechaModificacion,
        ch.Atributo AS AtributoAfectado,
        ch.Viejo AS ValorAnterior,
        ch.Nuevo AS ValorNuevo,
        COALESCE(TRY_CONVERT(INT, SESSION_CONTEXT(N'IdModificador')), i.IdPersona) AS IdModificadoPor,
        i.IdPersona AS IdEmpleado,
        i.IdContrato
    FROM inserted i
    JOIN deleted  d ON d.IdContrato = i.IdContrato
    CROSS APPLY (
        VALUES
            ('Puesto',
                CAST(d.Puesto AS VARCHAR(50)),
                CAST(i.Puesto AS VARCHAR(50))),

            ('Departamento',
                CAST(d.Departamento AS VARCHAR(50)),
                CAST(i.Departamento AS VARCHAR(50))),

            ('Salario',
                CONVERT(VARCHAR(50), d.Salario),
                CONVERT(VARCHAR(50), i.Salario)),

            ('Tipo',
                CAST(d.Tipo AS VARCHAR(50)),
                CAST(i.Tipo AS VARCHAR(50))),

            ('FechaInicio',
                CONVERT(VARCHAR(50), d.FechaInicio, 23),  -- YYYY-MM-DD
                CONVERT(VARCHAR(50), i.FechaInicio, 23)),

            ('FechaFin',
                CONVERT(VARCHAR(50), d.FechaFin, 23),
                CONVERT(VARCHAR(50), i.FechaFin, 23)),

            ('CuentaPago',
                CAST(d.CuentaPago AS VARCHAR(50)),
                CAST(i.CuentaPago AS VARCHAR(50)))
    ) ch(Atributo, Viejo, Nuevo)
    WHERE
        (ch.Viejo IS NULL AND ch.Nuevo IS NOT NULL)
        OR (ch.Viejo IS NOT NULL AND ch.Nuevo IS NULL)
        OR (ch.Viejo <> ch.Nuevo);
END

