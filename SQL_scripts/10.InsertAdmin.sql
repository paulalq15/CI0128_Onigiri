-- Insertar en Persona
INSERT INTO Persona (Cedula, Nombre1, Nombre2, Apellido1, Apellido2, Telefono, FechaNacimiento, TipoPersona)
VALUES ('12345678901', 'Angel', 'Jesus', 'Mena', 'Coudin', '88887777', '2004-05-12', 'Administrador');

-- Insertar en Usuario (usamos el IdPersona recién creado)
INSERT INTO Usuario (Correo, Contrasena, Estado, IdPersona)
VALUES ('admin@example', 'Admin123', 'Activo', SCOPE_IDENTITY());
