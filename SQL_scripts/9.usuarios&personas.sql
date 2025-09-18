CREATE TABLE Persona (
    IdPersona INT IDENTITY(1,1) PRIMARY KEY,
    Cedula CHAR(11) NOT NULL,
    Nombre1 VARCHAR(40) NOT NULL,
    Nombre2 VARCHAR(40),
    Apellido1 VARCHAR(40) NOT NULL,
    Apellido2 VARCHAR(40),
    Telefono CHAR(8),
    FechaNacimiento DATE NOT NULL,
    TipoPersona VARCHAR(13) NOT NULL 
        CHECK (TipoPersona IN ('Administrador', 'Empleador', 'Empleado', 'Aprobador'))
);

CREATE TABLE Usuario (
    IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
    Correo VARCHAR(40) NOT NULL,
    Contrasena VARCHAR(16) NOT NULL,
    Estado VARCHAR(8) NOT NULL 
        CHECK (Estado IN ('Activo', 'Inactivo')),
    IdPersona INT NOT NULL,
    CONSTRAINT FK_Usuario_Persona FOREIGN KEY (IdPersona) REFERENCES Persona(IdPersona)
);
