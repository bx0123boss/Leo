using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmCredito : Form
    {
        public double iva { get; set; }
        public frmCredito()
        {
            InitializeComponent();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permite borrar caracteres con la tecla Backspace
            if (e.KeyChar == (char)Keys.Back)
                return;

            // Permite solo números y el punto decimal
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // Permite un solo punto decimal
            if (e.KeyChar == '.' && textBox1.Text.Contains("."))
            {
                e.Handled = true;
            }
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (textBox1.Text == "")
                {
                    MessageBox.Show("INGRESE UNA CANTIDAD VALIDA", "ALTO!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (Convert.ToDouble(textBox1.Text) < 0)
                {
                    MessageBox.Show("INGRESE UNA CANTIDAD VALIDA", "ALTO!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    iva = Convert.ToDouble(textBox1.Text);
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
            }
        }
    }
}
