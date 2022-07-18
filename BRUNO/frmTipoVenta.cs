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
    public partial class frmTipoVenta : Form
    {
        public string usuario = "";
        public string idUsuario = "";
        public string NombreUsuario = "";
        public frmTipoVenta()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool abierto = false;
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(frmVentas))
                {
                    MessageBox.Show("Ya existe un modulo de ventas de contado abierto", "Alto!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    abierto = true;
                }
            }
            if (abierto)
            {

            }
            else
            {
                frmVentas vent = new frmVentas();
                vent.usuario = usuario;
                vent.lblUsuario.Text = NombreUsuario;
                vent.idUsuario = idUsuario;
                vent.lblCajero.Text = NombreUsuario;
                vent.Show();
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool abierto = false;
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(frmVentasCredito))
                {
                    MessageBox.Show("Ya existe un modulo de ventas a credito abierto", "Alto!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    abierto = true;
                }
            }
            if (abierto)
            {

            }
            else
            {
                frmVentasCredito credito = new frmVentasCredito();
                credito.usuario = usuario;
                credito.Show();
                this.Close();
            }

        }
    }
}
