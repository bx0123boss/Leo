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

namespace JaegerSoft
{
    public class ReciboAbonoCarta
    {
        private class ColoresPdf
        {
            public string Primario { get; set; } = "#720e1e";
            public string Secundario { get; set; } = "#FFD700";
        }

        private string _folio;
        private double _adeudoAnterior;
        private double _abono;
        private double _saldoInsoluto;
        private string _cliente, _idCliente, _direccion, _rfc, _telefono, _correo, _CP;
        private string _formaPago;
        private string _nombreLugar, _logoPath;
        private string[] _datosTicket, _pieDeTicket;
        private string _hexPrimario, _hexSecundario;

        public ReciboAbonoCarta(string folio, string idCliente, string clienteNombre, double adeudoAnterior, double abono, double saldoInsoluto, string formaPago,
                               string nombreLugar, string logoPath, string[] datosTicket, string[] pieDeTicket)
        {
            _folio = folio;
            _idCliente = idCliente;
            _cliente = clienteNombre;
            _adeudoAnterior = adeudoAnterior;
            _abono = abono;
            _saldoInsoluto = saldoInsoluto;
            _formaPago = formaPago;
            _nombreLugar = nombreLugar;
            _logoPath = logoPath;
            _datosTicket = datosTicket;
            _pieDeTicket = pieDeTicket;

            // 1. Cargar información ampliada del cliente desde Access
            CargarDatosCliente();

            // 2. Cargar paleta de colores desde SQL Server
            var colores = ObtenerColoresDesdeSQL();
            _hexPrimario = colores.Primario;
            _hexSecundario = colores.Secundario;
        }

        private void CargarDatosCliente()
        {
            if (string.IsNullOrEmpty(_idCliente) || _idCliente == "0") return;
            try
            {
                using (OleDbConnection con = new OleDbConnection(Conexion.CadCon))
                {
                    con.Open();
                    string query = "SELECT Direccion, RFC, Telefono, Correo, CP FROM Clientes WHERE Id = " + _idCliente;
                    using (OleDbCommand cmd = new OleDbCommand(query, con))
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            _direccion = reader["Direccion"]?.ToString() ?? "";
                            _rfc = reader["RFC"]?.ToString() ?? "";
                            _telefono = reader["Telefono"]?.ToString() ?? "";
                            _correo = reader["Correo"]?.ToString() ?? "";
                            _CP = reader["CP"]?.ToString() ?? "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al consultar datos de cliente en Access: " + ex.Message);
            }
        }

        private ColoresPdf ObtenerColoresDesdeSQL()
        {
            var colores = new ColoresPdf();
            try
            {
                using (var conn = new SqlConnection(Conexion.CadSQL))
                {
                    conn.Open();
                    string query = "SELECT TOP 1 ColorPrimario, ColorSecundario FROM ConfiguracionApariencia";
                    using (var cmd = new SqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            colores.Primario = reader["ColorPrimario"]?.ToString() ?? colores.Primario;
                            colores.Secundario = reader["ColorSecundario"]?.ToString() ?? colores.Secundario;
                        }
                    }
                }
            }
            catch
            {
                /* Si falla SQL Server usa colores predeterminados */
            }
            return colores;
        }

        public void ImprimirDirectamente(string nombreImpresora)
        {
            try
            {
                QuestPDF.Settings.License = LicenseType.Community;
                var colorPrincipal = QuestPDF.Infrastructure.Color.FromHex(_hexPrimario);
                var colorSecundario = QuestPDF.Infrastructure.Color.FromHex(_hexSecundario);

                var documento = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        // Formato Hoja Carta
                        page.Size(PageSizes.Letter);

                        page.MarginTop(1.0f, Unit.Centimetre);
                        page.MarginLeft(1.0f, Unit.Centimetre);
                        page.MarginRight(1.0f, Unit.Centimetre);
                        page.MarginBottom(1.0f, Unit.Centimetre);

                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

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
                        col.Item().PaddingTop(6).Row(row =>
                        {
                            if (!string.IsNullOrEmpty(_logoPath) && File.Exists(_logoPath))
                                row.ConstantItem(110).Image(_logoPath).FitArea();

                            row.RelativeItem().PaddingLeft(10).Column(info =>
                            {
                                info.Item().Text(_nombreLugar.ToUpper()).Bold().FontSize(15).FontColor(colorPrincipal);
                                if (_datosTicket != null)
                                {
                                    foreach (var l in _datosTicket)
                                        info.Item().Text(l.Trim()).FontSize(8);
                                }
                            });

                            // Encabezado Recibo de Abono
                            row.ConstantItem(150).Border(1).BorderColor(colorPrincipal).Column(box =>
                            {
                                box.Item().Background(colorPrincipal).Padding(3).AlignCenter()
                                   .Text("RECIBO DE ABONO").Bold().FontColor(Colors.White).FontSize(10);

                                box.Item().Padding(5).Column(c =>
                                {
                                    c.Item().AlignCenter().Text($"Folio Venta: {_folio}").FontSize(10).Bold();
                                    c.Item().PaddingTop(2).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                                    c.Item().PaddingTop(2).AlignCenter().Text($"{DateTime.Now:dd/MMM/yyyy HH:mm} hrs").FontSize(8);
                                });
                            });
                        });

                        col.Item().PaddingTop(10).LineHorizontal(2).LineColor(colorSecundario);
                    });
                }

                void ComposeContent(IContainer container)
                {
                    container.PaddingVertical(10).Column(column =>
                    {
                        // DATOS DEL CLIENTE
                        column.Item().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(8).Column(c =>
                        {
                            c.Item().Row(r =>
                            {
                                r.ConstantItem(5).Height(14).Background(colorSecundario);
                                r.RelativeItem().PaddingLeft(5).Text("DATOS DEL CLIENTE").Bold().FontSize(10).FontColor(colorPrincipal);
                            });

                            c.Item().PaddingTop(5).Row(r =>
                            {
                                r.RelativeItem().Column(d =>
                                {
                                    d.Item().Text($"Cliente: {_cliente}").Bold().FontSize(10);
                                    if (!string.IsNullOrEmpty(_direccion)) d.Item().Text($"Dirección: {_direccion} CP: {_CP}");
                                    if (!string.IsNullOrEmpty(_telefono)) d.Item().Text($"Teléfono: {_telefono}");
                                });

                                r.RelativeItem().Column(d =>
                                {
                                    d.Item().Text($"ID Cliente: {_idCliente}").Bold();
                                    if (!string.IsNullOrEmpty(_rfc)) d.Item().Text($"RFC: {_rfc}");
                                    if (!string.IsNullOrEmpty(_correo)) d.Item().Text($"Correo: {_correo}");
                                    d.Item().Text($"Método de Pago: {_formaPago}").Bold().FontColor(colorPrincipal);
                                });
                            });
                        });

                        // DESGLOSE FINANCIERO DEL ABONO
                        column.Item().PaddingTop(15).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(100);
                                columns.RelativeColumn();
                                columns.ConstantColumn(120);
                            });

                            table.Header(header =>
                            {
                                IContainer HeaderStyle(IContainer c) => c.Background(colorPrincipal).PaddingVertical(5).PaddingHorizontal(5);

                                header.Cell().Element(HeaderStyle).AlignCenter().Text("FOLIO/REF.").FontColor(Colors.White).Bold();
                                header.Cell().Element(HeaderStyle).Text("CONCEPTO").FontColor(Colors.White).Bold();
                                header.Cell().Element(HeaderStyle).AlignRight().Text("MONTO").FontColor(Colors.White).Bold();
                            });

                            // Fila Adeudo Anterior
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignCenter().Text(_folio);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("Saldo/Adeudo Anterior");
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text($"{_adeudoAnterior:C2}");

                            // Fila Abono
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignCenter().Text(_folio);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text($"Abono Realizado ({_formaPago})").Bold();
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text($"- {_abono:C2}").Bold().FontColor(colorPrincipal);
                        });

                        // RESUMEN / SALDO INSOLUTO
                        column.Item().AlignRight().PaddingTop(10).Row(row =>
                        {
                            row.ConstantItem(220).Border(1).BorderColor(colorPrincipal).Column(colTotales =>
                            {
                                colTotales.Item().Padding(4).Row(r =>
                                {
                                    r.RelativeItem().Text("ADEUDO ANTERIOR:").FontSize(9);
                                    r.RelativeItem().AlignRight().Text($"{_adeudoAnterior:C2}").FontSize(9);
                                });

                                colTotales.Item().Padding(4).Row(r =>
                                {
                                    r.RelativeItem().Text("ABONO APLICADO:").Bold().FontSize(9);
                                    r.RelativeItem().AlignRight().Text($"{_abono:C2}").Bold().FontSize(9);
                                });

                                colTotales.Item().Background(colorPrincipal).Padding(5).Row(r =>
                                {
                                    r.RelativeItem().Text("SALDO INSOLUTO:").Bold().FontSize(10).FontColor(Colors.White);
                                    r.RelativeItem().AlignRight().Text($"{_saldoInsoluto:C2}").Bold().FontSize(10).FontColor(Colors.White);
                                });
                            });
                        });

                        // FIRMAS Y CONFORMIDAD
                        //column.Item().PaddingTop(50).Row(row =>
                        //{
                        //    row.RelativeItem().PaddingHorizontal(20).Column(f =>
                        //    {
                        //        f.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
                        //        f.Item().PaddingTop(3).AlignCenter().Text("Firma del Cliente").FontSize(8).Bold();
                        //    });

                        //    row.RelativeItem().PaddingHorizontal(20).Column(f =>
                        //    {
                        //        f.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
                        //        f.Item().PaddingTop(3).AlignCenter().Text("Firma / Sello de Recepción").FontSize(8).Bold();
                        //    });
                        //});
                    });
                }

                void ComposeFooter(IContainer container)
                {
                    container.Column(c =>
                    {
                        if (_pieDeTicket != null)
                        {
                            foreach (var l in _pieDeTicket)
                            {
                                if (!string.IsNullOrWhiteSpace(l))
                                    c.Item().AlignCenter().Text(l.Trim()).Italic().FontSize(7);
                            }
                        }

                        c.Item().PaddingTop(3).AlignCenter().Text(x =>
                        {
                            x.DefaultTextStyle(s => s.FontSize(7));
                            x.Span("Pág. ");
                            x.CurrentPageNumber();
                            x.Span(" de ");
                            x.TotalPages();
                        });
                    });
                }

                var configuracionImagen = new QuestPDF.Infrastructure.ImageGenerationSettings
                {
                    RasterDpi = 300,
                    ImageCompressionQuality = ImageCompressionQuality.Best
                };

                var imagenes = documento.GenerateImages(configuracionImagen).ToList();

                PrintDocument pd = new PrintDocument();
                pd.PrinterSettings.PrinterName = nombreImpresora;
                pd.DefaultPageSettings.PaperSize = new PaperSize("Carta", 850, 1100);
                pd.DefaultPageSettings.Landscape = false;
                pd.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
                pd.OriginAtMargins = true;

                int pag = 0;
                pd.PrintPage += (s, e) =>
                {
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
            catch (Exception ex)
            {
                MessageBox.Show("Error al imprimir recibo de abono: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}