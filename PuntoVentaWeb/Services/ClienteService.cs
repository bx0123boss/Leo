namespace PuntoVentaWeb.Services;

using PuntoVentaWeb.Models;
using System.Data.Odbc;      // Para Access
using System.Data.SqlClient; // Para SQL Server (por si migras en el futuro)

public class ClienteService
{
    static string nombrePC = Environment.MachineName;

    // Conexión SQL (Reservada para módulos futuros o si migras la tabla Clientes)
    public static string _sqlString = $@"Server={nombrePC}\SQLEXPRESS;Database=PuntoDeVenta;Integrated Security=True;MultipleActiveResultSets=True;";

    // Conexión Access (Donde viven los clientes actualmente)
    private string _accessString = @"Driver={Microsoft Access Driver (*.mdb, *.accdb)};Dbq=C:\Jaeger Soft\Jaeger.accdb;Pwd=75941232;";

    /// <summary>
    /// Guarda un nuevo cliente directamente en la base de datos de Access para que el sistema de escritorio lo vea.
    /// </summary>
    public async Task<bool> GuardarCliente(Cliente cliente)
    {
        try
        {
            using (var con = new OdbcConnection(_accessString))
            {
                await con.OpenAsync();

                // 1. INSERTAR DATOS
                string query = @"INSERT INTO Clientes 
                            (Nombre, Telefono, Direccion, Referencia, RFC, Correo, Adeudo, Limite, UltimoPago, Estatus) 
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                using (var cmd = new OdbcCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("?", cliente.Nombre);
                    cmd.Parameters.AddWithValue("?", cliente.Telefono ?? "");
                    cmd.Parameters.AddWithValue("?", cliente.Direccion ?? "");
                    cmd.Parameters.AddWithValue("?", cliente.Referencia ?? "");
                    cmd.Parameters.AddWithValue("?", string.IsNullOrEmpty(cliente.RFC) ? "XAXX010101000" : cliente.RFC);
                    cmd.Parameters.AddWithValue("?", cliente.Correo ?? "");
                    cmd.Parameters.AddWithValue("?", 0);
                    cmd.Parameters.AddWithValue("?", cliente.Limite);
                    cmd.Parameters.AddWithValue("?", DateTime.Now);
                    cmd.Parameters.AddWithValue("?", "Activo");

                    await cmd.ExecuteNonQueryAsync();

                    // 2. RECUPERAR EL ID GENERADO (¡Esto es lo nuevo!)
                    // Reusamos el mismo comando y conexión abierta para obtener el ID inmediato
                    cmd.CommandText = "SELECT @@IDENTITY";
                    cmd.Parameters.Clear(); // Limpiamos parámetros anteriores

                    var resultado = await cmd.ExecuteScalarAsync();
                    if (resultado != DBNull.Value)
                    {
                        // Asignamos el ID real al objeto que tenemos en memoria
                        cliente.Id = Convert.ToInt32(resultado);
                    }

                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al guardar cliente en Access: {ex.Message}");
            return false;
        }
    }
    /// <summary>
    /// Busca clientes en Access por Nombre o RFC (Similar al buscador de tu cotización)
    /// </summary>
    public async Task<List<Cliente>> BuscarClientes(string busqueda)
    {
        var lista = new List<Cliente>();

        try
        {
            using (var con = new OdbcConnection(_accessString))
            {
                await con.OpenAsync();

                string query = @"SELECT TOP 50 Id, Nombre, RFC, Direccion, Telefono, Correo, Limite, Estatus, Referencia
                                 FROM Clientes 
                                 WHERE Nombre LIKE ? OR RFC LIKE ?
                                 ORDER BY Nombre";

                using (var cmd = new OdbcCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("?", "%" + busqueda + "%");
                    cmd.Parameters.AddWithValue("?", "%" + busqueda + "%");

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
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
    /// Obtiene los últimos clientes registrados (útil para la carga inicial de la página)
    /// </summary>
    public async Task<List<Cliente>> ObtenerUltimosClientes()
    {
        var lista = new List<Cliente>();
        try
        {
            using (var con = new OdbcConnection(_accessString))
            {
                await con.OpenAsync();
                // En Access a veces no hay campo 'FechaRegistro', usamos ID descendente como aproximación
                string query = "SELECT TOP 20 Id, Nombre, RFC, Direccion, Telefono, Correo, Limite, Estatus, Referencia FROM Clientes ORDER BY Id DESC";

                using (var cmd = new OdbcCommand(query, con))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
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
    /// Obtiene un cliente específico por su ID desde Access.
    /// </summary>
    public async Task<Cliente> ObtenerClientePorId(int id)
    {
        try
        {
            using (var con = new OdbcConnection(_accessString))
            {
                await con.OpenAsync();

                string query = @"SELECT Id, Nombre, RFC, Direccion, Telefono, Correo, Limite, Estatus, Referencia 
                                 FROM Clientes 
                                 WHERE Id = ?";

                using (var cmd = new OdbcCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("?", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.Read())
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

        return null; // Retorna null si no lo encuentra o hay error
    }
    // Helper para no repetir código de mapeo
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
            Estatus = HasColumn(reader, "Estatus") && reader["Estatus"] != DBNull.Value ? reader["Estatus"].ToString() : "Activo"
        };
    }

    // Método auxiliar seguro para verificar si una columna existe antes de leerla
    // (Útil porque Access a veces falla si pides columnas que no están en el SELECT)
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