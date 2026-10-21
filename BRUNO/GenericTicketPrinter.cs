using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;

namespace JaegerSoft
{
    public class GenericTicketPrinter
    {
        // Tipos de elementos que se pueden agregar al ticket
        public enum ElementType
        {
            Logo,
            HeaderTitle,
            Text,
            TextTwoColumns, // Para 'Clave: Valor' o 'Concepto' / 'Monto'
            LineSeparator,
            FillInField,    // Para Nombre / Firma
            Space
        }

        public class TicketElement
        {
            public ElementType Type { get; set; }
            public string TextLeft { get; set; }
            public string TextRight { get; set; }
            public Font CustomFont { get; set; }
            public StringAlignment Alignment { get; set; } = StringAlignment.Near;
            public int ExtraSpace { get; set; } = 0;
        }

        private readonly List<TicketElement> _elements = new List<TicketElement>();
        private string _logoPath;
        private float _ticketWidth = 280; // Ancho estándar de papel 80mm en pt

        public GenericTicketPrinter(string logoPath = null, float ticketWidth = 280)
        {
            _logoPath = logoPath;
            _ticketWidth = ticketWidth;
        }

        // --- MÉTODOS PARA CONSTRUIR EL TICKET AGILMENTE ---

        public GenericTicketPrinter AddTitle(string text)
        {
            _elements.Add(new TicketElement
            {
                Type = ElementType.HeaderTitle,
                TextLeft = text,
                Alignment = StringAlignment.Center,
                CustomFont = new Font("Arial", 11, FontStyle.Bold)
            });
            return this;
        }

        public GenericTicketPrinter AddText(string text, bool bold = false, StringAlignment alignment = StringAlignment.Near)
        {
            _elements.Add(new TicketElement
            {
                Type = ElementType.Text,
                TextLeft = text,
                Alignment = alignment,
                CustomFont = new Font("Arial", 8, bold ? FontStyle.Bold : FontStyle.Regular)
            });
            return this;
        }

        public GenericTicketPrinter AddRow(string label, string value, bool bold = false)
        {
            _elements.Add(new TicketElement
            {
                Type = ElementType.TextTwoColumns,
                TextLeft = label,
                TextRight = value,
                CustomFont = new Font("Arial", 8, bold ? FontStyle.Bold : FontStyle.Regular)
            });
            return this;
        }

        public GenericTicketPrinter AddLine()
        {
            _elements.Add(new TicketElement { Type = ElementType.LineSeparator });
            return this;
        }

        public GenericTicketPrinter AddFillField(string label)
        {
            _elements.Add(new TicketElement { Type = ElementType.FillInField, TextLeft = label });
            return this;
        }

        public GenericTicketPrinter AddSpace(int px = 10)
        {
            _elements.Add(new TicketElement { Type = ElementType.Space, ExtraSpace = px });
            return this;
        }

        // --- IMPRESIÓN DEL DOCUMENTO ---

        public void Print(string printerName = null)
        {
            PrintDocument pd = new PrintDocument();
            if (!string.IsNullOrEmpty(printerName))
            {
                pd.PrinterSettings.PrinterName = printerName;
            }
            pd.PrintPage += RenderPage;
            pd.Print();
        }

        private void RenderPage(object sender, PrintPageEventArgs e)
        {
            int posicionY = 10;
            StringFormat centerFormat = new StringFormat { Alignment = StringAlignment.Center };
            StringFormat rightFormat = new StringFormat { Alignment = StringAlignment.Far };

            // 1. Dibujar Logo si existe
            if (!string.IsNullOrEmpty(_logoPath) && System.IO.File.Exists(_logoPath))
            {
                Image logo = Image.FromFile(_logoPath);
                int targetWidth = logo.Width;
                int targetHeight = logo.Height;

                if (logo.Width > _ticketWidth)
                {
                    double ratio = (double)_ticketWidth / logo.Width;
                    targetWidth = (int)_ticketWidth;
                    targetHeight = (int)(logo.Height * ratio);
                }

                int xPos = (int)((_ticketWidth - targetWidth) / 2);
                e.Graphics.DrawImage(logo, new Rectangle(xPos, posicionY, targetWidth, targetHeight));
                posicionY += targetHeight + 10;
            }

            // 2. Renderizar Elementos Secuencialmente
            foreach (var elem in _elements)
            {
                switch (elem.Type)
                {
                    case ElementType.HeaderTitle:
                        e.Graphics.DrawString(elem.TextLeft, elem.CustomFont, Brushes.Black,
                            new RectangleF(0, posicionY, _ticketWidth, 20), centerFormat);
                        posicionY += 22;
                        break;

                    case ElementType.Text:
                        if (elem.Alignment == StringAlignment.Center)
                        {
                            e.Graphics.DrawString(elem.TextLeft, elem.CustomFont, Brushes.Black,
                                new RectangleF(0, posicionY, _ticketWidth, 18), centerFormat);
                        }
                        else
                        {
                            e.Graphics.DrawString(elem.TextLeft, elem.CustomFont, Brushes.Black, new PointF(1, posicionY));
                        }
                        posicionY += 16;
                        break;

                    case ElementType.TextTwoColumns:
                        e.Graphics.DrawString(elem.TextLeft, elem.CustomFont, Brushes.Black, new PointF(1, posicionY));
                        e.Graphics.DrawString(elem.TextRight, elem.CustomFont, Brushes.Black, new PointF(_ticketWidth - 5, posicionY), rightFormat);
                        posicionY += 16;
                        break;

                    case ElementType.LineSeparator:
                        posicionY += 3;
                        e.Graphics.DrawLine(new Pen(Color.Black), 1, posicionY, _ticketWidth, posicionY);
                        posicionY += 8;
                        break;

                    case ElementType.FillInField:
                        posicionY += 10;
                        e.Graphics.DrawString(elem.TextLeft, new Font("Arial", 8, FontStyle.Bold), Brushes.Black, new PointF(1, posicionY));
                        posicionY += 15;
                        // Línea punteada para escribir/firmar
                        Pen dottedPen = new Pen(Color.Black) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot };
                        e.Graphics.DrawLine(dottedPen, 1, posicionY, _ticketWidth - 5, posicionY);
                        posicionY += 15;
                        break;

                    case ElementType.Space:
                        posicionY += elem.ExtraSpace;
                        break;
                }
            }
        }
    }
}