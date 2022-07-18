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
    public partial class frmAgregarPrecioYComentario : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbCommand cmd;
        public frmAgregarPrecioYComentario()
        {
            InitializeComponent();
            conectar.Open();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cmd = new OleDbCommand("UPDATE Servicios set Comentarios='"+txtComentario.Text+"', CostoReal='"+txtFinal.Text+"' where Folio='"+lblFolio.Text+"';", conectar);
            //MessageBox.Show("UPDATE Servicios set Comentarios='" + txtComentario.Text + "', CostoReal='" + txtFinal.Text + "' where Folio='" + lblFolio.Text + "';");
            cmd.ExecuteNonQuery();
            MessageBox.Show("Se han agregado los comentarios y el precio final con exito", "AGREGADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void frmAgregarPrecioYComentario_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmServicios servi = new frmServicios();
            servi.Show();
        }
    }
}
