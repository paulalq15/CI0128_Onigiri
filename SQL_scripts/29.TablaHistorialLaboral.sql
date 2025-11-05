CREATE TABLE HistorialLaboral (
	IdHistoriaLaboral INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	FechaModificacion DATE NOT NULL,
	AtributoAfectado VARCHAR(12) NOT NULL,
	ValorAnterior VARCHAR(50) NOT NULL,
	ValorNuevo VARCHAR(50) NOT NULL,
	IdModificadoPor INT NOT NULL FOREIGN KEY (IdModificadoPor) REFERENCES Persona(IdPersona),
	IdEmpleado INT NOT NULL FOREIGN KEY (IdEmpleado) REFERENCES Persona(IdPersona),
	IdContrato INT NOT NULL FOREIGN KEY (IdContrato) REFERENCES Contrato(IdContrato)
);
