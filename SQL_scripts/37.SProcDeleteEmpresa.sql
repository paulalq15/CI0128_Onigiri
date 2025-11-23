Create Or Alter Procedure Sproc_delete_empresa(@IdEmpresa Int)
As
Begin
	Declare @RowsAffected int = 0;

	If Not Exists (
		Select 1
		From Empresa
		Where IdEmpresa = @IdEmpresa
	)
	Begin
		RAISERROR('No se encontró la empresa con IdEmpresa = %d', 16, 1, @IdEmpresa);
		Return;
	End;

	Begin Try
		Begin Transaction;

		If Exists (
			Select 1
			From NominaEmpresa
			Where IdEmpresa = @IdEmpresa
		)
		Begin
			-- Soft delete --

			-- Inactivar empresa
			Update Empresa
			Set Estado = 'Inactivo', isDelete = 1
			Where IdEmpresa = @IdEmpresa;

			Set @RowsAffected = @RowsAffected + @@ROWCOUNT;

			-- Inactivar usuarios
			Update u
			Set u.Estado = 'Inactivo'
			From Usuario u
			Inner Join UsuariosPorEmpresa upm On upm.IdUsuario = u.IdUsuario
			Where upm.IdEmpresa = @IdEmpresa
				And u.IdUsuario <> (Select IdCreadoPor From Empresa Where IdEmpresa = @IdEmpresa);

			Set @RowsAffected = @RowsAffected + @@ROWCOUNT;

			Commit Transaction;
			Return @RowsAffected;
		End

		-- Hard delete --

		-- Guardar Personas relacionadas a la empresa
        Declare @Personas Table (IdPersona Int Primary Key);

        Insert Into @Personas
        Select p.IdPersona
        From UsuariosPorEmpresa upm
        Inner Join Usuario u On u.IdUsuario = upm.IdUsuario
        Inner Join Persona p On p.IdPersona = u.IdPersona
        Where upm.IdEmpresa = @IdEmpresa
			And u.IdUsuario <> (Select IdCreadoPor From Empresa Where IdEmpresa = @IdEmpresa);

		-- Borrar en orden

		-- ElementosAplicados
		Delete ea
		From ElementoAplicado ea
		Inner Join Usuario u On u.IdUsuario = ea.IdUsuario
		WHERE u.IdPersona IN (SELECT IdPersona FROM @Personas);

		Set @RowsAffected = @RowsAffected + @@ROWCOUNT;

		-- ElementoPlanilla
		Delete From ElementoPlanilla
		Where IdEmpresa = @IdEmpresa;

		Set @RowsAffected = @RowsAffected + @@ROWCOUNT;

		-- HistorialLaboral
		Delete From HistorialLaboral
		Where IdEmpleado In (Select IdPersona From @Personas);

		Set @RowsAffected = @RowsAffected + @@ROWCOUNT;

		-- HojaHoras
		Delete From HojaHoras
		Where IdEmpleado In (Select IdPersona From @Personas);

		Set @RowsAffected = @RowsAffected + @@ROWCOUNT;

		-- Contrato
		Delete From Contrato
		Where IdPersona In (Select IdPersona From @Personas);

		-- Usuarios
		Delete From Usuario
		Where IdPersona In (Select IdPersona From @Personas);

		Set @RowsAffected = @RowsAffected + @@ROWCOUNT;

		-- Direccion persona
		Delete From Direccion
		Where IdPersona In (Select IdPersona From @Personas);

		-- Direccion empresa
		Delete From Direccion
		Where IdEmpresa = @IdEmpresa;

		Set @RowsAffected = @RowsAffected + @@ROWCOUNT;

		-- UsuariosPorEmpresa
		Delete From UsuariosPorEmpresa
		Where IdEmpresa = @IdEmpresa;

		Set @RowsAffected = @RowsAffected + @@ROWCOUNT;

		-- Personas
		Delete From Persona
		Where IdPersona In (Select IdPersona From @Personas);

		Set @RowsAffected = @RowsAffected + @@ROWCOUNT;

		-- Empresa
		Delete From Empresa
		Where IdEmpresa = @IdEmpresa;

		Set @RowsAffected = @RowsAffected + @@ROWCOUNT;

		Commit Transaction;
		Return @RowsAffected;
	End Try
	Begin Catch
		Rollback Transaction;
		
		Declare @ErrMsg Nvarchar(4000) = ERROR_MESSAGE();
        Declare @ErrLine int = ERROR_LINE();

		RAISERROR('Error en  Sproc_delete_empresa: %s (Linea %d)', 16, 1, @ErrMsg, @ErrLine);
	End Catch
End;