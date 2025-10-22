Create Or Alter Procedure GetMaxAmountBenefitsTakenInCompany(
	@companyId int)
As
Begin
	Select Top 1
		COUNT(ep.IdUsuario)
	From UsuariosPorEmpresa upe
	Inner Join Usuario u On u.IdUsuario = upe.IdUsuario
	Inner Join Persona p On p.IdPersona = u.IdPersona
	Inner Join ElementoAplicado ep On ep.IdUsuario = upe.IdUsuario
	Where upe.IdEmpresa = @companyId And p.TipoPersona = 'Empleado'
	Group by upe.IdUsuario
	Order By COUNT(ep.IdUsuario) Desc;
End

-- Ejemplo de ejecuci�n
Exec GetMaxAmountBenefitsTakenInCompany @companyId = 2;