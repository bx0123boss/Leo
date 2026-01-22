using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PuntoVentaWeb.Models;

namespace PuntoVentaWeb.Services;

public class PdfGenerator
{
    public static byte[] CrearPdfCotizacion(Cotizacion cotizacion, ConfiguracionNegocio config)
    {
        // Configuración de Licencia Community (Gratuita)
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                // 1. TAMAÑO CARTA COMPLETA (8.5 x 11 pulgadas)
                // Usamos el tamaño estándar para que la impresora no pida papel especial.
                page.Size(new PageSize(612, 792));

                // 2. EL TRUCO: MARGEN INFERIOR GIGANTE
                // Definimos márgenes normales para Arriba, Izquierda y Derecha (1 cm)
                page.MarginTop(1, Unit.Centimetre);
                page.MarginLeft(1, Unit.Centimetre);
                page.MarginRight(1, Unit.Centimetre);

                // Definimos un Margen Inferior de 5.5 pulgadas (396 puntos)
                // Esto bloquea la mitad de abajo de la hoja. El contenido y el pie de página
                // se detendrán exactamente a la mitad.
                page.MarginBottom(396);

                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }).GeneratePdf();

        // --- ENCABEZADO CON LOGO Y DATOS ---
        void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                // 1. LOGO (Izquierda)
                if (config != null && !string.IsNullOrEmpty(config.LogoPath) && System.IO.File.Exists(config.LogoPath))
                {
                    row.ConstantItem(60).Image(config.LogoPath).FitArea();
                }

                // 2. DATOS DEL NEGOCIO (Derecha)
                row.RelativeItem().PaddingLeft(10).Column(col =>
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

                    // Datos de la Cotización
                    col.Item().PaddingTop(5).Text($"Cotización #: {cotizacion.Id}").Bold().FontSize(10);
                    col.Item().Text($"Fecha: {cotizacion.Fecha:dd/MM/yyyy HH:mm}");
                    col.Item().Text($"Cliente: {cotizacion.ClienteNombre}");
                });
            });
        }

        // --- CONTENIDO (Observaciones + Tabla) ---
        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(10).Column(column =>
            {
                // 1. SECCIÓN DE OBSERVACIONES
                if (!string.IsNullOrEmpty(cotizacion.Observaciones))
                {
                    column.Item().PaddingBottom(10).Background(Colors.Grey.Lighten4).Padding(5).Column(c =>
                    {
                        c.Item().Text("Observaciones / Notas:").Bold().FontSize(8);
                        c.Item().Text(cotizacion.Observaciones).FontSize(9).Italic();
                    });
                }

                // 2. TABLA DE PRODUCTOS
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);  // Cant
                        columns.RelativeColumn();    // Descripcion
                        columns.ConstantColumn(55);  // Precio
                        columns.ConstantColumn(55);  // Total
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Cant").Bold();
                        header.Cell().Text("Descripción").Bold();
                        header.Cell().AlignRight().Text("P.Unit").Bold();
                        header.Cell().AlignRight().Text("Importe").Bold();

                        header.Cell().ColumnSpan(4)
                              .PaddingVertical(2).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                    });

                    foreach (var item in cotizacion.Detalles)
                    {
                        table.Cell().Text(item.Cantidad.ToString());
                        table.Cell().Text(item.Descripcion).FontSize(8);
                        table.Cell().AlignRight().Text($"{item.PrecioUnitario:N2}");
                        table.Cell().AlignRight().Text($"{item.Importe:N2}");
                    }

                    // Línea de Total
                    table.Footer(footer =>
                    {
                        footer.Cell().ColumnSpan(4).PaddingVertical(2).LineHorizontal(1).LineColor(Colors.Black);

                        footer.Cell().ColumnSpan(3).AlignRight().Text("TOTAL:").Bold().FontSize(11);
                        footer.Cell().AlignRight().Text($"{cotizacion.Total:C2}").Bold().FontSize(11).FontColor(Colors.Green.Darken2);
                    });
                });
            });
        }

        // --- PIE DE PÁGINA ---
        void ComposeFooter(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().PaddingVertical(5).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

                if (config != null && config.PieDeTicket != null)
                {
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