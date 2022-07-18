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
    public partial class frmAgregarUsuario : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbCommand cmd;
        public frmAgregarUsuario()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtPass.Text == textBox1.Text)
            {
                if (txtUsuario.Text != "Administrador")
                {
                    if (cmbTipo.Text != "" && txtUsuario.Text != "" && txtPass.Text != "")
                    {
                        cmd = new OleDbCommand("select count(*) from Usuarios where Usuario='" + txtUsuario.Text + "';", conectar);
                        int valor = int.Parse(cmd.ExecuteScalar().ToString());
                        if (valor == 0)
                        {
                            cmd = new OleDbCommand("insert into Usuarios(Usuario, Contraseña, Tipo) values('" + txtUsuario.Text + "','" + txtPass.Text + "','" + cmbTipo.Text + "');", conectar);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Se ha agregado el cliente con exito", "AGREGADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            frmUsuarios user = new frmUsuarios();
                            user.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Ya existe un usuario con ese nombre de usuario", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtUsuario.Clear();
                            txtPass.Clear();
                            textBox1.Clear();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Seleccione un tipo de usuario", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("NOMBRE DE USUARIO RESERVADO", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtUsuario.Focus();
                    txtUsuario.Clear();
                }
            }
            else
            {
                MessageBox.Show("Las contraseñas no coinciden", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPass.Clear();
                textBox1.Clear();
                txtPass.Focus();
            }
        }

        private void frmAgregarUsuario_Load(object sender, EventArgs e)
        {
            conectar.Open();
        }
    }
}
