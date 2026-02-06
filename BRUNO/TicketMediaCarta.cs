using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Data.OleDb;
using System.Data.SqlClient; // Necesario para SqlConnection
using System.Linq;
using System.Windows.Forms;
// Alias para evitar conflicto
using QImage = QuestPDF.Infrastructure.Image;

namespace BRUNO
{
    public class TicketMediaCarta
    {
        // Estructura interna para colores
        private class ColoresPdf
        {
            public string Primario { get; set; } = "#720e1e"; // Guinda Default
            public string Secundario { get; set; } = "#FFD700"; // Dorado Default
            public int DiasValidez { get; set; } = 15;
        }

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

        // Variables de diseño dinámicas
        private string _hexPrimario;
        private string _hexSecundario;
        private int _diasValidez;

        public TicketMediaCarta(List<Producto> productos, string folio, double total, string cliente, string idCliente, string formaPago,
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

            // 1. CARGAR DATOS DE CLIENTE (ACCESS)
            CargarDatosCliente();

            // 2. CARGAR COLORES (SQL SERVER usando Conexion.CadSQL)
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

        // --- MÉTODO CORREGIDO: Usa Conexion.CadSQL ---
        private ColoresPdf ObtenerColoresDesdeSQL()
        {
            var colores = new ColoresPdf();
            try
            {
                // AQUÍ USAMOS TU VARIABLE ESTÁTICA DE CONEXIÓN
                string connectionString = Conexion.CadSQL;

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
            catch (Exception)
            {
                // Si falla la conexión a SQL, usamos los defaults (Guinda/Dorado) sin tronar el programa
            }
            return colores;
        }

        public void ImprimirDirectamente(string nombreImpresora)
        {
            try
            {
                QuestPDF.Settings.License = LicenseType.Community;

                // Preparamos los colores QuestPDF
                var colorPrincipal = Color.FromHex(_hexPrimario);
                var colorSecundario = Color.FromHex(_hexSecundario);
                var colorGrisClaro = Colors.Grey.Lighten3;

                var documento = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(new PageSize(612, 792)); // Carta
                        page.MarginTop(1, Unit.Centimetre);
                        page.MarginLeft(1, Unit.Centimetre);
                        page.MarginRight(1, Unit.Centimetre);
                        page.MarginBottom(396); // Media hoja

                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                        page.Header().Element(ComposeHeader);
                        page.Content().Element(ComposeContent);
                        page.Footer().Element(ComposeFooter);
                    });
                });

                // --- MÉTODOS LOCALES DE DISEÑO ---

                void ComposeHeader(IContainer container)
                {
                    container.Column(col =>
                    {
                        // BARRA SUPERIOR DE COLOR
                        col.Item().Height(6).Background(colorPrincipal);

                        col.Item().PaddingTop(10).Row(row =>
                        {
                            // LOGO
                            if (!string.IsNullOrEmpty(_logoPath) && File.Exists(_logoPath))
                            {
                                row.ConstantItem(130).Image(_logoPath).FitArea();
                            }

                            // DATOS EMPRESA
                            row.RelativeItem().PaddingLeft(15).Column(infoCol =>
                            {
                                infoCol.Item().Text(_nombreLugar.ToUpper()).Bold().FontSize(16).FontColor(colorPrincipal);

                                if (_datosTicket != null)
                                {
                                    foreach (var linea in _datosTicket)
                                    {
                                        if (!string.IsNullOrWhiteSpace(linea))
                                            infoCol.Item().Text(linea.Trim()).FontSize(8).FontColor(Colors.Grey.Darken2);
                                    }
                                }
                                // Forma de pago (Si no es Turbollantas)
                                if (Conexion.lugar != "TURBOLLANTAS")
                                {
                                    infoCol.Item().PaddingTop(5).Text($"Forma de pago: {_formaPago}").FontSize(8);
                                }
                            });

                            // CUADRO DE FOLIO Y FECHA
                            row.ConstantItem(120).Border(1).BorderColor(colorPrincipal).Padding(0).Column(box =>
                            {
                                box.Item().Background(colorPrincipal).Padding(2).AlignCenter().Text("COTIZACIÓN/VENTA").Bold().FontColor(Colors.White);

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
                                    var nombreFinal = !string.IsNullOrEmpty(_cliente) ? _cliente : "Público General";
                                    datos.Item().Text(nombreFinal).Bold().FontSize(11);

                                    if (!string.IsNullOrEmpty(_direccion)) datos.Item().Text(_direccion);
                                    if (!string.IsNullOrEmpty(_telefono)) datos.Item().Text($"Tel: {_telefono}");
                                    if (!string.IsNullOrEmpty(_rfc)) datos.Item().Text($"RFC: {_rfc}");
                                    if (!string.IsNullOrEmpty(_correo)) datos.Item().Text($"Email: {_correo}");
                                });
                            });

                            // DERECHA: Observaciones
                            string notasAMostrar = !string.IsNullOrEmpty(_observaciones) ? _observaciones : "";

                            // Lógica TURBOLLANTAS original preservada
                            if (!string.IsNullOrEmpty(notasAMostrar) || Conexion.lugar != "TURBOLLANTAS")
                            {
                                row.RelativeItem().PaddingLeft(10).Column(c =>
                                {
                                    c.Item().Text("OBSERVACIONES").Bold().FontSize(9).FontColor(colorPrincipal);
                                    c.Item().Background(colorGrisClaro).Padding(5).Text(notasAMostrar).Italic().FontSize(8);
                                });
                            }
                        });

                        // --- DATOS EXTRA (Placas, Modelo, etc) ---
                        if (!string.IsNullOrEmpty(_datos))
                        {
                            column.Item().PaddingBottom(10).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(text =>
                            {
                                var listaDatos = _datos.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var dato in listaDatos)
                                {
                                    var partes = dato.Split(new[] { ':' }, 2);
                                    if (partes.Length == 2)
                                    {
                                        text.Span($"{partes[0].Trim()}: ").Bold().FontColor(colorPrincipal);
                                        text.Span($"{partes[1].Trim()}    ");
                                    }
                                }
                            });
                        }

                        // --- TABLA DE PRODUCTOS ---
                        column.Item().Table(table =>
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

                            // Encabezado
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
                                var bg = i % 2 == 0 ? Colors.White : colorGrisClaro;

                                IContainer CellStyle(IContainer c) => c.Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2).PaddingHorizontal(2);

                                table.Cell().Element(CellStyle).Text((i + 1).ToString()).FontSize(8).FontColor(Colors.Grey.Darken2);
                                table.Cell().Element(CellStyle).AlignCenter().Text(item.Cantidad.ToString("0.##")).Bold();
                                table.Cell().Element(CellStyle).Text(item.Nombre).FontSize(9);

                                table.Cell().Element(CellStyle).AlignRight().Text($"{item.PrecioUnitario:N2}");
                                table.Cell().Element(CellStyle).AlignRight().Text("16.00%");
                                table.Cell().Element(CellStyle).AlignRight().Text("$0.00");
                                table.Cell().Element(CellStyle).AlignRight().Text($"{item.Total:N2}").Bold();
                            }

                            // Footer Totales
                            table.Footer(footer =>
                            {
                                footer.Cell().ColumnSpan(7).PaddingTop(5).AlignRight().Row(row =>
                                {
                                    row.ConstantItem(200).Border(1).BorderColor(colorPrincipal).Padding(5).Column(c =>
                                    {
                                        c.Item().Row(r =>
                                        {
                                            r.RelativeItem().Text("TOTAL:").Bold().FontSize(12).FontColor(colorPrincipal);
                                            r.RelativeItem().AlignRight().Text($"{_total:C2}").Bold().FontSize(13).FontColor(colorSecundario);
                                        });
                                    });
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

                        if (_pieDeTicket != null)
                        {
                            col.Item().PaddingTop(2).Column(c =>
                            {
                                foreach (var linea in _pieDeTicket)
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

                // Generación de imágenes e Impresión
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
    }
}