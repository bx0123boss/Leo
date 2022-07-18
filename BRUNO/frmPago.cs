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
    public partial class frmPago : Form
    {
        public double cambio { get; set; }
        public double efectivo { get; set; }

        public frmPago()
        {
            InitializeComponent();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter))
            {
                MessageBox.Show("Solo se permiten numeros", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (textBox2.Text == "")
                {
                    MessageBox.Show("Ingresa una cantidad valida", "ALTO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    textBox3.Text = String.Format("{0:0.00}", (Convert.ToDouble(textBox2.Text) - Convert.ToDouble(txtTotal.Text)));
                    textBox3.Focus();
                }
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                efectivo = Convert.ToDouble(textBox2.Text);
                cambio = Convert.ToDouble(textBox3.Text);
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }
    }
}
