ALTER TABLE ElementoAplicado
ADD 
    TipoElemento VARCHAR(9) CHECK (TipoElemento IN ('Beneficio', 'Deduccion')),
    TipoPlan CHAR(1) CHECK (TipoPlan IN ('A', 'B', 'C')),
    CantidadDependientes INT;
