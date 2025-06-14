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
    public TicketPrinter(string[] encabezados, string[] pieDePagina, string logoPath, List<Producto> productos, string folio, string mesa, string mesero, double total, bool corte, Dictionary<string, double> totales)
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

    private void ImprimirPagina(object sender, PrintPageEventArgs e)
    {
        int posicion = 10;
        if (!_corte)
        {
            // Dibujar el logo
            if (System.IO.File.Exists(_logoPath))
            {
                Image logo = Image.FromFile(_logoPath);
                e.Graphics.DrawImage(logo, new PointF(1, posicion));
                posicion += 180;
            }
            e.Graphics.DrawString("   ********  NOTA DE VENTA  ********", new Font("Arial", 12, FontStyle.Bold), Brushes.Black, new Point(1, posicion));
            posicion += 20;
        }
        // Dibujar los encabezados
        foreach (var encabezado in _encabezados)
        {
            e.Graphics.DrawString(encabezado, new Font("Arial", 10, FontStyle.Bold), Brushes.Black, new Point(1, posicion));
            posicion += 18;
        }
        if (!_corte)
        {
            // Dibujar la información del ticket
            e.Graphics.DrawString("FOLIO DE VENTA: " + _folio, new Font("Arial", 10, FontStyle.Bold), Brushes.Black, new Point(1, posicion));
            posicion += 18;
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

        // Dibujar la lista de productos
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

        StringFormat sf = new StringFormat();
        sf.Alignment = StringAlignment.Far;
        if (!_corte)
        {
            foreach (var producto in _productos)
            {
                Font productFont = new Font("Arial", 8, FontStyle.Regular);
                float maxWidth = 145; // Ancho máximo para la columna de f
                List<string> lineasProducto = DivideTexto(e.Graphics, producto.Nombre, productFont, maxWidth);

                // Imprimir cantidad en primera línea
                e.Graphics.DrawString(producto.Cantidad.ToString("0.00", CultureInfo.InvariantCulture), productFont, Brushes.Black, new Point(1, posicion));
                // Imprimir cada línea del producto
                for (int j = 0; j < lineasProducto.Count; j++)
                {
                    if (j > 0) // Para líneas adicionales, no repetir cantidad
                    {
                        e.Graphics.DrawString("", productFont, Brushes.Black, new Point(1, posicion));
                    }

                    e.Graphics.DrawString(lineasProducto[j], productFont, Brushes.Black, new Point(40, posicion));

                    // Solo mostrar precio y unidad en la última línea
                    if (j == lineasProducto.Count - 1)
                    {
                        e.Graphics.DrawString($"{producto.PrecioUnitario:C}", productFont, Brushes.Black, new Point(215, posicion), sf);
                        e.Graphics.DrawString($"{producto.Total:C}", productFont, Brushes.Black, new Point(280, posicion), sf);
                    }

                    posicion += (j == lineasProducto.Count - 1) ? 20 : 15; // Más espacio después de la última línea
                }

                //e.Graphics.DrawString(producto.Nombre, new Font("Arial", 8, FontStyle.Regular), Brushes.Black, new Point(40, posicion));
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

            // Dibujar el pie de página
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

    // Función para dividir texto en múltiples líneas
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
                    // Palabra demasiado larga, partirla
                    for (int i = 0; i < palabra.Length; i++)
                    {
                        prueba = lineaActual + palabra[i];
                        if (g.MeasureString(prueba, font).Width > maxWidth)
                        {
                            lineas.Add(lineaActual);
                            lineaActual = "";
                            i--; // Reintentar este carácter
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
}
public class Producto
{
    public string Nombre { get; set; }
    public double Cantidad { get; set; }
    public double PrecioUnitario { get; set; }
    public double Total { get; set; }
}