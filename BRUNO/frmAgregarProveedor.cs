using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmAgregarProveedor : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbCommand cmd;
        public bool buscar = false;
        public string Nombre { get; set; }
        public frmAgregarProveedor()
        {
            InitializeComponent();
            conectar.Open();
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (this.Text == "Agregar")
            {
                cmd = new OleDbCommand("insert into Proveedores(Nombre, RFC, Direccion, Telefono, Correo, Referencia, Clave, Adeudo) values('" + txtNombre.Text + "','" + txtRFC.Text + "','" + txtDireccion.Text + "','" + txtTelefono.Text + "','" + txtCorreo.Text + "','" + txtReferencia.Text + "','" + textBox1.Text + "','0');", conectar);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Se ha agregado el cliente con exito", "AGREGADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (buscar)
                {
                     Nombre = txtNombre.Text;
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                {
                    this.Close();
                    frmProveedores cliente = new frmProveedores();
                    cliente.Show();
                }
            }
            else
            {
                cmd = new OleDbCommand("UPDATE Proveedores set Nombre='" + txtNombre.Text + "', Telefono='" + txtTelefono.Text + "', Direccion='" + txtDireccion.Text + "', Referencia='" + txtReferencia.Text + "', RFC='" + txtRFC.Text + "', Correo='" + txtCorreo.Text + "', Clave='" + textBox1.Text + "' where Id=" + lblID.Text + ";", conectar);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Se ha actualizado el cliente con exito", "AGREGADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
           
        }

        private void frmAgregarProveedor_Load(object sender, EventArgs e)
        {

        }

        private void frmAgregarProveedor_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void txtTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }
    }
}
