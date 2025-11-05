CREATE TABLE NominaEmpleado(
	IdNominaEmpleado INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	IdNominaEmpresa INT NOT NULL,
	IdEmpleado INT NOT NULL,
	MontoBruto DECIMAL(18,2) NOT NULL DEFAULT 0,
	MontoNeto DECIMAL(18,2) NOT NULL DEFAULT 0,
	DeduccionesEmpleado DECIMAL(18,2) NOT NULL DEFAULT 0,
	DeduccionesEmpleador DECIMAL(18,2) NOT NULL DEFAULT 0,
	Beneficios DECIMAL(18,2) NOT NULL DEFAULT 0,
	CONSTRAINT FK_NominaEmpresa_NominaEmpleado FOREIGN KEY (IdNominaEmpresa) REFERENCES NominaEmpresa(IdNominaEmpresa),
	CONSTRAINT FK_Empleado_NominaEmpleado FOREIGN KEY (IdEmpleado) REFERENCES Persona(IdPersona)
);