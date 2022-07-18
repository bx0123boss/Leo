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
    public partial class frmAgregarCategoria : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbCommand cmd;
        public frmAgregarCategoria()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            conectar.Open();
            cmd = new OleDbCommand("insert into Categorias(Nombre) Values('" + txtNombre.Text + "');", conectar);
            cmd.ExecuteNonQuery();
            MessageBox.Show("Se ha abonado con exito", "EXITO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            frmCategorias apart = new frmCategorias();
            apart.Show();
            this.Close();
        }
    }
}
