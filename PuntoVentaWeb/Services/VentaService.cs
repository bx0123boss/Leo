namespace PuntoVentaWeb.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PuntoVentaWeb.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Threading.Tasks;

public class VentaService
{
    private readonly IConfiguration _config;
    private readonly string _accessConn;
    private readonly ILogger<VentaService> _logger;

    public VentaService(IConfiguration config, ILogger<VentaService> logger)
    {
        _config = config;
        _logger = logger;
        _accessConn = _config.GetConnectionString("Access") ?? @"Driver={Microsoft Access Driver (*.mdb, *.accdb)};Dbq=C:\Jaeger Soft\Jaeger.accdb;Pwd=75941232;";
    }

    public async Task<List<CategoriaVenta>> ObtenerCategoriasAsync()
    {
        var lista = new List<CategoriaVenta>();
        try
        {
            using var con = new OdbcConnection(_accessConn);
            await con.OpenAsync();
            string query = "SELECT Id, Nombre FROM Categorias ORDER BY Nombre";
            using var cmd = new OdbcCommand(query, con);
            using var reader = (OdbcDataReader)await cmd.ExecuteReaderAsync();

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
            _logger.LogError(ex, "Error al obtener categorías desde Access");
            throw new Exception($"Error al cargar Categorias de Access: {ex.Message}");
        }
        return lista;
    }

    public async Task<List<ProductoVenta>> ObtenerProductosAsync(string categoria = "")
    {
        var lista = new List<ProductoVenta>();
        try
        {
            using var con = new OdbcConnection(_accessConn);
            await con.OpenAsync();

            string query = "SELECT Id, Nombre, PrecioVenta, PrecioVentaMayoreo, Especial, Categoria, IVA FROM Inventario";
            if (!string.IsNullOrWhiteSpace(categoria) && categoria != "0" && categoria != "TODOS")
            {
                query += " WHERE Categoria = ?";
            }
            query += " ORDER BY Nombre";

            using var cmd = new OdbcCommand(query, con);
            if (!string.IsNullOrWhiteSpace(categoria) && categoria != "0" && categoria != "TODOS")
            {
                cmd.Parameters.AddWithValue("?", categoria);
            }

            using var reader = (OdbcDataReader)await cmd.ExecuteReaderAsync();
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
                    Iva = iva
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener productos desde Access");
            throw new Exception($"Error al cargar Inventario de Access: {ex.Message}");
        }
        return lista;
    }

    public async Task<ProductoVenta?> BuscarProductoPorIdAsync(string idProducto)
    {
        try
        {
            using var con = new OdbcConnection(_accessConn);
            await con.OpenAsync();
            string query = "SELECT Id, Nombre, PrecioVenta, PrecioVentaMayoreo, Especial, Categoria, IVA FROM Inventario WHERE Id = ?";
            using var cmd = new OdbcCommand(query, con);
            cmd.Parameters.AddWithValue("?", idProducto);

            using var reader = (OdbcDataReader)await cmd.ExecuteReaderAsync();
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
            _logger.LogError(ex, "Error al buscar producto por ID");
        }
        return null;
    }

    public async Task<List<ClienteVentaDto>> BuscarClientesAsync(string busqueda = "")
    {
        var lista = new List<ClienteVentaDto>();
        try
        {
            using var con = new OdbcConnection(_accessConn);
            await con.OpenAsync();
            string query = "SELECT Id, Nombre, Telefono, Direccion, Correo, Adeudo, Limite FROM Clientes";
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                query += " WHERE Nombre LIKE ? OR Telefono LIKE ?";
            }
            query += " ORDER BY Nombre";

            using var cmd = new OdbcCommand(query, con);
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                cmd.Parameters.AddWithValue("?", $"%{busqueda}%");
                cmd.Parameters.AddWithValue("?", $"%{busqueda}%");
            }

            using var reader = (OdbcDataReader)await cmd.ExecuteReaderAsync();
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
            _logger.LogError(ex, "Error al buscar clientes");
        }
        return lista;
    }

    public async Task<(bool Exito, string Mensaje, string FolioFinal)> GuardarVentaAsync(VentaGuardarRequest req)
    {
        if (req.Items == null || req.Items.Count == 0)
            return (false, "El carrito no tiene productos.", "");

        string fechaStr = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

        try
        {
            using var con = new OdbcConnection(_accessConn);
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
            return (false, $"Error de conexión con la base de datos: {exCon.Message}", "");
        }
    }

    private async Task ProcesarVentaContado(OdbcConnection con, OdbcTransaction trans, VentaGuardarRequest req, string fechaStr)
    {
        string queryFolio = "SELECT Numero FROM Folios WHERE Folio='FolioContado'";
        using (var cmdF = new OdbcCommand(queryFolio, con, trans))
        {
            var fVal = await cmdF.ExecuteScalarAsync();
            int fol = fVal != null && fVal != DBNull.Value ? Convert.ToInt32(fVal) : 1;
            req.Folio = "VR" + fol.ToString("0000");

            using var cmdUpdF = new OdbcCommand("UPDATE Folios SET Numero = ? WHERE Folio = 'FolioContado'", con, trans);
            cmdUpdF.Parameters.AddWithValue("?", fol + 1);
            await cmdUpdF.ExecuteNonQueryAsync();
        }

        double totalUtilidadBruta = 0;
        foreach (var it in req.Items)
        {
            double venta = it.Precio * it.Cantidad;
            double compra = it.Costo * it.Cantidad;
            totalUtilidadBruta += (venta - compra);
        }

        foreach (var it in req.Items)
        {
            string categoria = it.Categoria ?? "";
            bool manejaInventario = true;
            double exis = 0;

            string qCheckInv = "SELECT Existencia, Categoria, ManejaInventario FROM Inventario WHERE Id = ?";
            using (var cmdChk = new OdbcCommand(qCheckInv, con, trans))
            {
                cmdChk.Parameters.AddWithValue("?", it.ProductoId);
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
                    }
                }
            }

            double precioVentaNeto = Math.Round(it.Precio / 1.16, 2);
            double utilidadBruta = (precioVentaNeto * it.Cantidad) - (it.Costo * it.Cantidad);
            double descuentoProporcional = totalUtilidadBruta > 0 ? (utilidadBruta / totalUtilidadBruta) * req.Descuento : 0;
            double nuevaUtilidad = utilidadBruta - descuentoProporcional;

            string insertDetalle = @"INSERT INTO VentasContado (FolioVenta, IdProducto, Cantidad, Producto, MontoTotal, idCliente, Fecha, Utilidad, Categoria) 
                                     VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)";
            using (var cmdDet = new OdbcCommand(insertDetalle, con, trans))
            {
                cmdDet.Parameters.AddWithValue("?", req.Folio);
                cmdDet.Parameters.AddWithValue("?", it.ProductoId);
                cmdDet.Parameters.AddWithValue("?", it.Cantidad.ToString());
                cmdDet.Parameters.AddWithValue("?", it.Nombre);
                cmdDet.Parameters.AddWithValue("?", it.Subtotal.ToString());
                cmdDet.Parameters.AddWithValue("?", string.IsNullOrEmpty(req.IdCliente) ? "0" : req.IdCliente);
                cmdDet.Parameters.AddWithValue("?", fechaStr);
                cmdDet.Parameters.AddWithValue("?", nuevaUtilidad.ToString());
                cmdDet.Parameters.AddWithValue("?", categoria);
                await cmdDet.ExecuteNonQueryAsync();
            }

            if (manejaInventario)
            {
                double nuevaExistencia = exis - it.Cantidad;
                using (var cmdInv = new OdbcCommand("UPDATE Inventario SET Existencia = ?, FechaUltimaVenta = NOW() WHERE Id = ? AND ManejaInventario = True", con, trans))
                {
                    cmdInv.Parameters.AddWithValue("?", nuevaExistencia);
                    cmdInv.Parameters.AddWithValue("?", it.ProductoId);
                    await cmdInv.ExecuteNonQueryAsync();
                }

                string kardexQ = @"INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha) 
                                   VALUES (?, 'SALIDA', ?, ?, ?, ?)";
                using (var cmdK = new OdbcCommand(kardexQ, con, trans))
                {
                    cmdK.Parameters.AddWithValue("?", it.ProductoId);
                    cmdK.Parameters.AddWithValue("?", $"VENTA DE ARTICULO FOLIO: {req.Folio}");
                    cmdK.Parameters.AddWithValue("?", exis);
                    cmdK.Parameters.AddWithValue("?", nuevaExistencia);
                    cmdK.Parameters.AddWithValue("?", fechaStr);
                    await cmdK.ExecuteNonQueryAsync();
                }
            }
        }

        string insertVenta = "INSERT INTO Ventas (Monto, Fecha, Folio, Estatus, Descuento, Pago) VALUES (?, ?, ?, 'COBRADO', ?, ?)";
        using (var cmdV = new OdbcCommand(insertVenta, con, trans))
        {
            cmdV.Parameters.AddWithValue("?", req.Total.ToString());
            cmdV.Parameters.AddWithValue("?", fechaStr);
            cmdV.Parameters.AddWithValue("?", req.Folio);
            cmdV.Parameters.AddWithValue("?", req.Descuento.ToString());
            cmdV.Parameters.AddWithValue("?", req.MetodoPago);
            await cmdV.ExecuteNonQueryAsync();
        }

        if (req.PagosDetalle != null && req.PagosDetalle.Count > 0)
        {
            foreach (var p in req.PagosDetalle)
            {
                if (p.Value > 0)
                {
                    string qPago = "INSERT INTO VentasPagos (FolioVenta, MetodoPago, Monto) VALUES (?, ?, ?)";
                    using (var cmdP = new OdbcCommand(qPago, con, trans))
                    {
                        cmdP.Parameters.AddWithValue("?", req.Folio);
                        cmdP.Parameters.AddWithValue("?", p.Key);
                        cmdP.Parameters.AddWithValue("?", p.Value);
                        await cmdP.ExecuteNonQueryAsync();
                    }

                    string qCorte = "INSERT INTO Corte (Concepto, Monto, FechaHora, Pago) VALUES (?, ?, ?, ?)";
                    using (var cmdC = new OdbcCommand(qCorte, con, trans))
                    {
                        cmdC.Parameters.AddWithValue("?", $"Venta a contado folio {req.Folio}");
                        cmdC.Parameters.AddWithValue("?", p.Value.ToString());
                        cmdC.Parameters.AddWithValue("?", fechaStr);
                        cmdC.Parameters.AddWithValue("?", p.Key);
                        await cmdC.ExecuteNonQueryAsync();
                    }
                }
            }
        }
        else
        {
            string qPago = "INSERT INTO VentasPagos (FolioVenta, MetodoPago, Monto) VALUES (?, ?, ?)";
            using (var cmdP = new OdbcCommand(qPago, con, trans))
            {
                cmdP.Parameters.AddWithValue("?", req.Folio);
                cmdP.Parameters.AddWithValue("?", req.MetodoPago);
                cmdP.Parameters.AddWithValue("?", req.Total);
                await cmdP.ExecuteNonQueryAsync();
            }

            string qCorte = "INSERT INTO Corte (Concepto, Monto, FechaHora, Pago) VALUES (?, ?, ?, ?)";
            using (var cmdC = new OdbcCommand(qCorte, con, trans))
            {
                cmdC.Parameters.AddWithValue("?", $"Venta a contado folio {req.Folio}");
                cmdC.Parameters.AddWithValue("?", req.Total.ToString());
                cmdC.Parameters.AddWithValue("?", fechaStr);
                cmdC.Parameters.AddWithValue("?", req.MetodoPago);
                await cmdC.ExecuteNonQueryAsync();
            }
        }

        string qCajero = "INSERT INTO VentasCajero (IdUsuario, Usuario, FolioVenta, Total, Fecha, Cajero) VALUES (?, ?, ?, ?, ?, ?)";
        using (var cmdCaj = new OdbcCommand(qCajero, con, trans))
        {
            cmdCaj.Parameters.AddWithValue("?", req.IdUsuario);
            cmdCaj.Parameters.AddWithValue("?", req.Usuario);
            cmdCaj.Parameters.AddWithValue("?", req.Folio);
            cmdCaj.Parameters.AddWithValue("?", req.Total.ToString());
            cmdCaj.Parameters.AddWithValue("?", fechaStr);
            cmdCaj.Parameters.AddWithValue("?", "CAJA 1");
            await cmdCaj.ExecuteNonQueryAsync();
        }
    }

    private async Task ProcesarVentaCredito(OdbcConnection con, OdbcTransaction trans, VentaGuardarRequest req, string fechaStr)
    {
        // 1. Validar Cliente y Límite de Crédito
        if (string.IsNullOrEmpty(req.IdCliente) || req.IdCliente == "0")
        {
            throw new Exception("Seleccione un cliente válido para realizar una venta a crédito.");
        }

        double adeudoActual = 0;
        double limiteCredito = 0;

        string qCliente = "SELECT Adeudo, Limite FROM Clientes WHERE Id = ?";
        using (var cmdCli = new OdbcCommand(qCliente, con, trans))
        {
            cmdCli.Parameters.AddWithValue("?", req.IdCliente);
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

        // Límite < Adeudo Actual + Total de la Venta
        if (limiteCredito < (adeudoActual + req.Total))
        {
            throw new Exception($"El límite de crédito (${limiteCredito:N2}) será excedido con la compra actual. Adeudo actual: ${adeudoActual:N2}");
        }

        // 2. Obtener y Formatear Folio de Crédito (Formato 'C0000')
        string queryFolio = "SELECT Numero FROM Folios WHERE Folio='FolioCredito'";
        using (var cmdF = new OdbcCommand(queryFolio, con, trans))
        {
            var fVal = await cmdF.ExecuteScalarAsync();
            int fol = fVal != null && fVal != DBNull.Value ? Convert.ToInt32(fVal) : 1;
            req.Folio = "C" + fol.ToString("0000");

            using var cmdUpdF = new OdbcCommand("UPDATE Folios SET Numero = Numero + 1 WHERE Folio = 'FolioCredito'", con, trans);
            await cmdUpdF.ExecuteNonQueryAsync();
        }

        // 3. Procesar Productos, VentasCredito, Inventario y Kardex
        foreach (var it in req.Items)
        {
            string categoria = it.Categoria ?? "";
            bool manejaInventario = true;
            double exis = 0;

            string qCheckInv = "SELECT Existencia, Categoria, ManejaInventario FROM Inventario WHERE Id = ?";
            using (var cmdChk = new OdbcCommand(qCheckInv, con, trans))
            {
                cmdChk.Parameters.AddWithValue("?", it.ProductoId);
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
                using (var cmdInv = new OdbcCommand("UPDATE Inventario SET Existencia = Existencia - ?, FechaUltimaVenta = NOW() WHERE Id = ? AND ManejaInventario = True", con, trans))
                {
                    cmdInv.Parameters.AddWithValue("?", it.Cantidad);
                    cmdInv.Parameters.AddWithValue("?", it.ProductoId);
                    await cmdInv.ExecuteNonQueryAsync();
                }
            }

            string insertDet = @"INSERT INTO VentasCredito (FolioVenta, IdProducto, Cantidad, Producto, MontoTotal, IdCliente, Fecha, categoria) 
                                 VALUES (?, ?, ?, ?, ?, ?, ?, ?)";
            using (var cmdDet = new OdbcCommand(insertDet, con, trans))
            {
                cmdDet.Parameters.AddWithValue("?", req.Folio);
                cmdDet.Parameters.AddWithValue("?", it.ProductoId);
                cmdDet.Parameters.AddWithValue("?", it.Cantidad);
                cmdDet.Parameters.AddWithValue("?", it.Nombre);
                cmdDet.Parameters.AddWithValue("?", it.Subtotal);
                cmdDet.Parameters.AddWithValue("?", req.IdCliente);
                cmdDet.Parameters.AddWithValue("?", fechaStr);
                cmdDet.Parameters.AddWithValue("?", categoria);
                await cmdDet.ExecuteNonQueryAsync();
            }

            if (manejaInventario)
            {
                string kardexQ = @"INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha) 
                                   VALUES (?, 'SALIDA', ?, ?, ?, ?)";
                using (var cmdK = new OdbcCommand(kardexQ, con, trans))
                {
                    cmdK.Parameters.AddWithValue("?", it.ProductoId);
                    cmdK.Parameters.AddWithValue("?", $"VENTA A CREDITO DE ARTICULO FOLIO: {req.Folio}");
                    cmdK.Parameters.AddWithValue("?", exis);
                    cmdK.Parameters.AddWithValue("?", exis - it.Cantidad);
                    cmdK.Parameters.AddWithValue("?", fechaStr);
                    await cmdK.ExecuteNonQueryAsync();
                }
            }
        }

        // 4. Procesar Abono Inicial (Si aplica) -> Inserción en Abonos y Corte
        if (req.AbonoInicial > 0)
        {
            string insertAbono = "INSERT INTO Abonos (Abono, idCliente, Fecha, Nombre, Folio, Estatus) VALUES (?, ?, ?, ?, ?, 'PAGADO')";
            using (var cmdAb = new OdbcCommand(insertAbono, con, trans))
            {
                cmdAb.Parameters.AddWithValue("?", req.AbonoInicial.ToString());
                cmdAb.Parameters.AddWithValue("?", req.IdCliente);
                cmdAb.Parameters.AddWithValue("?", fechaStr);
                cmdAb.Parameters.AddWithValue("?", req.NombreCliente);
                cmdAb.Parameters.AddWithValue("?", req.Folio);
                await cmdAb.ExecuteNonQueryAsync();
            }

            // Inserción en tabla CORTE
            string insertCorte = "INSERT INTO Corte (Concepto, Monto, FechaHora, Pago) VALUES (?, ?, ?, ?)";
            using (var cmdCor = new OdbcCommand(insertCorte, con, trans))
            {
                cmdCor.Parameters.AddWithValue("?", $"Abono de la venta a credito folio {req.Folio}");
                cmdCor.Parameters.AddWithValue("?", req.AbonoInicial.ToString());
                cmdCor.Parameters.AddWithValue("?", fechaStr);
                cmdCor.Parameters.AddWithValue("?", req.MetodoPago);
                await cmdCor.ExecuteNonQueryAsync();
            }
        }

        // 5. Insertar Encabezado de Venta a Crédito en Ventas2
        string insertVenta2 = "INSERT INTO Ventas2 (Monto, Fecha, Folio, IdCliente, Estatus, Saldo) VALUES (?, ?, ?, ?, 'COBRADO', ?)";
        using (var cmdV2 = new OdbcCommand(insertVenta2, con, trans))
        {
            cmdV2.Parameters.AddWithValue("?", req.Total.ToString());
            cmdV2.Parameters.AddWithValue("?", fechaStr);
            cmdV2.Parameters.AddWithValue("?", req.Folio);
            cmdV2.Parameters.AddWithValue("?", req.IdCliente);
            cmdV2.Parameters.AddWithValue("?", req.Total.ToString());
            await cmdV2.ExecuteNonQueryAsync();
        }

        // 6. Actualizar Nuevo Adeudo en Tabla Clientes (Adeudo + Total - AbonoInicial)
        double nuevoAdeudoCalculado = adeudoActual + (req.Total - req.AbonoInicial);
        string updateCliente = "UPDATE Clientes SET Adeudo = ? WHERE Id = ?";
        using (var cmdCl = new OdbcCommand(updateCliente, con, trans))
        {
            cmdCl.Parameters.AddWithValue("?", nuevoAdeudoCalculado);
            cmdCl.Parameters.AddWithValue("?", req.IdCliente);
            await cmdCl.ExecuteNonQueryAsync();
        }
    }
    private async Task ProcesarVentaApartado(OdbcConnection con, OdbcTransaction trans, VentaGuardarRequest req, string fechaStr)
    {
        // 1. Validar Cliente Obligatorio
        if (string.IsNullOrEmpty(req.IdCliente) || req.IdCliente == "0")
        {
            throw new Exception("Ingrese el cliente para poder hacer el apartado.");
        }

        // 2. Generación e incremento ATÓMICO del Folio (Formato 'A0000')
        string queryFolio = "SELECT Numero FROM Folios WHERE Folio='FolioApartado'";
        using (var cmdF = new OdbcCommand(queryFolio, con, trans))
        {
            var fVal = await cmdF.ExecuteScalarAsync();
            int fol = fVal != null && fVal != DBNull.Value ? Convert.ToInt32(fVal) : 1;
            req.Folio = "A" + fol.ToString("0000");

            using var cmdUpdF = new OdbcCommand("UPDATE Folios SET Numero = Numero + 1 WHERE Folio = 'FolioApartado'", con, trans);
            await cmdUpdF.ExecuteNonQueryAsync();
        }

        // 3. Procesar Productos, Existencias, VentasApartados y Kardex
        foreach (var it in req.Items)
        {
            double exis = 0;
            string qCheckInv = "SELECT Existencia FROM Inventario WHERE Id = ?";
            using (var cmdChk = new OdbcCommand(qCheckInv, con, trans))
            {
                cmdChk.Parameters.AddWithValue("?", it.ProductoId);
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
                // Si la existencia no alcanza, no se descuenta inventario y queda PENDIENTE
                estatusApartado = "PENDIENTE";

                // Actualizar únicamente FechaUltimaVenta
                using (var cmdUpdInv = new OdbcCommand("UPDATE Inventario SET FechaUltimaVenta = NOW() WHERE Id = ?", con, trans))
                {
                    cmdUpdInv.Parameters.AddWithValue("?", it.ProductoId);
                    await cmdUpdInv.ExecuteNonQueryAsync();
                }
            }
            else
            {
                // Si la existencia alcanza, se descuenta de inventario y se actualiza FechaUltimaVenta
                using (var cmdUpdInv = new OdbcCommand("UPDATE Inventario SET Existencia = ?, FechaUltimaVenta = NOW() WHERE Id = ?", con, trans))
                {
                    cmdUpdInv.Parameters.AddWithValue("?", existenciaFutura);
                    cmdUpdInv.Parameters.AddWithValue("?", it.ProductoId);
                    await cmdUpdInv.ExecuteNonQueryAsync();
                }

                // Insertar en Kardex
                string kardexQ = @"INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha) 
                                   VALUES (?, 'SALIDA', ?, ?, ?, ?)";
                using (var cmdK = new OdbcCommand(kardexQ, con, trans))
                {
                    cmdK.Parameters.AddWithValue("?", it.ProductoId);
                    cmdK.Parameters.AddWithValue("?", $"APARTADO DE ARTICULO FOLIO: {req.Folio}");
                    cmdK.Parameters.AddWithValue("?", exis);
                    cmdK.Parameters.AddWithValue("?", existenciaFutura);
                    cmdK.Parameters.AddWithValue("?", fechaStr);
                    await cmdK.ExecuteNonQueryAsync();
                }
            }

            string insertDet = @"INSERT INTO VentasApartados (FolioVenta, idProducto, Cantidad, Producto, MontoTotal, IdCliente, Fecha, Estatus) 
                                 VALUES (?, ?, ?, ?, ?, ?, ?, ?)";
            using (var cmdDet = new OdbcCommand(insertDet, con, trans))
            {
                cmdDet.Parameters.AddWithValue("?", req.Folio);
                cmdDet.Parameters.AddWithValue("?", it.ProductoId);
                cmdDet.Parameters.AddWithValue("?", it.Cantidad.ToString());
                cmdDet.Parameters.AddWithValue("?", it.Nombre);
                cmdDet.Parameters.AddWithValue("?", it.Subtotal.ToString());
                cmdDet.Parameters.AddWithValue("?", req.IdCliente);
                cmdDet.Parameters.AddWithValue("?", fechaStr);
                cmdDet.Parameters.AddWithValue("?", estatusApartado);
                await cmdDet.ExecuteNonQueryAsync();
            }
        }

        double abonoGuardar = req.AbonoInicial;
        double granTotal = req.Total; // Total ya con descuento
        double restanteGuardar = Math.Max(0, granTotal - abonoGuardar);

        string insertApartado = @"INSERT INTO Apartados (MontoTotal, Fecha, Folio, IdCliente, NombreCliente, Abono, Restante) 
                                  VALUES (?, ?, ?, ?, ?, ?, ?)";
        using (var cmdAp = new OdbcCommand(insertApartado, con, trans))
        {
            cmdAp.Parameters.AddWithValue("?", granTotal.ToString());
            cmdAp.Parameters.AddWithValue("?", fechaStr);
            cmdAp.Parameters.AddWithValue("?", req.Folio);
            cmdAp.Parameters.AddWithValue("?", req.IdCliente);
            cmdAp.Parameters.AddWithValue("?", req.NombreCliente);
            cmdAp.Parameters.AddWithValue("?", abonoGuardar.ToString());
            cmdAp.Parameters.AddWithValue("?", restanteGuardar.ToString());
            await cmdAp.ExecuteNonQueryAsync();
        }

        string insertCorte = "INSERT INTO Corte (Concepto, Monto, FechaHora, Pago) VALUES (?, ?, ?, ?)";
        using (var cmdCor = new OdbcCommand(insertCorte, con, trans))
        {
            cmdCor.Parameters.AddWithValue("?", $"Abono del apartado con folio {req.Folio}");
            cmdCor.Parameters.AddWithValue("?", abonoGuardar.ToString());
            cmdCor.Parameters.AddWithValue("?", fechaStr);
            cmdCor.Parameters.AddWithValue("?", req.MetodoPago);
            await cmdCor.ExecuteNonQueryAsync();
        }
    }
}