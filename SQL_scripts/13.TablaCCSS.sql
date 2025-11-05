CREATE TABLE CCSS(
	IdCCSS INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	FechaInicio DATE NOT NULL,
	FechaFin DATE,
	Porcentaje Decimal(5,2) NOT NULL,
	Categoria VARCHAR(38) NOT NULL CHECK (Categoria IN ('CCSS', 'Otras instituciones', 'Ley de Protección al Trabajador (LPT)')),
	Concepto VARCHAR(45) NOT NULL,
	PagadoPor VARCHAR(20) NOT NULL CHECK (PagadoPor IN ('Empleado', 'Empleador'))
);