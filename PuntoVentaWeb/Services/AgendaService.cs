using Microsoft.Data.SqlClient;
using PuntoVentaWeb.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PuntoVentaWeb.Services
{
    public class AgendaService
    {
        private readonly string _connectionString;

        public AgendaService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SQL") ?? "";
        }

        public async Task<List<AgendaApartadoDto>> ObtenerApartadosPorMesAsync(int anio, int mes)
        {
            var lista = new List<AgendaApartadoDto>();
            var inicioMes = new DateTime(anio, mes, 1);
            var finMes = inicioMes.AddMonths(1).AddDays(-1).Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            // 1. Consultar cabecera de Apartados en el rango de FechaEntrega
            string sqlApartados = @"
                SELECT Id, MontoTotal, Fecha, Folio, IdCliente, NombreCliente, Abono, Restante, FechaEntrega
                FROM dbo.Apartados
                WHERE FechaEntrega >= @Inicio AND FechaEntrega <= @Fin
                ORDER BY FechaEntrega ASC, Folio ASC;";

            using (var cmd = new SqlCommand(sqlApartados, conn))
            {
                cmd.Parameters.AddWithValue("@Inicio", inicioMes);
                cmd.Parameters.AddWithValue("@Fin", finMes);

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    string montoStr = reader["MontoTotal"]?.ToString() ?? "0";
                    string limpio = Regex.Replace(montoStr, @"[^\d.-]", "");
                    decimal.TryParse(limpio, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal montoTotal);

                    lista.Add(new AgendaApartadoDto
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Folio = reader["Folio"]?.ToString() ?? "",
                        IdCliente = reader["IdCliente"] != DBNull.Value ? Convert.ToInt32(reader["IdCliente"]) : 0,
                        NombreCliente = reader["NombreCliente"]?.ToString() ?? "SIN CLIENTE",
                        FechaRegistro = reader["Fecha"] != DBNull.Value ? Convert.ToDateTime(reader["Fecha"]) : DateTime.MinValue,
                        FechaEntrega = reader["FechaEntrega"] != DBNull.Value ? Convert.ToDateTime(reader["FechaEntrega"]) : null,
                        MontoTotal = montoTotal,
                        Abono = reader["Abono"] != DBNull.Value ? Convert.ToDecimal(reader["Abono"]) : 0m,
                        Restante = reader["Restante"] != DBNull.Value ? Convert.ToDecimal(reader["Restante"]) : 0m
                    });
                }
            }

            if (lista.Count == 0) return lista;

            // 2. Consultar los productos asociados en dbo.VentasApartados por FolioVenta
            var folios = lista.Select(x => x.Folio).Distinct().ToList();
            string sqlDetalle = @"
                SELECT Id, FolioVenta, IdProducto, Cantidad, Producto, MontoTotal, Estatus
                FROM dbo.VentasApartados
                WHERE FolioVenta IN (" + string.Join(",", folios.Select((_, idx) => $"@f{idx}")) + ");";

            using (var cmdDet = new SqlCommand(sqlDetalle, conn))
            {
                for (int i = 0; i < folios.Count; i++)
                {
                    cmdDet.Parameters.AddWithValue($"@f{i}", folios[i]);
                }

                using var readerDet = await cmdDet.ExecuteReaderAsync();
                while (await readerDet.ReadAsync())
                {
                    string folioVenta = readerDet["FolioVenta"]?.ToString() ?? "";
                    var apartado = lista.FirstOrDefault(a => a.Folio == folioVenta);
                    if (apartado != null)
                    {
                        string mStr = readerDet["MontoTotal"]?.ToString() ?? "0";
                        string mLimpio = Regex.Replace(mStr, @"[^\d.-]", "");
                        decimal.TryParse(mLimpio, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal mProd);

                        apartado.Productos.Add(new DetalleApartadoDto
                        {
                            Id = Convert.ToInt32(readerDet["Id"]),
                            IdProducto = readerDet["IdProducto"]?.ToString() ?? "",
                            Producto = readerDet["Producto"]?.ToString() ?? "",
                            Cantidad = readerDet["Cantidad"]?.ToString() ?? "1",
                            MontoTotal = mProd,
                            Estatus = readerDet["Estatus"]?.ToString() ?? ""
                        });

                        if (!string.IsNullOrEmpty(readerDet["Estatus"]?.ToString()))
                        {
                            apartado.Estatus = readerDet["Estatus"]!.ToString()!;
                        }
                    }
                }
            }

            return lista;
        }
    }
}