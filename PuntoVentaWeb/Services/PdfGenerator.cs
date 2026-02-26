using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PuntoVentaWeb.Models;
using Microsoft.Extensions.Configuration; // Necesario para leer appsettings
using System.Data.SqlClient; // O Microsoft.Data.SqlClient

namespace PuntoVentaWeb.Services;

public class PdfGenerator
{
    private class ColoresPdf
    {
        public string Primario { get; set; } = "#720e1e"; // Guinda Default
        public string Secundario { get; set; } = "#FFD700"; // Dorado Default
        public int DiasValidez { get; set; } = 15; // Valor por defecto
    }

    public static byte[] CrearPdfCotizacion(Cotizacion cotizacion, Cliente cliente, ConfiguracionNegocio config)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        // 1. OBTENER COLORES (Estética del código que te gustó)
        var coloresHex = ObtenerColoresDesdeBD();

        var colorPrincipal = Color.FromHex(coloresHex.Primario);
        var colorSecundario = Color.FromHex(coloresHex.Secundario);
        var colorGrisClaro = Colors.Grey.Lighten3;
        // CÁLCULO DE LA FECHA DE coloresHex
        var fechaVigencia = cotizacion.Fecha.AddDays(coloresHex.DiasValidez);
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(new PageSize(612, 396));
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }).GeneratePdf();

        // ---------------------------------------------------------
        // MÉTODOS DE DISEÑO
        // ---------------------------------------------------------

        void ComposeHeader(IContainer container)
        {
            container.Column(col =>
            {
                // BARRA SUPERIOR DE COLOR
                col.Item().Height(6).Background(colorPrincipal);

                col.Item().PaddingTop(10).Row(row =>
                {
                    // LOGO
                    if (config != null && !string.IsNullOrEmpty(config.LogoPath) && System.IO.File.Exists(config.LogoPath))
                    {
                        row.ConstantItem(130).Image(config.LogoPath).FitArea();
                    }

                    // DATOS DE LA EMPRESA
                    row.RelativeItem().PaddingLeft(15).Column(infoCol =>
                    {
                        if (config != null)
                        {
                            infoCol.Item().Text(config.NombreLugar.ToUpper()).Bold().FontSize(16).FontColor(colorPrincipal);

                            foreach (var linea in config.DatosTicket)
                            {
                                if (!string.IsNullOrWhiteSpace(linea))
                                    infoCol.Item().Text(linea.Trim()).FontSize(8).FontColor(Colors.Grey.Darken2);
                            }
                        }
                    });

                    // CUADRO DE FOLIO Y FECHA
                    row.ConstantItem(120).Border(1).BorderColor(colorPrincipal).Padding(0).Column(box =>
                    {
                        box.Item().Background(colorPrincipal).Padding(2).AlignCenter().Text("COTIZACIÓN").Bold().FontColor(Colors.White);

                        box.Item().Padding(4).Column(c => {
                            c.Item().AlignCenter().Text($"Folio: {cotizacion.Id}").FontSize(11).Bold().FontColor(Colors.Black);
                            c.Item().PaddingTop(2).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                            c.Item().PaddingTop(2).AlignCenter().Text($"{cotizacion.Fecha:dd/MMM/yyyy}").FontSize(8);
                            c.Item().AlignCenter().Text($"{cotizacion.Fecha:HH:mm} hrs").FontSize(7).FontColor(Colors.Grey.Darken2);

                            // --- AQUÍ MOSTRAMOS LA VIGENCIA CALCULADA ---
                            c.Item().PaddingTop(3).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                            c.Item().PaddingTop(2).AlignCenter().Text("Vigente hasta:").FontSize(6).Bold().FontColor(colorPrincipal);
                            c.Item().AlignCenter().Text($"{fechaVigencia:dd/MMM/yyyy}").FontSize(8).Bold();
                        });
                    });
                });

                // Línea separadora decorativa
                col.Item().PaddingTop(10).LineHorizontal(2).LineColor(colorSecundario);
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(10).Column(column =>
            {
                // --- FILA SUPERIOR: DATOS DEL CLIENTE Y OBSERVACIONES ---
                column.Item().PaddingBottom(10).Row(row =>
                {
                    // IZQUIERDA: Datos del Cliente
                    row.RelativeItem().Column(c =>
                    {
                        // Título con pequeña barra lateral
                        c.Item().Row(titleRow =>
                        {
                            titleRow.ConstantItem(5).Height(15).Background(colorSecundario);
                            titleRow.RelativeItem().PaddingLeft(5).Text("DATOS DEL CLIENTE").Bold().FontSize(10).FontColor(colorPrincipal);
                        });

                        c.Item().PaddingTop(3).PaddingLeft(10).Column(datos =>
                        {
                            var nombreFinal = cliente?.Nombre ?? cotizacion.ClienteNombre ?? "Público General";
                            datos.Item().Text(nombreFinal).Bold().FontSize(11);
                            if (cliente != null || config.NombreLugar == "TURBOLLANTAS")
                            {
                                datos.Item().Text($"Dirección: {cliente?.Direccion ?? "                                        "} CP: {cliente?.CP ?? ""}");
                                datos.Item().Text($"Teléfono: {cliente?.Telefono ?? ""}");
                                datos.Item().Text($"RFC: {cliente?.RFC ?? ""}");
                                
                            }
                        });
                    });

                    // DERECHA: Observaciones (¡LÓGICA RECUPERADA!)
                    // Se muestra si hay texto O si es TURBOLLANTAS (aunque esté vacío)
                    if (!string.IsNullOrEmpty(cotizacion.Observaciones) || (config != null && config.NombreLugar == "TURBOLLANTAS"))
                    {
                        row.RelativeItem().PaddingLeft(10).Column(c =>
                        {
                            c.Item().Text("OBSERVACIONES").Bold().FontSize(9).FontColor(colorPrincipal);

                            // Usamos un cuadro gris claro para destacar la nota
                            c.Item().Background(colorGrisClaro).Padding(5).Column(nota =>
                            {
                                // Si es null (caso turbollantas vacío), ponemos string.Empty
                                var textoNota = cotizacion.Observaciones ?? "";
                                nota.Item().Text(textoNota).Italic().FontSize(8);
                            });
                        });
                    }
                });

                // --- DATOS EXTRA (Placas, Modelo, etc.)
                if (!string.IsNullOrEmpty(cotizacion.Datos) || config.NombreLugar == "TURBOLLANTAS")
                {
                    // Los ponemos en un bloque separado antes de la tabla con borde del color principal
                    column.Item().PaddingBottom(10).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(text =>
                    {
                        var listaDatos = cotizacion.Datos.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        if (listaDatos.Length > 0)
                        {
                            foreach (var dato in listaDatos)
                            {
                                var partes = dato.Split(new[] { ':' }, 2);
                                if (partes.Length == 2)
                                {
                                    // La etiqueta en color principal (ej. "Placas:")
                                    text.Span($"{partes[0].Trim()}: ").Bold().FontColor(colorPrincipal);
                                    // El valor en negro (ej. "XYZ-123")
                                    text.Span($"{partes[1].Trim()}    ");
                                }
                            }
                        }
                        else
                        {
                            string[] camposDefault = { "Placas", "Modelo", "Color", "Kilometraje" };
                            var camposConfig = ObtenerCamposConfiguradosDesdeBD();
                            foreach (var campo in camposDefault)
                            {
                                text.Span($"{campo}: ").Bold().FontColor(colorPrincipal);
                                text.Span("________________    "); // Espacio vacío para llenar
                            }
                        }
                    });
                }
                // --- TABLA DE PRODUCTOS (Estilo Corporativo) ---
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30); // #
                        columns.ConstantColumn(40); // Cant
                        columns.RelativeColumn();   // Descripcion
                        columns.ConstantColumn(70); // P.Unit
                        columns.ConstantColumn(65); // TASA IVA
                        columns.ConstantColumn(50); // DESC
                        columns.ConstantColumn(70); // Importe
                    });

                    table.Header(header =>
                    {
                        IContainer HeaderStyle(IContainer c) => c.Background(colorPrincipal).PaddingVertical(3).PaddingHorizontal(2);
                        header.Cell().Element(HeaderStyle).Text("#").FontColor(Colors.White).Bold();
                        header.Cell().Element(HeaderStyle).AlignCenter().Text("CANT").FontColor(Colors.White).Bold();
                        header.Cell().Element(HeaderStyle).Text("DESCRIPCIÓN").FontColor(Colors.White).Bold();
                        header.Cell().Element(HeaderStyle).AlignRight().Text("P.UNIT").FontColor(Colors.White).Bold();
                        header.Cell().Element(HeaderStyle).AlignRight().Text("TASA IVA %").FontColor(Colors.White).Bold();
                        header.Cell().Element(HeaderStyle).AlignRight().Text("DESC").FontColor(Colors.White).Bold();
                        header.Cell().Element(HeaderStyle).AlignRight().Text("IMPORTE").FontColor(Colors.White).Bold();
                    });

                    for (int i = 0; i < cotizacion.Detalles.Count; i++)
                    {
                        var item = cotizacion.Detalles[i];
                        var bg = i % 2 == 0 ? Colors.White : colorGrisClaro;
                        IContainer CellStyle(IContainer c) => c.Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2).PaddingHorizontal(2);

                        table.Cell().Element(CellStyle).Text((i + 1).ToString()).FontSize(8).FontColor(Colors.Grey.Darken2);
                        table.Cell().Element(CellStyle).AlignCenter().Text(item.Cantidad.ToString()).Bold();
                        table.Cell().Element(CellStyle).Text(item.Descripcion).FontSize(9);
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.PrecioUnitario:N2}");
                        table.Cell().Element(CellStyle).AlignRight().Text("16.00%");
                        table.Cell().Element(CellStyle).AlignRight().Text("$0.00");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.Importe:N2}").Bold();
                    }

                    // IMPORTANTE: NO USAR table.Footer() aquí para el Total
                });

                // --- BLOQUE DE TOTALES (Fuera de la tabla para que salga solo al final) ---
                column.Item().AlignRight().Row(row =>
                {
                    row.ConstantItem(200).PaddingTop(5).Border(1).BorderColor(colorPrincipal).Padding(5).Column(c =>
                    {
                        c.Item().Row(r =>
                        {
                            r.RelativeItem().Text("TOTAL:").Bold().FontSize(12);
                            r.RelativeItem().AlignRight().Text($"{cotizacion.Total:C2}").Bold().FontSize(13);
                        });
                    });
                });
            });
        }

        void ComposeFooter(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().LineHorizontal(1).LineColor(colorPrincipal);

                if (config != null && config.PieDeTicket != null)
                {
                    col.Item().PaddingTop(2).Column(c =>
                    {
                        foreach (var linea in config.PieDeTicket)
                        {
                            if (!string.IsNullOrWhiteSpace(linea))
                                c.Item().AlignCenter().Text(linea.Trim()).FontSize(7).Italic().FontColor(Colors.Grey.Darken1);
                        }
                    });
                }

                col.Item().PaddingTop(2).Row(row =>
                {
                    row.RelativeItem().Text($"Generado el {DateTime.Now:g}").FontSize(6).FontColor(Colors.Grey.Lighten1);
                    row.RelativeItem().AlignRight().Text(x =>
                    {
                        x.Span("Página ").FontSize(6);
                        x.CurrentPageNumber().FontSize(6);
                        x.Span(" de ").FontSize(6);
                        x.TotalPages().FontSize(6);
                    });
                });
            });
        }
    }

    private static List<string> ObtenerCamposConfiguradosDesdeBD()
    {
        var campos = new List<string>();
        try
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();
            string connectionString = configuration.GetConnectionString("SQL");

            if (!string.IsNullOrEmpty(connectionString))
            {
                // 2. Consultamos los campos activos
                using (var con= new SqlConnection(connectionString))
                {
                    con.Open();
                    string sql = "SELECT NombreEtiqueta FROM CotizacionCamposConfig WHERE Activo = 1 ORDER BY Orden";
                    using (var cmd = new SqlCommand(sql, con))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                campos.Add(reader["NombreEtiqueta"].ToString());
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error obteniendo etiquetas para PDF: {ex.Message}");
        }
        return campos;
    }
    // -----------------------------------------------------------------------
    // MÉTODO PRIVADO: Obtiene los colores de SQL Server
    // -----------------------------------------------------------------------
    private static ColoresPdf ObtenerColoresDesdeBD()
    {
        var colores = new ColoresPdf();
        try
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();
            string connectionString = configuration.GetConnectionString("SQL");

            if (!string.IsNullOrEmpty(connectionString))
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT TOP 1 ColorPrimario, ColorSecundario, DiasValidez FROM ConfiguracionApariencia";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var c1 = reader["ColorPrimario"]?.ToString();
                                var c2 = reader["ColorSecundario"]?.ToString();

                                if (!string.IsNullOrEmpty(c1)) colores.Primario = c1;
                                if (!string.IsNullOrEmpty(c2)) colores.Secundario = c2;
                                if (reader["DiasValidez"] != DBNull.Value)
                                {
                                    colores.DiasValidez = Convert.ToInt32(reader["DiasValidez"]);
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error obteniendo colores para PDF: {ex.Message}");
        }
        return colores;
    }
}