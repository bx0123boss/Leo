namespace PuntoVentaWeb.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PuntoVentaWeb.Models;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

public class VentaService
{
    private readonly IConfiguration _config;
    private readonly string _sqlConn;
    private readonly ILogger<VentaService> _logger;

    public VentaService(IConfiguration config, ILogger<VentaService> logger)
    {
        _config = config;
        _logger = logger;
        _sqlConn = _config.GetConnectionString("SQL") ?? @"Server=.\SQLEXPRESS;Database=PuntoDeVenta;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True;";
    }

    public async Task<List<CategoriaVenta>> ObtenerCategoriasAsync()
    {
        var lista = new List<CategoriaVenta>();
        try
        {
            using var con = new SqlConnection(_sqlConn);
            await con.OpenAsync();
            string query = "SELECT Id, Nombre FROM Categorias ORDER BY Nombre";
            using var cmd = new SqlCommand(query, con);
            using var reader = await cmd.ExecuteReaderAsync();

            int index = 0;
            string[] colores = new[] { "0d6efd", "198754", "dc3545", "ffc107", "0dcaf0", "6610f2", "fd7e14", "20c997", "6c757d" };

            while (await reader.ReadAsync())
            {
                int id = 0;
                if (reader["Id"] != DBNull.Value)
                    int.TryParse(reader["Id"].ToString(), out id);

                string nombre = reader["Nombre"]?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(nombre)) continue;

                string color = colores[index % colores.Length];
                lista.Add(new CategoriaVenta
                {
                    Id = id,
                    Nombre = nombre.Trim(),
                    Color = color,
                    Letra = color == "ffc107" ? "#000000" : "#ffffff"
                });
                index++;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener categorías desde SQL Server");
            throw new Exception($"Error al cargar Categorias de SQL Server: {ex.Message}");
        }
        return lista;
    }

    public async Task<List<ProductoVenta>> ObtenerProductosAsync(string categoria = "")
    {
        var lista = new List<ProductoVenta>();
        try
        {
            using var con = new SqlConnection(_sqlConn);
            await con.OpenAsync();

            string query = "SELECT Id, Nombre, PrecioVenta, PrecioVentaMayoreo, Especial, Categoria, IVA, ImagenUrl FROM Inventario";
            bool filtraCat = !string.IsNullOrWhiteSpace(categoria) && categoria != "0" && categoria != "TODOS";
            if (filtraCat)
            {
                query += " WHERE Categoria = @Categoria";
            }
            query += " ORDER BY Nombre";

            using var cmd = new SqlCommand(query, con);
            if (filtraCat)
            {
                cmd.Parameters.AddWithValue("@Categoria", categoria);
            }

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                double precio = 0;
                if (reader["PrecioVenta"] != DBNull.Value)
                    double.TryParse(reader["PrecioVenta"].ToString(), out precio);
                else if (reader["PrecioVentaMayoreo"] != DBNull.Value)
                    double.TryParse(reader["PrecioVentaMayoreo"].ToString(), out precio);

                double costo = 0;
                if (reader["Especial"] != DBNull.Value)
                    double.TryParse(reader["Especial"].ToString(), out costo);

                double iva = 0;
                if (reader["IVA"] != DBNull.Value)
                    double.TryParse(reader["IVA"].ToString(), out iva);

                lista.Add(new ProductoVenta
                {
                    Id = reader["Id"]?.ToString() ?? "",
                    Nombre = reader["Nombre"]?.ToString() ?? "",
                    Precio = precio,
                    CostoTotal = costo,
                    Categoria = reader["Categoria"]?.ToString() ?? "",
                    Iva = iva,
                    ImagenUrl = reader["ImagenUrl"]?.ToString() ?? "",
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener productos desde SQL Server");
            throw new Exception($"Error al cargar Inventario de SQL Server: {ex.Message}");
        }
        return lista;
    }

    public async Task<ProductoVenta?> BuscarProductoPorIdAsync(string idProducto)
    {
        try
        {
            using var con = new SqlConnection(_sqlConn);
            await con.OpenAsync();
            string query = "SELECT Id, Nombre, PrecioVenta, PrecioVentaMayoreo, Especial, Categoria, IVA FROM Inventario WHERE Id = @Id";
            using var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Id", idProducto);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                double precio = 0;
                if (reader["PrecioVenta"] != DBNull.Value)
                    double.TryParse(reader["PrecioVenta"].ToString(), out precio);

                double costo = 0;
                if (reader["Especial"] != DBNull.Value)
                    double.TryParse(reader["Especial"].ToString(), out costo);

                double iva = 0;
                if (reader["IVA"] != DBNull.Value)
                    double.TryParse(reader["IVA"].ToString(), out iva);

                return new ProductoVenta
                {
                    Id = reader["Id"]?.ToString() ?? "",
                    Nombre = reader["Nombre"]?.ToString() ?? "",
                    Precio = precio,
                    CostoTotal = costo,
                    Categoria = reader["Categoria"]?.ToString() ?? "",
                    Iva = iva
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar producto por ID en SQL Server");
        }
        return null;
    }

    public async Task<List<ClienteVentaDto>> BuscarClientesAsync(string busqueda = "")
    {
        var lista = new List<ClienteVentaDto>();
        try
        {
            using var con = new SqlConnection(_sqlConn);
            await con.OpenAsync();
            string query = "SELECT Id, Nombre, Telefono, Direccion, Correo, Adeudo, Limite FROM Clientes";
            bool tieneBusqueda = !string.IsNullOrWhiteSpace(busqueda);
            if (tieneBusqueda)
            {
                query += " WHERE Nombre LIKE @busq OR Telefono LIKE @busq";
            }
            query += " ORDER BY Nombre";

            using var cmd = new SqlCommand(query, con);
            if (tieneBusqueda)
            {
                cmd.Parameters.AddWithValue("@busq", $"%{busqueda}%");
            }

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                double adeudo = 0;
                if (reader["Adeudo"] != DBNull.Value)
                    double.TryParse(reader["Adeudo"].ToString(), out adeudo);

                double limite = 0;
                if (reader["Limite"] != DBNull.Value)
                    double.TryParse(reader["Limite"].ToString(), out limite);

                lista.Add(new ClienteVentaDto
                {
                    Id = reader["Id"]?.ToString() ?? "",
                    Nombre = reader["Nombre"]?.ToString() ?? "",
                    Telefono = reader["Telefono"]?.ToString() ?? "",
                    Direccion = reader["Direccion"]?.ToString() ?? "",
                    Correo = reader["Correo"]?.ToString() ?? "",
                    Adeudo = adeudo,
                    Limite = limite
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar clientes en SQL Server");
        }
        return lista;
    }

    public async Task<(bool Exito, string Mensaje, string FolioFinal)> GuardarVentaAsync(VentaGuardarRequest req)
    {
        if (req.Items == null || req.Items.Count == 0)
            return (false, "El carrito no tiene productos.", "");

        string fechaStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        try
        {
            using var con = new SqlConnection(_sqlConn);
            await con.OpenAsync();

            using var trans = con.BeginTransaction();
            try
            {
                switch (req.TipoVenta.ToUpper())
                {
                    case "CONTADO":
                        await ProcesarVentaContado(con, trans, req, fechaStr);
                        break;
                    case "CREDITO":
                        await ProcesarVentaCredito(con, trans, req, fechaStr);
                        break;
                    case "APARTADO":
                        await ProcesarVentaApartado(con, trans, req, fechaStr);
                        break;
                    default:
                        throw new Exception("Tipo de venta no reconocido");
                }

                trans.Commit();
                return (true, "Venta registrada con éxito", req.Folio);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return (false, $"Error al guardar la venta: {ex.Message}", "");
            }
        }
        catch (Exception exCon)
        {
            return (false, $"Error de conexión con SQL Server: {exCon.Message}", "");
        }
    }

    private async Task<List<(string IdInsumo, string NombreInsumo, double CantidadRequerida)>>
        ObtenerRecetaAsync(SqlConnection con, SqlTransaction trans, string idProducto)
    {
        var receta = new List<(string, string, double)>();

        string q = @"SELECT R.IdInsumo, I.Nombre AS Insumo, R.CantidadRequerida
                 FROM ProductoReceta R
                 INNER JOIN Inventario I ON R.IdInsumo = I.Id
                 WHERE R.IdProductoFinal = @IdProductoFinal";

        using var cmd = new SqlCommand(q, con, trans);
        cmd.Parameters.AddWithValue("@IdProductoFinal", idProducto);
        using var rdr = await cmd.ExecuteReaderAsync();
        while (await rdr.ReadAsync())
        {
            string idIns = rdr["IdInsumo"]?.ToString() ?? "";
            string nomIns = rdr["Insumo"]?.ToString() ?? "";
            double cant = 0;
            if (rdr["CantidadRequerida"] != DBNull.Value)
                double.TryParse(rdr["CantidadRequerida"].ToString(), out cant);

            if (!string.IsNullOrEmpty(idIns) && cant > 0)
                receta.Add((idIns, nomIns, cant));
        }

        return receta;
    }

    private async Task DescontarInsumoAsync(
        SqlConnection con, SqlTransaction trans,
        string idInsumo, double cantidadDescontar,
        string folioVenta, string fechaStr, string tipoMovimiento)
    {
        double exisIns = 0;
        string qExis = "SELECT Existencia FROM Inventario WHERE Id = @Id";
        using (var cmdE = new SqlCommand(qExis, con, trans))
        {
            cmdE.Parameters.AddWithValue("@Id", idInsumo);
            using var rdr = await cmdE.ExecuteReaderAsync();
            if (await rdr.ReadAsync() && rdr["Existencia"] != DBNull.Value)
                double.TryParse(rdr["Existencia"].ToString(), out exisIns);
        }

        double nuevaExisIns = exisIns - cantidadDescontar;
        if (nuevaExisIns < 0)
            throw new Exception($"Stock insuficiente en insumo ID {idInsumo}. Requerido: {cantidadDescontar:N3}, disponible: {exisIns:N3}");

        using (var cmdU = new SqlCommand(
            "UPDATE Inventario SET Existencia = @Existencia, FechaUltimaVenta = GETDATE() WHERE Id = @Id", con, trans))
        {
            cmdU.Parameters.AddWithValue("@Existencia", nuevaExisIns);
            cmdU.Parameters.AddWithValue("@Id", idInsumo);
            await cmdU.ExecuteNonQueryAsync();
        }

        string kardexQ = @"INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha)
                       VALUES (@IdProducto, 'SALIDA', @Descripcion, @ExistenciaAntes, @ExistenciaDespues, @Fecha)";
        using (var cmdK = new SqlCommand(kardexQ, con, trans))
        {
            cmdK.Parameters.AddWithValue("@IdProducto", idInsumo);
            cmdK.Parameters.AddWithValue("@Descripcion", $"{tipoMovimiento} (INSUMO DE RECETA) FOLIO: {folioVenta}");
            cmdK.Parameters.AddWithValue("@ExistenciaAntes", exisIns);
            cmdK.Parameters.AddWithValue("@ExistenciaDespues", nuevaExisIns);
            cmdK.Parameters.AddWithValue("@Fecha", fechaStr);
            await cmdK.ExecuteNonQueryAsync();
        }
    }

    private async Task ProcesarVentaContado(SqlConnection con, SqlTransaction trans, VentaGuardarRequest req, string fechaStr)
    {
        // 1. FOLIO DE CONTADO
        string queryFolio = "SELECT Numero FROM Folios WHERE Folio='FolioContado'";
        using (var cmdF = new SqlCommand(queryFolio, con, trans))
        {
            var fVal = await cmdF.ExecuteScalarAsync();
            int fol = fVal != null && fVal != DBNull.Value ? Convert.ToInt32(fVal) : 1;
            req.Folio = "VR" + fol.ToString("0000");

            using var cmdUpdF = new SqlCommand("UPDATE Folios SET Numero = @Numero WHERE Folio = 'FolioContado'", con, trans);
            cmdUpdF.Parameters.AddWithValue("@Numero", fol + 1);
            await cmdUpdF.ExecuteNonQueryAsync();
        }

        // 2. CÁLCULO DE UTILIDAD BRUTA
        double totalUtilidadBruta = 0;
        foreach (var it in req.Items)
        {
            double venta = it.Precio * it.Cantidad;
            double compra = it.Costo * it.Cantidad;
            totalUtilidadBruta += (venta - compra);
        }

        // 3. PRODUCTOS, INVENTARIO Y KARDEX
        foreach (var it in req.Items)
        {
            string categoria = it.Categoria ?? "";
            bool manejaInventario = true, esReceta = false;
            double exis = 0;

            string qCheckInv = "SELECT Existencia, Categoria, ManejaInventario, EsReceta FROM Inventario WHERE Id = @Id";
            using (var cmdChk = new SqlCommand(qCheckInv, con, trans))
            {
                cmdChk.Parameters.AddWithValue("@Id", it.ProductoId);
                using (var rdr = await cmdChk.ExecuteReaderAsync())
                {
                    if (await rdr.ReadAsync())
                    {
                        if (rdr["Existencia"] != DBNull.Value)
                            double.TryParse(rdr["Existencia"].ToString(), out exis);

                        if (rdr["Categoria"] != DBNull.Value)
                            categoria = rdr["Categoria"].ToString() ?? "";

                        if (rdr["ManejaInventario"] != DBNull.Value)
                            bool.TryParse(rdr["ManejaInventario"].ToString(), out manejaInventario);
                        if (rdr["EsReceta"] != DBNull.Value)
                            bool.TryParse(rdr["EsReceta"].ToString(), out esReceta);
                    }
                }
            }

            double precioVentaNeto = Math.Round(it.Precio / 1.16, 2);
            double utilidadBruta = (precioVentaNeto * it.Cantidad) - (it.Costo * it.Cantidad);
            double descuentoProporcional = totalUtilidadBruta > 0 ? (utilidadBruta / totalUtilidadBruta) * req.Descuento : 0;
            double nuevaUtilidad = utilidadBruta - descuentoProporcional;

            string insertDetalle = @"INSERT INTO VentasContado (FolioVenta, IdProducto, Cantidad, Producto, MontoTotal, idCliente, Fecha, Utilidad, Categoria) 
                                 VALUES (@FolioVenta, @IdProducto, @Cantidad, @Producto, @MontoTotal, @IdCliente, @Fecha, @Utilidad, @Categoria)";
            using (var cmdDet = new SqlCommand(insertDetalle, con, trans))
            {
                cmdDet.Parameters.AddWithValue("@FolioVenta", req.Folio);
                cmdDet.Parameters.AddWithValue("@IdProducto", it.ProductoId);
                cmdDet.Parameters.AddWithValue("@Cantidad", it.Cantidad);
                cmdDet.Parameters.AddWithValue("@Producto", it.Nombre);
                cmdDet.Parameters.AddWithValue("@MontoTotal", it.Subtotal);
                cmdDet.Parameters.AddWithValue("@IdCliente", string.IsNullOrEmpty(req.IdCliente) ? "0" : req.IdCliente);
                cmdDet.Parameters.AddWithValue("@Fecha", fechaStr);
                cmdDet.Parameters.AddWithValue("@Utilidad", nuevaUtilidad);
                cmdDet.Parameters.AddWithValue("@Categoria", categoria);
                await cmdDet.ExecuteNonQueryAsync();
            }

            if (manejaInventario)
            {
                double nuevaExistencia = exis - it.Cantidad;
                using (var cmdInv = new SqlCommand("UPDATE Inventario SET Existencia = @Existencia, FechaUltimaVenta = GETDATE() WHERE Id = @Id AND ManejaInventario = 1", con, trans))
                {
                    cmdInv.Parameters.AddWithValue("@Existencia", nuevaExistencia);
                    cmdInv.Parameters.AddWithValue("@Id", it.ProductoId);
                    await cmdInv.ExecuteNonQueryAsync();
                }

                string kardexQ = @"INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha) 
                               VALUES (@IdProducto, 'SALIDA', @Descripcion, @ExistenciaAntes, @ExistenciaDespues, @Fecha)";
                using (var cmdK = new SqlCommand(kardexQ, con, trans))
                {
                    cmdK.Parameters.AddWithValue("@IdProducto", it.ProductoId);
                    cmdK.Parameters.AddWithValue("@Descripcion", $"VENTA DE ARTICULO FOLIO: {req.Folio}");
                    cmdK.Parameters.AddWithValue("@ExistenciaAntes", exis);
                    cmdK.Parameters.AddWithValue("@ExistenciaDespues", nuevaExistencia);
                    cmdK.Parameters.AddWithValue("@Fecha", fechaStr);
                    await cmdK.ExecuteNonQueryAsync();
                }
            }
        }

        // 4. PREPARAR DICCIONARIO Y DETERMINAR TIPO DE PAGO CABECERA
        var pagosAProcesar = new Dictionary<string, double>();

        if (req.PagosDetalle != null && req.PagosDetalle.Count > 0)
        {
            pagosAProcesar = new Dictionary<string, double>(req.PagosDetalle);
        }
        else
        {
            string metodoUnico = string.IsNullOrEmpty(req.MetodoPago) ? "01=EFECTIVO" : req.MetodoPago;
            pagosAProcesar.Add(metodoUnico, req.Total);
        }

        string tipoPagoCabecera = pagosAProcesar.Count > 1 ? "MIXTO" : pagosAProcesar.First().Key;

        if (req.Cambio > 0)
        {
            string? keyEfectivo = pagosAProcesar.Keys.FirstOrDefault(k => k.Contains("EFECTIVO", StringComparison.OrdinalIgnoreCase));
            if (keyEfectivo != null)
            {
                pagosAProcesar[keyEfectivo] = Math.Max(0, pagosAProcesar[keyEfectivo] - req.Cambio);
            }
        }

        // 5. INSERTAR CABECERA EN VENTAS
        string insertVenta = "INSERT INTO Ventas (Monto, Fecha, Folio, Estatus, Descuento, Pago) VALUES (@Monto, @Fecha, @Folio, 'COBRADO', @Descuento, @Pago)";
        using (var cmdV = new SqlCommand(insertVenta, con, trans))
        {
            cmdV.Parameters.AddWithValue("@Monto", req.Total);
            cmdV.Parameters.AddWithValue("@Fecha", fechaStr);
            cmdV.Parameters.AddWithValue("@Folio", req.Folio);
            cmdV.Parameters.AddWithValue("@Descuento", req.Descuento);
            cmdV.Parameters.AddWithValue("@Pago", tipoPagoCabecera);
            await cmdV.ExecuteNonQueryAsync();
        }

        // 6. DETALLE DE PAGOS Y CORTE
        foreach (var p in pagosAProcesar)
        {
            if (p.Value > 0)
            {
                string qPago = "INSERT INTO VentasPagos (FolioVenta, MetodoPago, Monto) VALUES (@FolioVenta, @MetodoPago, @Monto)";
                using (var cmdP = new SqlCommand(qPago, con, trans))
                {
                    cmdP.Parameters.AddWithValue("@FolioVenta", req.Folio);
                    cmdP.Parameters.AddWithValue("@MetodoPago", p.Key);
                    cmdP.Parameters.AddWithValue("@Monto", p.Value);
                    await cmdP.ExecuteNonQueryAsync();
                }

                string qCorte = "INSERT INTO Corte (Concepto, Monto, FechaHora, Pago) VALUES (@Concepto, @Monto, @FechaHora, @Pago)";
                using (var cmdC = new SqlCommand(qCorte, con, trans))
                {
                    cmdC.Parameters.AddWithValue("@Concepto", $"Venta a contado folio {req.Folio}");
                    cmdC.Parameters.AddWithValue("@Monto", p.Value);
                    cmdC.Parameters.AddWithValue("@FechaHora", fechaStr);
                    cmdC.Parameters.AddWithValue("@Pago", p.Key);
                    await cmdC.ExecuteNonQueryAsync();
                }
            }
        }

        // 7. REGISTRO EN VENTASCAJERO
        string qCajero = "INSERT INTO VentasCajero (IdUsuario, Usuario, FolioVenta, Total, Fecha, Cajero) VALUES (@IdUsuario, @Usuario, @FolioVenta, @Total, @Fecha, @Cajero)";
        using (var cmdCaj = new SqlCommand(qCajero, con, trans))
        {
            cmdCaj.Parameters.AddWithValue("@IdUsuario", string.IsNullOrEmpty(req.IdUsuario) ? "1" : req.IdUsuario);
            cmdCaj.Parameters.AddWithValue("@Usuario", string.IsNullOrEmpty(req.Usuario) ? "Admin" : req.Usuario);
            cmdCaj.Parameters.AddWithValue("@FolioVenta", req.Folio);
            cmdCaj.Parameters.AddWithValue("@Total", req.Total);
            cmdCaj.Parameters.AddWithValue("@Fecha", fechaStr);
            cmdCaj.Parameters.AddWithValue("@Cajero", string.IsNullOrEmpty(req.Usuario) ? "Admin" : req.Usuario);
            await cmdCaj.ExecuteNonQueryAsync();
        }
    }

    private async Task ProcesarVentaCredito(SqlConnection con, SqlTransaction trans, VentaGuardarRequest req, string fechaStr)
    {
        // 1. Validar Cliente y Límite de Crédito
        if (string.IsNullOrEmpty(req.IdCliente) || req.IdCliente == "0")
        {
            throw new Exception("Seleccione un cliente válido para realizar una venta a crédito.");
        }

        double adeudoActual = 0;
        double limiteCredito = 0;

        string qCliente = "SELECT Adeudo, Limite FROM Clientes WHERE Id = @Id";
        using (var cmdCli = new SqlCommand(qCliente, con, trans))
        {
            cmdCli.Parameters.AddWithValue("@Id", req.IdCliente);
            using (var rdr = await cmdCli.ExecuteReaderAsync())
            {
                if (await rdr.ReadAsync())
                {
                    if (rdr["Adeudo"] != DBNull.Value) double.TryParse(rdr["Adeudo"].ToString(), out adeudoActual);
                    if (rdr["Limite"] != DBNull.Value) double.TryParse(rdr["Limite"].ToString(), out limiteCredito);
                }
                else
                {
                    throw new Exception("El cliente seleccionado no existe.");
                }
            }
        }

        if (limiteCredito < (adeudoActual + req.Total))
        {
            throw new Exception($"El límite de crédito (${limiteCredito:N2}) será excedido con la compra actual. Adeudo actual: ${adeudoActual:N2}");
        }

        // 2. Obtener Folio de Crédito
        string queryFolio = "SELECT Numero FROM Folios WHERE Folio='FolioCredito'";
        using (var cmdF = new SqlCommand(queryFolio, con, trans))
        {
            var fVal = await cmdF.ExecuteScalarAsync();
            int fol = fVal != null && fVal != DBNull.Value ? Convert.ToInt32(fVal) : 1;
            req.Folio = "C" + fol.ToString("0000");

            using var cmdUpdF = new SqlCommand("UPDATE Folios SET Numero = Numero + 1 WHERE Folio = 'FolioCredito'", con, trans);
            await cmdUpdF.ExecuteNonQueryAsync();
        }

        // 3. Procesar Productos, VentasCredito, Inventario y Kardex
        foreach (var it in req.Items)
        {
            string categoria = it.Categoria ?? "";
            bool manejaInventario = true;
            double exis = 0;

            string qCheckInv = "SELECT Existencia, Categoria, ManejaInventario FROM Inventario WHERE Id = @Id";
            using (var cmdChk = new SqlCommand(qCheckInv, con, trans))
            {
                cmdChk.Parameters.AddWithValue("@Id", it.ProductoId);
                using (var rdr = await cmdChk.ExecuteReaderAsync())
                {
                    if (await rdr.ReadAsync())
                    {
                        if (rdr["Existencia"] != DBNull.Value) double.TryParse(rdr["Existencia"].ToString(), out exis);
                        if (rdr["Categoria"] != DBNull.Value) categoria = rdr["Categoria"].ToString() ?? "";
                        if (rdr["ManejaInventario"] != DBNull.Value) bool.TryParse(rdr["ManejaInventario"].ToString(), out manejaInventario);
                    }
                }
            }

            if (manejaInventario && (exis - it.Cantidad) < 0)
            {
                throw new Exception($"Stock insuficiente en: {it.Nombre}");
            }

            if (manejaInventario)
            {
                using (var cmdInv = new SqlCommand("UPDATE Inventario SET Existencia = Existencia - @Cantidad, FechaUltimaVenta = GETDATE() WHERE Id = @Id AND ManejaInventario = 1", con, trans))
                {
                    cmdInv.Parameters.AddWithValue("@Cantidad", it.Cantidad);
                    cmdInv.Parameters.AddWithValue("@Id", it.ProductoId);
                    await cmdInv.ExecuteNonQueryAsync();
                }
            }

            string insertDet = @"INSERT INTO VentasCredito (FolioVenta, IdProducto, Cantidad, Producto, MontoTotal, IdCliente, Fecha, categoria) 
                                 VALUES (@FolioVenta, @IdProducto, @Cantidad, @Producto, @MontoTotal, @IdCliente, @Fecha, @Categoria)";
            using (var cmdDet = new SqlCommand(insertDet, con, trans))
            {
                cmdDet.Parameters.AddWithValue("@FolioVenta", req.Folio);
                cmdDet.Parameters.AddWithValue("@IdProducto", it.ProductoId);
                cmdDet.Parameters.AddWithValue("@Cantidad", it.Cantidad);
                cmdDet.Parameters.AddWithValue("@Producto", it.Nombre);
                cmdDet.Parameters.AddWithValue("@MontoTotal", it.Subtotal);
                cmdDet.Parameters.AddWithValue("@IdCliente", req.IdCliente);
                cmdDet.Parameters.AddWithValue("@Fecha", fechaStr);
                cmdDet.Parameters.AddWithValue("@Categoria", categoria);
                await cmdDet.ExecuteNonQueryAsync();
            }

            if (manejaInventario)
            {
                string kardexQ = @"INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha) 
                                   VALUES (@IdProducto, 'SALIDA', @Descripcion, @ExistenciaAntes, @ExistenciaDespues, @Fecha)";
                using (var cmdK = new SqlCommand(kardexQ, con, trans))
                {
                    cmdK.Parameters.AddWithValue("@IdProducto", it.ProductoId);
                    cmdK.Parameters.AddWithValue("@Descripcion", $"VENTA A CREDITO DE ARTICULO FOLIO: {req.Folio}");
                    cmdK.Parameters.AddWithValue("@ExistenciaAntes", exis);
                    cmdK.Parameters.AddWithValue("@ExistenciaDespues", exis - it.Cantidad);
                    cmdK.Parameters.AddWithValue("@Fecha", fechaStr);
                    await cmdK.ExecuteNonQueryAsync();
                }
            }
        }

        // 4. Procesar Abono Inicial
        if (req.AbonoInicial > 0)
        {
            string insertAbono = "INSERT INTO Abonos (Abono, idCliente, Fecha, Nombre, Folio, Estatus) VALUES (@Abono, @IdCliente, @Fecha, @Nombre, @Folio, 'PAGADO')";
            using (var cmdAb = new SqlCommand(insertAbono, con, trans))
            {
                cmdAb.Parameters.AddWithValue("@Abono", req.AbonoInicial.ToString());
                cmdAb.Parameters.AddWithValue("@IdCliente", req.IdCliente);
                cmdAb.Parameters.AddWithValue("@Fecha", fechaStr);
                cmdAb.Parameters.AddWithValue("@Nombre", req.NombreCliente ?? "");
                cmdAb.Parameters.AddWithValue("@Folio", req.Folio);
                await cmdAb.ExecuteNonQueryAsync();
            }

            string insertCorte = "INSERT INTO Corte (Concepto, Monto, FechaHora, Pago) VALUES (@Concepto, @Monto, @FechaHora, @Pago)";
            using (var cmdCor = new SqlCommand(insertCorte, con, trans))
            {
                cmdCor.Parameters.AddWithValue("@Concepto", $"Abono de la venta a credito folio {req.Folio}");
                cmdCor.Parameters.AddWithValue("@Monto", req.AbonoInicial.ToString());
                cmdCor.Parameters.AddWithValue("@FechaHora", fechaStr);
                cmdCor.Parameters.AddWithValue("@Pago", req.MetodoPago ?? "01=EFECTIVO");
                await cmdCor.ExecuteNonQueryAsync();
            }
        }

        // 5. Insertar Encabezado en Ventas2
        string insertVenta2 = "INSERT INTO Ventas2 (Monto, Fecha, Folio, IdCliente, Estatus, Saldo) VALUES (@Monto, @Fecha, @Folio, @IdCliente, 'COBRADO', @Saldo)";
        using (var cmdV2 = new SqlCommand(insertVenta2, con, trans))
        {
            cmdV2.Parameters.AddWithValue("@Monto", req.Total.ToString());
            cmdV2.Parameters.AddWithValue("@Fecha", fechaStr);
            cmdV2.Parameters.AddWithValue("@Folio", req.Folio);
            cmdV2.Parameters.AddWithValue("@IdCliente", req.IdCliente);
            cmdV2.Parameters.AddWithValue("@Saldo", req.Total.ToString());
            await cmdV2.ExecuteNonQueryAsync();
        }

        // 6. Actualizar Nuevo Adeudo en Clientes
        double nuevoAdeudoCalculado = adeudoActual + (req.Total - req.AbonoInicial);
        string updateCliente = "UPDATE Clientes SET Adeudo = @Adeudo WHERE Id = @Id";
        using (var cmdCl = new SqlCommand(updateCliente, con, trans))
        {
            cmdCl.Parameters.AddWithValue("@Adeudo", nuevoAdeudoCalculado);
            cmdCl.Parameters.AddWithValue("@Id", req.IdCliente);
            await cmdCl.ExecuteNonQueryAsync();
        }
    }

    private async Task ProcesarVentaApartado(SqlConnection con, SqlTransaction trans, VentaGuardarRequest req, string fechaStr)
    {
        // 1. Validar Cliente Obligatorio
        if (string.IsNullOrEmpty(req.IdCliente) || req.IdCliente == "0")
        {
            throw new Exception("Ingrese el cliente para poder hacer el apartado.");
        }

        // 2. Generación e incremento ATÓMICO del Folio (Formato 'A0000')
        string queryFolio = "SELECT Numero FROM Folios WHERE Folio='FolioApartado'";
        using (var cmdF = new SqlCommand(queryFolio, con, trans))
        {
            var fVal = await cmdF.ExecuteScalarAsync();
            int fol = fVal != null && fVal != DBNull.Value ? Convert.ToInt32(fVal) : 1;
            req.Folio = "A" + fol.ToString("0000");

            using var cmdUpdF = new SqlCommand("UPDATE Folios SET Numero = Numero + 1 WHERE Folio = 'FolioApartado'", con, trans);
            await cmdUpdF.ExecuteNonQueryAsync();
        }

        // 3. Procesar Productos, Existencias, VentasApartados y Kardex
        foreach (var it in req.Items)
        {
            double exis = 0;
            string qCheckInv = "SELECT Existencia FROM Inventario WHERE Id = @Id";
            using (var cmdChk = new SqlCommand(qCheckInv, con, trans))
            {
                cmdChk.Parameters.AddWithValue("@Id", it.ProductoId);
                using (var rdr = await cmdChk.ExecuteReaderAsync())
                {
                    if (await rdr.ReadAsync() && rdr["Existencia"] != DBNull.Value)
                    {
                        double.TryParse(rdr["Existencia"].ToString(), out exis);
                    }
                }
            }

            double existenciaFutura = exis - it.Cantidad;
            string estatusApartado = "DESCONTADO DE INVENTARIO";

            if (existenciaFutura < 0)
            {
                estatusApartado = "PENDIENTE";
                using (var cmdUpdInv = new SqlCommand("UPDATE Inventario SET FechaUltimaVenta = GETDATE() WHERE Id = @Id", con, trans))
                {
                    cmdUpdInv.Parameters.AddWithValue("@Id", it.ProductoId);
                    await cmdUpdInv.ExecuteNonQueryAsync();
                }
            }
            else
            {
                using (var cmdUpdInv = new SqlCommand("UPDATE Inventario SET Existencia = @Existencia, FechaUltimaVenta = GETDATE() WHERE Id = @Id", con, trans))
                {
                    cmdUpdInv.Parameters.AddWithValue("@Existencia", existenciaFutura);
                    cmdUpdInv.Parameters.AddWithValue("@Id", it.ProductoId);
                    await cmdUpdInv.ExecuteNonQueryAsync();
                }

                string kardexQ = @"INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha) 
                                   VALUES (@IdProducto, 'SALIDA', @Descripcion, @ExistenciaAntes, @ExistenciaDespues, @Fecha)";
                using (var cmdK = new SqlCommand(kardexQ, con, trans))
                {
                    cmdK.Parameters.AddWithValue("@IdProducto", it.ProductoId);
                    cmdK.Parameters.AddWithValue("@Descripcion", $"APARTADO DE ARTICULO FOLIO: {req.Folio}");
                    cmdK.Parameters.AddWithValue("@ExistenciaAntes", exis);
                    cmdK.Parameters.AddWithValue("@ExistenciaDespues", existenciaFutura);
                    cmdK.Parameters.AddWithValue("@Fecha", fechaStr);
                    await cmdK.ExecuteNonQueryAsync();
                }
            }

            string insertDet = @"INSERT INTO VentasApartados (FolioVenta, idProducto, Cantidad, Producto, MontoTotal, IdCliente, Fecha, Estatus) 
                                 VALUES (@FolioVenta, @IdProducto, @Cantidad, @Producto, @MontoTotal, @IdCliente, @Fecha, @Estatus)";
            using (var cmdDet = new SqlCommand(insertDet, con, trans))
            {
                cmdDet.Parameters.AddWithValue("@FolioVenta", req.Folio);
                cmdDet.Parameters.AddWithValue("@IdProducto", it.ProductoId);
                cmdDet.Parameters.AddWithValue("@Cantidad", it.Cantidad.ToString());
                cmdDet.Parameters.AddWithValue("@Producto", it.Nombre);
                cmdDet.Parameters.AddWithValue("@MontoTotal", it.Subtotal.ToString());
                cmdDet.Parameters.AddWithValue("@IdCliente", req.IdCliente);
                cmdDet.Parameters.AddWithValue("@Fecha", fechaStr);
                cmdDet.Parameters.AddWithValue("@Estatus", estatusApartado);
                await cmdDet.ExecuteNonQueryAsync();
            }
        }

        double abonoGuardar = req.AbonoInicial;
        double granTotal = req.Total;
        double restanteGuardar = Math.Max(0, granTotal - abonoGuardar);

        string insertApartado = @"INSERT INTO Apartados (MontoTotal, Fecha, Folio, IdCliente, NombreCliente, Abono, Restante) 
                                  VALUES (@MontoTotal, @Fecha, @Folio, @IdCliente, @NombreCliente, @Abono, @Restante)";
        using (var cmdAp = new SqlCommand(insertApartado, con, trans))
        {
            cmdAp.Parameters.AddWithValue("@MontoTotal", granTotal.ToString());
            cmdAp.Parameters.AddWithValue("@Fecha", fechaStr);
            cmdAp.Parameters.AddWithValue("@Folio", req.Folio);
            cmdAp.Parameters.AddWithValue("@IdCliente", req.IdCliente);
            cmdAp.Parameters.AddWithValue("@NombreCliente", req.NombreCliente ?? "");
            cmdAp.Parameters.AddWithValue("@Abono", abonoGuardar.ToString());
            cmdAp.Parameters.AddWithValue("@Restante", restanteGuardar.ToString());
            await cmdAp.ExecuteNonQueryAsync();
        }

        string insertCorte = "INSERT INTO Corte (Concepto, Monto, FechaHora, Pago) VALUES (@Concepto, @Monto, @FechaHora, @Pago)";
        using (var cmdCor = new SqlCommand(insertCorte, con, trans))
        {
            cmdCor.Parameters.AddWithValue("@Concepto", $"Abono del apartado con folio {req.Folio}");
            cmdCor.Parameters.AddWithValue("@Monto", abonoGuardar.ToString());
            cmdCor.Parameters.AddWithValue("@FechaHora", fechaStr);
            cmdCor.Parameters.AddWithValue("@Pago", req.MetodoPago ?? "01=EFECTIVO");
            await cmdCor.ExecuteNonQueryAsync();
        }
    }
}
