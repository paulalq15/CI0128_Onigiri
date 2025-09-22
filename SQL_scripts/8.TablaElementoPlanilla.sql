CREATE TABLE ElementoPlanilla(
IdElemento INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
Nombre CHAR(40) NOT NULL,
PagadoPor VARCHAR(8) NOT NULL,
Tipo VARCHAR(10) NOT NULL,
Valor DECIMAL(10,2) NOT NULL,
Estado VARCHAR(8) NOT NULL DEFAULT 'Activo',
CONSTRAINT CK_ElementoPagadoPor CHECK (PagadoPor IN ('Empleado','Empleador')),
CONSTRAINT CK_TipoCalculo CHECK (Tipo IN ('Monto','Porcentaje','API')),
CONSTRAINT CK_EstadoElemento CHECK (Estado IN ('Activo','Inactivo'))
)
GO