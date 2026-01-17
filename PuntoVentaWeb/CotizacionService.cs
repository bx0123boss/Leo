namespace PuntoVentaWeb;

using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using PuntoVentaWeb.Models;
public class CotizacionService
{
    // Cadena para leer el inventario actual (Access)
    // NOTA: En entorno web, asegúrate que el usuario del IIS tenga permisos a esa ruta de red o carpeta C:
    private string _accessString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Jaeger Soft\Jaeger.accdb; Jet OLEDB:Database Password=75941232";

    // Cadena para guardar la cotización nueva (SQL Server)
    private string _sqlString = "Server=LOCALHOST;Database=JoyeriaWeb;User Id=sa;Password=tu_password;";
    // Dentro de CotizacionService.cs

    public async Task<List<Producto>> BuscarProductos(string busqueda)
    {
        var lista = new List<Producto>();

        using (var con = new OleDbConnection(_accessString))
        {
            await con.OpenAsync();

            // Consulta ajustada a tus columnas reales
            // Usamos TOP 20 para no saturar la red si hay muchos resultados
            string query = @"SELECT TOP 20 
                            Id, 
                            Nombre, 
                            PrecioventaMayoreo, 
                            precioventa, 
                            Existencia, 
                            [Límite], 
                            [Categoría], 
                            Especial, 
                            Iva, 
                            Unidad 
                        FROM Inventario 
                        WHERE Nombre LIKE @busqueda OR CStr(Id) LIKE @busqueda";
            // Nota: CStr(Id) convierte el ID a texto para poder buscar "101"

            using (var cmd = new OleDbCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@busqueda", "%" + busqueda + "%");

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Producto
                        {
                            Id = (reader["Id"]).ToString(),
                            Nombre = reader["Nombre"].ToString(),
                            PrecioventaMayoreo = reader["PrecioventaMayoreo"] != DBNull.Value ? Convert.ToDecimal(reader["PrecioventaMayoreo"]) : 0,
                            Precioventa = reader["precioventa"] != DBNull.Value ? Convert.ToDecimal(reader["precioventa"]) : 0,
                            Existencia = reader["Existencia"] != DBNull.Value ? Convert.ToDouble(reader["Existencia"]) : 0,
                            Limite = reader["Límite"] != DBNull.Value ? Convert.ToDouble(reader["Límite"]) : 0,
                            Categoria = reader["Categoría"].ToString(),
                            Especial =Convert.ToDecimal(reader["Especial"]),
                            Iva = reader["Iva"] != DBNull.Value ? (reader["Iva"]).ToString() : "0",
                            Unidad = reader["Unidad"].ToString()
                        });
                    }
                }
            }
        }
        return lista;
    }

    // 2. GUARDAR COTIZACIÓN EN SQL SERVER
    public async Task GuardarCotizacion(Cotizacion cotizacion)
    {
        using (var con = new SqlConnection(_sqlString))
        {
            await con.OpenAsync();
            using (var transaction = con.BeginTransaction())
            {
                try
                {
                    // Insertar Cabecera
                    string sqlHead = "INSERT INTO Cotizaciones (Fecha, ClienteNombre, Total) OUTPUT INSERTED.Id VALUES (@Fecha, @Cliente, @Total)";
                    int newId;

                    using (var cmd = new SqlCommand(sqlHead, con, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Fecha", cotizacion.Fecha);
                        cmd.Parameters.AddWithValue("@Cliente", cotizacion.ClienteNombre ?? "Público General");
                        cmd.Parameters.AddWithValue("@Total", cotizacion.Total);
                        newId = (int)await cmd.ExecuteScalarAsync();
                    }

                    // Insertar Detalles
                    string sqlDet = "INSERT INTO DetalleCotizacion (CotizacionId, ProductoCodigo, Descripcion, Cantidad, PrecioUnitario, Importe) VALUES (@Id, @Cod, @Desc, @Cant, @Prec, @Imp)";

                    foreach (var det in cotizacion.Detalles)
                    {
                        using (var cmd = new SqlCommand(sqlDet, con, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Id", newId);
                            cmd.Parameters.AddWithValue("@Cod", det.ProductoCodigo);
                            cmd.Parameters.AddWithValue("@Desc", det.Descripcion);
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
}
