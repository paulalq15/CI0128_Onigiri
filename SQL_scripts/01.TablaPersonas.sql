CREATE TABLE Persona (
    IdPersona INT IDENTITY(1,1) PRIMARY KEY,
    Cedula CHAR(11) NOT NULL,
    Nombre1 VARCHAR(40) NOT NULL,
    Nombre2 VARCHAR(40),
    Apellido1 VARCHAR(40) NOT NULL,
    Apellido2 VARCHAR(40) NOT NULL,
    Telefono CHAR(9) NOT NULL,
    FechaNacimiento DATE NOT NULL,
    TipoPersona VARCHAR(13) NOT NULL 
        CHECK (TipoPersona IN ('Administrador', 'Empleador', 'Empleado', 'Aprobador'))
);
