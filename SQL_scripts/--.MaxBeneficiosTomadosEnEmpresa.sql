Use [Onigiri_PI]
Go

Create Procedure GetMaxAmountBenefitsTakenInCompany(
	@companyId int,
	@MaxAmount int Output)
As
Begin
	Select Top 1
		@MaxAmount = COUNT(ep.IdUsuario)
	From UsuariosPorEmpresa upe
	Inner Join Usuario u On u.IdUsuario = upe.IdUsuario
	Inner Join Persona p On p.IdPersona = u.IdPersona
	Inner Join ElementoAplicado ep On ep.IdUsuario = upe.IdUsuario
	Where upe.IdEmpresa = @companyId And p.TipoPersona = 'Empleado'
	Group by upe.IdUsuario
	Order By COUNT(ep.IdUsuario) Desc;
End

-- Ejemplo de ejecución
Declare @result Int;
Exec GetMaxAmountBenefitsTakenInCompany @companyId = 2, @MaxAmount = @result Output;

Select @result As MaxBenefitsCount