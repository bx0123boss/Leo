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
    public partial class frmTipoVentaDetalles : frmBase
    {
        public String usuario = "";
        public frmTipoVentaDetalles()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool abierto = false;
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(frmReporteVentas))
                {
                    MessageBox.Show("Ya existe un modulo de reporte de ventas abierto", "Alto!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    abierto = true;
                }
            }
            if (abierto)
            {

            }
            else
            {
                frmReporteVentas REPORTE = new frmReporteVentas();
                REPORTE.usuario = usuario;
                REPORTE.Show();
                this.Close();
            } 

        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool abierto = false;
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(frmReporteVentasCredito))
                {
                    MessageBox.Show("Ya existe un modulo de reporte de ventas a credito abierto", "Alto!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    abierto = true;
                }
            }
            if (abierto)
            {

            }
            else
            {
                frmReporteVentasCredito repor = new frmReporteVentasCredito();
                repor.usuario = usuario;
                repor.Show();
                this.Close();
            } 

        }

        private void frmTipoVentaDetalles_Load(object sender, EventArgs e)
        {
            EstilizarBotonPrimario(button1);
            EstilizarBotonPrimario(button3);
            EstilizarBotonPrimario(button5);
            EstilizarBotonAdvertencia(button2);
            EstilizarBotonAdvertencia(button4);
            EstilizarBotonAdvertencia(button6);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bool abierto = false;
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(frmReporteVentasCredito))
                {
                    MessageBox.Show("Ya existe un modulo de reporte de ventas a credito abierto", "Alto!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    abierto = true;
                }
            }
            if (abierto)
            {

            }
            else
            {
                frmVentasDiariasVendedor ven = new frmVentasDiariasVendedor();
                ven.Show();
                this.Close();
            } 
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bool abierto = false;
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(frmReporteVentasCredito))
                {
                    MessageBox.Show("Ya existe un modulo de reporte de ventas a credito abierto", "Alto!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    abierto = true;
                }
            }
            if (abierto)
            {

            }
            else
            {
                frmVentasVendedorFechas fech = new frmVentasVendedorFechas();
                fech.Show();
                this.Close();
            } 

            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            frmVentasPorProducto veta = new frmVentasPorProducto();
            veta.Show();
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            frmProductoMasVendido mas = new frmProductoMasVendido();
            mas.Show();
            this.Close();
        }
    }
}
