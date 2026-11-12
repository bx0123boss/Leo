using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JaegerSoft
{
    public partial class frmAgregarCategoria : frmBase
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbCommand cmd;
        public bool Editar { get; set; }
        public int Id { get; set; }
        public string Nombre { get; set; }
        public frmAgregarCategoria()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            conectar.Open();
            if (Editar)
            {
                cmd = new OleDbCommand("update Inventario set Categoria = '" + txtNombre.Text + "' where Categoria='" + Nombre + "';", conectar);
                cmd.ExecuteNonQuery();
                cmd = new OleDbCommand("update Categorias set Nombre = '" + txtNombre.Text + "' where Id=" + Id + ";", conectar);
                cmd.ExecuteNonQuery();
            }
            else
            {
                cmd = new OleDbCommand("insert into Categorias(Nombre) Values('" + txtNombre.Text + "');", conectar);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Se ha abonado con exito", "EXITO", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            if (!Editar)
                MessageBox.Show("Se ha agregado la categoría con exito", "EXITO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Se ha editado la categoría con exito", "EXITO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
        private void frmAgregarCategoria_Load(object sender, EventArgs e)
        {
            if (Editar)
            {
                this.Text = "Editar Categoría";
            }
            EstilizarBotonPrimario(button1);
            EstilizarTextBox(txtNombre);
        }
    }
}
