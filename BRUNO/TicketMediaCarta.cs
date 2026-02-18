using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
// Alias para evitar conflictos
using QImage = QuestPDF.Infrastructure.Image;

namespace BRUNO
{
    public class TicketMediaCarta
    {
        private class ColoresPdf
        {
            public string Primario { get; set; } = "#720e1e";
            public string Secundario { get; set; } = "#FFD700";
            public int DiasValidez { get; set; } = 15;
        }

        private List<Producto> _productos;
        private string _folio;
        private double _total;
        private string _cliente, _idCliente, _direccion, _rfc, _telefono, _correo, _formaPago;
        private string _datos, _observaciones;
        private string _nombreLugar, _logoPath;
        private string[] _datosTicket, _pieDeTicket;
        private string _hexPrimario, _hexSecundario;
        private double _descuento;
        private int _diasValidez;

        public TicketMediaCarta(List<Producto> productos, string folio, double descuento, double total, string cliente, string idCliente, string formaPago,
                                string datos, string observaciones,
                                string nombreLugar, string logoPath, string[] datosTicket, string[] pieDeTicket)
        {
            _productos = productos;
            _folio = folio;
            _descuento = descuento;
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

            // 1. Cargar datos desde Access
            CargarDatosCliente();

            // 2. Cargar colores desde SQL Server
            var colores = ObtenerColoresDesdeSQL();
            _hexPrimario = colores.Primario;
            _hexSecundario = colores.Secundario;
            _diasValidez = colores.DiasValidez;
        }

        private void CargarDatosCliente()
        {
            if (string.IsNullOrEmpty(_idCliente) || _idCliente == "0") return;
            try
            {
                using (OleDbConnection con = new OleDbConnection(Conexion.CadCon))
                {
                    con.Open();
                    string query = "SELECT Direccion, RFC, Telefono, Correo FROM Clientes WHERE Id = " + _idCliente;
                    using (OleDbCommand cmd = new OleDbCommand(query, con))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                _direccion = reader["Direccion"]?.ToString() ?? "";
                                _rfc = reader["RFC"]?.ToString() ?? "";
                                _telefono = reader["Telefono"]?.ToString() ?? "";
                                _correo = reader["Correo"]?.ToString() ?? "";
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Error Access: " + ex.Message); }
        }

        private ColoresPdf ObtenerColoresDesdeSQL()
        {
            var colores = new ColoresPdf();
            try
            {
                using (var conn = new SqlConnection(Conexion.CadSQL))
                {
                    conn.Open();
                    string query = "SELECT TOP 1 ColorPrimario, ColorSecundario, DiasValidez FROM ConfiguracionApariencia";
                    using (var cmd = new SqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            colores.Primario = reader["ColorPrimario"]?.ToString() ?? colores.Primario;
                            colores.Secundario = reader["ColorSecundario"]?.ToString() ?? colores.Secundario;
                            if (reader["DiasValidez"] != DBNull.Value)
                                colores.DiasValidez = Convert.ToInt32(reader["DiasValidez"]);
                        }
                    }
                }
            }
            catch { /* Usa defaults si falla SQL */ }
            return colores;
        }

        private List<string> ObtenerCamposConfiguradosDesdeSQL()
        {
            var campos = new List<string>();
            try
            {
                using (var con = new SqlConnection(Conexion.CadSQL))
                {
                    con.Open();
                    string sql = "SELECT NombreEtiqueta FROM CotizacionCamposConfig WHERE Activo = 1 ORDER BY Orden";
                    using (var cmd = new SqlCommand(sql, con))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) campos.Add(reader["NombreEtiqueta"].ToString());
                    }
                }
            }
            catch { }
            return campos;
        }

        // --- MÉTODO CORREGIDO Y OPTIMIZADO ---
        public void ImprimirDirectamente(string nombreImpresora)
        {
            try
            {
                QuestPDF.Settings.License = LicenseType.Community;
                var colorPrincipal = QuestPDF.Infrastructure.Color.FromHex(_hexPrimario);
                var colorSecundario = QuestPDF.Infrastructure.Color.FromHex(_hexSecundario);

                // --- Cálculos de Totales ---
                double total = _productos.Sum(p => p.Total);
                double subtotal = total / 1.16;
                double iva = (total / 1.16) * 0.16;
                double totalFinal = total;
                // ---------------------------

                var documento = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        // ESTRATEGIA: Usar Hoja Carta Completa (Vertical)
                        // Esto evita que la impresora intente rotar el contenido.
                        page.Size(PageSizes.Letter); // 8.5 x 11 pulgadas

                        page.MarginTop(0.5f, Unit.Centimetre);
                        page.MarginLeft(0.5f, Unit.Centimetre);
                        page.MarginRight(0.5f, Unit.Centimetre);

                        // BLOQUEO: Margen inferior gigante (396 puntos = 5.5 pulgadas)
                        // Esto fuerza a que todo el contenido se dibuje solo en la mitad superior.
                        page.MarginBottom(396);

                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(8).FontFamily("Arial"));

                        page.Header().Element(ComposeHeader);
                        page.Content().Element(ComposeContent);
                        page.Footer().Element(ComposeFooter);
                    });
                });

                void ComposeHeader(IContainer container)
                {
                    container.Column(col =>
                    {
                        col.Item().Height(4).Background(colorPrincipal);
                        col.Item().PaddingTop(5).Row(row =>
                        {
                            if (!string.IsNullOrEmpty(_logoPath) && File.Exists(_logoPath))
                                row.ConstantItem(100).Image(_logoPath).FitArea();

                            row.RelativeItem().PaddingLeft(10).Column(info =>
                            {
                                info.Item().Text(_nombreLugar.ToUpper()).Bold().FontSize(14).FontColor(colorPrincipal);
                                if (_datosTicket != null)
                                    foreach (var l in _datosTicket) info.Item().Text(l.Trim()).FontSize(7);
                            });

                            // CUADRO DE FOLIO Y FECHA
                            row.ConstantItem(120).Border(1).BorderColor(colorPrincipal).Padding(0).Column(box =>
                            {
                                box.Item().Background(colorPrincipal).Padding(2).AlignCenter().Text("COTIZACIÓN").Bold().FontColor(Colors.White);

                                box.Item().Padding(4).Column(c =>
                                {
                                    c.Item().AlignCenter().Text($"Folio: {_folio}").FontSize(11).Bold().FontColor(Colors.Black);
                                    c.Item().PaddingTop(2).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                                    c.Item().PaddingTop(2).AlignCenter().Text($"{DateTime.Now:dd/MMM/yyyy}").FontSize(8);
                                    c.Item().AlignCenter().Text($"{DateTime.Now:HH:mm} hrs").FontSize(7).FontColor(Colors.Grey.Darken2);

                                    // Vigencia calculada
                                    var fechaVigencia = DateTime.Now.AddDays(_diasValidez);
                                    c.Item().PaddingTop(3).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                                    c.Item().PaddingTop(2).AlignCenter().Text("Vigente hasta:").FontSize(6).Bold().FontColor(colorPrincipal);
                                    c.Item().AlignCenter().Text($"{fechaVigencia:dd/MMM/yyyy}").FontSize(8).Bold();
                                });
                            });
                        });

                        // Línea separadora
                        col.Item().PaddingTop(10).LineHorizontal(2).LineColor(colorSecundario);
                    });
                }
                void ComposeContent(IContainer container)
                {
                    container.PaddingVertical(5).Column(column =>
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

                                c.Item().PaddingTop(3).PaddingLeft(10).Column(d =>
                                {
                                    var nombreFinal = !string.IsNullOrEmpty(_cliente) ? _cliente : "Público General";
                                    d.Item().Text(nombreFinal).Bold().FontSize(11);
                                    d.Item().Text(_cliente ?? "Público General").Bold();
                                    if (Conexion.lugar == "TURBOLLANTAS" || !string.IsNullOrEmpty(_direccion))
                                        d.Item().Text($"Dirección: {_direccion ?? ""}");
                                    if (Conexion.lugar == "TURBOLLANTAS" || !string.IsNullOrEmpty(_telefono))
                                        d.Item().Text($"Teléfono: {_telefono ?? ""}");
                                    if (Conexion.lugar == "TURBOLLANTAS" || !string.IsNullOrEmpty(_rfc))
                                        d.Item().Text($"RFC: {_rfc ?? ""}");
                                });
                            });
                            // DERECHA: Observaciones
                            string notasAMostrar = !string.IsNullOrEmpty(_observaciones) ? _observaciones : "";

                            // Lógica TURBOLLANTAS original preservada
                            if (!string.IsNullOrEmpty(notasAMostrar) || Conexion.lugar == "TURBOLLANTAS")
                            {
                                row.RelativeItem().PaddingLeft(10).Column(c =>
                                {
                                    c.Item().Text("OBSERVACIONES").Bold().FontSize(9).FontColor(colorPrincipal);
                                    c.Item().Background(colorSecundario).Padding(5).Text(notasAMostrar).Italic().FontSize(8);
                                });
                            }
                        });

                        // 2. DATOS EXTRA
                        column.Item().PaddingTop(5).Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(text =>
                        {
                            var lista = !string.IsNullOrEmpty(_datos) ? _datos.Split(';') : Array.Empty<string>();
                            if (lista.Length > 0)
                            {
                                foreach (var d in lista)
                                {
                                    var p = d.Split(':');
                                    if (p.Length == 2)
                                    {
                                        text.Span($"{p[0].Trim()}: ").Bold().FontColor(colorPrincipal);
                                        text.Span($"{p[1].Trim()}    ");
                                    }
                                }
                            }
                            else if (Conexion.lugar == "TURBOLLANTAS")
                            {
                                foreach (var etiqueta in ObtenerCamposConfiguradosDesdeSQL())
                                {
                                    text.Span($"{etiqueta}: ").Bold().FontColor(colorPrincipal);
                                    text.Span("                           ");
                                }
                            }
                        });

                        // 3. TABLA PRODUCTOS
                        column.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30); // #
                                columns.ConstantColumn(40); // Cant
                                columns.RelativeColumn();   // Desc
                                columns.ConstantColumn(70); // P.Unit
                                columns.ConstantColumn(65); // TASA IVA (Estática)
                                columns.ConstantColumn(50); // DESC (Estática)
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
                            // Filas
                            for (int i = 0; i < _productos.Count; i++)
                            {
                                var item = _productos[i];
                                var bg = i % 2 == 0 ? Colors.White : colorSecundario;

                                IContainer CellStyle(IContainer c) => c.Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2).PaddingHorizontal(2);

                                table.Cell().Element(CellStyle).Text((i + 1).ToString()).FontSize(8).FontColor(Colors.Grey.Darken2);
                                table.Cell().Element(CellStyle).AlignCenter().Text(item.Cantidad.ToString("0.##")).Bold();
                                table.Cell().Element(CellStyle).Text(item.Nombre).FontSize(9);

                                table.Cell().Element(CellStyle).AlignRight().Text($"{item.PrecioUnitario:N2}");
                                table.Cell().Element(CellStyle).AlignRight().Text("16.00%");
                                table.Cell().Element(CellStyle).AlignRight().Text("$0.00");
                                table.Cell().Element(CellStyle).AlignRight().Text($"{item.Total:N2}").Bold();
                            }
                        });

                        // 4. TOTALES 
                        column.Item().AlignRight().PaddingTop(5).Row(row =>
                        {
                            row.ConstantItem(150).Border(0.5f).BorderColor(colorPrincipal).Column(colTotales =>
                            {
                                void FilaTotal(string etiqueta, string valor, bool esBold = false, float fontSize = 8, string colorFondo = null)
                                {
                                    var item = colTotales.Item();
                                    if (colorFondo != null) item = item.Background(colorFondo);

                                    item.PaddingHorizontal(3).PaddingVertical(1).Row(r =>
                                    {
                                        var estilo = TextStyle.Default.FontSize(fontSize);
                                        if (esBold) estilo = estilo.Bold();
                                        if (colorFondo != null) estilo = estilo.FontColor(Colors.White);

                                        r.RelativeItem().Text(etiqueta).Style(estilo);
                                        r.RelativeItem().AlignRight().Text(valor).Style(estilo);
                                    });
                                }

                                FilaTotal("SUBTOTAL:", $"{subtotal:N2}");
                                FilaTotal("DESCUENTO:", $"{_descuento:N2}");
                                FilaTotal("I.V.A. (16%):", $"{iva:N2}");
                                FilaTotal("TOTAL:", $"{totalFinal:C2}", true, 10, colorPrincipal);
                            });
                        });
                    });
                }

                void ComposeFooter(IContainer container)
                {
                    container.Column(c => {
                        if (_pieDeTicket != null)
                        {
                            foreach (var l in _pieDeTicket)
                            {
                                if (!string.IsNullOrWhiteSpace(l))
                                    c.Item().AlignCenter().Text(l.Trim()).Italic().FontSize(6);
                            }
                        }

                        c.Item().AlignCenter().Text(x => {
                            x.DefaultTextStyle(s => s.FontSize(6));
                            x.Span("Pág. ");
                            x.CurrentPageNumber();
                            x.Span(" de ");
                            x.TotalPages();
                        });
                    });
                }

                // CONFIGURACIÓN DE ALTA CALIDAD
                var configuracionImagen = new QuestPDF.Infrastructure.ImageGenerationSettings
                {
                    RasterDpi = 300, // 300 DPI para texto nítido
                    ImageCompressionQuality = ImageCompressionQuality.Best
                };

                // OPTIMIZACIÓN: Generar lista UNA sola vez antes de entrar al loop de impresión
                var imagenes = documento.GenerateImages(configuracionImagen).ToList();

                PrintDocument pd = new PrintDocument();
                pd.PrinterSettings.PrinterName = nombreImpresora;

                // CONFIGURACIÓN CLAVE: Papel Carta Vertical (Estándar)
                // 8.5" x 11" en centésimas de pulgada = 850 x 1100
                pd.DefaultPageSettings.PaperSize = new PaperSize("Carta", 850, 1100);
                pd.DefaultPageSettings.Landscape = false; // Vertical
                pd.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
                pd.OriginAtMargins = true;

                int pag = 0;
                pd.PrintPage += (s, e) => {
                    if (pag < imagenes.Count)
                    {
                        using (var ms = new MemoryStream(imagenes[pag]))
                        using (var img = System.Drawing.Image.FromStream(ms))
                        {
                            e.Graphics.DrawImage(img, 0, 0, 850, 1100);
                        }
                        pag++;
                        e.HasMorePages = (pag < imagenes.Count);
                    }
                };
                pd.Print();
            }
            catch (Exception ex) { MessageBox.Show("Error al imprimir: " + ex.Message); }
        }
    }
}