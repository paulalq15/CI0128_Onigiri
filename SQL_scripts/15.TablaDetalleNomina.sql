CREATE TABLE DetalleNomina(
	IdDetalleNomina INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	IdNominaEmpleado INT NOT NULL,
	IdCCSS INT,
	IdImpuestoRenta INT,
	IdElementoAplicado INT,
	Descripcion VARCHAR(255) NOT NULL,
	Monto DECIMAL(18,2) NOT NULL DEFAULT 0,
	Tipo VARCHAR(10) NOT NULL CHECK (Tipo IN ('Salario', 'Deduccion Empleado', 'Deduccion Empleador', 'Beneficio Empleado')) ,
	CONSTRAINT FK_NominaEmpleado_DetalleNomina FOREIGN KEY (IdNominaEmpleado) REFERENCES NominaEmpleado(IdNominaEmpleado),
	CONSTRAINT FK_CCSS_DetalleNomina FOREIGN KEY (IdCCSS) REFERENCES CCSS(IdCCSS),
	CONSTRAINT FK_ImpuestoRenta_DetalleNomina FOREIGN KEY (IdImpuestoRenta) REFERENCES ImpuestoRenta(IdImpuestoRenta),
	CONSTRAINT FK_ElementoAplicado_DetalleNomina FOREIGN KEY (IdElementoAplicado) REFERENCES ElementoAplicado(IdElementoAplicado)
);