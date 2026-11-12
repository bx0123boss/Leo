USE [PuntoDeVenta];
GO
-- Limpiar tablas previas incompatibles
DROP TABLE IF EXISTS dbo.[ConsignaCliente];
DROP TABLE IF EXISTS dbo.[ConsignaMovimientos];
DROP TABLE IF EXISTS dbo.[VentasPagos];
DROP TABLE IF EXISTS dbo.[RespaldoInventario];
GO
IF OBJECT_ID(N'dbo.[Abonos]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Abonos] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Abono] NVARCHAR(255) NULL,
    [idCliente] INT NULL,
    [Fecha] DATETIME NULL,
    [Nombre] NVARCHAR(255) NULL,
    [Folio] NVARCHAR(255) NULL,
    [Estatus] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Abonos] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[AbonosApartado]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[AbonosApartado] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Abono] NVARCHAR(255) NULL,
    [idCliente] NVARCHAR(255) NULL,
    [Fecha] DATETIME NULL,
    [Folio] NVARCHAR(255) NULL,
    CONSTRAINT [PK_AbonosApartado] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[AbonosProveedores]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[AbonosProveedores] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [idProveedor] NVARCHAR(255) NULL,
    [Fecha] DATETIME NULL,
    [Monto] NVARCHAR(255) NULL,
    [AdeudoActual] NVARCHAR(255) NULL,
    CONSTRAINT [PK_AbonosProveedores] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[Almacenes]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Almacenes] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Nombre] NVARCHAR(100) NULL
);
END
GO
IF OBJECT_ID(N'dbo.[Apartados]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Apartados] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [MontoTotal] NVARCHAR(255) NULL,
    [Fecha] DATETIME NULL,
    [Folio] NVARCHAR(255) NULL,
    [IdCliente] INT NULL,
    [NombreCliente] NVARCHAR(255) NULL,
    [Abono] DECIMAL(18, 4) NULL,
    [Restante] DECIMAL(18, 4) NULL,
    CONSTRAINT [PK_Apartados] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[Cambio]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Cambio] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Oro] DECIMAL(18, 4) NULL,
    [Dolar] NVARCHAR(255) NULL,
    [Inicio] INT NULL,
    CONSTRAINT [PK_Cambio] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[Categorias]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Categorias] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Nombre] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Categorias] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[Clientes]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Clientes] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Nombre] NVARCHAR(255) NULL,
    [Telefono] NVARCHAR(255) NULL,
    [Direccion] NVARCHAR(255) NULL,
    [Referencia] NVARCHAR(255) NULL,
    [RFC] NVARCHAR(255) NULL,
    [Correo] NVARCHAR(255) NULL,
    [Adeudo] DECIMAL(18, 4) NULL,
    [Limite] DECIMAL(18, 4) NULL,
    [UltimoPago] DATETIME NULL,
    [Estatus] NVARCHAR(255) NULL,
    [CP] NVARCHAR(255) NULL,
    [Regimen] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Clientes] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[Compras]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Compras] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Fecha] DATETIME NULL,
    [Folio] NVARCHAR(255) NULL,
    [Tipo] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Compras] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[Compras2]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Compras2] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Movimiento] NVARCHAR(255) NULL,
    [Fecha] DATETIME NULL,
    [Capturo] NVARCHAR(255) NULL,
    [Relacion] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Compras2] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[ComprasC]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[ComprasC] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [FolioCompra] NVARCHAR(100) NULL,
    [IdProducto] NVARCHAR(255) NULL,
    [CantidadAnterior] FLOAT NULL,
    [CantidadActual] FLOAT NULL,
    [Producto] NVARCHAR(255) NULL,
    [Fecha] DATETIME NULL,
    CONSTRAINT [PK_ComprasC] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[ComprasMovimientos]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[ComprasMovimientos] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Folio] NVARCHAR(255) NULL,
    [IdProducto] NVARCHAR(255) NULL,
    [Nombre] NVARCHAR(255) NULL,
    [ExistenciasAntiguas] NVARCHAR(255) NULL,
    [Movimiento] NVARCHAR(255) NULL,
    [ExistenciasActuales] NVARCHAR(255) NULL,
    [Tipo] NVARCHAR(255) NULL,
    [Fecha] DATETIME NULL,
    CONSTRAINT [PK_ComprasMovimientos] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[Configuracion]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Configuracion] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Lugar] NVARCHAR(255) NULL,
    [DatosTicket] NVARCHAR(MAX) NULL,
    [PieDeTicket] NVARCHAR(MAX) NULL,
    [Font] NVARCHAR(255) NULL,
    [Impresora] NVARCHAR(255) NULL,
    [MaxChar] INT NULL,
    [FontSize] INT NULL,
    [MaxCharDescription] INT NULL,
    [ConIva] BIT NULL,
    [LogoPath] NVARCHAR(255) NULL,
    [CorreoEmisor] NVARCHAR(255) NULL,
    [PasswordEmisor] NVARCHAR(255) NULL,
    [FooterCorreoCotizacion] NVARCHAR(255) NULL,
    [MediaCarta] BIT NULL,
    [Whatsapp] NVARCHAR(255) NULL,
    [Bascula] BIT NULL,
    [PuntoB] BIT NULL,
    [WhatsAppPhoneNumberId] NVARCHAR(255) NULL,
    [WhatsAppBusinessId] NVARCHAR(255) NULL,
    [WhatsAppAccessToken] NVARCHAR(MAX) NULL,
    [WhatsAppApiVersion] NVARCHAR(255) NULL,
    [AbrirTeclado] BIT NULL,
    CONSTRAINT [PK_Configuracion] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[ConsignaCliente]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[ConsignaCliente] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [ClienteId] INT NULL,
    [ProductoId] NVARCHAR(255) NULL,
    [PrecioCongelado] DECIMAL(18, 4) NULL,
    [EnConsigna] INT NULL,
    [Vendidos] INT NULL,
    [Devueltos] INT NULL,
    CONSTRAINT [PK_ConsignaCliente] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[ConsignaMovimientos]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[ConsignaMovimientos] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [ClienteId] INT NULL,
    [ProductoId] NVARCHAR(255) NULL,
    [TipoMovimiento] NVARCHAR(255) NULL,
    [Cantidad] INT NULL,
    [PrecioVigente] DECIMAL(18, 4) NULL,
    [Fecha] DATETIME NULL,
    [ReferenciaVentaId] NVARCHAR(255) NULL,
    CONSTRAINT [PK_ConsignaMovimientos] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[Corte]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Corte] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Concepto] NVARCHAR(255) NULL,
    [Monto] INT NULL,
    [FechaHora] DATETIME NULL,
    [Pago] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Corte] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[Cortes]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Cortes] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Concepto] NVARCHAR(255) NULL,
    [Monto] NVARCHAR(255) NULL,
    [FechaHora] DATETIME NULL,
    [idCorte] NVARCHAR(255) NULL,
    [Tipo] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Cortes] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[CortesB]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[CortesB] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Concepto] NVARCHAR(255) NULL,
    [Monto] NVARCHAR(255) NULL,
    [FechaHora] DATETIME NULL,
    [idCorte] NVARCHAR(255) NULL,
    [Tipo] NVARCHAR(255) NULL,
    CONSTRAINT [PK_CortesB] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[Credito]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Credito] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Concepto] NVARCHAR(255) NULL,
    [Monto] INT NULL,
    [FechaHora] DATETIME NULL,
    CONSTRAINT [PK_Credito] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[Errores de pegado]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Errores de pegado] (
    [F1] NVARCHAR(255) NULL,
    [F2] NVARCHAR(255) NULL,
    [F3] FLOAT NULL,
    [F4] FLOAT NULL,
    [F5] FLOAT NULL,
    [F6] FLOAT NULL,
    [F7] NVARCHAR(255) NULL,
    [F8] FLOAT NULL,
    [F9] FLOAT NULL,
    [F10] NVARCHAR(255) NULL,
    [F11] NVARCHAR(255) NULL,
    [F12] NVARCHAR(255) NULL,
    [F13] NVARCHAR(255) NULL,
    [F14] NVARCHAR(255) NULL,
    [F15] FLOAT NULL,
    [F16] FLOAT NULL,
    [F17] BIT NULL,
    [F18] BIT NULL,
    [F19] NVARCHAR(255) NULL
);
END
GO
IF OBJECT_ID(N'dbo.[Fech]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Fech] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Caja] NVARCHAR(255) NULL,
    [Fecha] DATETIME NULL,
    CONSTRAINT [PK_Fech] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[Fisico]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Fisico] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Fecha] DATETIME NULL,
    CONSTRAINT [PK_Fisico] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[FisicoDetallado]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[FisicoDetallado] (
    [IdAut] INT IDENTITY(1,1) NOT NULL,
    [Id] NVARCHAR(255) NULL,
    [Nombre] NVARCHAR(255) NULL,
    [Existen] NVARCHAR(255) NULL,
    [Fisico] NVARCHAR(255) NULL,
    [Diferencia] NVARCHAR(255) NULL,
    [IdFisico] NVARCHAR(255) NULL,
    CONSTRAINT [PK_FisicoDetallado] PRIMARY KEY CLUSTERED ([IdAut])
);
END
GO
IF OBJECT_ID(N'dbo.[Folios]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Folios] (
    [Folio] NVARCHAR(255) NOT NULL,
    [Numero] INT NULL,
    CONSTRAINT [PK_Folios] PRIMARY KEY CLUSTERED ([Folio])
);
END
GO
IF OBJECT_ID(N'dbo.[GastosDetallados]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[GastosDetallados] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [idGasto] NVARCHAR(255) NULL,
    [Total] NVARCHAR(255) NULL,
    [Fecha] DATETIME NULL,
    CONSTRAINT [PK_GastosDetallados] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[GastosGeneral]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[GastosGeneral] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Nombre] NVARCHAR(255) NULL,
    [Lapso] NVARCHAR(255) NULL,
    [Total] DECIMAL(18, 4) NULL,
    CONSTRAINT [PK_GastosGeneral] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[histocortes]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[histocortes] (
    [Id] INT NOT NULL,
    [Monto] NVARCHAR(255) NULL,
    [Fecha] DATETIME NULL,
    [Mas] NVARCHAR(255) NULL,
    [Menos] NVARCHAR(255) NULL,
    [Tarjeta] NVARCHAR(255) NULL,
    [Utilidad] NVARCHAR(255) NULL,
    [Inversion] NVARCHAR(255) NULL,
    CONSTRAINT [PK_histocortes] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[histocortesB]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[histocortesB] (
    [Id] INT NOT NULL,
    [Monto] NVARCHAR(255) NULL,
    [Fecha] DATETIME NULL,
    [Mas] NVARCHAR(255) NULL,
    [Menos] NVARCHAR(255) NULL,
    [Tarjeta] NVARCHAR(255) NULL,
    [Utilidad] NVARCHAR(255) NULL,
    [Inversion] NVARCHAR(255) NULL,
    [CubreFolio] NVARCHAR(255) NULL,
    CONSTRAINT [PK_histocortesB] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[Inventario]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Inventario] (
    [Id] NVARCHAR(255) NOT NULL,
    [Nombre] NVARCHAR(255) NULL,
    [PrecioVentaMayoreo] DECIMAL(18, 4) NULL,
    [PrecioVenta] DECIMAL(18, 4) NULL,
    [Existencia] FLOAT NULL,
    [Limite] INT NULL,
    [Categoria] NVARCHAR(255) NULL,
    [Especial] FLOAT NULL,
    [IVA] NVARCHAR(255) NULL,
    [Unidad] NVARCHAR(255) NULL,
    [Uni] NVARCHAR(255) NULL,
    [ProveedorPrincipal] NVARCHAR(255) NULL,
    [FechaUltimaCompra] DATETIME NULL,
    [FechaUltimaVenta] DATETIME NULL,
    [StockMaximo] FLOAT NULL,
    [DiasReposicion] INT NULL,
    [ManejaInventario] BIT NULL,
    [EsInsumo] BIT NULL,
    [EsReceta] BIT NULL,
    [ImagenUrl] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_Inventario] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[InventarioAlmacenes]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[InventarioAlmacenes] (
    [IdAlmacen] INT NULL,
    [IdProducto] NVARCHAR(255) NULL,
    [Existencia] FLOAT NULL
);
END
GO
IF OBJECT_ID(N'dbo.[InventarioSusp]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[InventarioSusp] (
    [Id] NVARCHAR(255) NOT NULL,
    [Nombre] NVARCHAR(255) NULL,
    [PrecioVentaMayoreo] DECIMAL(18, 4) NULL,
    [PrecioVenta] DECIMAL(18, 4) NULL,
    [Existencia] FLOAT NULL,
    [Limite] INT NULL,
    [Categoria] NVARCHAR(255) NULL,
    [Especial] FLOAT NULL,
    [IVA] NVARCHAR(255) NULL,
    [Unidad] NVARCHAR(255) NULL,
    [Uni] NVARCHAR(255) NULL,
    CONSTRAINT [PK_InventarioSusp] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[Kardex]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Kardex] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [IdProducto] NVARCHAR(255) NULL,
    [Tipo] NVARCHAR(255) NULL,
    [Descripcion] NVARCHAR(255) NULL,
    [ExistenciaAntes] FLOAT NULL,
    [ExistenciaDespues] FLOAT NULL,
    [Fecha] DATETIME NULL,
    [Precio] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Kardex] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[Linea]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Linea] (
    [Linea] NVARCHAR(255) NOT NULL,
    [Cantidad] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Linea] PRIMARY KEY CLUSTERED ([Linea])
);
END
GO
IF OBJECT_ID(N'dbo.[MovimientosAlmacenes]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[MovimientosAlmacenes] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [IdOrigen] INT NULL,
    [IdDestino] INT NULL,
    [IdProducto] NVARCHAR(255) NULL,
    [Cantidad] FLOAT NULL,
    [Fecha] DATETIME NULL,
    [Usuario] NVARCHAR(100) NULL,
    [Cancelado] BIT NULL
);
END
GO
IF OBJECT_ID(N'dbo.[PermisosUsuario]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[PermisosUsuario] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [IdUsuario] NVARCHAR(255) NULL,
    [Permiso] NVARCHAR(255) NULL,
    CONSTRAINT [PK_PermisosUsuario] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[Poliza]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Poliza] (
    [Folio] NVARCHAR(MAX) NULL,
    [Fecha] DATETIME NULL,
    [FechaCaptura] DATETIME NULL,
    [CostoTotal] NVARCHAR(255) NULL,
    [CostoExtra] NVARCHAR(255) NULL,
    [Id] INT IDENTITY(1,1) NOT NULL,
    [IdProv] NVARCHAR(255) NULL,
    [Proveedor] NVARCHAR(255) NULL,
    [IVA] NVARCHAR(255) NULL,
    [Total] NVARCHAR(255) NULL,
    [IdAlmacen] INT NULL,
    CONSTRAINT [PK_Poliza] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[Poliza2]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Poliza2] (
    [Folio] NVARCHAR(MAX) NULL,
    [Fecha] DATETIME NULL,
    [FechaCancelacion] DATETIME NULL,
    [CostoTotal] NVARCHAR(255) NULL,
    [CostoExtra] NVARCHAR(255) NULL,
    [Id] INT IDENTITY(1,1) NOT NULL,
    CONSTRAINT [PK_Poliza2] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[Porcentaje]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Porcentaje] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Nombre] NVARCHAR(255) NULL,
    [Valor] DECIMAL(18, 4) NULL,
    CONSTRAINT [PK_Porcentaje] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[ProduccionLotes]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[ProduccionLotes] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Folio] NVARCHAR(255) NULL,
    [IdProductoFinal] NVARCHAR(255) NULL,
    [CantidadProducida] INT NULL,
    [Fecha] DATETIME NULL,
    [Usuario] NVARCHAR(255) NULL,
    CONSTRAINT [PK_ProduccionLotes] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[ProductoReceta]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[ProductoReceta] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [IdProductoFinal] NVARCHAR(255) NULL,
    [IdInsumo] NVARCHAR(255) NULL,
    [CantidadRequerida] FLOAT NULL,
    CONSTRAINT [PK_ProductoReceta] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[ProductosPoliza]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[ProductosPoliza] (
    [Id] NVARCHAR(255) NULL,
    [Nombre] NVARCHAR(255) NULL,
    [Cantidad] NVARCHAR(255) NULL,
    [Costo] NVARCHAR(255) NULL,
    [CostoExtra] NVARCHAR(255) NULL,
    [CostoReal] NVARCHAR(255) NULL,
    [IVA] NVARCHAR(255) NULL,
    [FolioPoliza] NVARCHAR(255) NULL
);
END
GO
IF OBJECT_ID(N'dbo.[ProductosPoliza2]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[ProductosPoliza2] (
    [Id] NVARCHAR(255) NULL,
    [Nombre] NVARCHAR(255) NULL,
    [Cantidad] NVARCHAR(255) NULL,
    [Costo] NVARCHAR(255) NULL,
    [CostoExtra] NVARCHAR(255) NULL,
    [CostoReal] NVARCHAR(255) NULL,
    [PrecioVenta] NVARCHAR(255) NULL,
    [IdPoliza] NVARCHAR(255) NULL
);
END
GO
IF OBJECT_ID(N'dbo.[Proveedores]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Proveedores] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Nombre] NVARCHAR(255) NULL,
    [RFC] NVARCHAR(255) NULL,
    [Direccion] NVARCHAR(255) NULL,
    [Telefono] NVARCHAR(255) NULL,
    [Correo] NVARCHAR(255) NULL,
    [Referencia] NVARCHAR(255) NULL,
    [Clave] NVARCHAR(255) NULL,
    [Adeudo] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Proveedores] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[RespaldoInventario]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[RespaldoInventario] (
    [Id] NVARCHAR(255) NOT NULL,
    [Precio] INT NULL,
    CONSTRAINT [PK_RespaldoInventario] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[Servicios]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Servicios] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Producto] NVARCHAR(255) NULL,
    [Peso] NVARCHAR(255) NULL,
    [Descripcion] NVARCHAR(255) NULL,
    [Servicio] NVARCHAR(255) NULL,
    [Comentarios] NVARCHAR(255) NULL,
    [Fecha] DATETIME NULL,
    [FechaEntrega] DATETIME NULL,
    [Presupuesto] NVARCHAR(255) NULL,
    [CostoReal] NVARCHAR(255) NULL,
    [idCliente] NVARCHAR(255) NULL,
    [NombreCliente] NVARCHAR(255) NULL,
    [Folio] NVARCHAR(255) NULL,
    [Abono] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Servicios] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[Tarjeta]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Tarjeta] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Nombre] NVARCHAR(255) NULL,
    [Porcentaje] DECIMAL(18, 4) NULL,
    CONSTRAINT [PK_Tarjeta] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[TmpVentasPeriodo]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[TmpVentasPeriodo] (
    [IdProducto] NVARCHAR(255) NULL,
    [TotalVendido] FLOAT NULL
);
END
GO
IF OBJECT_ID(N'dbo.[Unidades]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Unidades] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Nombre] NVARCHAR(255) NULL,
    [Abreviatura] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Unidades] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[Usuarios]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Usuarios] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Usuario] NVARCHAR(255) NULL,
    [Contraseña] NVARCHAR(255) NULL,
    [Tipo] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Usuarios] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[Ventas]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Ventas] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Monto] NVARCHAR(255) NULL,
    [Fecha] DATETIME NULL,
    [Folio] NVARCHAR(255) NULL,
    [Estatus] NVARCHAR(255) NULL,
    [Descuento] NVARCHAR(255) NULL,
    [Pago] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Ventas] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[Ventas2]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Ventas2] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Monto] NVARCHAR(255) NULL,
    [Fecha] DATETIME NULL,
    [Folio] NVARCHAR(255) NULL,
    [IdCliente] INT NULL,
    [Estatus] NVARCHAR(255) NULL,
    [Saldo] DECIMAL(18, 4) NULL,
    CONSTRAINT [PK_Ventas2] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[VentasApartados]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[VentasApartados] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [FolioVenta] NVARCHAR(100) NULL,
    [IdProducto] NVARCHAR(255) NULL,
    [Cantidad] NVARCHAR(255) NULL,
    [Producto] NVARCHAR(255) NULL,
    [MontoTotal] NVARCHAR(255) NULL,
    [IdCliente] NVARCHAR(255) NULL,
    [Fecha] DATETIME NULL,
    [Estatus] NVARCHAR(255) NULL,
    CONSTRAINT [PK_VentasApartados] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[VentasB]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[VentasB] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Monto] NVARCHAR(255) NULL,
    [Fecha] DATETIME NULL,
    [FolioA] NVARCHAR(255) NULL,
    [Estatus] NVARCHAR(255) NULL,
    [Descuento] NVARCHAR(255) NULL,
    [Pago] NVARCHAR(255) NULL,
    [FolioB] NVARCHAR(255) NULL,
    CONSTRAINT [PK_VentasB] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[VentasCajero]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[VentasCajero] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [IdUsuario] NVARCHAR(255) NULL,
    [Usuario] NVARCHAR(255) NULL,
    [FolioVenta] NVARCHAR(255) NULL,
    [Total] NVARCHAR(255) NULL,
    [Fecha] DATETIME NULL,
    [Cajero] NVARCHAR(255) NULL,
    CONSTRAINT [PK_VentasCajero] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[VentasContado]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[VentasContado] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [FolioVenta] NVARCHAR(100) NULL,
    [IdProducto] NVARCHAR(255) NULL,
    [Cantidad] FLOAT NULL,
    [Producto] NVARCHAR(255) NULL,
    [MontoTotal] DECIMAL(18, 4) NULL,
    [IdCliente] INT NULL,
    [Fecha] DATETIME NULL,
    [Utilidad] DECIMAL(18, 4) NULL,
    [Categoria] NVARCHAR(255) NULL,
    CONSTRAINT [PK_VentasContado] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[VentasCredito]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[VentasCredito] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [FolioVenta] NVARCHAR(100) NULL,
    [IdProducto] NVARCHAR(255) NULL,
    [Cantidad] INT NULL,
    [Producto] NVARCHAR(255) NULL,
    [MontoTotal] DECIMAL(18, 4) NULL,
    [IdCliente] INT NULL,
    [Fecha] DATETIME NULL,
    [Categoria] NVARCHAR(255) NULL,
    CONSTRAINT [PK_VentasCredito] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
IF OBJECT_ID(N'dbo.[VentasPagos]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[VentasPagos] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [FolioVenta] NVARCHAR(255) NULL,
    [MetodoPago] NVARCHAR(255) NULL,
    [Monto] NVARCHAR(255) NULL,
    CONSTRAINT [PK_VentasPagos] PRIMARY KEY CLUSTERED ([Id])
);
END
GO
