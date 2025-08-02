CREATE DATABASE DBPRACTICA




USE [DBPRACTICA]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Empleado](
	[Codigo] [int] IDENTITY(1,1) NOT NULL,
	[Puesto] [nvarchar](100) NOT NULL,
	[Nombre] [nvarchar](100) NOT NULL,
	[CodigoJefe] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Codigo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Empleado]  WITH CHECK ADD  CONSTRAINT [FK_Empleados_Jefe] FOREIGN KEY([CodigoJefe])
REFERENCES [dbo].[Empleado] ([Codigo])
GO
ALTER TABLE [dbo].[Empleado] CHECK CONSTRAINT [FK_Empleados_Jefe]
GO






CREATE PROCEDURE sp_ObtenerJerarquia
AS
BEGIN
    WITH Jerarquia AS (
        SELECT 
            Codigo,
            Puesto,
            Nombre,
            CodigoJefe,
            0 AS Nivel
        FROM Empleado
        WHERE CodigoJefe IS NULL

        UNION ALL

        SELECT 
            e.Codigo,
            e.Puesto,
            e.Nombre,
            e.CodigoJefe,
            j.Nivel + 1
        FROM Empleado e
        INNER JOIN Jerarquia j ON e.CodigoJefe = j.Codigo
    )
    SELECT * FROM Jerarquia
    ORDER BY Nivel, CodigoJefe;
END


-- Insertar
CREATE PROCEDURE sp_InsertarEmpleado
    @Puesto NVARCHAR(50),
    @Nombre NVARCHAR(100),
    @CodigoJefe INT = NULL
AS
BEGIN
    INSERT INTO Empleado (Puesto, Nombre, CodigoJefe)
    VALUES (@Puesto, @Nombre, @CodigoJefe);
END
GO

-- Actualizar
CREATE PROCEDURE sp_ActualizarEmpleado
    @Codigo INT,
    @Puesto NVARCHAR(50),
    @Nombre NVARCHAR(100),
    @CodigoJefe INT = NULL
AS
BEGIN
    UPDATE Empleado
    SET Puesto = @Puesto, Nombre = @Nombre, CodigoJefe = @CodigoJefe
    WHERE Codigo = @Codigo;
END
GO

-- Eliminar
CREATE PROCEDURE sp_EliminarEmpleado
    @Codigo INT
AS
BEGIN
    DELETE FROM Empleado WHERE Codigo = @Codigo;
END
GO
