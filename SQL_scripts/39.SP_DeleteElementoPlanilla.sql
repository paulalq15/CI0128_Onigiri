CREATE OR ALTER PROCEDURE sp_DeleteElementoPlanilla
  @elementId INT
AS
BEGIN
    SET NOCOUNT ON;

    SET XACT_ABORT ON;

    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS (
            SELECT 1
            FROM DetalleNomina d
            JOIN ElementoAplicado ea ON ea.IdElementoAplicado = d.IdElementoAplicado
            WHERE ea.IdElemento = @elementId
        )
        BEGIN
            -- SOFT DELETE
            DECLARE @FechaFin DATE = EOMONTH(GETDATE());

            UPDATE ElementoAplicado
            SET FechaFin = @FechaFin
            WHERE IdElemento = @elementId
              AND FechaFin IS NULL;

            UPDATE ElementoPlanilla
            SET IsDeleted = 1
            WHERE IdElemento = @elementId;
        END
        ELSE
        BEGIN
            -- HARD DELETE
            DELETE FROM ElementoAplicado
            WHERE IdElemento = @elementId;

            DELETE FROM ElementoPlanilla
            WHERE IdElemento = @elementId;
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
END;
GO
