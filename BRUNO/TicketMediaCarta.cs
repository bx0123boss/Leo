using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static QuestPDF.Helpers.Colors;
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

        public void ImprimirDirectamente(string nombreImpresora)
        {
            try
            {
                QuestPDF.Settings.License = LicenseType.Community;
                var colorPrincipal = Color.FromHex(_hexPrimario);
                var colorSecundario = Color.FromHex(_hexSecundario);
                var colorGrisClaro = Colors.Grey.Lighten3;
                // --- Cálculos de Totales ---
                double total = _productos.Sum(p => p.Total);
                double subtotal = total/1.16;
                double iva = (total / 1.16) * 0.16;
                double totalFinal = total;
                // ---------------------------

                var documento = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        // TAMAÑO MEDIA CARTA EXACTO
                        page.Size(new PageSize(612, 396));
                        page.Margin(0.7f, Unit.Centimetre);
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

                            row.ConstantItem(110).Border(1).BorderColor(colorPrincipal).Column(box =>
                            {
                                box.Item().Background(colorPrincipal).Padding(1).AlignCenter().Text("COTIZACIÓN").Bold().FontColor(Colors.White);
                                box.Item().Padding(2).Column(c => {
                                    c.Item().AlignCenter().Text($"Folio: {_folio}").Bold();
                                    c.Item().AlignCenter().Text($"{DateTime.Now:dd/MM/yyyy}").FontSize(7);
                                });
                            });
                        });
                        col.Item().PaddingTop(5).LineHorizontal(1).LineColor(colorSecundario);
                    });
                }

                void ComposeContent(IContainer container)
                {
                    container.PaddingVertical(5).Column(column =>
                    {
                        // 1. DATOS CLIENTE (Lógica para TurboLlantas incluida)
                        column.Item().Row(row => {
                            row.RelativeItem().Column(c => {
                                c.Item().Text("DATOS DEL CLIENTE").Bold().FontSize(8).FontColor(colorPrincipal);
                                c.Item().PaddingLeft(5).Column(d => {
                                    d.Item().Text(_cliente ?? "Público General").Bold();
                                    if (Conexion.lugar == "TURBOLLANTAS" || !string.IsNullOrEmpty(_direccion))
                                        d.Item().Text($"Dirección: {_direccion ?? ""}");
                                    if (Conexion.lugar == "TURBOLLANTAS" || !string.IsNullOrEmpty(_telefono))
                                        d.Item().Text($"Teléfono: {_telefono ?? ""}");
                                    if (Conexion.lugar == "TURBOLLANTAS" || !string.IsNullOrEmpty(_rfc))
                                        d.Item().Text($"RFC: {_rfc ?? ""}");
                                });
                            });
                        });

                        // 2. DATOS EXTRA / VEHÍCULO
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
                                        text.Span($"{p[1].Trim()}   ");
                                    }
                                }
                            }
                            else if (Conexion.lugar == "TURBOLLANTAS")
                            {
                                foreach (var etiqueta in ObtenerCamposConfiguradosDesdeSQL())
                                {
                                    text.Span($"{etiqueta}: ").Bold().FontColor(colorPrincipal);
                                    text.Span("________________   ");
                                }
                            }
                        });

                        // 3. TABLA PRODUCTOS
                        column.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(cols => {
                                cols.ConstantColumn(25); cols.ConstantColumn(35);
                                cols.RelativeColumn(); cols.ConstantColumn(60); cols.ConstantColumn(60);
                            });

                            table.Header(h => {
                                IContainer Style(IContainer c) => c.Background(colorPrincipal).Padding(2);
                                h.Cell().Element(Style).Text("#").FontColor(Colors.White);
                                h.Cell().Element(Style).AlignCenter().Text("CANT").FontColor(Colors.White);
                                h.Cell().Element(Style).Text("DESCRIPCIÓN").FontColor(Colors.White);
                                h.Cell().Element(Style).AlignRight().Text("P.UNIT").FontColor(Colors.White);
                                h.Cell().Element(Style).AlignRight().Text("TOTAL").FontColor(Colors.White);
                            });

                            for (int i = 0; i < _productos.Count; i++)
                            {
                                var item = _productos[i];
                                IContainer Style(IContainer c) => c.BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(1);
                                table.Cell().Element(Style).Text((i + 1).ToString());
                                table.Cell().Element(Style).AlignCenter().Text(item.Cantidad.ToString("0.##"));
                                table.Cell().Element(Style).Text(item.Nombre).FontSize(7);
                                table.Cell().Element(Style).AlignRight().Text($"{item.PrecioUnitario:N2}");
                                table.Cell().Element(Style).AlignRight().Text($"{item.Total:N2}").Bold();
                            }
                        });

                        // 4. TOTALES DESGLOSADOS
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
                                        if (colorFondo != null) estilo = estilo.FontColor(Colors.White); // Letra blanca si hay fondo

                                        r.RelativeItem().Text(etiqueta).Style(estilo);
                                        r.RelativeItem().AlignRight().Text(valor).Style(estilo);
                                    });
                                }

                                FilaTotal("SUBTOTAL:", $"{subtotal:N2}");
                                FilaTotal("DESCUENTO:", $"{_descuento:N2}");
                                FilaTotal("I.V.A. (16%):", $"{iva:N2}");

                                // Fila de Total con color de fondo para resaltar
                                FilaTotal("TOTAL:", $"{totalFinal:C2}", true, 10, colorPrincipal);
                            });
                        });

                        // TOTAL FINAL EN TEXTO (OPCIONAL)
                        // column.Item().AlignRight().Text($"Son: {ConvertirNumeroALetras(totalFinal)}").FontSize(7).Italic();
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

                // IMPRESIÓN
                var imagenes = documento.GenerateImages();
                PrintDocument pd = new PrintDocument();
                pd.PrinterSettings.PrinterName = nombreImpresora;
                pd.DefaultPageSettings.PaperSize = new PaperSize("MediaCarta", 612, 396);
                int pag = 0;
                pd.PrintPage += (s, e) => {
                    if (pag < imagenes.Count())
                    {
                        using (var ms = new MemoryStream(imagenes.ElementAt(pag)))
                        using (var img = System.Drawing.Image.FromStream(ms))
                            e.Graphics.DrawImage(img, 0, 0, 612f * 100 / 72, 396f * 100 / 72);
                        pag++; e.HasMorePages = (pag < imagenes.Count());
                    }
                };
                pd.Print();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}