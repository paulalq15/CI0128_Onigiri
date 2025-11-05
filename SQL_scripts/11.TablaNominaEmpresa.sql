CREATE TABLE NominaEmpresa(
	IdNominaEmpresa INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	FechaInicio DATE NOT NULL,
	FechaFin DATE NOT NULL,
	FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
	MontoBruto DECIMAL(18,2) NOT NULL DEFAULT 0,
	MontoNeto DECIMAL(18,2) NOT NULL DEFAULT 0,
	DeduccionesEmpleado DECIMAL(18,2) NOT NULL DEFAULT 0,
	DeduccionesEmpleador DECIMAL(18,2) NOT NULL DEFAULT 0,
	Beneficios DECIMAL(18,2) NOT NULL DEFAULT 0,
	Estado VARCHAR(10) NOT NULL CHECK (Estado IN ('Creado', 'Pagado')) DEFAULT 'Creado',
	CreadoPor INT NOT NULL,
	IdEmpresa INT NOT NULL,
	CONSTRAINT FK_Creador_NominaEmpresa FOREIGN KEY (CreadoPor) REFERENCES Persona(IdPersona),
	CONSTRAINT FK_Empresa_NominaEmpresa FOREIGN KEY (IdEmpresa) REFERENCES Empresa(IdEmpresa)
);