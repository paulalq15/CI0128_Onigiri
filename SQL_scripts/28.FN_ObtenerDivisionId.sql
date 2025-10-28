CREATE OR ALTER FUNCTION fn_GetDivisionIdByPostal(@CodigoPostal CHAR(5))
RETURNS INT
AS
BEGIN
    DECLARE @IdDivision INT;

    SELECT @IdDivision = t.IdDivision
    FROM DivisionTerritorialCR AS t
    WHERE t.CodigoPostal = @CodigoPostal

    RETURN @IdDivision;
END;

