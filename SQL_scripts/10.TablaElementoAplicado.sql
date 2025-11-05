CREATE TABLE ElementoAplicado (
    IdElementoAplicado INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdUsuario INT FOREIGN KEY REFERENCES Usuario(IdUsuario) NOT NULL,
    IdElemento INT FOREIGN KEY REFERENCES ElementoPlanilla(IdElemento) NOT NULL,
    FechaInicio DATE NOT NULL DEFAULT (GETDATE()),
    FechaFin DATE NULL,
    Estado AS (
        CASE 
            WHEN FechaFin IS NULL THEN 'Activo'
                                  ELSE 'Inactivo'
        END
    )
);
