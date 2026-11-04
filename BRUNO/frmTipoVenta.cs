using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JaegerSoft
{
    public partial class frmTipoVenta : frmBase
    {
        public string usuario = "";
        public string idUsuario = "";
        public string NombreUsuario = "";
        public bool IsServidorActivo = false;
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
                vent.MinimizeBox = false;  // Elimina el botón de minimizar
                vent.ControlBox = true;
                vent.usuario = usuario;
                vent.lblUsuario.Text = NombreUsuario;
                vent.idUsuario = idUsuario;
                vent.lblCajero.Text = NombreUsuario;
                vent.ShowDialog();
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

        private void frmTipoVenta_Load(object sender, EventArgs e)
        {
            EstilizarBotonPrimario(this.button1);
            EstilizarBotonAdvertencia(this.button2);
            EstilizarBotonAdvertencia(this.button3);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string url = "http://localhost:5000/ventas";
            if (IsServidorActivo)
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo abrir el navegador: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Servidor inactivo.\n\nContacte a soporte técnico para actualizar su sistema o verificar la instalación.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}

