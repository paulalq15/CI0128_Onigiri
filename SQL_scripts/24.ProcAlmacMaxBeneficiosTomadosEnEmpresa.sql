Create Or Alter Function GetMaxAmountBenefitsTakenInCompany(@companyId int)
Returns int
As
Begin
	Declare @maxBenefits int;

	Select Top 1
		@maxBenefits = COUNT(ep.IdUsuario)
	From UsuariosPorEmpresa upe
	Inner Join Usuario u On u.IdUsuario = upe.IdUsuario
	Inner Join Persona p On p.IdPersona = u.IdPersona
	Inner Join ElementoAplicado ep On ep.IdUsuario = upe.IdUsuario
	Inner Join ElementoPlanilla epl On epl.IdElemento = ep.IdElemento
	Where upe.IdEmpresa = @companyId 
	  And p.TipoPersona = 'Empleado'
	  And epl.PagadoPor = 'Empleador'
	Group by upe.IdUsuario
	Order By COUNT(ep.IdUsuario) Desc;

	Return IsNull(@maxBenefits, 0);
End;
GO

-- Ejemplo de uso:
Select dbo.GetMaxAmountBenefitsTakenInCompany(2) As MaxBenefitsTaken;