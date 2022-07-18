using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmEntradas : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbCommand cmd;

        public frmEntradas()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtConcepto.Text) || string.IsNullOrEmpty(txtIngreso.Text))
            {
                MessageBox.Show("Faltan campos por llenar", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                conectar.Open();
                cmd = new OleDbCommand("insert into Corte(Concepto,Monto,FechaHora,Pago) Values('Entrada en caja por concepto: " + txtConcepto.Text + "'," + txtIngreso.Text + ",'" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','01=EFECTIVO');", conectar);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Se ha depositado con exito", "DEPOSITADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                conectar.Close();
                this.Close();
            }
        }

        private void frmEntradas_Load(object sender, EventArgs e)
        {

        }

        private void txtIngreso_KeyPress(object sender, KeyPressEventArgs e)
        {
            CultureInfo cc = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (char.IsNumber(e.KeyChar) || e.KeyChar.ToString() == cc.NumberFormat.NumberDecimalSeparator || Convert.ToInt32(e.KeyChar) == 8)
                e.Handled = false;
            else
                e.Handled = true;
        }
    }
}
                                                     