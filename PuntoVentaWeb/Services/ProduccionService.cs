namespace PuntoVentaWeb.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PuntoVentaWeb.Models;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

public class ProduccionService
{
    private readonly IConfiguration _config;
    private readonly string _sqlConn;
    private readonly ILogger<ProduccionService> _logger;

    public ProduccionService(IConfiguration config, ILogger<ProduccionService> logger)
    {
        _config = config;
        _logger = logger;
        _sqlConn = _config.GetConnectionString("SQL") ?? @"Server=.\SQLEXPRESS;Database=PuntoDeVenta;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True;";
    }

    /// <summary>
    /// Obtiene únicamente los productos configurados con receta (EsReceta = 1)
    /// </summary>
    public async Task<List<ProductoVenta>> ObtenerProductosElaboradosAsync()
    {
        var lista = new List<ProductoVenta>();
        try
        {
            using var con = new SqlConnection(_sqlConn);
            await con.OpenAsync();

            string query = @"SELECT Id, Nombre, PrecioVenta, PrecioVentaMayoreo, Existencia, Especial, Categoria, IVA 
                            FROM Inventario 
                            WHERE EsReceta = 1 
                            ORDER BY Nombre";

            using var cmd = new SqlCommand(query, con);
            using var reader = await cmd.ExecuteReaderAsync();

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
            _logger.LogError(ex, "Error al obtener productos elaborados desde SQL Server");
            throw new Exception($"Error al cargar productos elaborados de SQL Server: {ex.Message}");
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
            using var con = new SqlConnection(_sqlConn);
            await con.OpenAsync();

            string query = @"SELECT R.IdInsumo, I.Nombre AS Insumo, I.Uni AS Unidad, R.CantidadRequerida, I.Existencia AS StockActual
                             FROM ProductoReceta R
                             INNER JOIN Inventario I ON R.IdInsumo = I.Id
                             WHERE R.IdProductoFinal = @IdProductoFinal";

            using var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@IdProductoFinal", idProductoFinal);

            using var reader = await cmd.ExecuteReaderAsync();
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
            throw new Exception($"Error al consultar receta de SQL Server: {ex.Message}");
        }
        return desglose;
    }

    public async Task<List<string>> ObtenerUsuariosAsync()
    {
        var usuarios = new List<string>();

        using (var conexion = new SqlConnection(_sqlConn))
        {
            await conexion.OpenAsync();
            string query = "SELECT Usuario FROM Usuarios ORDER BY Usuario ASC";

            using (var cmd = new SqlCommand(query, conexion))
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
    /// Procesa la orden de producción con transacción en SQL Server:
    /// 1. Descuenta insumos en Inventario e inserta Salidas en Kardex.
    /// 2. Suma piezas al Pan en Inventario e inserta Entrada en Kardex.
    /// 3. Guarda el historial del lote en ProduccionLotes.
    /// </summary>
    public async Task<(bool Exito, string Mensaje, string FolioLote)> RegistrarProduccionLoteAsync(ProduccionRequest req)
    {
        if (req.InsumosAfectados == null || req.InsumosAfectados.Count == 0)
            return (false, "No se encontraron insumos a descontar para este producto.", "");

        string fechaStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string folioLote = "LOT-" + DateTime.Now.ToString("yyyyMMddHHmmss");

        try
        {
            using var con = new SqlConnection(_sqlConn);
            await con.OpenAsync();

            using var trans = con.BeginTransaction();
            try
            {
                // 1. Validar y descontar stock de insumos
                foreach (var insumo in req.InsumosAfectados)
                {
                    double exisInsumoActual = 0;
                    string qCheck = "SELECT Existencia FROM Inventario WHERE Id = @Id";
                    using (var cmdChk = new SqlCommand(qCheck, con, trans))
                    {
                        cmdChk.Parameters.AddWithValue("@Id", insumo.IdInsumo);
                        var resExis = await cmdChk.ExecuteScalarAsync();
                        if (resExis != null && resExis != DBNull.Value)
                            double.TryParse(resExis.ToString(), out exisInsumoActual);
                    }

                    double nuevaExisInsumo = exisInsumoActual - insumo.CantidadTotalRequerida;

                    // Update inventario del insumo
                    string qUpdInsumo = "UPDATE Inventario SET Existencia = @Existencia, FechaUltimaCompra = GETDATE() WHERE Id = @Id";
                    using (var cmdUpdIn = new SqlCommand(qUpdInsumo, con, trans))
                    {
                        cmdUpdIn.Parameters.AddWithValue("@Existencia", nuevaExisInsumo);
                        cmdUpdIn.Parameters.AddWithValue("@Id", insumo.IdInsumo);
                        await cmdUpdIn.ExecuteNonQueryAsync();
                    }

                    // Registrar Kardex del insumo (Salida por producción)
                    string kardexInsumo = @"INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha) 
                                           VALUES (@IdProducto, 'SALIDA', @Descripcion, @ExistenciaAntes, @ExistenciaDespues, @Fecha)";
                    using (var cmdKIn = new SqlCommand(kardexInsumo, con, trans))
                    {
                        cmdKIn.Parameters.AddWithValue("@IdProducto", insumo.IdInsumo);
                        cmdKIn.Parameters.AddWithValue("@Descripcion", $"CONSUMO PRODUCCION LOTE: {folioLote} ({req.NombreProductoFinal})");
                        cmdKIn.Parameters.AddWithValue("@ExistenciaAntes", exisInsumoActual);
                        cmdKIn.Parameters.AddWithValue("@ExistenciaDespues", nuevaExisInsumo);
                        cmdKIn.Parameters.AddWithValue("@Fecha", fechaStr);
                        await cmdKIn.ExecuteNonQueryAsync();
                    }
                }

                // 2. Incrementar stock del producto elaborado (Pan)
                double exisPanActual = 0;
                string qCheckPan = "SELECT Existencia FROM Inventario WHERE Id = @Id";
                using (var cmdChkPan = new SqlCommand(qCheckPan, con, trans))
                {
                    cmdChkPan.Parameters.AddWithValue("@Id", req.IdProductoFinal);
                    var resPan = await cmdChkPan.ExecuteScalarAsync();
                    if (resPan != null && resPan != DBNull.Value)
                        double.TryParse(resPan.ToString(), out exisPanActual);
                }

                double nuevaExisPan = exisPanActual + req.CantidadProducida;

                string qUpdPan = "UPDATE Inventario SET Existencia = @Existencia WHERE Id = @Id";
                using (var cmdUpdP = new SqlCommand(qUpdPan, con, trans))
                {
                    cmdUpdP.Parameters.AddWithValue("@Existencia", nuevaExisPan);
                    cmdUpdP.Parameters.AddWithValue("@Id", req.IdProductoFinal);
                    await cmdUpdP.ExecuteNonQueryAsync();
                }

                // Registrar Kardex del Pan (Entrada por producción)
                string kardexPan = @"INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha) 
                                     VALUES (@IdProducto, 'ENTRADA', @Descripcion, @ExistenciaAntes, @ExistenciaDespues, @Fecha)";
                using (var cmdKPan = new SqlCommand(kardexPan, con, trans))
                {
                    cmdKPan.Parameters.AddWithValue("@IdProducto", req.IdProductoFinal);
                    cmdKPan.Parameters.AddWithValue("@Descripcion", $"ENTRADA POR HORNEADO/PRODUCCION LOTE: {folioLote}");
                    cmdKPan.Parameters.AddWithValue("@ExistenciaAntes", exisPanActual);
                    cmdKPan.Parameters.AddWithValue("@ExistenciaDespues", nuevaExisPan);
                    cmdKPan.Parameters.AddWithValue("@Fecha", fechaStr);
                    await cmdKPan.ExecuteNonQueryAsync();
                }

                // 3. Registrar en la tabla ProduccionLotes
                string insertLote = @"INSERT INTO ProduccionLotes (Folio, IdProductoFinal, CantidadProducida, Fecha, Usuario) 
                                     VALUES (@Folio, @IdProductoFinal, @CantidadProducida, @Fecha, @Usuario)";
                using (var cmdLote = new SqlCommand(insertLote, con, trans))
                {
                    cmdLote.Parameters.AddWithValue("@Folio", folioLote);
                    cmdLote.Parameters.AddWithValue("@IdProductoFinal", req.IdProductoFinal);
                    cmdLote.Parameters.AddWithValue("@CantidadProducida", req.CantidadProducida);
                    cmdLote.Parameters.AddWithValue("@Fecha", fechaStr);
                    cmdLote.Parameters.AddWithValue("@Usuario", string.IsNullOrEmpty(req.Usuario) ? "PANADERO" : req.Usuario);
                    await cmdLote.ExecuteNonQueryAsync();
                }

                trans.Commit();
                return (true, "Producción procesada correctamente.", folioLote);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                _logger.LogError(ex, "Error durante la transacción de registro de producción en SQL Server");
                return (false, $"Error al registrar lote: {ex.Message}", "");
            }
        }
        catch (Exception exCon)
        {
            _logger.LogError(exCon, "Error de conexión al procesar producción");
            return (false, $"Error de conexión con la base de datos: {exCon.Message}", "");
        }
    }

    /// <summary>
    /// Consulta el historial de lotes de producciÃ³n con filtros
    /// </summary>
    public async Task<List<ProduccionLoteConsultaDto>> ConsultarProduccionesAsync(DateTime? fDesde, DateTime? fHasta, string filtroTexto, string usuario, string estado)
    {
        var lista = new List<ProduccionLoteConsultaDto>();
        try
        {
            using var con = new SqlConnection(_sqlConn);
            await con.OpenAsync();

            string query = @"SELECT L.Id, L.Folio, L.Fecha, L.IdProductoFinal, 
                                   ISNULL(I.Nombre, 'Desconocido') AS ProductoFinal, 
                                   ISNULL(L.CantidadProducida, 0) AS CantidadProducida, 
                                   L.Usuario, 
                                   ISNULL(L.Cancelado, 0) AS Cancelado,
                                   L.FechaCancelado, 
                                   L.UsuarioCancelado
                            FROM ProduccionLotes L
                            LEFT JOIN Inventario I ON L.IdProductoFinal = I.Id
                            WHERE 1=1 ";

            if (fDesde.HasValue && fHasta.HasValue)
            {
                query += " AND L.Fecha >= @FechaDesde AND L.Fecha <= @FechaHasta ";
            }

            if (!string.IsNullOrWhiteSpace(filtroTexto))
            {
                query += " AND (L.Folio LIKE @Filtro OR L.IdProductoFinal LIKE @Filtro OR I.Nombre LIKE @Filtro) ";
            }

            if (!string.IsNullOrWhiteSpace(usuario) && usuario != "Todos")
            {
                query += " AND L.Usuario = @Usuario ";
            }

            if (estado == "Activos")
            {
                query += " AND ISNULL(L.Cancelado, 0) = 0 ";
            }
            else if (estado == "Cancelados")
            {
                query += " AND ISNULL(L.Cancelado, 0) = 1 ";
            }

            query += " ORDER BY L.Fecha DESC, L.Id DESC";

            using var cmd = new SqlCommand(query, con);
            if (fDesde.HasValue && fHasta.HasValue)
            {
                cmd.Parameters.AddWithValue("@FechaDesde", fDesde.Value);
                cmd.Parameters.AddWithValue("@FechaHasta", fHasta.Value);
            }
            if (!string.IsNullOrWhiteSpace(filtroTexto))
            {
                cmd.Parameters.AddWithValue("@Filtro", "%" + filtroTexto.Trim() + "%");
            }
            if (!string.IsNullOrWhiteSpace(usuario) && usuario != "Todos")
            {
                cmd.Parameters.AddWithValue("@Usuario", usuario);
            }

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                double cant = 0;
                if (reader["CantidadProducida"] != DBNull.Value)
                    double.TryParse(reader["CantidadProducida"].ToString(), out cant);

                DateTime? fecha = null;
                if (reader["Fecha"] != DBNull.Value)
                    fecha = Convert.ToDateTime(reader["Fecha"]);

                DateTime? fechaCanc = null;
                if (reader["FechaCancelado"] != DBNull.Value)
                    fechaCanc = Convert.ToDateTime(reader["FechaCancelado"]);

                lista.Add(new ProduccionLoteConsultaDto
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Folio = reader["Folio"]?.ToString() ?? "",
                    IdProductoFinal = reader["IdProductoFinal"]?.ToString() ?? "",
                    ProductoFinal = reader["ProductoFinal"]?.ToString() ?? "",
                    CantidadProducida = cant,
                    Fecha = fecha,
                    Usuario = reader["Usuario"]?.ToString() ?? "",
                    Cancelado = reader["Cancelado"] != DBNull.Value && Convert.ToBoolean(reader["Cancelado"]),
                    FechaCancelado = fechaCanc,
                    UsuarioCancelado = reader["UsuarioCancelado"]?.ToString() ?? ""
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al consultar historial de producciÃ³n");
            throw;
        }
        return lista;
    }

    /// <summary>
    /// Cancela un lote de producciÃ³n: revierte existencias de pan y materias primas, registra Kardex y marca el lote como Cancelado.
    /// </summary>
    public async Task<(bool Exito, string Mensaje)> CancelarProduccionLoteAsync(int idLote, string usuarioCancelacion)
    {
        try
        {
            using var con = new SqlConnection(_sqlConn);
            await con.OpenAsync();

            using var trans = con.BeginTransaction();
            try
            {
                string qCheck = "SELECT Cancelado, Folio, IdProductoFinal, CantidadProducida FROM ProduccionLotes WHERE Id = @Id";
                string folio = "";
                string idProd = "";
                double cantProd = 0;
                bool cancelado = false;

                using (var cmdChk = new SqlCommand(qCheck, con, trans))
                {
                    cmdChk.Parameters.AddWithValue("@Id", idLote);
                    using var rdr = await cmdChk.ExecuteReaderAsync();
                    if (await rdr.ReadAsync())
                    {
                        cancelado = rdr["Cancelado"] != DBNull.Value && Convert.ToBoolean(rdr["Cancelado"]);
                        folio = rdr["Folio"]?.ToString() ?? "";
                        idProd = rdr["IdProductoFinal"]?.ToString() ?? "";
                        if (rdr["CantidadProducida"] != DBNull.Value)
                            double.TryParse(rdr["CantidadProducida"].ToString(), out cantProd);
                    }
                    else
                    {
                        trans.Rollback();
                        return (false, "No se encontrÃ³ el lote especificado.");
                    }
                }

                if (cancelado)
                {
                    trans.Rollback();
                    return (false, "El lote ya se encuentra cancelado.");
                }

                string fechaStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                string nombreProd = "";
                double stockPanActual = 0;
                string qPan = "SELECT Nombre, Existencia FROM Inventario WHERE Id = @Id";
                using (var cmdPan = new SqlCommand(qPan, con, trans))
                {
                    cmdPan.Parameters.AddWithValue("@Id", idProd);
                    using var rdrPan = await cmdPan.ExecuteReaderAsync();
                    if (await rdrPan.ReadAsync())
                    {
                        nombreProd = rdrPan["Nombre"]?.ToString() ?? "";
                        if (rdrPan["Existencia"] != DBNull.Value)
                            double.TryParse(rdrPan["Existencia"].ToString(), out stockPanActual);
                    }
                }

                double nuevoStockPan = stockPanActual - cantProd;

                string qUpdPan = "UPDATE Inventario SET Existencia = @Existencia WHERE Id = @Id";
                using (var cmdUpdPan = new SqlCommand(qUpdPan, con, trans))
                {
                    cmdUpdPan.Parameters.AddWithValue("@Existencia", nuevoStockPan);
                    cmdUpdPan.Parameters.AddWithValue("@Id", idProd);
                    await cmdUpdPan.ExecuteNonQueryAsync();
                }

                string qKPan = @"INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha) 
                                VALUES (@IdProducto, 'SALIDA', @Descripcion, @ExistenciaAntes, @ExistenciaDespues, @Fecha)";
                using (var cmdK = new SqlCommand(qKPan, con, trans))
                {
                    cmdK.Parameters.AddWithValue("@IdProducto", idProd);
                    cmdK.Parameters.AddWithValue("@Descripcion", $"CANCELACION PRODUCCION LOTE: {folio} ({nombreProd})");
                    cmdK.Parameters.AddWithValue("@ExistenciaAntes", stockPanActual);
                    cmdK.Parameters.AddWithValue("@ExistenciaDespues", nuevoStockPan);
                    cmdK.Parameters.AddWithValue("@Fecha", fechaStr);
                    await cmdK.ExecuteNonQueryAsync();
                }

                var insumosAReintegrar = new List<(string IdInsumo, double Consumo, string Nombre)>();

                string qKInsumos = @"SELECT K.IdProducto, (K.ExistenciaAntes - K.ExistenciaDespues) AS Consumo, I.Nombre 
                                     FROM Kardex K 
                                     INNER JOIN Inventario I ON K.IdProducto = I.Id
                                     WHERE K.Descripcion LIKE @DescLote AND K.Tipo = 'SALIDA'";
                using (var cmdKI = new SqlCommand(qKInsumos, con, trans))
                {
                    cmdKI.Parameters.AddWithValue("@DescLote", $"CONSUMO PRODUCCION LOTE: {folio}%");
                    using var rdrKI = await cmdKI.ExecuteReaderAsync();
                    while (await rdrKI.ReadAsync())
                    {
                        string idIn = rdrKI["IdProducto"].ToString();
                        double cant = Convert.ToDouble(rdrKI["Consumo"]);
                        string nom = rdrKI["Nombre"].ToString();
                        insumosAReintegrar.Add((idIn, cant, nom));
                    }
                }

                if (insumosAReintegrar.Count == 0)
                {
                    string qReceta = @"SELECT R.IdInsumo, (R.CantidadRequerida * @CantProd) AS Consumo, I.Nombre 
                                       FROM ProductoReceta R
                                       INNER JOIN Inventario I ON R.IdInsumo = I.Id
                                       WHERE R.IdProductoFinal = @IdPan";
                    using (var cmdR = new SqlCommand(qReceta, con, trans))
                    {
                        cmdR.Parameters.AddWithValue("@CantProd", cantProd);
                        cmdR.Parameters.AddWithValue("@IdPan", idProd);
                        using var rdrR = await cmdR.ExecuteReaderAsync();
                        while (await rdrR.ReadAsync())
                        {
                            string idIn = rdrR["IdInsumo"].ToString();
                            double cant = Convert.ToDouble(rdrR["Consumo"]);
                            string nom = rdrR["Nombre"].ToString();
                            insumosAReintegrar.Add((idIn, cant, nom));
                        }
                    }
                }

                foreach (var ins in insumosAReintegrar)
                {
                    double stockInsActual = 0;
                    string qStk = "SELECT Existencia FROM Inventario WHERE Id = @Id";
                    using (var cmdS = new SqlCommand(qStk, con, trans))
                    {
                        cmdS.Parameters.AddWithValue("@Id", ins.IdInsumo);
                        var objS = await cmdS.ExecuteScalarAsync();
                        if (objS != null && objS != DBNull.Value)
                            double.TryParse(objS.ToString(), out stockInsActual);
                    }

                    double nuevoStockIns = stockInsActual + ins.Consumo;

                    string qUpdIn = "UPDATE Inventario SET Existencia = @Existencia WHERE Id = @Id";
                    using (var cmdUpdIn = new SqlCommand(qUpdIn, con, trans))
                    {
                        cmdUpdIn.Parameters.AddWithValue("@Existencia", nuevoStockIns);
                        cmdUpdIn.Parameters.AddWithValue("@Id", ins.IdInsumo);
                        await cmdUpdIn.ExecuteNonQueryAsync();
                    }

                    string qKIn = @"INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha) 
                                   VALUES (@IdProducto, 'ENTRADA', @Descripcion, @ExistenciaAntes, @ExistenciaDespues, @Fecha)";
                    using (var cmdKIn = new SqlCommand(qKIn, con, trans))
                    {
                        cmdKIn.Parameters.AddWithValue("@IdProducto", ins.IdInsumo);
                        cmdKIn.Parameters.AddWithValue("@Descripcion", $"CANCELACION CONSUMO PRODUCCION LOTE: {folio} ({nombreProd})");
                        cmdKIn.Parameters.AddWithValue("@ExistenciaAntes", stockInsActual);
                        cmdKIn.Parameters.AddWithValue("@ExistenciaDespues", nuevoStockIns);
                        cmdKIn.Parameters.AddWithValue("@Fecha", fechaStr);
                        await cmdKIn.ExecuteNonQueryAsync();
                    }
                }

                string qUpdLote = @"UPDATE ProduccionLotes 
                                    SET Cancelado = 1, FechaCancelado = GETDATE(), UsuarioCancelado = @Usuario 
                                    WHERE Id = @Id";
                using (var cmdCanc = new SqlCommand(qUpdLote, con, trans))
                {
                    cmdCanc.Parameters.AddWithValue("@Usuario", string.IsNullOrEmpty(usuarioCancelacion) ? "SISTEMA" : usuarioCancelacion);
                    cmdCanc.Parameters.AddWithValue("@Id", idLote);
                    await cmdCanc.ExecuteNonQueryAsync();
                }

                trans.Commit();
                return (true, "Lote cancelado correctamente y existencias revertidas.");
            }
            catch (Exception ex)
            {
                trans.Rollback();
                _logger.LogError(ex, "Error al cancelar lote de producciÃ³n");
                return (false, $"Error al cancelar lote: {ex.Message}");
            }
        }
        catch (Exception exCon)
        {
            _logger.LogError(exCon, "Error de conexiÃ³n al cancelar producciÃ³n");
            return (false, $"Error de conexiÃ³n con la base de datos: {exCon.Message}");
        }
    }
}