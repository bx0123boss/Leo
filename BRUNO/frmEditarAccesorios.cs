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
    public partial class frmEditarAccesorios : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbCommand cmd;
        public string usuario = "";

        public frmEditarAccesorios()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            conectar.Open();
            double utilidad = Convert.ToDouble(txtVenta.Text) - Convert.ToDouble(txtCompra.Text);
            cmd = new OleDbCommand("update Inventario set Nombre='" + txtProducto.Text + "', PrecioCompra='" + txtCompra.Text + "', PrecioVenta='" + txtVenta.Text + "', Limite=" + txtLimite.Text + ", Categoria='" + cmbCategoria.Text + "', SubCategoria='" + cmbSub.Text + "', Utilidad='" + utilidad + "' where Id='" + txtID.Text + "';", conectar);
            cmd.ExecuteNonQuery();
            conectar.Close();
            MessageBox.Show("Se ha actualizado el producto con exito!", "Editar Oro", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void frmEditarAccesorios_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmAccesorios invent = new frmAccesorios();
            invent.Show();
        }
    }
}
