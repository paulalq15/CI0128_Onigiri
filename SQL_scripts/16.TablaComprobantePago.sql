CREATE TABLE ComprobantePago(
	IdPago INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	Referencia VARCHAR(50) NOT NULL,
	FechaPago DATETIME NOT NULL DEFAULT GETDATE(),
	Monto DECIMAL(18,2) NOT NULL,
	IdNominaEmpleado INT NOT NULL,
	IdCreadoPor INT NOT NULL,
	CONSTRAINT FK_NominaEmpleado_ComprobantePago FOREIGN KEY (IdNominaEmpleado) REFERENCES NominaEmpleado(IdNominaEmpleado),
	CONSTRAINT FK_Creador_ComprobantePago FOREIGN KEY (IdCreadoPor) REFERENCES Persona(IdPersona)
);