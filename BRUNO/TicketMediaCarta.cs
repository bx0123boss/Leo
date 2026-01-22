using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing; // Para WinForms e Image
using System.Drawing.Printing; // Para PrintDocument
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

// Alias para evitar conflicto entre System.Drawing.Image y QuestPDF.Infrastructure.Image
using QImage = QuestPDF.Infrastructure.Image;

namespace BRUNO
{
    public class TicketMediaCarta
    {
        // Datos de la venta
        private List<Producto> _productos;
        private string _folio;
        private double _total;
        private string _cliente;
        private string _formaPago; // Se mapea a "Observaciones"

        // Datos del Negocio
        private string _nombreLugar;
        private string _logoPath;
        private string[] _datosTicket;
        private string[] _pieDeTicket;

        public TicketMediaCarta(List<Producto> productos, string folio, double total, string cliente, string formaPago,
                                string nombreLugar, string logoPath, string[] datosTicket, string[] pieDeTicket)
        {
            _productos = productos;
            _folio = folio;
            _total = total;
            _cliente = cliente;
            _formaPago = formaPago;

            _nombreLugar = nombreLugar;
            _logoPath = logoPath;
            _datosTicket = datosTicket;
            _pieDeTicket = pieDeTicket;
        }

        public void ImprimirDirectamente(string nombreImpresora)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            // 1. Crear el documento (Usando las dimensiones de tu ejemplo Web: 396x312)
            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(new PageSize(396, 312)); // Dimensiones copiadas de tu código
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeFooter);
                });
            });

            // 2. Generar imágenes (Renderizado)
            var imagenesDePaginas = documento.GenerateImages();

            // 3. Configurar la Impresora
            PrintDocument pd = new PrintDocument();
            pd.PrinterSettings.PrinterName = nombreImpresora;

            // CORRECCIÓN: Orientación VERTICAL (Parada)
            pd.DefaultPageSettings.Landscape = false;

            int paginaActual = 0;
            pd.PrintPage += (sender, e) =>
            {
                if (paginaActual < imagenesDePaginas.Count())
                {
                    using (var ms = new MemoryStream(imagenesDePaginas.ElementAt(paginaActual)))
                    {
                        using (System.Drawing.Image img = System.Drawing.Image.FromStream(ms))
                        {
                            // Calcular dimensiones en centésimas de pulgada para DrawImage
                            // 396 puntos = 5.5 pulgadas = 550 centésimas
                            // 312 puntos = 4.33 pulgadas = 433 centésimas
                            float anchoImpresion = 396f * 100f / 72f;
                            float altoImpresion = 312f * 100f / 72f;

                            // Dibujar imagen
                            e.Graphics.DrawImage(img, 0, 0, anchoImpresion, altoImpresion);
                        }
                    }
                    paginaActual++;
                    e.HasMorePages = (paginaActual < imagenesDePaginas.Count());
                }
            };

            try
            {
                pd.Print();
            }
            catch (Exception ex)
            {
                throw new Exception("Error de impresión: " + ex.Message);
            }
        }

        // --- ENCABEZADO (Idéntico al código web) ---
        void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                // 1. LOGO
                if (!string.IsNullOrEmpty(_logoPath) && File.Exists(_logoPath))
                {
                    row.ConstantItem(60).Image(_logoPath).FitArea();
                }

                // 2. DATOS DEL NEGOCIO
                row.RelativeItem().PaddingLeft(10).Column(col =>
                {
                    col.Item().Text(_nombreLugar).Bold().FontSize(12).FontColor(Colors.Blue.Darken2);

                    if (_datosTicket != null)
                    {
                        foreach (var linea in _datosTicket)
                        {
                            if (!string.IsNullOrWhiteSpace(linea))
                                col.Item().Text(linea.Trim()).FontSize(8);
                        }
                    }

                    // Datos del Folio (Mapeado de "Cotización")
                    col.Item().PaddingTop(5).Text($"Servicio: {_folio}").Bold().FontSize(10);
                    col.Item().Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}");
                    col.Item().Text($"Cliente: {_cliente}");
                });
            });
        }

        // --- CONTENIDO (Idéntico al código web) ---
        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(10).Column(column =>
            {
                // 1. SECCIÓN DE OBSERVACIONES (Aquí ponemos la forma de pago)
                if (!string.IsNullOrEmpty(_formaPago))
                {
                    column.Item().PaddingBottom(10).Background(Colors.Grey.Lighten4).Padding(5).Column(c =>
                    {
                        c.Item().Text("Forma de Pago / Notas:").Bold().FontSize(8);
                        c.Item().Text(_formaPago).FontSize(9).Italic();
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

                    foreach (var item in _productos)
                    {
                        table.Cell().Text(item.Cantidad.ToString("0.##"));
                        table.Cell().Text(item.Nombre).FontSize(8); // Descripcion
                        table.Cell().AlignRight().Text($"{item.PrecioUnitario:N2}");
                        table.Cell().AlignRight().Text($"{item.Total:N2}");
                    }

                    // Línea de Total
                    table.Footer(footer =>
                    {
                        footer.Cell().ColumnSpan(4).PaddingVertical(2).LineHorizontal(1).LineColor(Colors.Black);

                        footer.Cell().ColumnSpan(3).AlignRight().Text("TOTAL:").Bold().FontSize(11);
                        footer.Cell().AlignRight().Text($"{_total:C2}").Bold().FontSize(11).FontColor(Colors.Green.Darken2);
                    });
                });
            });
        }

        // --- PIE DE PÁGINA (Idéntico al código web) ---
        void ComposeFooter(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().PaddingVertical(5).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

                if (_pieDeTicket != null)
                {
                    foreach (var linea in _pieDeTicket)
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