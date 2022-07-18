using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmTipoCliente : Form
    {
        public String usuario = "";
        public frmTipoCliente()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool abierto = false;
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(frmClientes))
                {
                    MessageBox.Show("Ya existe un modulo de clientes abierto", "Alto!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    abierto = true;
                }
            }
            if (abierto)
            {

            }
            else
            {
                frmClientes cliente = new frmClientes();
                cliente.usuario = usuario;
                cliente.Show();
                this.Close();
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool abierto = false;
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(frmClientesCredito))
                {
                    MessageBox.Show("Ya existe un modulo de clientes abierto", "Alto!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    abierto = true;
                }
            }
            if (abierto)
            {

            }
            else
            {
                frmClientesCredito credi = new frmClientesCredito();
                credi.usuario = usuario;
                credi.Show();
                this.Close();
            }

            
        }
    }
}
