namespace PuntoVentaWeb.Services;

using PuntoVentaWeb.Models;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
public class CotizacionService
{
    static string nombrePC = Environment.MachineName;

    public static string _sqlString = $@"Server={nombrePC}\SQLEXPRESS;Database=PuntoDeVenta;Integrated Security=True;MultipleActiveResultSets=True;";
    private string _accessString = @"Driver={Microsoft Access Driver (*.mdb, *.accdb)};Dbq=C:\Jaeger Soft\Jaeger.accdb;Pwd=75941232;";

    public async Task<List<Cotizacion>> ObtenerHistorial()
    {
        var lista = new List<Cotizacion>();

        using (var con = new SqlConnection(_sqlString))
        {
            await con.OpenAsync();

            // Seleccionamos directamente el Total y Observaciones de la tabla
            string sql = @"SELECT TOP 100 
                        Id, 
                        Fecha, 
                        ClienteNombre, 
                        Total, 
                        Observaciones 
                       FROM Cotizaciones 
                       ORDER BY Id DESC";

            using (var cmd = new SqlCommand(sql, con))
            {
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Cotizacion
                        {
                            Id = (int)reader["Id"],
                            Fecha = (DateTime)reader["Fecha"],
                            ClienteNombre = reader["ClienteNombre"] != DBNull.Value ? reader["ClienteNombre"].ToString() : "Público",
                            Total = reader["Total"] != DBNull.Value ? Convert.ToDecimal(reader["Total"]) : 0,
                            Observaciones = reader["Observaciones"] != DBNull.Value ? reader["Observaciones"].ToString() : ""
                        });
                    }
                }
            }
        }
        return lista;
    }

    public async Task<List<Producto>> BuscarProductos(string busqueda)
    {
        var lista = new List<Producto>();

        // Usamos OdbcConnection en vez de OleDbConnection
        using (var con = new OdbcConnection(_accessString))
        {
            await con.OpenAsync();

            // La consulta es igual, pero ODBC usa '?' en vez de '@parametro' a veces. 
            // Sin embargo, System.Data.Odbc soporta nombres en muchos casos. 
            // Si falla, cambiaremos a '?'. Probemos así primero:
            string query = @"SELECT TOP 20 
                                    Id, Nombre, PrecioventaMayoreo, precioventa, Existencia 
                                FROM Inventario 
                                WHERE Nombre LIKE ? OR Id LIKE ?";

            using (var cmd = new OdbcCommand(query, con))
            {
                cmd.Parameters.AddWithValue("?", "%" + busqueda + "%");
                cmd.Parameters.AddWithValue("?", "%" + busqueda + "%");

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Producto
                        {
                            Id = reader["Id"].ToString(), // Access a veces devuelve int, el ToString() lo asegura
                            Nombre = reader["Nombre"].ToString(),
                            PrecioventaMayoreo = reader["PrecioventaMayoreo"] != DBNull.Value ? Convert.ToDecimal(reader["PrecioventaMayoreo"]) : 0,
                            Precioventa = reader["precioventa"] != DBNull.Value ? Convert.ToDecimal(reader["precioventa"]) : 0,
                            Existencia = reader["Existencia"] != DBNull.Value ? Convert.ToDouble(reader["Existencia"]) : 0,
                            // Agrega el resto de campos si los necesitas
                        });
                    }
                }
            }
        }
        return lista;
    }

    public async Task<List<Cliente>> BuscarClientes(string busqueda)
    {
        var lista = new List<Cliente>();

        using (var con = new OdbcConnection(_accessString))
        {
            await con.OpenAsync();
            string query = @"SELECT TOP 20 Id, Nombre, RFC, Direccion, Telefono 
                                 FROM Clientes 
                                 WHERE Nombre LIKE ? OR RFC LIKE ?";

            using (var cmd = new OdbcCommand(query, con))
            {
                cmd.Parameters.AddWithValue("?", "%" + busqueda + "%");
                cmd.Parameters.AddWithValue("?", "%" + busqueda + "%");

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Cliente
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nombre = reader["Nombre"].ToString(),
                            RFC = reader["RFC"].ToString(),
                            Direccion = reader["Direccion"].ToString(),
                            Telefono = reader["Telefono"].ToString()
                        });
                    }
                }
            }
        }
        return lista;
    }
    public async Task GuardarCotizacion(Cotizacion cotizacion)
    {
        using (var con = new SqlConnection(_sqlString))
        {
            await con.OpenAsync();
            using (var transaction = con.BeginTransaction())
            {
                try
                {
                    string sqlHead = @"INSERT INTO Cotizaciones (Fecha, ClienteNombre, Total, Observaciones) 
                                   OUTPUT INSERTED.Id 
                                   VALUES (@Fecha, @Cliente, @Total, @Observaciones)";

                    int newId;

                    using (var cmd = new SqlCommand(sqlHead, con, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Fecha", cotizacion.Fecha);
                        cmd.Parameters.AddWithValue("@Cliente", cotizacion.ClienteNombre ?? "Público General");
                        cmd.Parameters.AddWithValue("@Total", cotizacion.Total);
                        cmd.Parameters.AddWithValue("@Observaciones",
                            string.IsNullOrEmpty(cotizacion.Observaciones) ? DBNull.Value : cotizacion.Observaciones);

                        newId = (int)await cmd.ExecuteScalarAsync();
                    }


                    string sqlDet = "INSERT INTO DetalleCotizacion (CotizacionId, ProductoCodigo, Descripcion, Cantidad, PrecioUnitario, Importe) VALUES (@Id, @Cod, @Desc, @Cant, @Prec, @Imp)";
                    foreach (var det in cotizacion.Detalles)
                    {
                        using (var cmd = new SqlCommand(sqlDet, con, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Id", newId);
                            cmd.Parameters.AddWithValue("@Cod", det.ProductoCodigo ?? "");
                            cmd.Parameters.AddWithValue("@Desc", det.Descripcion ?? "");
                            cmd.Parameters.AddWithValue("@Cant", det.Cantidad);
                            cmd.Parameters.AddWithValue("@Prec", det.PrecioUnitario);
                            cmd.Parameters.AddWithValue("@Imp", det.Importe);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }

    public async Task<Cotizacion> ObtenerCotizacionPorId(int id)
    {
        Cotizacion cotizacion = null;

        using (var con = new SqlConnection(_sqlString))
        {
            await con.OpenAsync();

            // 1. Obtener la Cabecera
            string sqlHead = "SELECT * FROM Cotizaciones WHERE Id = @Id";
            using (var cmd = new SqlCommand(sqlHead, con))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        cotizacion = new Cotizacion
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Fecha = Convert.ToDateTime(reader["Fecha"]),
                            ClienteNombre = reader["ClienteNombre"].ToString(),
                            // CAMBIO: Recuperar el ID y las Observaciones
                            ClienteId = reader.GetSchemaTable().Select("ColumnName = 'ClienteId'").Length > 0 && reader["ClienteId"] != DBNull.Value
                                        ? Convert.ToInt32(reader["ClienteId"]) : 0,
                            Observaciones = reader.GetSchemaTable().Select("ColumnName = 'Observaciones'").Length > 0 && reader["Observaciones"] != DBNull.Value
                                            ? reader["Observaciones"].ToString() : ""
                        };

                        // Recuperamos el total si existe columna
                        if (reader.HasRows && reader.GetSchemaTable().Select("ColumnName = 'Total'").Length > 0 && reader["Total"] != DBNull.Value)
                        {
                            cotizacion.Total = Convert.ToDecimal(reader["Total"]);
                        }
                    }
                }
            }

            if (cotizacion != null)
            {
                // ... (El bloque de cargar detalles queda igual)
                string sqlDet = "SELECT * FROM DetalleCotizacion WHERE CotizacionId = @Id";
                using (var cmd = new SqlCommand(sqlDet, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            cotizacion.Detalles.Add(new DetalleCotizacion
                            {
                                ProductoCodigo = reader["ProductoCodigo"].ToString(),
                                Descripcion = reader["Descripcion"].ToString(),
                                Cantidad = Convert.ToInt32(reader["Cantidad"]),
                                PrecioUnitario = Convert.ToDecimal(reader["PrecioUnitario"])
                            });
                        }
                    }
                }
            }
        }
        return cotizacion;
    }

    // Dentro de CotizacionService.cs

    public async Task<ConfiguracionNegocio> ObtenerConfiguracion()
    {
        var config = new ConfiguracionNegocio();

        try
        {
            using (var con = new OdbcConnection(_accessString))
            {
                await con.OpenAsync();

                string query = "SELECT TOP 1 Lugar, DatosTicket, FooterCorreoCotizacion, ConIva, LogoPath, CorreoEmisor,  PasswordEmisor FROM Configuracion";

                using (var cmd = new OdbcCommand(query, con))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            config.NombreLugar = reader["Lugar"].ToString();

                            string datosRaw = reader["DatosTicket"].ToString();
                            if (!string.IsNullOrEmpty(datosRaw))
                                config.DatosTicket = datosRaw.Split('|');

                            string pieRaw = reader["FooterCorreoCotizacion"].ToString();
                            if (!string.IsNullOrEmpty(pieRaw))
                                config.PieDeTicket = pieRaw.Split('|');

                            if (reader["ConIva"] != DBNull.Value)
                                config.ConIva = Convert.ToBoolean(reader["ConIva"]);

                            config.LogoPath = reader["LogoPath"].ToString();
                            config.CorreoEmisor = reader["CorreoEmisor"] != DBNull.Value ? reader["CorreoEmisor"].ToString() : "";
                            config.PasswordEmisor = reader["PasswordEmisor"] != DBNull.Value ? reader["PasswordEmisor"].ToString() : "";
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error leyendo configuración: " + ex.Message);
        }

        return config;
    }
    // En CotizacionService.cs
    public async Task<int> ObtenerUltimoFolio()
    {
        int folio = 0;
        try
        {
            using (var con = new SqlConnection(_sqlString)) // O OdbcConnection si es Access
            {
                await con.OpenAsync();
                // Para SQL Server:
                string query = "SELECT ISNULL(MAX(Id), 0) FROM Cotizaciones";
                // Para Access: "SELECT MAX(Id) FROM Cotizaciones" (y manejar nulos)

                using (var cmd = new SqlCommand(query, con))
                {
                    var result = await cmd.ExecuteScalarAsync();
                    if (result != null && int.TryParse(result.ToString(), out int f))
                    {
                        folio = f;
                    }
                }
            }
        }
        catch { /* Ignorar errores, devolverá 0 */ }
        return folio;
    }

}
