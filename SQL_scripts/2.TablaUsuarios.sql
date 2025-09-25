CREATE TABLE Usuario (
    IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
    Correo VARCHAR(40) NOT NULL,
    Contrasena VARCHAR(16) NOT NULL,
    Estado VARCHAR(8) NOT NULL CHECK (Estado IN ('Activo', 'Inactivo')),
    IdPersona INT NOT NULL,
    CONSTRAINT FK_Usuario_Persona FOREIGN KEY (IdPersona) REFERENCES Persona(IdPersona) ON DELETE CASCADE ON UPDATE CASCADE
);
