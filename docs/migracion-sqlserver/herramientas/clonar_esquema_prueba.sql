/* ============================================================================
   clonar_esquema_prueba.sql
   ----------------------------------------------------------------------------
   Crea una base de PRUEBAS con el mismo esquema (tablas, columnas, orden,
   tipos, IDENTITY y nulabilidad) que PuntoDeVenta, pero SIN datos.

   Sirve para ensayar la migracion con migrar_access_a_sql.py antes de
   ejecutarla contra produccion:

       sqlcmd -S .\SQLEXPRESS -E -i clonar_esquema_prueba.sql
       python migrar_access_a_sql.py --mode load --database PuntoDeVenta_ETL_Test --yes --deep-check

   Por seguridad SOLO actua si el nombre de la base destino termina en
   "_ETL_Test" (o "_Test"): nunca toca PuntoDeVenta.
   ============================================================================ */

SET NOCOUNT ON;
GO

DECLARE @origen  sysname = N'PuntoDeVenta';
DECLARE @destino sysname = N'PuntoDeVenta_ETL_Test';
DECLARE @recrear bit     = 1;   -- 1 = borra y vuelve a crear la base de prueba

IF RIGHT(@destino, 9) <> N'_ETL_Test' AND RIGHT(@destino, 5) <> N'_Test'
BEGIN
    RAISERROR('Por seguridad el destino debe terminar en _ETL_Test o _Test. No se hace nada.', 16, 1);
    RETURN;
END

DECLARE @sql nvarchar(max);

IF @recrear = 1 AND DB_ID(@destino) IS NOT NULL
BEGIN
    PRINT 'Eliminando base de prueba existente: ' + @destino;
    SET @sql = N'ALTER DATABASE ' + QUOTENAME(@destino) +
               N' SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE ' +
               QUOTENAME(@destino) + N';';
    EXEC (@sql);
END

IF DB_ID(@destino) IS NULL
BEGIN
    PRINT 'Creando base de prueba: ' + @destino;
    SET @sql = N'CREATE DATABASE ' + QUOTENAME(@destino) + N';';
    EXEC (@sql);
END

DECLARE @tablas TABLE (nombre sysname PRIMARY KEY);
INSERT INTO @tablas (nombre) VALUES
 ('Abonos'),('AbonosApartado'),('AbonosProveedores'),('Almacenes'),('Apartados'),
 ('Cambio'),('Categorias'),('Clientes'),('Compras'),('Compras2'),('ComprasC'),
 ('ComprasMovimientos'),('Configuracion'),('ConsignaCliente'),('ConsignaMovimientos'),
 ('Corte'),('Cortes'),('CortesB'),('Credito'),('Errores de pegado'),('Fech'),
 ('Fisico'),('FisicoDetallado'),('Folios'),('GastosDetallados'),('GastosGeneral'),
 ('histocortes'),('histocortesB'),('Inventario'),('InventarioAlmacenes'),
 ('InventarioSusp'),('Kardex'),('Linea'),('MovimientosAlmacenes'),('PermisosUsuario'),
 ('Poliza'),('Poliza2'),('Porcentaje'),('ProduccionLotes'),('ProductoReceta'),
 ('ProductosPoliza'),('ProductosPoliza2'),('Proveedores'),('RespaldoInventario'),
 ('Servicios'),('Tarjeta'),('TmpVentasPeriodo'),('Unidades'),('Usuarios'),('Ventas'),
 ('Ventas2'),('VentasApartados'),('VentasB'),('VentasCajero'),('VentasContado'),
 ('VentasCredito'),('VentasPagos');

DECLARE @tabla sysname, @columnas nvarchar(max), @ddl nvarchar(max), @creadas int = 0;

DECLARE cur CURSOR LOCAL FAST_FORWARD FOR SELECT nombre FROM @tablas ORDER BY nombre;
OPEN cur;
FETCH NEXT FROM cur INTO @tabla;
WHILE @@FETCH_STATUS = 0
BEGIN
    IF OBJECT_ID(QUOTENAME(@destino) + N'.dbo.' + QUOTENAME(@tabla)) IS NULL
    BEGIN
        SELECT @columnas = STRING_AGG(
            CAST(
                QUOTENAME(c.name) + N' ' +
                CASE
                    WHEN ty.name IN (N'varchar', N'char', N'varbinary', N'binary') THEN
                        ty.name + N'(' + CASE WHEN c.max_length = -1 THEN N'max'
                                              ELSE CAST(c.max_length AS nvarchar(10)) END + N')'
                    WHEN ty.name IN (N'nvarchar', N'nchar') THEN
                        ty.name + N'(' + CASE WHEN c.max_length = -1 THEN N'max'
                                              ELSE CAST(c.max_length / 2 AS nvarchar(10)) END + N')'
                    WHEN ty.name IN (N'decimal', N'numeric') THEN
                        ty.name + N'(' + CAST(c.precision AS nvarchar(10)) + N',' +
                        CAST(c.scale AS nvarchar(10)) + N')'
                    WHEN ty.name IN (N'datetime2', N'time', N'datetimeoffset') THEN
                        ty.name + N'(' + CAST(c.scale AS nvarchar(10)) + N')'
                    ELSE ty.name
                END +
                CASE WHEN c.is_identity = 1 THEN N' IDENTITY(1,1)' ELSE N'' END +
                CASE WHEN c.is_nullable = 1 THEN N' NULL' ELSE N' NOT NULL' END
                AS nvarchar(max)
            ), N', ')
            WITHIN GROUP (ORDER BY c.column_id)
        FROM PuntoDeVenta.sys.columns c
        JOIN PuntoDeVenta.sys.types ty ON ty.user_type_id = c.user_type_id
        WHERE c.object_id = OBJECT_ID(QUOTENAME(@origen) + N'.dbo.' + QUOTENAME(@tabla));

        IF @columnas IS NULL
        BEGIN
            PRINT '  AVISO: la tabla origen no existe: ' + @tabla;
        END
        ELSE
        BEGIN
            SET @ddl = N'CREATE TABLE ' + QUOTENAME(@destino) + N'.dbo.' + QUOTENAME(@tabla) +
                       N' (' + @columnas + N');';
            EXEC (@ddl);
            SET @creadas += 1;
        END
    END
    FETCH NEXT FROM cur INTO @tabla;
END
CLOSE cur;
DEALLOCATE cur;

PRINT 'Tablas creadas en ' + @destino + N': ' + CAST(@creadas AS nvarchar(10));
GO
