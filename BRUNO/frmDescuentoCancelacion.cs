using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmDescuentoCancelacion : Form
    {
        public string id;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbDataAdapter da;
        OleDbCommand cmd;
        public frmDescuentoCancelacion()
        {
            InitializeComponent();
        }

        private void frmDescuentoCancelacion_Load(object sender, EventArgs e)
        {
            conectar.Open();
            cmd = new OleDbCommand("select * from Clientes where Id="+id+";", conectar);
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                lblCliente.Text = Convert.ToString(reader[1].ToString());
                txtAdeudo.Text = Convert.ToString(reader[7].ToString());
            }
        }

        private void lblCliente_Click(object sender, EventArgs e)
        {

        }

        private void txtAbono_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                txtRestante.Text = "" + (Convert.ToDouble(txtAdeudo.Text) - Convert.ToDouble(txtAbono.Text));
                button1.Focus();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cmd = new OleDbCommand("UPDATE Clientes set Adeudo=" + txtRestante.Text + " where Id=" + id + ";", conectar);
             cmd.ExecuteNonQuery();
            MessageBox.Show("DESCUENTO REALIZADO CON EXITO", "DESCUENTO!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
    }
}
