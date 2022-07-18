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
    public partial class frmClaveVendedor : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbCommand cmd;
        public int Id { get; set; }
        public string Usuario { get; set; }
        public string Tipo { get; set; }
        public frmClaveVendedor()
        {
            InitializeComponent();
        }

        private void frmClaveVendedor_Load(object sender, EventArgs e)
        {
            conectar.Open();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clave();
        }

        private void txtPass_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                Clave();
            }
        }

        public void Clave()
        {
            if (txtPass.Text == "")
            {

            }
            else
            {
                cmd = new OleDbCommand("select Id,Usuario from Usuarios where Contraseña='" + txtPass.Text + "';", conectar);
                OleDbDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Id = Convert.ToInt32(reader[0].ToString());
                    Usuario = reader[1].ToString();
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("No se encuentra el usuario, favor de verificar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPass.Clear();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
