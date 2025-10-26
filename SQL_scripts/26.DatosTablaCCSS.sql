Insert Into CCSS(FechaInicio, Porcentaje, Categoria, Concepto, PagadoPor)
Values
	(CAST(GETDATE() AS DATE), 9.25, 'CCSS', 'SEM', 'Empleador'),
	(CAST(GETDATE() AS DATE), 5.50, 'CCSS', 'SEM', 'Empleado'),
	(CAST(GETDATE() AS DATE), 5.42, 'CCSS', 'IVM', 'Empleador'),
	(CAST(GETDATE() AS DATE), 4.17, 'CCSS', 'IVM', 'Empleado'),
	(CAST(GETDATE() AS DATE), 0.25, 'Otras instituciones', 'Banco Popular (BPOP)', 'Empleador'),
	(CAST(GETDATE() AS DATE), 5.00, 'Otras instituciones', 'Asignaciones familiares', 'Empleador'),
	(CAST(GETDATE() AS DATE), 0.50, 'Otras instituciones', 'IMAS', 'Empleador'),
	(CAST(GETDATE() AS DATE), 1.50, 'Otras instituciones', 'INA', 'Empleador'),
	(CAST(GETDATE() AS DATE), 0.25, 'Ley de Protección al Trabajador (LPT)', 'Banco Popular (BPOP)', 'Empleador'),
	(CAST(GETDATE() AS DATE), 1.00, 'Ley de Protección al Trabajador (LPT)', 'Banco Popular (BPOP)', 'Empleado'),
	(CAST(GETDATE() AS DATE), 1.50, 'Ley de Protección al Trabajador (LPT)', 'Fondo de Capitalización Laboral (FCL)', 'Empleador'),
	(CAST(GETDATE() AS DATE), 2.00, 'Ley de Protección al Trabajador (LPT)', 'Operadora de pensiones complementaria (OPC)', 'Empleador'),
	(CAST(GETDATE() AS DATE), 1.00, 'Ley de Protección al Trabajador (LPT)', 'INS', 'Empleador');
