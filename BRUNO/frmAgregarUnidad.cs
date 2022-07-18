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
    public partial class frmAgregarUnidad : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbCommand cmd;
        public string id;
        OleDbDataAdapter da;
        private DataSet ds;

        public frmAgregarUnidad()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            if (this.Text == "Agregar Unidad")
            {
                if (txtNombre.Text != "" && txtAbrev.Text != "")
                {
                    conectar.Open();
                    cmd = new OleDbCommand("insert into Unidades(Nombre,Abreviatura) values('" + txtNombre.Text + "','" + txtAbrev.Text + "');", conectar);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Se ha agregado la unidad con exito", "AGREGADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    frmUnidades a = new frmUnidades();
                    a.Show();
                    this.Close();
                }
            }
            else
            {
                if (txtNombre.Text != "" && txtAbrev.Text != "")
                {
                    conectar.Open();
                    cmd = new OleDbCommand("Update Unidades set Nombre ='" + txtNombre.Text + "', Abreviatura='" + txtAbrev.Text + "' where Id=" + id + ";", conectar);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Se ha editado la unidad con exito", "EDITAR", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    frmUnidades a = new frmUnidades();
                    a.Show();
                    this.Close();
                }
            }
            
        }
    }
}
