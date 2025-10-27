CREATE TABLE HojaHoras(
	IdHoras INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Fecha DATE NOT NULL,
	Horas TINYINT NOT NULL,
	Descripcion VARCHAR(200) NULL,
	Estado VARCHAR(10) NOT NULL CHECK (Estado IN ('Enviado', 'Aprobado', 'Rechazado')) DEFAULT 'Enviado',
	IdEmpleado INT NOT NULL FOREIGN KEY (IdEmpleado) REFERENCES Persona(IdPersona),
	IdAprovador INT NULL FOREIGN KEY (IdAprovador) REFERENCES Persona(IdPersona)
);