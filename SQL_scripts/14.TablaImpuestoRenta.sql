CREATE TABLE ImpuestoRenta(
	IdImpuestoRenta INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	FechaInicio DATE NOT NULL,
	FechaFin DATE NOT NULL,
	Porcentaje Decimal(5,2) NOT NULL,
	MontoMinimo INT NOT NULL,
	MontoMaximo INT
);