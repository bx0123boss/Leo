namespace PuntoVentaWeb.Services;

using JaegerSoft;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PuntoVentaWeb.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ProductoTicket = global::Producto;

public class ImpresionPosService
{
    private readonly IConfiguration _config;
    private readonly ILogger<ImpresionPosService> _logger;
    private readonly string _sqlConn;

    public ImpresionPosService(IConfiguration config, ILogger<ImpresionPosService> logger)
    {
        _config = config;
        _logger = logger;
        _sqlConn = _config.GetConnectionString("SQL") ?? @"Server=.\SQLEXPRESS;Database=PuntoDeVenta;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True;";
    }

    public Task ImprimirTicketVentaAsync(VentaGuardarRequest req)
    {
        try
        {
            var config = CargarConfiguracionNegocio();

            List<ProductoTicket> productosParaImprimir = new List<ProductoTicket>();
            foreach (var item in req.Items)
            {
                productosParaImprimir.Add(new ProductoTicket
                {
                    Nombre = item.Nombre,
                    PrecioUnitario = Math.Round(item.Precio / 1.16, 2),
                    Cantidad = item.Cantidad,
                    Total = item.Subtotal,
                    Comentario = item.Comentario ?? ""
                });
            }

            Dictionary<string, double> totales = new Dictionary<string, double>();

            if (req.Descuento > 0)
            {
                totales.Add("Descuento", req.Descuento);
            }

            if (config.ConIva)
            {
                totales.Add("Subtotal", Math.Round(req.Total / 1.16, 2));
                totales.Add("IVA", Math.Round((req.Total / 1.16) * 0.16, 2));
            }

            totales.Add("Total", req.Total);

            if (req.TipoVenta.Equals("CONTADO", StringComparison.OrdinalIgnoreCase))
            {
                totales.Add("Recibido", req.MontoRecibido > 0 ? req.MontoRecibido : req.Total);
                totales.Add("Cambio", req.Cambio);
            }
            else if (req.TipoVenta.Equals("CREDITO", StringComparison.OrdinalIgnoreCase))
            {
                totales.Add("Abono", req.AbonoInicial);
                totales.Add("Saldo Insoluto", req.Restante);
            }
            else if (req.TipoVenta.Equals("APARTADO", StringComparison.OrdinalIgnoreCase))
            {
                totales.Add("Abono", req.AbonoInicial);
                totales.Add("Restante", req.Restante);
            }

                global::TicketPrinter ticket = new global::TicketPrinter(
                    config.DatosTicket,
                    config.PieDeTicket,
                    config.LogoPath,
                    productosParaImprimir,
                    req.Folio,
                    "",
                    req.NombreCliente,
                    req.Total,
                    false,
                    totales,
                    req.MetodoPago
                );
            if (Conexion.Ticket)
            {
                if (!string.IsNullOrWhiteSpace(config.Impresora))
                {
                    ticket.ImprimirTicket(config.Impresora);
                }
                else if (Conexion.Ticket)
                {
                    ticket.ImprimirTicket();
                }
            }
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar la impresión de la venta desde ImpresionPosService");
        }

        return Task.CompletedTask;
    }

    private (string Lugar, string Impresora, string LogoPath, string[] DatosTicket, string[] PieDeTicket, bool ConIva, bool EsMediaCarta) CargarConfiguracionNegocio()
    {
        string lugar = "";
        string impresora = "";
        string logoPath = @"C:\Jaeger Soft\LOGO.png";
        string[] datosTicket = Array.Empty<string>();
        string[] pieDeTicket = Array.Empty<string>();
        bool conIva = false;
        bool esMediaCarta = false;

        try
        {
            using (var conexion = new SqlConnection(_sqlConn))
            {
                conexion.Open();
                string query = "SELECT TOP 1 * FROM Configuracion WHERE Id = 1";
                using var cmd = new SqlCommand(query, conexion);
                using var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    var schemaTable = reader.GetSchemaTable();
                    var columnasDisponibles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    if (schemaTable != null)
                    {
                        foreach (DataRow row in schemaTable.Rows)
                        {
                            columnasDisponibles.Add(row["ColumnName"].ToString() ?? "");
                        }
                    }

                    if (columnasDisponibles.Contains("Lugar"))
                        lugar = reader["Lugar"]?.ToString() ?? "";

                    if (columnasDisponibles.Contains("Impresora"))
                        impresora = reader["Impresora"]?.ToString() ?? "";

                    if (columnasDisponibles.Contains("logoPath") && !string.IsNullOrWhiteSpace(reader["logoPath"]?.ToString()))
                        logoPath = reader["logoPath"].ToString();

                    if (columnasDisponibles.Contains("ConIva") && reader["ConIva"] != DBNull.Value)
                        conIva = Convert.ToBoolean(reader["ConIva"]);

                    if (columnasDisponibles.Contains("MediaCarta") && reader["MediaCarta"] != DBNull.Value)
                        esMediaCarta = Convert.ToBoolean(reader["MediaCarta"]);

                    if (columnasDisponibles.Contains("DatosTicket"))
                    {
                        string rawDatos = reader["DatosTicket"]?.ToString() ?? "";
                        datosTicket = rawDatos.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    }

                    if (columnasDisponibles.Contains("PieDeTicket"))
                    {
                        string rawPie = reader["PieDeTicket"]?.ToString() ?? "";
                        pieDeTicket = rawPie.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consultando la tabla Configuracion en Access");
        }

        return (lugar, impresora, logoPath, datosTicket, pieDeTicket, conIva, esMediaCarta);
    }
}