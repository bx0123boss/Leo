using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PuntoVentaWeb.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuntoVentaWeb.Services
{
    public class ConfiguracionService
    {
        private readonly string _connectionString;

        public ConfiguracionService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SQL");
        }

        // --- MÉTODOS PARA CAMPOS DINÁMICOS ---

        public async Task<List<CotizacionCampoConfig>> ObtenerCamposCotizacion()
        {
            var lista = new List<CotizacionCampoConfig>();
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT Id, NombreEtiqueta, TipoDato, Orden, Activo FROM CotizacionCamposConfig ORDER BY Orden", conn);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new CotizacionCampoConfig
                        {
                            Id = (int)reader["Id"],
                            NombreEtiqueta = reader["NombreEtiqueta"].ToString(),
                            TipoDato = reader["TipoDato"].ToString(),
                            Orden = (int)reader["Orden"],
                            Activo = (bool)reader["Activo"]
                        });
                    }
                }
            }
            return lista;
        }

        public async Task GuardarCampo(CotizacionCampoConfig campo)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string query;

                if (campo.Id == 0) // Insertar
                {
                    query = "INSERT INTO CotizacionCamposConfig (NombreEtiqueta, TipoDato, Orden, Activo) VALUES (@Nombre, @Tipo, @Orden, @Activo)";
                }
                else // Actualizar
                {
                    query = "UPDATE CotizacionCamposConfig SET NombreEtiqueta=@Nombre, TipoDato=@Tipo, Orden=@Orden, Activo=@Activo WHERE Id=@Id";
                }

                var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Nombre", campo.NombreEtiqueta);
                cmd.Parameters.AddWithValue("@Tipo", campo.TipoDato);
                cmd.Parameters.AddWithValue("@Orden", campo.Orden);
                cmd.Parameters.AddWithValue("@Activo", campo.Activo);
                if (campo.Id != 0) cmd.Parameters.AddWithValue("@Id", campo.Id);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EliminarCampo(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("DELETE FROM CotizacionCamposConfig WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                await cmd.ExecuteNonQueryAsync();
            }
        }


        public async Task<ConfiguracionApariencia> ObtenerColores()
        {
            var config = new ConfiguracionApariencia();
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                // Siempre traemos el primer registro o valores por defecto
                var cmd = new SqlCommand("SELECT TOP 1 Id, ColorPrimario, ColorSecundario, DiasValidez FROM ConfiguracionApariencia", conn);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        config.Id = (int)reader["Id"];
                        config.ColorPrimario = reader["ColorPrimario"].ToString();
                        config.ColorSecundario = reader["ColorSecundario"].ToString();

                        if (reader["DiasValidez"] != DBNull.Value)
                            config.DiasValidez = (int)reader["DiasValidez"];
                    }
                }
            }
            return config;
        }

        public async Task GuardarColores(ConfiguracionApariencia config)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                // Actualizamos siempre el registro existente o insertamos si está vacío
                string query = @"
                    MERGE ConfiguracionApariencia AS target
                    USING (SELECT @Id AS Id) AS source
                    ON (target.Id = source.Id)
                    WHEN MATCHED THEN
                        UPDATE SET ColorPrimario = @C1, ColorSecundario = @C2, DiasValidez = @Dias, UltimaModificacion = GETDATE()
                    WHEN NOT MATCHED THEN
                        INSERT (ColorPrimario, ColorSecundario, DiasValidez) VALUES (@C1, @C2, @Dias);";

                var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", config.Id == 0 ? 1 : config.Id);
                cmd.Parameters.AddWithValue("@C1", config.ColorPrimario);
                cmd.Parameters.AddWithValue("@C2", config.ColorSecundario);
                cmd.Parameters.AddWithValue("@Dias", config.DiasValidez); //

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}