CREATE TABLE Contrato (
    IdContrato INT IDENTITY(1,1) PRIMARY KEY,
    Puesto VARCHAR(50) NOT NULL,
    Departamento VARCHAR(50) NOT NULL,
    Salario DECIMAL(9,2) NOT NULL,
    Tipo VARCHAR(25) NOT NULL CHECK (Tipo IN ('Tiempo Completo', 'Medio Tiempo', 'Servicios Profesionales')),
    FechaInicio DATE NOT NULL,
    FechaFin DATE NULL,
    CuentaPago VARCHAR(22) NOT NULL,
    IdPersona INT NOT NULL,
    CONSTRAINT FK_Contrato_Persona FOREIGN KEY (IdPersona)
        REFERENCES Persona(IdPersona)
        ON DELETE NO ACTION
        ON UPDATE CASCADE
);
