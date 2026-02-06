using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing.Printing; // Para PrintDocument
using System.IO;
using System.Data.OleDb;
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
        private string _idCliente;
        private string _direccion;
        private string _rfc;
        private string _telefono;
        private string _correo;
        private string _formaPago;

        // CAMPOS ADICIONALES
        private string _datos;
        private string _observaciones;

        // Datos del Negocio
        private string _nombreLugar;
        private string _logoPath;
        private string[] _datosTicket;
        private string[] _pieDeTicket;

        public TicketMediaCarta(List<Producto> productos, string folio, double total, string cliente, string idCliente,string formaPago,
                                string datos, string observaciones,
                                string nombreLugar, string logoPath, string[] datosTicket, string[] pieDeTicket)
        {
            _productos = productos;
            _folio = folio;
            _total = total;
            _cliente = cliente;
            _idCliente = idCliente;
            _formaPago = formaPago;

            _datos = datos;
            _observaciones = observaciones;

            _nombreLugar = nombreLugar;
            _logoPath = logoPath;
            _datosTicket = datosTicket;
            _pieDeTicket = pieDeTicket;
            CargarDatosCliente();
        }
        private void CargarDatosCliente()
        {
            // Si el ID es 0, nulo o vacío, asumimos Público General y no buscamos
            if (string.IsNullOrEmpty(_idCliente) || _idCliente == "0") return;

            try
            {
                using (OleDbConnection con = new OleDbConnection(Conexion.CadCon))
                {
                    con.Open();
                    string query = "SELECT Direccion, RFC, Telefono, Correo FROM Clientes WHERE Id = @Id";

                    using (OleDbCommand cmd = new OleDbCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Id", _idCliente);

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                _direccion = reader["Direccion"] != DBNull.Value ? reader["Direccion"].ToString() : "";
                                _rfc = reader["RFC"] != DBNull.Value ? reader["RFC"].ToString() : "";
                                _telefono = reader["Telefono"] != DBNull.Value ? reader["Telefono"].ToString() : "";
                                _correo = reader["Correo"] != DBNull.Value ? reader["Correo"].ToString() : "";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error cargando datos cliente: " + ex.Message);
            }
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
                        // 1. TAMAÑO CARTA COMPLETA
                        page.Size(new PageSize(612, 792));

                        // 2. MÁRGENES
                        page.MarginTop(1, Unit.Centimetre);
                        page.MarginLeft(1, Unit.Centimetre);
                        page.MarginRight(1, Unit.Centimetre);
                        page.MarginBottom(396); // Bloqueo de media hoja

                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                        page.Header().Element(ComposeHeader);
                        page.Content().Element(ComposeContent);
                        page.Footer().Element(ComposeFooter);
                    });
                });

                // Generar imágenes para imprimir con PrintDocument
                var imagenesDePaginas = documento.GenerateImages();

                PrintDocument pd = new PrintDocument();
                pd.PrinterSettings.PrinterName = nombreImpresora;

                if (!pd.PrinterSettings.IsValid)
                    throw new InvalidPrinterException(pd.PrinterSettings);

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
                                    // Ajuste de escala para impresión correcta (72 DPI vs 100 DPI)
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
                MessageBox.Show(ex.ToString(), "ERROR QuestPDF", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método auxiliar para estilo de celda (Movido aquí para compatibilidad C# 7.3)
        private IContainer CellStyle(IContainer container)
        {
            return container
                .PaddingVertical(2)
                .BorderBottom(1)
                .BorderColor(Colors.Black);
        }

        void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                // LOGO
                if (!string.IsNullOrEmpty(_logoPath) && File.Exists(_logoPath))
                {
                    row.ConstantItem(140).Image(_logoPath).FitArea();
                }

                // DATOS EMPRESA
                row.RelativeItem().PaddingLeft(15).Column(col =>
                {
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
                    if (Conexion.lugar != "TURBOLLANTAS")
                    {
                        col.Item().PaddingTop(8).Text($"Forma de pago: {_formaPago}");
                    }
                });
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(10).Column(column =>
            {
                // 1. CUADRO DE DATOS DEL CLIENTE
                column.Item().PaddingBottom(5).Border(1).BorderColor(Colors.Grey.Medium).Padding(5).Column(colCliente =>
                {
                    colCliente.Item().PaddingBottom(2).Text("DATOS DEL CLIENTE").Bold().FontSize(8).FontColor(Colors.Grey.Darken2);
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
                    AgregarDato("Nombre", _cliente ?? "Público General");

                    // B. Resto de datos (Solo aparecerán si pasas el dato al constructor)
                    // Asegúrate de declarar estas variables en tu clase TicketMediaCarta
                    AgregarDato("Dirección", _direccion);
                    AgregarDato("RFC", _rfc);
                    AgregarDato("Teléfono", _telefono);
                    AgregarDato("Correo", _correo);
                });

                // 2. CUADRO DE DATOS EXTRA (DINÁMICOS)
                if (!string.IsNullOrEmpty(_datos))
                {
                    column.Item().PaddingBottom(5).Border(1).BorderColor(Colors.Black).Padding(4).Text(text =>
                    {
                        var listaDatos = _datos.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

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

                // 3. OBSERVACIONES
                string notasAMostrar = !string.IsNullOrEmpty(_observaciones) ? _observaciones : "";

                if (!string.IsNullOrEmpty(notasAMostrar) || Conexion.lugar != "TURBOLLANTAS")
                {
                    column.Item().PaddingBottom(5).Border(1).BorderColor(Colors.Black).Padding(4).Column(c =>
                    {
                        c.Item().Text("Observaciones:").Bold().FontSize(8);
                        c.Item().Text(notasAMostrar).FontSize(9).Italic();
                    });
                }

                // 4. TABLA DE PRODUCTOS
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(35); // Cant
                        columns.RelativeColumn();   // Descripción
                        columns.ConstantColumn(70); // P.Unit
                        columns.ConstantColumn(70); // Importe
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Cant").Bold();
                        header.Cell().Element(CellStyle).Text("Descripción").Bold();
                        header.Cell().Element(CellStyle).AlignRight().Text("P.Unit").Bold();
                        header.Cell().Element(CellStyle).AlignRight().Text("Importe").Bold();
                    });

                    foreach (var item in _productos)
                    {
                        table.Cell().PaddingVertical(1).Text(item.Cantidad.ToString("0.##"));
                        table.Cell().PaddingVertical(1).Text(item.Nombre).FontSize(8);
                        table.Cell().PaddingVertical(1).AlignRight().Text($"{item.PrecioUnitario:N2}");
                        table.Cell().PaddingVertical(1).AlignRight().Text($"{item.Total:N2}");
                    }

                    table.Footer(footer =>
                    {
                        footer.Cell().ColumnSpan(4).PaddingVertical(2).LineHorizontal(1);
                        footer.Cell().ColumnSpan(3).AlignRight().PaddingRight(5).Text("TOTAL:").Bold().FontSize(11);
                        footer.Cell().AlignRight().Text($"{_total:C2}").Bold().FontSize(11).FontColor(Colors.Green.Darken2);
                    });
                });
            });
        }

        void ComposeFooter(IContainer container)
        {
            container.Column(col =>
            {
                if (_pieDeTicket != null)
                {
                    col.Item().PaddingTop(5).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
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