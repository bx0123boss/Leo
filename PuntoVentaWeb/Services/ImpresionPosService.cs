namespace PuntoVentaWeb.Services;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PuntoVentaWeb.Models;

public class ImpresionPosService
{
    private readonly IConfiguration _config;
    private readonly ILogger<ImpresionPosService> _logger;
    private readonly string _accessConn;

    public ImpresionPosService(IConfiguration config, ILogger<ImpresionPosService> logger)
    {
        _config = config;
        _logger = logger;
        _accessConn = _config.GetConnectionString("Access") ?? @"Driver={Microsoft Access Driver (*.mdb, *.accdb)};Dbq=C:\Jaeger Soft\Jaeger.accdb;Pwd=75941232;";
    }

    public async Task ImprimirTicketVentaAsync(VentaGuardarRequest req)
    {
        await Task.Run(() =>
        {
            try
            {
                // 1. Cargar configuración de negocio desde Access
                string lugar = "";
                string impresora = "";
                string logoPath = "";
                string[] datosTicket = Array.Empty<string>();
                string[] pieDeTicket = Array.Empty<string>();
                bool conIva = false;

                using (var conexion = new OdbcConnection(_accessConn))
                {
                    conexion.Open();
                    string query = "SELECT TOP 1 * FROM Configuracion WHERE Id = 1";
                    using var cmd = new OdbcCommand(query, conexion);
                    using var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        lugar = reader["Lugar"]?.ToString() ?? "";
                        impresora = reader["Impresora"]?.ToString() ?? "";
                        logoPath = reader["logoPath"]?.ToString() ?? "";
                        conIva = reader["ConIva"] != DBNull.Value && Convert.ToBoolean(reader["ConIva"]);

                        string rawDatos = reader["DatosTicket"]?.ToString() ?? "";
                        datosTicket = rawDatos.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                        string rawPie = reader["PieDeTicket"]?.ToString() ?? "";
                        pieDeTicket = rawPie.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    }
                }

                // 2. Mapear productos al modelo Producto que espera TicketPrinter
                List<global::Producto> productosTicket = new List<global::Producto>();
                foreach (var item in req.Items)
                {
                    productosTicket.Add(new global::Producto
                    {
                        Nombre = item.Nombre,
                        PrecioUnitario = item.Precio,
                        Cantidad = item.Cantidad,
                        Total = item.Subtotal,
                        Comentario = item.Comentario
                    });
                }

                // 3. Preparar el diccionario de totales
                Dictionary<string, double> totales = new Dictionary<string, double>();
                if (req.Descuento > 0)
                {
                    totales.Add("Descuento", req.Descuento);
                }

                if (conIva)
                {
                    totales.Add("Subtotal", Math.Round(req.Total / 1.16, 2));
                    totales.Add("IVA", Math.Round((req.Total / 1.16) * 0.16, 2));
                }

                totales.Add("Total", req.Total);

                string clienteImpresion = req.NombreCliente;
                string metodoPagoImpresion = req.MetodoPago;

                if (req.TipoVenta.Equals("CONTADO", StringComparison.OrdinalIgnoreCase))
                {
                    totales.Add("Recibido", req.MontoRecibido > 0 ? req.MontoRecibido : req.Total);
                    totales.Add("Cambio", req.Cambio);
                }
                else if (req.TipoVenta.Equals("CREDITO", StringComparison.OrdinalIgnoreCase))
                {
                    totales.Add("Abono", req.AbonoInicial);
                    totales.Add("Saldo Actual", req.Restante);
                }
                else if (req.TipoVenta.Equals("APARTADO", StringComparison.OrdinalIgnoreCase))
                {
                    totales.Add("Abono", req.AbonoInicial);
                    totales.Add("Restante", req.Restante);
                }

                // 4. Instanciar e Imprimir con TicketPrinter
                TicketPrinter ticketPrinter = new TicketPrinter(
                    datosTicket,
                    pieDeTicket,
                    logoPath,
                    productosTicket,
                    req.Folio,
                    "",
                    clienteImpresion,
                    req.Total,
                    false,
                    totales,
                    metodoPagoImpresion
                );

                if (!string.IsNullOrWhiteSpace(impresora))
                {
                    ticketPrinter.ImprimirTicket(impresora);
                }
                else
                {
                    ticketPrinter.ImprimirTicket();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al imprimir ticket con TicketPrinter");
            }
        });
    }
}
