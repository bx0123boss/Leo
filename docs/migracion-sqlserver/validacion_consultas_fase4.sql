-- Validacion de las consultas migradas (Fase 4) contra la base real.
-- SET NOEXEC ON: compila y valida (sintaxis, tablas, columnas, parametros)
-- pero NO ejecuta NADA (ni SELECT ni INSERT/UPDATE).
SET NOCOUNT ON;
SET NOEXEC ON;
GO

-- ============ frmConsigna: saldo del cliente ============
DECLARE @IdCliente int = 1, @IdProducto nvarchar(255) = 'X', @Cantidad float = 1,
        @PrecioCongelado decimal(18,2) = 1, @Fecha datetime = GETDATE(),
        @Descripcion nvarchar(255) = '', @ExistenciaAntes float = 0,
        @ExistenciaDespues float = 0, @Precio float = 0, @FolioVenta nvarchar(255) = '',
        @CantidadVendida float = 1, @CantidadDevuelta float = 1, @Existencia float = 0;

SELECT c.ProductoId, i.Nombre AS Producto, c.PrecioCongelado, c.EnConsigna, c.Vendidos, c.Devueltos
FROM ConsignaCliente c
INNER JOIN Inventario i ON c.ProductoId = i.Id
WHERE c.ClienteId = @IdCliente;

SELECT m.ProductoId, m.Fecha, m.TipoMovimiento AS Movimiento, i.Nombre AS Producto,
       m.Cantidad, m.PrecioVigente AS Precio, m.ReferenciaVentaId AS FolioTicket
FROM ConsignaMovimientos m
INNER JOIN Inventario i ON m.ProductoId = i.Id
WHERE m.ClienteId = @IdCliente
ORDER BY m.Fecha DESC;

SELECT EnConsigna FROM ConsignaCliente WHERE ClienteId = @IdCliente AND ProductoId = @IdProducto AND PrecioCongelado = @PrecioCongelado;

UPDATE ConsignaCliente SET EnConsigna = EnConsigna - @Cantidad
WHERE ClienteId = @IdCliente AND ProductoId = @IdProducto AND PrecioCongelado = @PrecioCongelado;

UPDATE ConsignaCliente SET EnConsigna = EnConsigna + @Cantidad, Devueltos = Devueltos - @CantidadDevuelta
WHERE ClienteId = @IdCliente AND ProductoId = @IdProducto AND PrecioCongelado = @PrecioCongelado;

INSERT INTO ConsignaMovimientos (ClienteId, ProductoId, TipoMovimiento, Cantidad, PrecioVigente, Fecha, ReferenciaVentaId)
VALUES (@IdCliente, @IdProducto, 'VENTA', @Cantidad, @PrecioCongelado, GETDATE(), @FolioVenta);

INSERT INTO ConsignaMovimientos (ClienteId, ProductoId, TipoMovimiento, Cantidad, PrecioVigente, Fecha)
VALUES (@IdCliente, @IdProducto, 'DEVOLUCION', @Cantidad, @PrecioCongelado, GETDATE());

SELECT Existencia FROM Inventario WHERE Id = @IdProducto;

UPDATE Inventario SET Existencia = @Existencia WHERE Id = @IdProducto;

UPDATE Inventario SET Existencia = Existencia - @Cantidad WHERE Id = @IdProducto AND ManejaInventario = 1;

INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha)
VALUES (@IdProducto, 'SALIDA', @Descripcion, @ExistenciaAntes, @ExistenciaDespues, @Fecha);
GO

-- ============ frmVentasCredito / Abonos ============
DECLARE @abono float=1, @idCliente int=1, @fecha datetime=GETDATE(), @cliente nvarchar(255)='',
        @folio nvarchar(255)='', @estatus nvarchar(255)='', @adeudo float=0, @id nvarchar(255)='X',
        @cant float=1;

INSERT INTO Abonos (Abono,idCliente,Fecha,Nombre,Folio,Estatus)
VALUES (@abono, @idCliente, @fecha, @cliente, @folio, @estatus);

UPDATE Clientes SET Adeudo = @adeudo WHERE Id = @id;

SELECT COUNT(*) FROM VentasCredito WHERE FolioVenta = @folio;

SELECT Categoria, ManejaInventario FROM Inventario WHERE Id = @id;

UPDATE Inventario SET Existencia = Existencia - @cant, FechaUltimaVenta = GETDATE()
WHERE Id = @id AND ManejaInventario = 1;

INSERT INTO VentasCredito (FolioVenta, IdProducto, Cantidad, Producto, MontoTotal, IdCliente, Fecha, categoria)
VALUES (@folio, @id, @cant, 'P', 1, @idCliente, @fecha, 'C');
GO

-- ============ frmEditarProducto ============
DECLARE @Nombre nvarchar(255)='', @PrecioVenta decimal(18,4)=0, @PrecioVentaMayoreo decimal(18,4)=0,
        @Categoria nvarchar(255)='', @Especial float=0, @IVA nvarchar(255)='', @Unidad nvarchar(255)='',
        @Uni nvarchar(255)='', @ProveedorPrincipal nvarchar(255)='', @StockMaximo float=0,
        @DiasReposicion int=0, @Limite int=0, @ManejaInventario bit=1, @EsInsumo bit=0, @EsReceta bit=0,
        @ImagenUrl nvarchar(max)='', @Id nvarchar(255)='X', @FechaK datetime=GETDATE(), @PrecioK float=0;

UPDATE Inventario SET
  Nombre = @Nombre, PrecioVenta = @PrecioVenta, PrecioVentaMayoreo = @PrecioVentaMayoreo,
  Categoria = @Categoria, Especial = @Especial, IVA = @IVA, Unidad = @Unidad, Uni = @Uni,
  ProveedorPrincipal = @ProveedorPrincipal, StockMaximo = @StockMaximo,
  DiasReposicion = @DiasReposicion, Limite = @Limite, ManejaInventario = @ManejaInventario,
  EsInsumo = @EsInsumo, EsReceta = @EsReceta, ImagenUrl = @ImagenUrl
WHERE Id = @Id;

INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha, Precio)
VALUES (@Id, 'PRECIO', 'MODIFICACION DE PRECIO', 0, '0', @FechaK, @PrecioK);

SELECT ManejaInventario, EsInsumo, EsReceta, ImagenUrl FROM Inventario WHERE Id = @Id;
GO

-- ============ Reportes con fechas ISO ============
DECLARE @fi nvarchar(30)='2026-01-01 00:00:00', @ff nvarchar(30)='2026-12-31 23:59:59';

SELECT Id, Monto, Fecha, FolioB AS Folio, Estatus, Descuento, Pago, FolioA
FROM VentasB WHERE Fecha >= '2026-01-01 00:00:00' AND Fecha <= '2026-12-31 23:59:59';

SELECT V.Id, V.Monto, V.Fecha, V.Folio, V.Estatus, V.Descuento, V.Pago AS [Tipo Pago],
  (SELECT IIF(SUM(TRY_CAST(REPLACE(Monto,'$','') AS DECIMAL(18,2))) IS NULL, 0, SUM(TRY_CAST(REPLACE(Monto,'$','') AS DECIMAL(18,2)))) FROM VentasPagos WHERE FolioVenta = V.Folio AND MetodoPago = '01=EFECTIVO') AS Efectivo,
  (SELECT IIF(SUM(TRY_CAST(REPLACE(Monto,'$','') AS DECIMAL(18,2))) IS NULL, 0, SUM(TRY_CAST(REPLACE(Monto,'$','') AS DECIMAL(18,2)))) FROM VentasPagos WHERE FolioVenta = V.Folio AND MetodoPago LIKE '%TARJETA%') AS Tarjeta,
  (SELECT IIF(SUM(TRY_CAST(REPLACE(Monto,'$','') AS DECIMAL(18,2))) IS NULL, 0, SUM(TRY_CAST(REPLACE(Monto,'$','') AS DECIMAL(18,2)))) FROM VentasPagos WHERE FolioVenta = V.Folio AND MetodoPago = '03=TRANFERENCIA ELECTRONICA DE FONDOS') AS Transferencia
FROM Ventas V WHERE Fecha >= '2026-01-01 00:00:00' AND Fecha <= '2026-12-31 23:59:59' ORDER BY Fecha DESC;

SELECT * FROM histocortes WHERE Fecha >= '2026-01-01 00:00:00' AND Fecha <= '2026-01-01 23:59:59' ORDER BY Id DESC;

SELECT MontoTotal,Utilidad,Id FROM VentasContado WHERE Fecha >= '2026-01-01 00:00:00' AND Fecha <= '2026-01-01 23:59:59';

SELECT Id,Folio, CAST(REPLACE(Monto,'$','') AS DECIMAL(18,2)) / (1 + (16.0 / 100)) AS [Monto sin IVA], CAST(REPLACE(Monto,'$','') AS DECIMAL(18,2)) - (CAST(REPLACE(Monto,'$','') AS DECIMAL(18,2)) / (1 + (16.0 / 100))) AS [IVA], Monto,Descuento, Fecha, Estatus, Pago
FROM Ventas WHERE Estatus ='COBRADO' AND Fecha >= '2026-01-01 00:00:00' AND Fecha <= '2026-01-01 23:59:59';

SELECT * FROM Apartados WHERE Fecha >= '2026-01-01 00:00:00' AND Fecha <= '2026-01-01 23:59:59';
SELECT * FROM Compras WHERE Fecha >= '2026-01-01 00:00:00' AND Fecha <= '2026-01-01 23:59:59';
SELECT * FROM Fisico WHERE Fecha >= '2026-01-01 00:00:00' AND Fecha <= '2026-01-01 23:59:59';
SELECT * FROM Servicios WHERE Fecha >= '2026-01-01 00:00:00' AND Fecha <= '2026-01-01 23:59:59';
SELECT * FROM Ventas2 WHERE Fecha >= '2026-01-01 00:00:00' AND Fecha <= '2026-01-01 23:59:59';
SELECT * FROM VentasCajero WHERE Fecha >= '2026-01-01 00:00:00' AND Fecha <= '2026-01-01 23:59:59';
SELECT * FROM Poliza2 WHERE Fecha >= '2026-01-01 00:00:00' AND Fecha <= '2026-01-01 23:59:59';
SELECT * FROM GastosDetallados WHERE Fecha >= '2026-01-01 00:00:00' AND Fecha <= '2026-01-01 23:59:59';
GO


-- ============ frmPrincipal: adeudos ============
DECLARE @IdClienteP int = 1, @Adeudo float = 0, @IdP nvarchar(255) = 'X';

SELECT Sum(Saldo) FROM Ventas2 WHERE IdCliente = @IdClienteP;

UPDATE Clientes SET Adeudo = @Adeudo WHERE Id = @IdP;
GO

-- ============ frmInventario: rotacion de ventas ============
SELECT IdProducto,
       SUM(IIF(MONTH(Fecha) = MONTH(GETDATE()) AND YEAR(Fecha) = YEAR(GETDATE()), CantidadDouble, 0)) AS VentasMes,
       SUM(IIF(YEAR(Fecha) = YEAR(GETDATE()), CantidadDouble, 0)) AS VentasAnio
FROM (
  SELECT IdProducto, Cantidad AS CantidadDouble, Fecha FROM VentasContado WHERE FolioVenta IN (SELECT Folio FROM Ventas WHERE Estatus <> 'CANCELADO')
  UNION ALL
  SELECT IdProducto, Cantidad AS CantidadDouble, Fecha FROM VentasCredito WHERE FolioVenta IN (SELECT Folio FROM Ventas2 WHERE Estatus <> 'CANCELADO')
  UNION ALL
  SELECT IdProducto, ISNULL(TRY_CAST(REPLACE(ISNULL(Cantidad,''),'$','') AS FLOAT), 0) AS CantidadDouble, Fecha FROM VentasApartados WHERE Estatus <> 'CANCELADO'
) AS Combined
GROUP BY IdProducto;
GO

PRINT 'Todas las consultas compilaron correctamente.';
