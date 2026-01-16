using System.Windows.Forms;
using System.Drawing;
using System;

namespace BRUNO
{
    public partial class frmBase : Form
    {
        public frmBase()
        {
            // Estilos base del Formulario
            this.BackColor = Color.FromArgb(25, 25, 25);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Puedes agregar más propiedades comunes aquí
            // this.Icon = ... 
            // this.MaximizeBox = false;
        }


      

        /// <summary>
        /// Aplica el estilo "dark mode" estándar a un DataGridView.
        /// </summary>
        /// <summary>
        /// Aplica el estilo "dark mode" estándar a un DataGridView,
        /// incluyendo filas con colores alternos (estilo cebra).
        /// </summary>
        public void EstilizarDataGridView(DataGridView dgv)
        {
            // Estilos generales del DataGridView
            dgv.BackgroundColor = Color.FromArgb(40, 40, 40);
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(60, 60, 60);
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.RowHeadersVisible = false;
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.ReadOnly = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            // Estilo del Encabezado
            DataGridViewCellStyle headerStyle = new DataGridViewCellStyle();
            headerStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            headerStyle.BackColor = Color.FromArgb(60, 60, 60);
            headerStyle.Font = new Font("Segoe UI Semibold", 12.5F, FontStyle.Bold);
            headerStyle.ForeColor = Color.White;
            headerStyle.SelectionBackColor = Color.FromArgb(70, 130, 180);
            headerStyle.SelectionForeColor = Color.White;
            headerStyle.WrapMode = DataGridViewTriState.True;
            dgv.ColumnHeadersDefaultCellStyle = headerStyle;
            dgv.ColumnHeadersHeight = 32;

            // Estilo de Celdas (Filas Pares)
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();
            cellStyle.BackColor = Color.FromArgb(35, 35, 35); // Color base (oscuro)
            cellStyle.ForeColor = Color.White;
            cellStyle.Font = new Font("Segoe UI Semibold", 12.5F, FontStyle.Regular);
            cellStyle.SelectionBackColor = Color.FromArgb(70, 130, 180);
            cellStyle.SelectionForeColor = Color.White;
            dgv.DefaultCellStyle = cellStyle;

            // ***************************************************************
            // * AJUSTE: Aumentar el contraste del color alterno
            // ***************************************************************
            DataGridViewCellStyle alternatingCellStyle = new DataGridViewCellStyle();
            // Aumentamos el brillo para que sea más fácil de distinguir
            alternatingCellStyle.BackColor = Color.FromArgb(55, 55, 55);
            alternatingCellStyle.ForeColor = Color.White;
            alternatingCellStyle.Font = new Font("Segoe UI Semibold", 12.5F, FontStyle.Regular);
            alternatingCellStyle.SelectionBackColor = Color.FromArgb(70, 130, 180);
            alternatingCellStyle.SelectionForeColor = Color.White;

            dgv.AlternatingRowsDefaultCellStyle = alternatingCellStyle;
        }

        // --- Métodos de estilo para Botones ---

        private void ConfigurarBotonBase(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold);
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
            Rectangle textBounds = e.Bounds;
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