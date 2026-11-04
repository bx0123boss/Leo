namespace PuntoVentaWeb.Services;

using PuntoVentaWeb.Models;
using Microsoft.Data.SqlClient;

public class ClienteService
{
    static string nombrePC = Environment.MachineName;

    // Conexión SQL Server
    public static string _sqlString = $@"Server={nombrePC}\SQLEXPRESS;Database=PuntoDeVenta;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True;";

    /// <summary>
    /// Guarda un nuevo cliente directamente en la base de datos de SQL Server.
    /// </summary>
    public async Task<bool> GuardarCliente(Cliente cliente)
    {
        try
        {
            using (var con = new SqlConnection(_sqlString))
            {
                await con.OpenAsync();

                // 1. INSERTAR DATOS
                string query = @"INSERT INTO Clientes 
                            (Nombre, Telefono, Direccion, Referencia, RFC, Correo, Adeudo, Limite, UltimoPago, Estatus, CP) 
                            VALUES (@Nombre, @Telefono, @Direccion, @Referencia, @RFC, @Correo, @Adeudo, @Limite, @UltimoPago, @Estatus, @CP)";

                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Nombre", cliente.Nombre ?? "");
                    cmd.Parameters.AddWithValue("@Telefono", cliente.Telefono ?? "");
                    cmd.Parameters.AddWithValue("@Direccion", cliente.Direccion ?? "");
                    cmd.Parameters.AddWithValue("@Referencia", cliente.Referencia ?? "");
                    cmd.Parameters.AddWithValue("@RFC", string.IsNullOrEmpty(cliente.RFC) ? "XAXX010101000" : cliente.RFC);
                    cmd.Parameters.AddWithValue("@Correo", cliente.Correo ?? "");
                    cmd.Parameters.AddWithValue("@Adeudo", 0);
                    cmd.Parameters.AddWithValue("@Limite", cliente.Limite);
                    cmd.Parameters.AddWithValue("@UltimoPago", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Estatus", "Activo");
                    cmd.Parameters.AddWithValue("@CP", cliente.CP ?? "");

                    await cmd.ExecuteNonQueryAsync();

                    // 2. RECUPERAR EL ID GENERADO
                    cmd.CommandText = "SELECT SCOPE_IDENTITY()";
                    cmd.Parameters.Clear();

                    var resultado = await cmd.ExecuteScalarAsync();
                    if (resultado != null && resultado != DBNull.Value)
                    {
                        cliente.Id = Convert.ToInt32(resultado);
                    }

                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al guardar cliente en SQL Server: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Busca clientes en SQL Server por Nombre o RFC
    /// </summary>
    public async Task<List<Cliente>> BuscarClientes(string busqueda)
    {
        var lista = new List<Cliente>();

        try
        {
            using (var con = new SqlConnection(_sqlString))
            {
                await con.OpenAsync();

                string query = @"SELECT TOP 50 Id, Nombre, RFC, Direccion, CP, Telefono, Correo, Limite, Estatus, Referencia
                                 FROM Clientes 
                                 WHERE Nombre LIKE @busqueda OR RFC LIKE @busqueda
                                 ORDER BY Nombre";

                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@busqueda", "%" + busqueda + "%");

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(MapearReaderACliente(reader));
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error buscando clientes: {ex.Message}");
        }

        return lista;
    }

    /// <summary>
    /// Obtiene los últimos clientes registrados
    /// </summary>
    public async Task<List<Cliente>> ObtenerUltimosClientes()
    {
        var lista = new List<Cliente>();
        try
        {
            using (var con = new SqlConnection(_sqlString))
            {
                await con.OpenAsync();
                string query = "SELECT TOP 20 Id, Nombre, RFC, Direccion, CP, Telefono, Correo, Limite, Estatus, Referencia FROM Clientes ORDER BY Id DESC";

                using (var cmd = new SqlCommand(query, con))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(MapearReaderACliente(reader));
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error obteniendo últimos clientes: {ex.Message}");
        }
        return lista;
    }

    /// <summary>
    /// Obtiene un cliente específico por su ID desde SQL Server.
    /// </summary>
    public async Task<Cliente> ObtenerClientePorId(int id)
    {
        try
        {
            using (var con = new SqlConnection(_sqlString))
            {
                await con.OpenAsync();

                string query = @"SELECT Id, Nombre, RFC, Direccion, CP, Telefono, Correo, Limite, Estatus, Referencia 
                                 FROM Clientes 
                                 WHERE Id = @id";

                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapearReaderACliente(reader);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener cliente por ID: {ex.Message}");
        }

        return null;
    }

    private Cliente MapearReaderACliente(System.Data.Common.DbDataReader reader)
    {
        return new Cliente
        {
            Id = Convert.ToInt32(reader["Id"]),
            Nombre = reader["Nombre"] != DBNull.Value ? reader["Nombre"].ToString() : "",
            RFC = reader["RFC"] != DBNull.Value ? reader["RFC"].ToString() : "",
            Direccion = reader["Direccion"] != DBNull.Value ? reader["Direccion"].ToString() : "",
            Telefono = reader["Telefono"] != DBNull.Value ? reader["Telefono"].ToString() : "",
            Correo = HasColumn(reader, "Correo") && reader["Correo"] != DBNull.Value ? reader["Correo"].ToString() : "",
            Referencia = HasColumn(reader, "Referencia") && reader["Referencia"] != DBNull.Value ? reader["Referencia"].ToString() : "",
            Limite = HasColumn(reader, "Limite") && reader["Limite"] != DBNull.Value ? Convert.ToDecimal(reader["Limite"]) : 0,
            Estatus = HasColumn(reader, "Estatus") && reader["Estatus"] != DBNull.Value ? reader["Estatus"].ToString() : "Activo",
            CP = HasColumn(reader, "CP") && reader["CP"] != DBNull.Value ? reader["CP"].ToString() : "00000"
        };
    }

    private bool HasColumn(System.Data.Common.DbDataReader reader, string columnName)
    {
        for (int i = 0; i < reader.FieldCount; i++)
        {
            if (reader.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                return true;
        }
        return false;
    }
}
