using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PuntoVentaWeb.Models;

namespace PuntoVentaWeb.Services;

public class PdfGenerator
{
    // AHORA RECIBIMOS EL OBJETO 'CLIENTE' COMO PARÁMETRO ADICIONAL
    public static byte[] CrearPdfCotizacion(Cotizacion cotizacion, Cliente cliente, ConfiguracionNegocio config)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(new PageSize(612, 792));
                page.MarginTop(1, Unit.Centimetre);
                page.MarginLeft(1, Unit.Centimetre);
                page.MarginRight(1, Unit.Centimetre);
                page.MarginBottom(396);

                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }).GeneratePdf();

        void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                // LOGO
                if (config != null && !string.IsNullOrEmpty(config.LogoPath) && System.IO.File.Exists(config.LogoPath))
                {
                    row.ConstantItem(140).Image(config.LogoPath).FitArea();
                }

                // DATOS EMPRESA
                row.RelativeItem().PaddingLeft(15).Column(col =>
                {
                    if (config != null)
                    {
                        col.Item().Text(config.NombreLugar).Bold().FontSize(12).FontColor(Colors.Blue.Darken2);
                        foreach (var linea in config.DatosTicket)
                        {
                            if (!string.IsNullOrWhiteSpace(linea))
                                col.Item().Text(linea.Trim()).FontSize(8);
                        }
                    }
                    col.Item().PaddingTop(5).Text($"Cotización #: {cotizacion.Id}").Bold().FontSize(10);
                    col.Item().Text($"Fecha: {cotizacion.Fecha:dd/MM/yyyy HH:mm}");
                });
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(10).Column(column =>
            {
                // --- 1. CUADRO DE DATOS DEL CLIENTE ---
                column.Item().PaddingBottom(5).Border(1).BorderColor(Colors.Grey.Medium).Padding(5).Column(colCliente =>
                {
                    colCliente.Item().PaddingBottom(2).Text("DATOS DEL CLIENTE").Bold().FontSize(8).FontColor(Colors.Grey.Darken2);

                    // Función auxiliar local
                    void AgregarDato(string etiqueta, string valor)
                    {
                        if (!string.IsNullOrWhiteSpace(valor))
                        {
                            colCliente.Item().Text(text =>
                            {
                                text.Span($"{etiqueta}: ").Bold();
                                text.Span(valor);
                            });
                        }
                    }

                    // A. NOMBRE: Usamos el objeto cliente si existe, si no el histórico de la cotización
                    var nombreFinal = cliente?.Nombre ?? cotizacion.ClienteNombre ?? "Público General";
                    AgregarDato("Nombre", nombreFinal);

                    // B. RESTO DE DATOS (Usando el parámetro 'cliente')
                    if (cliente != null)
                    {
                        AgregarDato("Teléfono", cliente.Telefono);
                        AgregarDato("Dirección", cliente.Direccion);
                        AgregarDato("Referencia", cliente.Referencia);
                        AgregarDato("RFC", cliente.RFC);
                        AgregarDato("Correo", cliente.Correo);
                    }
                });

                // --- 2. DATOS EXTRA / DINÁMICOS ---
                if (!string.IsNullOrEmpty(cotizacion.Datos))
                {
                    column.Item().PaddingBottom(5).Border(1).BorderColor(Colors.Black).Padding(4).Text(text =>
                    {
                        var listaDatos = cotizacion.Datos.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var dato in listaDatos)
                        {
                            var partes = dato.Split(new[] { ':' }, 2);
                            if (partes.Length == 2)
                            {
                                text.Span($"{partes[0].Trim()}: ").Bold();
                                text.Span($"{partes[1].Trim()}   ");
                            }
                        }
                    });
                }

                // --- 3. OBSERVACIONES ---
                if (!string.IsNullOrEmpty(cotizacion.Observaciones) || (config != null && config.NombreLugar == "TURBOLLANTAS"))
                {
                    column.Item().PaddingBottom(5).Border(1).BorderColor(Colors.Black).Padding(4).Column(c =>
                    {
                        c.Item().Text("Observaciones / Notas:").Bold().FontSize(8);
                        c.Item().Text(cotizacion.Observaciones).FontSize(9).Italic();
                    });
                }

                // --- 4. TABLA DE PRODUCTOS ---
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(35); // Cant
                        columns.RelativeColumn();   // Desc
                        columns.ConstantColumn(70); // P.Unit
                        columns.ConstantColumn(70); // Importe
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Cant").Bold();
                        header.Cell().Element(CellStyle).Text("Descripción").Bold();
                        header.Cell().Element(CellStyle).AlignRight().Text("P.Unit").Bold();
                        header.Cell().Element(CellStyle).AlignRight().Text("Importe").Bold();

                        static IContainer CellStyle(IContainer container) => container.PaddingVertical(2).BorderBottom(1).BorderColor(Colors.Black);
                    });

                    foreach (var item in cotizacion.Detalles)
                    {
                        table.Cell().PaddingVertical(1).Text(item.Cantidad.ToString());
                        table.Cell().PaddingVertical(1).Text(item.Descripcion).FontSize(8);
                        table.Cell().PaddingVertical(1).AlignRight().Text($"{item.PrecioUnitario:N2}");
                        table.Cell().PaddingVertical(1).AlignRight().Text($"{item.Importe:N2}");
                    }

                    table.Footer(footer =>
                    {
                        footer.Cell().ColumnSpan(4).PaddingVertical(2).LineHorizontal(1);
                        footer.Cell().ColumnSpan(3).AlignRight().PaddingRight(5).Text("TOTAL:").Bold().FontSize(11);
                        footer.Cell().AlignRight().Text($"{cotizacion.Total:C2}").Bold().FontSize(11).FontColor(Colors.Green.Darken2);
                    });
                });
            });
        }

        void ComposeFooter(IContainer container)
        {
            container.Column(col =>
            {
                if (config != null && config.PieDeTicket != null)
                {
                    col.Item().PaddingTop(5).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
                    foreach (var linea in config.PieDeTicket)
                    {
                        if (!string.IsNullOrWhiteSpace(linea))
                            col.Item().AlignCenter().Text(linea.Trim()).Italic().FontSize(7);
                    }
                }
                col.Item().AlignCenter().Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                    x.Span(" de ");
                    x.TotalPages();
                });
            });
        }
    }
}