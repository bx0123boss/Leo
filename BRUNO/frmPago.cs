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
        public double total {  get; set; }
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
                
                else if ((Convert.ToDouble(textBox2.Text) - total) < 0)
                {
                    textBox2.Clear();
                    MessageBox.Show("Ingresa una cantidad valida", "ALTO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
                else
                {
                    efectivo = Convert.ToDouble(textBox2.Text);
                    cambio = Convert.ToDouble(textBox2.Text) - total;
                    textBox3.Text = $"{(Convert.ToDouble(textBox2.Text) - total):C}";
                    textBox2.Text = $"{Convert.ToDouble(textBox2.Text):C}";
                    textBox3.Focus();
                }
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
