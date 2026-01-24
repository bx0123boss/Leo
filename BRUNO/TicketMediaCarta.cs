using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing; // Para WinForms e Image
using System.Drawing.Printing; // Para PrintDocument
using System.IO;
using System.Linq;
using System.Windows.Forms;
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
        private string _formaPago;

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
            try
            {
                QuestPDF.Settings.License = LicenseType.Community;

                var documento = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        // 1. TAMAÑO CARTA COMPLETA (Para que la impresora no falle)
                        // 612 pt = 8.5 pulgadas (Ancho)
                        // 792 pt = 11 pulgadas (Alto)
                        page.Size(new PageSize(612, 792));

                        // 2. EL TRUCO: MARGEN INFERIOR GIGANTE
                        // Margen normal arriba y lados (1 cm aprox = 28 pts)
                        page.MarginTop(1, Unit.Centimetre);
                        page.MarginLeft(1, Unit.Centimetre);
                        page.MarginRight(1, Unit.Centimetre);

                        // Margen inferior de 5.5 pulgadas (396 pts)
                        // Esto obliga a que el "Footer" y el contenido terminen 
                        // justo a la mitad de la hoja física.
                        page.MarginBottom(396);

                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                        page.Header().Element(ComposeHeader);
                        page.Content().Element(ComposeContent);
                        page.Footer().Element(ComposeFooter);
                    });
                });

                // Generar imagen
                var imagenesDePaginas = documento.GenerateImages();

                PrintDocument pd = new PrintDocument();
                pd.PrinterSettings.PrinterName = nombreImpresora;

                if (!pd.PrinterSettings.IsValid)
                    throw new InvalidPrinterException(pd.PrinterSettings);

                // La hoja entra parada (Vertical)
                pd.DefaultPageSettings.Landscape = false;

                int paginaActual = 0;
                pd.PrintPage += (sender, e) =>
                {
                    try
                    {
                        if (paginaActual < imagenesDePaginas.Count())
                        {
                            using (var ms = new MemoryStream(imagenesDePaginas.ElementAt(paginaActual)))
                            {
                                using (System.Drawing.Image img = System.Drawing.Image.FromStream(ms))
                                {
                                    // 3. IMPRIMIR TAMAÑO CARTA COMPLETO
                                    // Así la impresora recibe una imagen de 8.5x11 y no se confunde.
                                    // La mitad de abajo de esa imagen ya vendrá blanca por el margen que pusimos.
                                    float anchoImpresion = 612f * 100f / 72f;
                                    float altoImpresion = 792f * 100f / 72f;

                                    e.Graphics.DrawImage(img, 0, 0, anchoImpresion, altoImpresion);
                                }
                            }
                            paginaActual++;
                            e.HasMorePages = (paginaActual < imagenesDePaginas.Count());
                        }
                    }
                    catch (Exception exInt)
                    {
                        throw new Exception($"Error renderizando: {exInt.Message}", exInt);
                    }
                };

                pd.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                 ex.ToString(),
                 "ERROR QuestPDF",
                 MessageBoxButtons.OK,
                 MessageBoxIcon.Error
             );
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendLine($"Error: {ex.Message}");
                if (ex.InnerException != null) sb.AppendLine($"Interno: {ex.InnerException.Message}");
                throw new Exception(sb.ToString());
              
            }
        }
        void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                if (!string.IsNullOrEmpty(_logoPath) && File.Exists(_logoPath))
                {
                    // ANTES: 60 (muy pequeño)
                    // AHORA: 140 (bastante grande, aprox. 5 cm de ancho)
                    // FitArea() ajustará la altura proporcionalmente.
                    row.ConstantItem(140).Image(_logoPath).FitArea();
                }

                // El texto se ajusta al espacio restante
                row.RelativeItem().PaddingLeft(15).Column(col =>
                {
                    // Aumenté un poco la fuente del título también
                    col.Item().Text(_nombreLugar).Bold().FontSize(14).FontColor(Colors.Blue.Darken2);

                    if (_datosTicket != null)
                    {
                        foreach (var linea in _datosTicket)
                        {
                            if (!string.IsNullOrWhiteSpace(linea))
                                col.Item().Text(linea.Trim()).FontSize(9);
                        }
                    }

                    col.Item().PaddingTop(8).Text($"Presupuesto: {_folio}").Bold().FontSize(11);
                    col.Item().Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}");
                    col.Item().Text($"Cliente: {_cliente}");
                });
            });
        }

        // --- CONTENIDO ---
        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(10).Column(column =>
            {
                if (!string.IsNullOrEmpty(_formaPago) || Conexion.lugar != "TURBOLLANTAS")
                {
                    column.Item().PaddingBottom(10).Background(Colors.Grey.Lighten4).Padding(5).Column(c =>
                    {
                        c.Item().Text("Forma de Pago / Notas:").Bold().FontSize(8);
                        c.Item().Text(_formaPago).FontSize(9).Italic();
                    });
                }

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);
                        columns.RelativeColumn();
                        columns.ConstantColumn(55);
                        columns.ConstantColumn(55);
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
                        table.Cell().Text(item.Nombre).FontSize(8);
                        table.Cell().AlignRight().Text($"{item.PrecioUnitario:N2}");
                        table.Cell().AlignRight().Text($"{item.Total:N2}");
                    }

                    table.Footer(footer =>
                    {
                        footer.Cell().ColumnSpan(4).PaddingVertical(2).LineHorizontal(1).LineColor(Colors.Black);
                        footer.Cell().ColumnSpan(3).AlignRight().Text("TOTAL:").Bold().FontSize(11);
                        footer.Cell().AlignRight().Text($"{_total:C2}").Bold().FontSize(11).FontColor(Colors.Green.Darken2);
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