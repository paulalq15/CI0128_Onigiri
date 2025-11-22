ALTER DATABASE SistemaPlanillas SET ALLOW_SNAPSHOT_ISOLATION ON;

CREATE INDEX IX_NominaEmpresa_Fechas
ON NominaEmpresa (IdEmpresa, FechaInicio DESC)
INCLUDE (FechaFin);

CREATE INDEX IX_NominaEmpleado_NominaEmpresa
ON NominaEmpleado (IdEmpleado, IdNominaEmpresa);