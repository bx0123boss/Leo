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
            // Leemos directo de la tabla Categorias en Access
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
            _logger.LogError(ex, "Error al obtener categorías desde tabla Categorias en Access");
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

            string query = "SELECT Id, Nombre, PrecioVenta, PrecioVentaMayoreo, Especial, Existencia, Categoria, IVA FROM Inventario";
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

                double exis = 0;
                if (reader["Existencia"] != DBNull.Value)
                    double.TryParse(reader["Existencia"].ToString(), out exis);

                double iva = 0;
                if (reader["IVA"] != DBNull.Value)
                    double.TryParse(reader["IVA"].ToString(), out iva);

                lista.Add(new ProductoVenta
                {
                    Id = reader["Id"]?.ToString() ?? "",
                    Nombre = reader["Nombre"]?.ToString() ?? "",
                    Precio = precio,
                    CostoTotal = costo,
                    Existencia = exis,
                    Categoria = reader["Categoria"]?.ToString() ?? "",
                    Iva = iva
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener productos desde tabla Inventario en Access");
            throw new Exception($"Error al cargar Inventario de Access: {ex.Message}");
        }
        return lista;
    }

    public async Task<string> ObtenerFolioAsync(string tipoVenta)
    {
        string campoFolio = tipoVenta.ToUpper() switch
        {
            "CREDITO" => "FolioCredito",
            "APARTADO" => "FolioApartado",
            _ => "FolioContado"
        };

        try
        {
            using var con = new OdbcConnection(_accessConn);
            await con.OpenAsync();
            string query = "SELECT Numero FROM Folios WHERE Folio = ?";
            using var cmd = new OdbcCommand(query, con);
            cmd.Parameters.AddWithValue("?", campoFolio);

            var val = await cmd.ExecuteScalarAsync();
            int num = val != null && val != DBNull.Value ? Convert.ToInt32(val) : 1;

            if (tipoVenta.ToUpper() == "APARTADO")
                return "A" + num.ToString("0000");

            return num.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener folio");
            return "1";
        }
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
            req.Folio = fol.ToString();

            using var cmdUpdF = new OdbcCommand("UPDATE Folios SET Numero = ? WHERE Folio = 'FolioContado'", con, trans);
            cmdUpdF.Parameters.AddWithValue("?", fol + 1);
            await cmdUpdF.ExecuteNonQueryAsync();
        }

        foreach (var it in req.Items)
        {
            double utilidad = Math.Round((it.Precio - it.Costo) * it.Cantidad, 2);
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
                cmdDet.Parameters.AddWithValue("?", utilidad.ToString());
                cmdDet.Parameters.AddWithValue("?", it.Categoria ?? "");
                await cmdDet.ExecuteNonQueryAsync();
            }

            using (var cmdInv = new OdbcCommand("UPDATE Inventario SET Existencia = Existencia - ? WHERE Id = ?", con, trans))
            {
                cmdInv.Parameters.AddWithValue("?", it.Cantidad);
                cmdInv.Parameters.AddWithValue("?", it.ProductoId);
                await cmdInv.ExecuteNonQueryAsync();
            }

            string kardexQ = @"INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha) 
                               VALUES (?, 'SALIDA', ?, ?, ?, ?)";
            using (var cmdK = new OdbcCommand(kardexQ, con, trans))
            {
                cmdK.Parameters.AddWithValue("?", it.ProductoId);
                cmdK.Parameters.AddWithValue("?", $"VENTA FOLIO: {req.Folio}");
                cmdK.Parameters.AddWithValue("?", it.Existencia);
                cmdK.Parameters.AddWithValue("?", it.Existencia - it.Cantidad);
                cmdK.Parameters.AddWithValue("?", fechaStr);
                await cmdK.ExecuteNonQueryAsync();
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
                    using var cmdP = new OdbcCommand(qPago, con, trans);
                    cmdP.Parameters.AddWithValue("?", req.Folio);
                    cmdP.Parameters.AddWithValue("?", p.Key);
                    cmdP.Parameters.AddWithValue("?", p.Value);
                    await cmdP.ExecuteNonQueryAsync();
                }
            }
        }
        else
        {
            string qPago = "INSERT INTO VentasPagos (FolioVenta, MetodoPago, Monto) VALUES (?, ?, ?)";
            using var cmdP = new OdbcCommand(qPago, con, trans);
            cmdP.Parameters.AddWithValue("?", req.Folio);
            cmdP.Parameters.AddWithValue("?", req.MetodoPago);
            cmdP.Parameters.AddWithValue("?", req.Total);
            await cmdP.ExecuteNonQueryAsync();
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
        string queryFolio = "SELECT Numero FROM Folios WHERE Folio='FolioCredito'";
        using (var cmdF = new OdbcCommand(queryFolio, con, trans))
        {
            var fVal = await cmdF.ExecuteScalarAsync();
            int fol = fVal != null && fVal != DBNull.Value ? Convert.ToInt32(fVal) : 1;
            req.Folio = fol.ToString();

            using var cmdUpdF = new OdbcCommand("UPDATE Folios SET Numero = ? WHERE Folio = 'FolioCredito'", con, trans);
            cmdUpdF.Parameters.AddWithValue("?", fol + 1);
            await cmdUpdF.ExecuteNonQueryAsync();
        }

        foreach (var it in req.Items)
        {
            string insertDet = @"INSERT INTO VentasCredito (FolioVenta, IdProducto, Cantidad, Producto, MontoTotal, IdCliente, Fecha, categoria) 
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
                cmdDet.Parameters.AddWithValue("?", it.Categoria ?? "");
                await cmdDet.ExecuteNonQueryAsync();
            }

            using (var cmdInv = new OdbcCommand("UPDATE Inventario SET Existencia = Existencia - ? WHERE Id = ?", con, trans))
            {
                cmdInv.Parameters.AddWithValue("?", it.Cantidad);
                cmdInv.Parameters.AddWithValue("?", it.ProductoId);
                await cmdInv.ExecuteNonQueryAsync();
            }

            string kardexQ = @"INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha) 
                               VALUES (?, 'SALIDA', ?, ?, ?, ?)";
            using (var cmdK = new OdbcCommand(kardexQ, con, trans))
            {
                cmdK.Parameters.AddWithValue("?", it.ProductoId);
                cmdK.Parameters.AddWithValue("?", $"VENTA A CREDITO FOLIO: {req.Folio}");
                cmdK.Parameters.AddWithValue("?", it.Existencia);
                cmdK.Parameters.AddWithValue("?", it.Existencia - it.Cantidad);
                cmdK.Parameters.AddWithValue("?", fechaStr);
                await cmdK.ExecuteNonQueryAsync();
            }
        }

        if (req.AbonoInicial > 0)
        {
            string insertAbono = "INSERT INTO Abonos (Abono, idCliente, Fecha, Nombre, Folio, Estatus) VALUES (?, ?, ?, ?, ?, 'ABONO DE VENTA')";
            using var cmdAb = new OdbcCommand(insertAbono, con, trans);
            cmdAb.Parameters.AddWithValue("?", req.AbonoInicial.ToString());
            cmdAb.Parameters.AddWithValue("?", req.IdCliente);
            cmdAb.Parameters.AddWithValue("?", fechaStr);
            cmdAb.Parameters.AddWithValue("?", req.NombreCliente);
            cmdAb.Parameters.AddWithValue("?", req.Folio);
            await cmdAb.ExecuteNonQueryAsync();

            string insertCorte = "INSERT INTO Corte (Concepto, Monto, FechaHora, Pago) VALUES (?, ?, ?, ?)";
            using var cmdCor = new OdbcCommand(insertCorte, con, trans);
            cmdCor.Parameters.AddWithValue("?", $"Abono de la venta a credito folio {req.Folio}");
            cmdCor.Parameters.AddWithValue("?", req.AbonoInicial.ToString());
            cmdCor.Parameters.AddWithValue("?", fechaStr);
            cmdCor.Parameters.AddWithValue("?", req.MetodoPago);
            await cmdCor.ExecuteNonQueryAsync();
        }

        double saldoPendiente = req.Total - req.AbonoInicial;
        string insertVenta2 = "INSERT INTO Ventas2 (Monto, Fecha, Folio, IdCliente, Estatus, Saldo) VALUES (?, ?, ?, ?, 'COBRADO', ?)";
        using (var cmdV2 = new OdbcCommand(insertVenta2, con, trans))
        {
            cmdV2.Parameters.AddWithValue("?", req.Total.ToString());
            cmdV2.Parameters.AddWithValue("?", fechaStr);
            cmdV2.Parameters.AddWithValue("?", req.Folio);
            cmdV2.Parameters.AddWithValue("?", req.IdCliente);
            cmdV2.Parameters.AddWithValue("?", saldoPendiente.ToString());
            await cmdV2.ExecuteNonQueryAsync();
        }

        string updateCliente = "UPDATE Clientes SET Adeudo = Adeudo + ? WHERE Id = ?";
        using (var cmdCl = new OdbcCommand(updateCliente, con, trans))
        {
            cmdCl.Parameters.AddWithValue("?", saldoPendiente);
            cmdCl.Parameters.AddWithValue("?", req.IdCliente);
            await cmdCl.ExecuteNonQueryAsync();
        }
    }

    private async Task ProcesarVentaApartado(OdbcConnection con, OdbcTransaction trans, VentaGuardarRequest req, string fechaStr)
    {
        string queryFolio = "SELECT Numero FROM Folios WHERE Folio='FolioApartado'";
        int fol = 1;
        using (var cmdF = new OdbcCommand(queryFolio, con, trans))
        {
            var fVal = await cmdF.ExecuteScalarAsync();
            fol = fVal != null && fVal != DBNull.Value ? Convert.ToInt32(fVal) : 1;
            req.Folio = "A" + fol.ToString("0000");

            using var cmdUpdF = new OdbcCommand("UPDATE Folios SET Numero = ? WHERE Folio = 'FolioApartado'", con, trans);
            cmdUpdF.Parameters.AddWithValue("?", fol + 1);
            await cmdUpdF.ExecuteNonQueryAsync();
        }

        foreach (var it in req.Items)
        {
            string insertDet = @"INSERT INTO VentasApartados (FolioVenta, idProducto, Cantidad, Producto, MontoTotal, IdCliente, Fecha, Estatus) 
                                 VALUES (?, ?, ?, ?, ?, ?, ?, 'DESCONTADO DE INVENTARIO')";
            using (var cmdDet = new OdbcCommand(insertDet, con, trans))
            {
                cmdDet.Parameters.AddWithValue("?", req.Folio);
                cmdDet.Parameters.AddWithValue("?", it.ProductoId);
                cmdDet.Parameters.AddWithValue("?", it.Cantidad.ToString());
                cmdDet.Parameters.AddWithValue("?", it.Nombre);
                cmdDet.Parameters.AddWithValue("?", it.Subtotal.ToString());
                cmdDet.Parameters.AddWithValue("?", req.IdCliente);
                cmdDet.Parameters.AddWithValue("?", fechaStr);
                await cmdDet.ExecuteNonQueryAsync();
            }

            using (var cmdInv = new OdbcCommand("UPDATE Inventario SET Existencia = Existencia - ? WHERE Id = ?", con, trans))
            {
                cmdInv.Parameters.AddWithValue("?", it.Cantidad);
                cmdInv.Parameters.AddWithValue("?", it.ProductoId);
                await cmdInv.ExecuteNonQueryAsync();
            }

            string kardexQ = @"INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha) 
                               VALUES (?, 'SALIDA', ?, ?, ?, ?)";
            using (var cmdK = new OdbcCommand(kardexQ, con, trans))
            {
                cmdK.Parameters.AddWithValue("?", it.ProductoId);
                cmdK.Parameters.AddWithValue("?", $"APARTADO DE ARTICULO FOLIO: {req.Folio}");
                cmdK.Parameters.AddWithValue("?", it.Existencia);
                cmdK.Parameters.AddWithValue("?", it.Existencia - it.Cantidad);
                cmdK.Parameters.AddWithValue("?", fechaStr);
                await cmdK.ExecuteNonQueryAsync();
            }
        }

        double abono = req.AbonoInicial;
        double restante = req.Total - abono;
        string insertApartado = @"INSERT INTO Apartados (MontoTotal, Fecha, Folio, IdCliente, NombreCliente, Abono, Restante) 
                                  VALUES (?, ?, ?, ?, ?, ?, ?)";
        using (var cmdAp = new OdbcCommand(insertApartado, con, trans))
        {
            cmdAp.Parameters.AddWithValue("?", req.Total.ToString());
            cmdAp.Parameters.AddWithValue("?", fechaStr);
            cmdAp.Parameters.AddWithValue("?", req.Folio);
            cmdAp.Parameters.AddWithValue("?", req.IdCliente);
            cmdAp.Parameters.AddWithValue("?", req.NombreCliente);
            cmdAp.Parameters.AddWithValue("?", abono.ToString());
            cmdAp.Parameters.AddWithValue("?", restante.ToString());
            await cmdAp.ExecuteNonQueryAsync();
        }

        if (abono > 0)
        {
            string insertCorte = "INSERT INTO Corte (Concepto, Monto, FechaHora, Pago) VALUES (?, ?, ?, ?)";
            using var cmdCor = new OdbcCommand(insertCorte, con, trans);
            cmdCor.Parameters.AddWithValue("?", $"Abono del apartado con folio {req.Folio}");
            cmdCor.Parameters.AddWithValue("?", abono.ToString());
            cmdCor.Parameters.AddWithValue("?", fechaStr);
            cmdCor.Parameters.AddWithValue("?", req.MetodoPago);
            await cmdCor.ExecuteNonQueryAsync();
        }
    }
}
