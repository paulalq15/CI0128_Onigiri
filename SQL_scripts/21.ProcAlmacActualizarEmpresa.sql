Create Or Alter Procedure UpdateCompanyData(
    @CompanyUniqueId Int,
    @Name Nvarchar(150),
    @Telephone Nvarchar(9),
    @MaxBenefits Int,
    @ZipCode Nvarchar(5),
    @OtherSigns Nvarchar(250))
As
Begin
    Update Empresa
    Set
        Nombre = @Name,
        Telefono = @Telephone,
        CantidadBeneficios = @MaxBenefits
    Where IdEmpresa = @CompanyUniqueId;

    Declare @newIdDivision Int;

    Select @newIdDivision = IdDivision
    From DivisionTerritorialCR
    Where CodigoPostal = @ZipCode;

    Update Direccion
    Set
        idDivision = @newIdDivision,
        OtrasSenas = @OtherSigns
    Where IdEmpresa = @CompanyUniqueId;
End;
