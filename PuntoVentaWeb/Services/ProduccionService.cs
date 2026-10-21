namespace PuntoVentaWeb.Services;

using JaegerSoft;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PuntoVentaWeb.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Threading.Tasks;

public class ProduccionService
{
    private readonly IConfiguration _config;
    private readonly string _accessConn;
    private readonly ILogger<ProduccionService> _logger;

    public ProduccionService(IConfiguration config, ILogger<ProduccionService> logger)
    {
        _config = config;
        _logger = logger;
        _accessConn = _config.GetConnectionString("Access") ?? @"Driver={Microsoft Access Driver (*.mdb, *.accdb)};Dbq=C:\Jaeger Soft\Jaeger.accdb;Pwd=75941232;";
    }

    /// <summary>
    /// Obtiene únicamente los productos configurados con receta (EsReceta = True)
    /// </summary>
    public async Task<List<ProductoVenta>> ObtenerProductosElaboradosAsync()
    {
        var lista = new List<ProductoVenta>();
        try
        {
            using var con = new OdbcConnection(_accessConn);
            await con.OpenAsync();

            string query = @"SELECT Id, Nombre, PrecioVenta, PrecioVentaMayoreo, Existencia, Especial, Categoria, IVA 
                            FROM Inventario 
                            WHERE EsReceta = True 
                            ORDER BY Nombre";

            using var cmd = new OdbcCommand(query, con);
            using var reader = (OdbcDataReader)await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                double precio = 0;
                if (reader["PrecioVenta"] != DBNull.Value)
                    double.TryParse(reader["PrecioVenta"].ToString(), out precio);
                else if (reader["PrecioVentaMayoreo"] != DBNull.Value)
                    double.TryParse(reader["PrecioVentaMayoreo"].ToString(), out precio);

                double exis = 0;
                if (reader["Existencia"] != DBNull.Value)
                    double.TryParse(reader["Existencia"].ToString(), out exis);

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
                    Existencia = exis,
                    CostoTotal = costo,
                    Categoria = reader["Categoria"]?.ToString() ?? "",
                    Iva = iva
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener productos elaborados desde Access");
            throw new Exception($"Error al cargar productos elaborados de Access: {ex.Message}");
        }
        return lista;
    }

    /// <summary>
    /// Obtiene los insumos que componen la receta del pan seleccionado y calcula el requerimiento
    /// </summary>
    public async Task<List<InsumoDesgloseDto>> ObtenerDesgloseInsumosAsync(string idProductoFinal, double cantidadProduccion)
    {
        var desglose = new List<InsumoDesgloseDto>();
        try
        {
            using var con = new OdbcConnection(_accessConn);
            await con.OpenAsync();

            string query = @"SELECT R.IdInsumo, I.Nombre AS Insumo, I.Uni AS Unidad, R.CantidadRequerida, I.Existencia AS StockActual
                             FROM ProductoReceta R
                             INNER JOIN Inventario I ON R.IdInsumo = I.Id
                             WHERE R.IdProductoFinal = ?";

            using var cmd = new OdbcCommand(query, con);
            cmd.Parameters.AddWithValue("?", idProductoFinal);

            using var reader = (OdbcDataReader)await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                double cantReq = 0;
                if (reader["CantidadRequerida"] != DBNull.Value)
                    double.TryParse(reader["CantidadRequerida"].ToString(), out cantReq);

                double stock = 0;
                if (reader["StockActual"] != DBNull.Value)
                    double.TryParse(reader["StockActual"].ToString(), out stock);

                desglose.Add(new InsumoDesgloseDto
                {
                    IdInsumo = reader["IdInsumo"]?.ToString() ?? "",
                    NombreInsumo = reader["Insumo"]?.ToString() ?? "",
                    Unidad = reader["Unidad"]?.ToString() ?? "",
                    CantidadUnitaria = cantReq,
                    CantidadTotalRequerida = cantReq * cantidadProduccion,
                    StockActual = stock
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener desglose de insumos para la receta");
            throw new Exception($"Error al consultar receta de Access: {ex.Message}");
        }
        return desglose;
    }
    public async Task<List<string>> ObtenerUsuariosAsync()
    {
        var usuarios = new List<string>();

        using (var conexion = new OleDbConnection(Conexion.CadCon))
        {
            await conexion.OpenAsync();
            string query = "SELECT Usuario FROM Usuarios ORDER BY Usuario ASC";

            using (var cmd = new OleDbCommand(query, conexion))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    if (!reader.IsDBNull(0))
                    {
                        usuarios.Add(reader.GetString(0));
                    }
                }
            }
        }

        return usuarios;
    }

    /// <summary>
    /// Procesa la orden de producción con transacción ODBC en Access:
    /// 1. Descuenta insumos en Inventario e inserta Salidas en Kardex.
    /// 2. Suma piezas al Pan en Inventario e inserta Entrada en Kardex.
    /// 3. Guarda el historial del lote en ProduccionLotes.
    /// </summary>
    public async Task<(bool Exito, string Mensaje, string FolioLote)> RegistrarProduccionLoteAsync(ProduccionRequest req)
    {
        if (req.InsumosAfectados == null || req.InsumosAfectados.Count == 0)
            return (false, "No se encontraron insumos a descontar para este producto.", "");

        string fechaStr = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
        string folioLote = "LOT-" + DateTime.Now.ToString("yyyyMMddHHmmss");

        try
        {
            using var con = new OdbcConnection(_accessConn);
            await con.OpenAsync();

            using var trans = con.BeginTransaction();
            try
            {
                // 1. Validar y descontar stock de insumos
                foreach (var insumo in req.InsumosAfectados)
                {
                    double exisInsumoActual = 0;
                    string qCheck = "SELECT Existencia FROM Inventario WHERE Id = ?";
                    using (var cmdChk = new OdbcCommand(qCheck, con, trans))
                    {
                        cmdChk.Parameters.AddWithValue("?", insumo.IdInsumo);
                        var resExis = await cmdChk.ExecuteScalarAsync();
                        if (resExis != null && resExis != DBNull.Value)
                            double.TryParse(resExis.ToString(), out exisInsumoActual);
                    }
/*
                    if (exisInsumoActual < insumo.CantidadTotalRequerida)
                    {
                        throw new Exception($"Stock insuficiente del insumo: {insumo.NombreInsumo}. Requerido: {insumo.CantidadTotalRequerida}, Disponible: {exisInsumoActual}");
                    }
*/

                    double nuevaExisInsumo = exisInsumoActual - insumo.CantidadTotalRequerida;

                    // Update inventario del insumo
                    string qUpdInsumo = "UPDATE Inventario SET Existencia = ? WHERE Id = ?";
                    using (var cmdUpdIn = new OdbcCommand(qUpdInsumo, con, trans))
                    {
                        cmdUpdIn.Parameters.AddWithValue("?", nuevaExisInsumo);
                        cmdUpdIn.Parameters.AddWithValue("?", insumo.IdInsumo);
                        await cmdUpdIn.ExecuteNonQueryAsync();
                    }

                    // Registrar Kardex del insumo (Salida por producción)
                    string kardexInsumo = @"INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha) 
                                           VALUES (?, 'SALIDA', ?, ?, ?, ?)";
                    using (var cmdKIn = new OdbcCommand(kardexInsumo, con, trans))
                    {
                        cmdKIn.Parameters.AddWithValue("?", insumo.IdInsumo);
                        cmdKIn.Parameters.AddWithValue("?", $"CONSUMO PRODUCCION LOTE: {folioLote} ({req.NombreProductoFinal})");
                        cmdKIn.Parameters.AddWithValue("?", exisInsumoActual);
                        cmdKIn.Parameters.AddWithValue("?", nuevaExisInsumo);
                        cmdKIn.Parameters.AddWithValue("?", fechaStr);
                        await cmdKIn.ExecuteNonQueryAsync();
                    }
                }

                // 2. Incrementar stock del producto elaborado (Pan)
                double exisPanActual = 0;
                string qCheckPan = "SELECT Existencia FROM Inventario WHERE Id = ?";
                using (var cmdChkPan = new OdbcCommand(qCheckPan, con, trans))
                {
                    cmdChkPan.Parameters.AddWithValue("?", req.IdProductoFinal);
                    var resPan = await cmdChkPan.ExecuteScalarAsync();
                    if (resPan != null && resPan != DBNull.Value)
                        double.TryParse(resPan.ToString(), out exisPanActual);
                }

                double nuevaExisPan = exisPanActual + req.CantidadProducida;

                string qUpdPan = "UPDATE Inventario SET Existencia = ? WHERE Id = ?";
                using (var cmdUpdP = new OdbcCommand(qUpdPan, con, trans))
                {
                    cmdUpdP.Parameters.AddWithValue("?", nuevaExisPan);
                    cmdUpdP.Parameters.AddWithValue("?", req.IdProductoFinal);
                    await cmdUpdP.ExecuteNonQueryAsync();
                }

                // Registrar Kardex del Pan (Entrada por producción)
                string kardexPan = @"INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha) 
                                     VALUES (?, 'ENTRADA', ?, ?, ?, ?)";
                using (var cmdKPan = new OdbcCommand(kardexPan, con, trans))
                {
                    cmdKPan.Parameters.AddWithValue("?", req.IdProductoFinal);
                    cmdKPan.Parameters.AddWithValue("?", $"ENTRADA POR HORNEADO/PRODUCCION LOTE: {folioLote}");
                    cmdKPan.Parameters.AddWithValue("?", exisPanActual);
                    cmdKPan.Parameters.AddWithValue("?", nuevaExisPan);
                    cmdKPan.Parameters.AddWithValue("?", fechaStr);
                    await cmdKPan.ExecuteNonQueryAsync();
                }

                // 3. Registrar en la tabla ProduccionLotes
                string insertLote = @"INSERT INTO ProduccionLotes (Folio, IdProductoFinal, CantidadProducida, Fecha, Usuario) 
                                     VALUES (?, ?, ?, ?, ?)";
                using (var cmdLote = new OdbcCommand(insertLote, con, trans))
                {
                    cmdLote.Parameters.AddWithValue("?", folioLote);
                    cmdLote.Parameters.AddWithValue("?", req.IdProductoFinal);
                    cmdLote.Parameters.AddWithValue("?", req.CantidadProducida);
                    cmdLote.Parameters.AddWithValue("?", fechaStr);
                    cmdLote.Parameters.AddWithValue("?", string.IsNullOrEmpty(req.Usuario) ? "PANADERO" : req.Usuario);
                    await cmdLote.ExecuteNonQueryAsync();
                }

                trans.Commit();
                return (true, "Producción procesada correctamente.", folioLote);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                _logger.LogError(ex, "Error durante la transacción de registro de producción en Access");
                return (false, $"Error al registrar lote: {ex.Message}", "");
            }
        }
        catch (Exception exCon)
        {
            _logger.LogError(exCon, "Error de conexión ODBC al procesar producción");
            return (false, $"Error de conexión con la base de datos: {exCon.Message}", "");
        }
    }
}