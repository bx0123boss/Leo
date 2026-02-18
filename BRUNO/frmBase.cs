using iTextSharp.text;
using iTextSharp.text.pdf;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using Document = iTextSharp.text.Document;
using Excel = Microsoft.Office.Interop.Excel;

namespace BRUNO
{
    public partial class frmBase : Form
    {
        public frmBase()
        {
            // Estilos base del Formulario
            this.BackColor = Color.FromArgb(25, 25, 25);
            this.ForeColor = Color.White;
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, FontStyle.Regular);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Puedes agregar más propiedades comunes aquí
            // this.Icon = ... 
            // this.MaximizeBox = false;
        }



        /// <summary>
        /// Aplica el estilo "dark mode" estándar a un DataGridView
        /// y agrega funcionalidad de exportación con Click Derecho.
        /// </summary>
        public void EstilizarDataGridView(DataGridView dgv)
        {
            // --- Estilos Visuales (Tu código original) ---
            dgv.BackgroundColor = Color.FromArgb(40, 40, 40);
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(60, 60, 60);
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.RowHeadersVisible = false;
            // Nota: Cambié MultiSelect a true para que la experiencia sea mejor, 
            // pero puedes dejarlo en false si tu lógica lo requiere.
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.ReadOnly = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            // Estilo del Encabezado
            DataGridViewCellStyle headerStyle = new DataGridViewCellStyle();
            headerStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            headerStyle.BackColor = Color.FromArgb(60, 60, 60);
            headerStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 12.5F, FontStyle.Bold);
            headerStyle.ForeColor = Color.White;
            headerStyle.SelectionBackColor = Color.FromArgb(70, 130, 180);
            headerStyle.SelectionForeColor = Color.White;
            headerStyle.WrapMode = DataGridViewTriState.True;
            dgv.ColumnHeadersDefaultCellStyle = headerStyle;
            dgv.ColumnHeadersHeight = 32;

            // Estilo de Celdas
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();
            cellStyle.BackColor = Color.FromArgb(35, 35, 35);
            cellStyle.ForeColor = Color.White;
            cellStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 12.5F, FontStyle.Regular);
            cellStyle.SelectionBackColor = Color.FromArgb(70, 130, 180);
            cellStyle.SelectionForeColor = Color.White;
            dgv.DefaultCellStyle = cellStyle;

            // Estilo Alterno
            DataGridViewCellStyle alternatingCellStyle = new DataGridViewCellStyle();
            alternatingCellStyle.BackColor = Color.FromArgb(55, 55, 55);
            alternatingCellStyle.ForeColor = Color.White;
            alternatingCellStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 12.5F, FontStyle.Regular);
            alternatingCellStyle.SelectionBackColor = Color.FromArgb(70, 130, 180);
            alternatingCellStyle.SelectionForeColor = Color.White;
            dgv.AlternatingRowsDefaultCellStyle = alternatingCellStyle;

            // --- NUEVO: AGREGAR MENÚ CONTEXTUAL PARA EXCEL ---
            AgregarMenuContextualExcel(dgv);
            AgregarMenuContextualPDF(dgv, this.Text);
        }

        /// <summary>
        /// Crea un ContextMenuStrip y lo asigna al DataGridView
        /// </summary>
        private void AgregarMenuContextualExcel(DataGridView dgv)
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            // Creamos el item del menú
            ToolStripMenuItem itemExportar = new ToolStripMenuItem();
            itemExportar.Text = "Exportar a Excel";
            // Si tienes un icono en tus recursos, descomenta la siguiente línea:
            // itemExportar.Image = Properties.Resources.excel_icon; 

            // Asignamos el evento Click
            itemExportar.Click += (s, e) => { ExportarGridAExcel(dgv); };

            menu.Items.Add(itemExportar);

            // Asignamos el menú al Grid. 
            // Si el Grid ya tiene un menú, habría que agregarlo a la colección existente,
            // pero asumiremos que no tiene o que este es el principal.
            if (dgv.ContextMenuStrip == null)
            {
                dgv.ContextMenuStrip = menu;
            }
            else
            {
                dgv.ContextMenuStrip.Items.Add(new ToolStripSeparator());
                dgv.ContextMenuStrip.Items.Add(itemExportar);
            }
        }
        // Método para agregar la opción al menú contextual (igual que el de Excel)
        public void AgregarMenuContextualPDF(DataGridView dgv, string tituloReporte)
        {
            string nombreArchivoSeguro = tituloReporte;
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                nombreArchivoSeguro = nombreArchivoSeguro.Replace(c, '_');
            }
            //
            // Si el menú no existe, lo creamos
            if (dgv.ContextMenuStrip == null)
            {
                dgv.ContextMenuStrip = new ContextMenuStrip();
            }

            // Creamos el item del menú para PDF
            ToolStripMenuItem itemExportarPdf = new ToolStripMenuItem();
            itemExportarPdf.Text = "Exportar a PDF";
            // itemExportarPdf.Image = Properties.Resources.pdf_icon; // Si tienes icono

            // Asignamos el evento Click
            itemExportarPdf.Click += (s, e) => { ExportarGridAPDF(dgv, tituloReporte); };

            // Agregamos un separador si ya hay items (como el de Excel)
            if (dgv.ContextMenuStrip.Items.Count > 0)
            {
                dgv.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            }

            dgv.ContextMenuStrip.Items.Add(itemExportarPdf);
        }

        private void ExportarGridAPDF(DataGridView dgv, string titulo)
        {
            if (dgv.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Limpieza del nombre del archivo
            string nombreArchivoSeguro = titulo;
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                nombreArchivoSeguro = nombreArchivoSeguro.Replace(c, '_');
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf";
            saveFileDialog.FileName = nombreArchivoSeguro + "_" + DateTime.Now.ToString("ddMMyyyy") + ".pdf";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // 1. Configuración del documento (Carta Horizontal)
                    Document doc = new Document(iTextSharp.text.PageSize.LETTER.Rotate(), 10, 10, 10, 10);
                    PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(saveFileDialog.FileName, FileMode.Create));

                    doc.Open();

                    // 2. Encabezados
                    iTextSharp.text.Font fuenteTitulo = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 14, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                    Paragraph parrafoTitulo = new Paragraph(titulo, fuenteTitulo);
                    parrafoTitulo.Alignment = Element.ALIGN_CENTER;
                    doc.Add(parrafoTitulo);

                    iTextSharp.text.Font fuenteFecha = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.DARK_GRAY);
                    Paragraph parrafoFecha = new Paragraph("Fecha de emisión: " + DateTime.Now.ToString(), fuenteFecha);
                    parrafoFecha.Alignment = Element.ALIGN_CENTER;
                    doc.Add(parrafoFecha);

                    doc.Add(iTextSharp.text.Chunk.NEWLINE);

                    // 3. Tabla
                    int columnasVisibles = 0;
                    foreach (DataGridViewColumn col in dgv.Columns) { if (col.Visible) columnasVisibles++; }

                    PdfPTable pdfTable = new PdfPTable(columnasVisibles);
                    pdfTable.WidthPercentage = 100;

                    iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                    iTextSharp.text.Font _headerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9, iTextSharp.text.Font.BOLD, BaseColor.WHITE);

                    // 4. Agregar Encabezados
                    foreach (DataGridViewColumn col in dgv.Columns)
                    {
                        if (col.Visible)
                        {
                            PdfPCell clHeader = new PdfPCell(new Phrase(col.HeaderText, _headerFont));
                            clHeader.BorderWidth = 0;
                            clHeader.BorderWidthBottom = 0.75f;
                            clHeader.BackgroundColor = new BaseColor(50, 50, 50);
                            clHeader.HorizontalAlignment = Element.ALIGN_CENTER;
                            clHeader.Padding = 5;
                            pdfTable.AddCell(clHeader);
                        }
                    }

                    // 5. Agregar Datos
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            foreach (DataGridViewColumn col in dgv.Columns)
                            {
                                if (col.Visible)
                                {
                                    // CAMBIO CLAVE: Usamos .FormattedValue en lugar de .Value
                                    // Esto trae el texto tal cual se ve en pantalla (con signos de $ y comas)
                                    string valor = row.Cells[col.Index].FormattedValue != null ? row.Cells[col.Index].FormattedValue.ToString() : "";

                                    // SI LA COLUMNA NO TIENE FORMATO EN PANTALLA PERO ES DINERO, FORZAMOS EL FORMATO AQUÍ:
                                    // Descomenta las siguientes líneas si tus grids no tienen formato de moneda visible pero quieres que salga en el PDF:
                                    /*
                                    if (!valor.Contains("$") && (col.HeaderText.Contains("Monto") || col.HeaderText.Contains("Precio") || col.HeaderText.Contains("Total")))
                                    {
                                        if (decimal.TryParse(row.Cells[col.Index].Value?.ToString(), out decimal d))
                                            valor = d.ToString("C2");
                                    }
                                    */

                                    PdfPCell clData = new PdfPCell(new Phrase(valor, _standardFont));
                                    clData.BorderWidth = 0;
                                    clData.BorderWidthBottom = 0.25f;
                                    clData.BorderColorBottom = BaseColor.LIGHT_GRAY;

                                    // ALINEACIÓN INTELIGENTE
                                    // Si tiene signo de pesos o es número, a la derecha. Fechas al centro.
                                    if (valor.Contains("$") || decimal.TryParse(valor, out _))
                                        clData.HorizontalAlignment = Element.ALIGN_RIGHT;
                                    else if (DateTime.TryParse(valor, out _))
                                        clData.HorizontalAlignment = Element.ALIGN_CENTER;
                                    else
                                        clData.HorizontalAlignment = Element.ALIGN_LEFT;

                                    pdfTable.AddCell(clData);
                                }
                            }
                        }
                    }

                    doc.Add(pdfTable);
                    doc.Close();
                    writer.Close();

                    MessageBox.Show("PDF generado exitosamente en:\n" + saveFileDialog.FileName, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    try { System.Diagnostics.Process.Start(saveFileDialog.FileName); } catch { }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al exportar PDF: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        /// <summary>
        /// Lógica para recorrer el Grid y pasarlo a Excel usando Interop.
        /// </summary>
        private void ExportarGridAExcel(DataGridView dgv)
        {
            if (dgv.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Cambiar el cursor a espera
                this.Cursor = Cursors.WaitCursor;

                // 1. Crear la aplicación de Excel
                Excel.Application excelApp = new Excel.Application();

                // 2. Crear un libro nuevo
                Excel.Workbook workbook = excelApp.Workbooks.Add();
                Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Worksheets[1];

                // 3. Exportar Encabezados
                int colIndex = 1; // Excel empieza en 1
                for (int i = 0; i < dgv.Columns.Count; i++)
                {
                    // Solo exportamos columnas visibles
                    if (dgv.Columns[i].Visible)
                    {
                        worksheet.Cells[1, colIndex] = dgv.Columns[i].HeaderText;
                        colIndex++;
                    }
                }

                // Dar formato negrita a la cabecera
                Excel.Range headerRange = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, colIndex - 1]];
                headerRange.Font.Bold = true;
                headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.LightGray);

                // 4. Exportar Filas
                // NOTA: Para muchos datos esto puede ser lento celda por celda.
                // Si tienes miles de registros, se recomienda usar arreglos de objetos [,]
                // Para uso normal de POS (cientos de registros), esto funciona bien.

                int rowIndex = 2; // Fila 1 es cabecera
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    // Evitar la fila de 'nuevo registro' si existe
                    if (row.IsNewRow) continue;

                    colIndex = 1;
                    for (int i = 0; i < dgv.Columns.Count; i++)
                    {
                        if (dgv.Columns[i].Visible)
                        {
                            object valor = row.Cells[i].Value;

                            // Manejo básico de fechas para que Excel no las invierta
                            if (valor != null)
                            {
                                if (valor is DateTime)
                                {
                                    worksheet.Cells[rowIndex, colIndex] = ((DateTime)valor).ToString("dd/MM/yyyy HH:mm");
                                }
                                else
                                {
                                    worksheet.Cells[rowIndex, colIndex] = valor.ToString();
                                }
                            }
                            colIndex++;
                        }
                    }
                    rowIndex++;
                }

                // 5. Ajustar columnas automáticamente
                worksheet.Columns.AutoFit();

                // 6. Mostrar Excel
                excelApp.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al exportar a Excel: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        // --- Métodos de estilo para Botones ---

        private void ConfigurarBotonBase(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, FontStyle.Bold);
            btn.UseVisualStyleBackColor = false; // Importante para que respete el BackColor
        }

        /// <summary>
        /// Estilo para botones de acción principal (Ej. "Agregar", "Guardar")
        /// </summary>
        public void EstilizarBotonPrimario(Button btn)
        {
            ConfigurarBotonBase(btn);
            btn.BackColor = Color.FromArgb(52, 152, 219); // Azul
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 128, 185);
        }

        /// <summary>
        /// Estilo para botones de peligro (Ej. "Eliminar", "Cancelar")
        /// </summary>
        public void EstilizarBotonPeligro(Button btn)
        {
            ConfigurarBotonBase(btn);
            btn.BackColor = Color.FromArgb(231, 76, 60); // Rojo
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 57, 43);
        }

        /// <summary>
        /// Estilo para botones de advertencia (Ej. "Editar", "Modificar")
        /// </summary>
        public void EstilizarBotonAdvertencia(Button btn)
        {
            ConfigurarBotonBase(btn);
            btn.BackColor = Color.FromArgb(241, 196, 15); // Amarillo
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(243, 156, 18);
        }
        // Agrégalo en frmBase.cs
        public void EstilizarTextBox(TextBox txt)
        {
            txt.BorderStyle = BorderStyle.FixedSingle; // O BorderStyle.FixedSingle
            txt.BackColor = Color.FromArgb(60, 60, 60);
            txt.ForeColor = Color.White;
        }

        /// <summary>
        /// Aplica el estilo "dark mode" a un ComboBox, habilitando OwnerDraw.
        /// </summary>
        public void EstilizarComboBox(ComboBox cmb)
        {
            cmb.FlatStyle = FlatStyle.Flat;
            cmb.BackColor = Color.FromArgb(60, 60, 60); // Color de la caja
            cmb.ForeColor = Color.White; // Color del texto seleccionado

            // Habilitar el dibujo personalizado para la lista desplegable
            cmb.DrawMode = DrawMode.OwnerDrawFixed;
            int paddingVertical = 8;
            cmb.ItemHeight = cmb.Font.Height + paddingVertical;

            // Suscribirse al evento DrawItem
            // (Nos desuscribimos primero para evitar suscripciones múltiples)
            cmb.DrawItem -= ComboBox_DrawItem;
            cmb.DrawItem += new DrawItemEventHandler(ComboBox_DrawItem);
        }

        /// <summary>
        /// Evento privado que DIBUJA cada item de la lista desplegable del ComboBox.
        /// </summary>
        // Reemplaza o edita el método existente en frmBase.cs

        private void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            // Si no hay nada que dibujar, salir
            if (e.Index < 0) return;

            ComboBox cmb = (ComboBox)sender;
            string text = cmb.GetItemText(cmb.Items[e.Index]);

            // Determinar el color de fondo
            Color backgroundColor;
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                // Color cuando el item está seleccionado (hover)
                backgroundColor = Color.FromArgb(70, 130, 180);
            }
            else
            {
                // Color de fondo normal del dropdown
                backgroundColor = Color.FromArgb(45, 45, 45);
            }

            // Dibujar el fondo del item
            e.Graphics.FillRectangle(new SolidBrush(backgroundColor), e.Bounds);

            // ***************************************************************
            // * MEJORA: Crear un rectángulo de texto con padding izquierdo
            // ***************************************************************
            System.Drawing.Rectangle textBounds = e.Bounds;
            int padding = 4; // 4 píxeles de margen izquierdo
            textBounds.X += padding;
            textBounds.Width -= padding;

            // Dibujar el texto del item con el nuevo rectángulo y elipsis si no cabe
            TextRenderer.DrawText(e.Graphics, text, e.Font, textBounds,
                                  Color.White,
                                  TextFormatFlags.Left |
                                  TextFormatFlags.VerticalCenter |
                                  TextFormatFlags.EndEllipsis); // EndEllipsis pone "..." si el texto se corta

            // Dibujar el rectángulo de foco si es necesario
            e.DrawFocusRectangle();
        }
        public void EstilizarCheckBox(CheckBox chk)
        {
            chk.ForeColor = Color.White;
            chk.BackColor = Color.Transparent;
            // La fuente la heredará del formulario base.
        }
        // Agrégalo en frmBase.cs

        /// <summary>
        /// Mide todos los items de un ComboBox y ajusta el DropDownWidth
        /// para mostrar el item más largo sin cortar el texto.
        /// </summary>
        public void AjustarAnchoDropDown(ComboBox cmb)
        {
            int maxWidth = 0;

            // Usamos CreateGraphics() para obtener un contexto de dibujo
            // y poder medir el texto correctamente.
            using (Graphics g = cmb.CreateGraphics())
            {
                foreach (object item in cmb.Items)
                {
                    // Obtenemos la representación en texto del item
                    string text = cmb.GetItemText(item);

                    // Medimos el tamaño que ocupará ese texto con la fuente actual
                    Size size = TextRenderer.MeasureText(g, text, cmb.Font);

                    // Guardamos el ancho más grande encontrado
                    if (size.Width > maxWidth)
                    {
                        maxWidth = size.Width;
                    }
                }
            }

            // Agregamos un "padding" (relleno)
            // 25px es un buen valor para dar espacio al scrollbar vertical y un margen.
            int padding = 25;
            int newWidth = maxWidth + padding;

            // Nos aseguramos de que el dropdown no sea MÁS PEQUEÑO que el control
            if (newWidth < cmb.Width)
            {
                cmb.DropDownWidth = cmb.Width;
            }
            else
            {
                cmb.DropDownWidth = newWidth;
            }
        }
    }
}