using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;

public class TicketPrinter
{
    private string[] _encabezados;
    private string[] _pieDePagina;
    private string _logoPath;
    private List<Producto> _productos;
    private string _folio;
    private string _mesa;
    private string _mesero;
    private double _total;
    private Dictionary<string, double> _totales = new Dictionary<string, double>();
    private bool _corte;
    private string formaPago;

    // Variables para impresión dividida de corte
    private string _tituloSeccionCorte;
    private List<ItemCorteCategoria> _itemsCategoriasCorte;
    private List<ItemCorteSalida> _itemsSalidasCorte;
    private double _totalSalidasCorte;

    public TicketPrinter(string[] encabezados, string[] pieDePagina, string logoPath, List<Producto> productos, string folio, string mesa, string mesero, double total, bool corte, Dictionary<string, double> totales, string formaPago = null)
    {
        _encabezados = encabezados;
        _pieDePagina = pieDePagina;
        _logoPath = logoPath;
        _productos = productos;
        _folio = folio;
        if (mesa != "")
            _mesa = mesa;
        if (mesero != "")
            _mesero = mesero;
        _total = total;
        _corte = corte;
        _totales = totales;
        this.formaPago = formaPago;
        if (formaPago != null)
            this.formaPago = formaPago;
    }

    public TicketPrinter(List<Producto> productos, string mesa, string mesero)
    {
        _productos = productos;
        if (mesa != "")
            _mesa = mesa;
        if (mesero != "")
            _mesero = mesero;
    }

    public void ImprimirComanda(string printerName = null)
    {
        PrintDocument pd = new PrintDocument();
        if (!string.IsNullOrEmpty(printerName))
        {
            pd.PrinterSettings.PrinterName = printerName;
        }
        pd.PrintPage += new PrintPageEventHandler(ImprimirPaginaComanda);
        pd.Print();
    }

    public void ImprimirTicket(string printerName = null)
    {
        PrintDocument pd = new PrintDocument();
        if (!string.IsNullOrEmpty(printerName))
        {
            pd.PrinterSettings.PrinterName = printerName;
        }
        pd.PrintPage += new PrintPageEventHandler(ImprimirPagina);
        pd.Print();
    }

    public void ImprimirCategoriasCorte(string tituloSeccion, string fechaApertura, List<ItemCorteCategoria> items, string printerName = null)
    {
        _tituloSeccionCorte = tituloSeccion;
        _encabezados = new string[]
        {
            "******* DESGLOSE DE CORTE *******",
            tituloSeccion,
            "Apertura: " + fechaApertura,
            "Corte: " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()
        };
        _itemsCategoriasCorte = items;

        PrintDocument pd = new PrintDocument();
        if (!string.IsNullOrEmpty(printerName))
        {
            pd.PrinterSettings.PrinterName = printerName;
        }
        pd.PrintPage += new PrintPageEventHandler(ImprimirPaginaCategoriasCorte);
        pd.Print();
    }

    public void ImprimirSalidasCorte(string fechaApertura, List<ItemCorteSalida> salidas, double totalSalidas, string printerName = null)
    {
        _encabezados = new string[]
        {
            "******* DETALLE DE SALIDAS *******",
            "Apertura: " + fechaApertura,
            "Corte: " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()
        };
        _itemsSalidasCorte = salidas;
        _totalSalidasCorte = totalSalidas;

        PrintDocument pd = new PrintDocument();
        if (!string.IsNullOrEmpty(printerName))
        {
            pd.PrinterSettings.PrinterName = printerName;
        }
        pd.PrintPage += new PrintPageEventHandler(ImprimirPaginaSalidasCorte);
        pd.Print();
    }

    private void ImprimirPaginaComanda(object sender, PrintPageEventArgs e)
    {
        int posicion = 10;
        int leftMargin = 10;
        int cantidadWidth = 60;
        int productoWidth = 150;
        int comentarioWidth = 150;

        e.Graphics.DrawString("   ********  COMANDA  ********",
                             new Font("Arial", 12, FontStyle.Bold),
                             Brushes.Black, leftMargin, posicion);
        posicion += 25;
        e.Graphics.DrawString($"FECHA: {DateTime.Now:dd/MM/yyyy HH:mm}",
                            new Font("Arial", 10),
                            Brushes.Black, leftMargin, posicion);
        posicion += 20;
        e.Graphics.DrawString($"Mesa: {_mesa}  , Mesero: {_mesero}",
                            new Font("Arial", 10),
                            Brushes.Black, leftMargin, posicion);
        posicion += 20;

        e.Graphics.DrawString("Cant", new Font("Arial", 10, FontStyle.Bold),
                             Brushes.Black, leftMargin, posicion);
        e.Graphics.DrawString("Producto", new Font("Arial", 10, FontStyle.Bold),
                             Brushes.Black, leftMargin + cantidadWidth, posicion);
        e.Graphics.DrawString("Comentario", new Font("Arial", 10, FontStyle.Bold),
                             Brushes.Black, leftMargin + cantidadWidth + productoWidth, posicion);

        posicion += 20;
        e.Graphics.DrawLine(new Pen(Color.Black), leftMargin, posicion,
                           leftMargin + cantidadWidth + productoWidth + comentarioWidth, posicion);
        posicion += 10;

        Font productFont = new Font("Arial", 9);

        foreach (var producto in _productos)
        {
            List<string> lineasProducto = DivideTexto(e.Graphics, producto.Nombre, productFont, productoWidth);
            List<string> lineasComentario = DivideTexto(e.Graphics, producto.Comentario ?? "", productFont, comentarioWidth);

            int lineasNecesarias = Math.Max(lineasProducto.Count, lineasComentario.Count);

            for (int i = 0; i < lineasNecesarias; i++)
            {
                if (i == 0)
                {
                    e.Graphics.DrawString(producto.Cantidad.ToString("0.00"), productFont,
                                        Brushes.Black, leftMargin, posicion);
                }

                if (i < lineasProducto.Count)
                {
                    e.Graphics.DrawString(lineasProducto[i], productFont,
                                        Brushes.Black, leftMargin + cantidadWidth, posicion);
                }

                if (i < lineasComentario.Count)
                {
                    e.Graphics.DrawString(lineasComentario[i], productFont,
                                        Brushes.Black, leftMargin + cantidadWidth + productoWidth, posicion);
                }

                posicion += 15;
            }

            posicion += 5;
        }
    }

    private void ImprimirPagina(object sender, PrintPageEventArgs e)
    {
        int posicion = 10;
        float ticketWidth = 280;
        if (!_corte)
        {
            if (System.IO.File.Exists(_logoPath))
            {
                Image logo = Image.FromFile(_logoPath);

                int origWidth = logo.Width;
                int origHeight = logo.Height;
                int targetWidth = origWidth;
                int targetHeight = origHeight;

                if (origWidth > ticketWidth)
                {
                    double ratio = (double)ticketWidth / origWidth;
                    targetWidth = (int)ticketWidth;
                    targetHeight = (int)(origHeight * ratio);
                }

                int xPos = (int)((ticketWidth - targetWidth) / 2);
                e.Graphics.DrawImage(logo, new Rectangle(xPos, posicion, targetWidth, targetHeight));
                posicion += targetHeight + 10;
            }
            e.Graphics.DrawString("   ********  NOTA DE VENTA  ********", new Font("Arial", 12, FontStyle.Bold), Brushes.Black, new Point(1, posicion));
            posicion += 20;
        }

        StringFormat centerFormat = new StringFormat();
        centerFormat.Alignment = StringAlignment.Center;

        foreach (var encabezado in _encabezados)
        {
            e.Graphics.DrawString(encabezado, new Font("Arial", 10, FontStyle.Bold),
                                  Brushes.Black,
                                  new RectangleF(0, posicion, ticketWidth, 20),
                                  centerFormat);
            posicion += 18;
        }

        if (!_corte)
        {
            e.Graphics.DrawString("FOLIO DE VENTA: " + _folio, new Font("Arial", 10, FontStyle.Bold), Brushes.Black, new Point(1, posicion));
            posicion += 18;
            if (formaPago != null)
            {
                e.Graphics.DrawString("MÉTODO DE PAGO: " + formaPago, new Font("Arial", 10, FontStyle.Bold), Brushes.Black, new Point(1, posicion));
                posicion += 18;
            }
            if (_mesa != null)
            {
                e.Graphics.DrawString("MESA: " + _mesa, new Font("Arial", 12, FontStyle.Bold), Brushes.Black, new Point(1, posicion));
                posicion += 20;
                e.Graphics.DrawString("LE ATENDIO: " + _mesero, new Font("Arial", 12, FontStyle.Bold), Brushes.Black, new Point(1, posicion));
                posicion += 20;
            }
        }

        e.Graphics.DrawString("FECHA: " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString(), new Font("Arial", 12, FontStyle.Bold), Brushes.Black, new Point(1, posicion));
        posicion += 30;

        if (_productos.Count > 0)
        {
            if (!_corte)
            {
                e.Graphics.DrawString("Cant   Producto         P.unit Importe", new Font("Arial", 12, FontStyle.Bold), Brushes.Black, new Point(1, posicion));
            }
            else
            {
                e.Graphics.DrawString("Folio         Sin IVA        Monto Total", new Font("Arial", 12, FontStyle.Bold), Brushes.Black, new Point(1, posicion));
            }
            posicion += 20;
            e.Graphics.DrawLine(new Pen(Color.Black), 1, posicion, 420, posicion);
            posicion += 10;
        }

        StringFormat sf = new StringFormat();
        sf.Alignment = StringAlignment.Far;
        if (!_corte)
        {
            foreach (var producto in _productos)
            {
                Font productFont = new Font("Arial", 8, FontStyle.Regular);
                float maxWidth = 145;
                List<string> lineasProducto = DivideTexto(e.Graphics, producto.Nombre, productFont, maxWidth);

                e.Graphics.DrawString(producto.Cantidad.ToString("0.00", CultureInfo.InvariantCulture), productFont, Brushes.Black, new Point(1, posicion));
                for (int j = 0; j < lineasProducto.Count; j++)
                {
                    if (j > 0)
                    {
                        e.Graphics.DrawString("", productFont, Brushes.Black, new Point(1, posicion));
                    }

                    e.Graphics.DrawString(lineasProducto[j], productFont, Brushes.Black, new Point(40, posicion));

                    if (j == lineasProducto.Count - 1)
                    {
                        e.Graphics.DrawString($"{producto.PrecioUnitario:C}", productFont, Brushes.Black, new Point(215, posicion), sf);
                        e.Graphics.DrawString($"{producto.Total:C}", productFont, Brushes.Black, new Point(280, posicion), sf);
                    }

                    posicion += (j == lineasProducto.Count - 1) ? 20 : 15;
                }
            }

            if (_totales.Count > 0)
            {
                e.Graphics.DrawLine(new Pen(Color.Black), 210, posicion + 10, 420, posicion + 10);
                posicion += 15;
                foreach (var par in _totales)
                {
                    e.Graphics.DrawString($"{par.Key}: {par.Value:C}", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, new Point(280, posicion), sf);
                    posicion += 20;
                }
            }

            foreach (var pie in _pieDePagina)
            {
                e.Graphics.DrawString(pie, new Font("Arial", 10, FontStyle.Bold), Brushes.Black, new Point(1, posicion));
                posicion += 20;
            }
        }
        else
        {
            foreach (var producto in _productos)
            {
                e.Graphics.DrawString(producto.Nombre, new Font("Arial", 8, FontStyle.Regular), Brushes.Black, new Point(1, posicion));
                e.Graphics.DrawString($"{producto.PrecioUnitario:C}", new Font("Arial", 8, FontStyle.Regular), Brushes.Black, new Point(140, posicion), sf);
                e.Graphics.DrawString($"{producto.Total:C}", new Font("Arial", 8, FontStyle.Regular), Brushes.Black, new Point(240, posicion), sf);
                posicion += 20;
            }
            if (_totales.Count > 0)
            {
                e.Graphics.DrawLine(new Pen(Color.Black), 210, posicion + 10, 420, posicion + 10);
                posicion += 15;
                foreach (var par in _totales)
                {
                    e.Graphics.DrawString($"{par.Key}:  {par.Value:C}", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, new Point(270, posicion), sf);
                    posicion += 20;
                }
            }
        }
    }

    private void ImprimirPaginaCategoriasCorte(object sender, PrintPageEventArgs e)
    {
        int posicion = 10;
        float ticketWidth = 280;

        StringFormat centerFormat = new StringFormat { Alignment = StringAlignment.Center };
        StringFormat rightFormat = new StringFormat { Alignment = StringAlignment.Far };

        foreach (var encabezado in _encabezados)
        {
            e.Graphics.DrawString(encabezado, new Font("Arial", 9, FontStyle.Bold),
                                  Brushes.Black, new RectangleF(0, posicion, ticketWidth, 18), centerFormat);
            posicion += 17;
        }

        posicion += 8;

        Font headerFont = new Font("Arial", 9, FontStyle.Bold);
        e.Graphics.DrawString("Categoría / Grupo", headerFont, Brushes.Black, new Point(1, posicion));
        e.Graphics.DrawString("Pzs", headerFont, Brushes.Black, new Point(195, posicion), rightFormat);
        e.Graphics.DrawString("Total", headerFont, Brushes.Black, new Point(280, posicion), rightFormat);
        posicion += 18;

        e.Graphics.DrawLine(new Pen(Color.Black), 1, posicion, ticketWidth, posicion);
        posicion += 8;

        Font rowFont = new Font("Arial", 8, FontStyle.Regular);
        Font totalFont = new Font("Arial", 9, FontStyle.Bold);

        foreach (var item in _itemsCategoriasCorte)
        {
            bool esTotalGeneral = item.Nombre.Equals("TOTAL GENERAL", StringComparison.OrdinalIgnoreCase);

            if (esTotalGeneral)
            {
                posicion += 4;
                e.Graphics.DrawLine(new Pen(Color.Black), 1, posicion, ticketWidth, posicion);
                posicion += 8;
            }

            Font currentFont = esTotalGeneral ? totalFont : rowFont;
            List<string> lineasNombre = DivideTexto(e.Graphics, item.Nombre, currentFont, 150);

            for (int j = 0; j < lineasNombre.Count; j++)
            {
                e.Graphics.DrawString(lineasNombre[j], currentFont, Brushes.Black, new Point(1, posicion));

                if (j == lineasNombre.Count - 1)
                {
                    e.Graphics.DrawString(item.Piezas.ToString("N0"), currentFont, Brushes.Black, new Point(195, posicion), rightFormat);
                    e.Graphics.DrawString($"{item.Total:C2}", currentFont, Brushes.Black, new Point(280, posicion), rightFormat);
                }

                posicion += 15;
            }
            posicion += 3;
        }
    }

    private void ImprimirPaginaSalidasCorte(object sender, PrintPageEventArgs e)
    {
        int posicion = 10;
        float ticketWidth = 280;

        StringFormat centerFormat = new StringFormat { Alignment = StringAlignment.Center };
        StringFormat rightFormat = new StringFormat { Alignment = StringAlignment.Far };

        foreach (var encabezado in _encabezados)
        {
            e.Graphics.DrawString(encabezado, new Font("Arial", 9, FontStyle.Bold),
                                  Brushes.Black, new RectangleF(0, posicion, ticketWidth, 18), centerFormat);
            posicion += 17;
        }

        posicion += 8;

        Font headerFont = new Font("Arial", 9, FontStyle.Bold);
        e.Graphics.DrawString("Concepto", headerFont, Brushes.Black, new Point(1, posicion));
        e.Graphics.DrawString("Monto", headerFont, Brushes.Black, new Point(280, posicion), rightFormat);
        posicion += 18;

        e.Graphics.DrawLine(new Pen(Color.Black), 1, posicion, ticketWidth, posicion);
        posicion += 8;

        Font rowFont = new Font("Arial", 8, FontStyle.Regular);

        if (_itemsSalidasCorte.Count == 0)
        {
            e.Graphics.DrawString("Sin salidas registradas en este turno.", rowFont, Brushes.Black, new Point(1, posicion));
            posicion += 20;
        }
        else
        {
            foreach (var salida in _itemsSalidasCorte)
            {
                List<string> lineasConcepto = DivideTexto(e.Graphics, salida.Concepto, rowFont, 195);

                for (int j = 0; j < lineasConcepto.Count; j++)
                {
                    e.Graphics.DrawString(lineasConcepto[j], rowFont, Brushes.Black, new Point(1, posicion));

                    if (j == lineasConcepto.Count - 1)
                    {
                        e.Graphics.DrawString($"{salida.Monto:C2}", rowFont, Brushes.Black, new Point(280, posicion), rightFormat);
                    }

                    posicion += 15;
                }
                posicion += 4;
            }
        }

        posicion += 5;
        e.Graphics.DrawLine(new Pen(Color.Black), 1, posicion, ticketWidth, posicion);
        posicion += 10;

        e.Graphics.DrawString($"TOTAL SALIDAS: {_totalSalidasCorte:C2}", new Font("Arial", 10, FontStyle.Bold),
                              Brushes.Black, new Point(280, posicion), rightFormat);
        posicion += 35;

        e.Graphics.DrawLine(new Pen(Color.Black), 40, posicion, 240, posicion);
        posicion += 5;
        e.Graphics.DrawString("Firma Cajero / Supervisor", new Font("Arial", 8, FontStyle.Regular),
                              Brushes.Black, new RectangleF(0, posicion, ticketWidth, 16), centerFormat);
    }

    private List<string> DivideTexto(Graphics g, string texto, Font font, float maxWidth)
    {
        List<string> lineas = new List<string>();
        string[] palabras = texto.Split(' ');
        string lineaActual = "";

        foreach (string palabra in palabras)
        {
            string prueba = lineaActual + (lineaActual.Length > 0 ? " " : "") + palabra;
            if (g.MeasureString(prueba, font).Width <= maxWidth)
            {
                lineaActual = prueba;
            }
            else
            {
                if (lineaActual.Length > 0)
                {
                    lineas.Add(lineaActual);
                    lineaActual = palabra;
                }
                else
                {
                    for (int i = 0; i < palabra.Length; i++)
                    {
                        prueba = lineaActual + palabra[i];
                        if (g.MeasureString(prueba, font).Width > maxWidth)
                        {
                            lineas.Add(lineaActual);
                            lineaActual = "";
                            i--;
                        }
                        else
                        {
                            lineaActual = prueba;
                        }
                    }
                }
            }
        }

        if (lineaActual.Length > 0)
            lineas.Add(lineaActual);

        return lineas;
    }

    public void AbrirCajon(string printerName)
    {
        if (string.IsNullOrEmpty(printerName)) return;

        try
        {
            byte[] drawerPulse = new byte[] { 27, 112, 0, 25, 250 };
            RawPrinterHelper.SendBytesToPrinter(printerName, drawerPulse);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al intentar abrir el cajón: {ex.Message}");
        }
    }
}

public class Producto
{
    public string Nombre { get; set; }
    public double Cantidad { get; set; }
    public double PrecioUnitario { get; set; }
    public double Total { get; set; }
    public string Comentario { get; set; }
}

public class ItemCorteCategoria
{
    public string Nombre { get; set; }
    public double Piezas { get; set; }
    public double Total { get; set; }
}

public class ItemCorteSalida
{
    public string Concepto { get; set; }
    public double Monto { get; set; }
}