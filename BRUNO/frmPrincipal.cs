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
    public partial class frmPrincipal : Form
    {
        public String usuario = "";
        public string NombreUsuario = "";
        public string idUsuario = "";
        public frmPrincipal()
        {
            InitializeComponent();
        }

        private void BtnInventario_Click(object sender, EventArgs e)
        {
            bool abierto = false;
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(frmInventario))
                {
                    MessageBox.Show("Ya existe un modulo de inventarios abierto", "Alto!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    abierto = true;
                }
            }
            if (abierto)
            {

            }
            else
            {
                frmInventario invent = new frmInventario();
                invent.usuario = usuario;
                invent.Show();
            } 
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmServicios services = new frmServicios();
            services.Show();
        }

        private void BtnCobrar_Click(object sender, EventArgs e)
        {
            bool abierto = false;
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(frmTipoVenta))
                {
                    abierto = true;
                }
            }
            if (abierto)
            {

            }
            else
            {
                frmTipoVenta vent = new frmTipoVenta();
                vent.usuario = usuario;
                vent.NombreUsuario = NombreUsuario;
                vent.idUsuario = idUsuario;
                vent.Show();
            } 
        }

        private void BtnDevoluciones_Click(object sender, EventArgs e)
        {
            frmTipoVentaDetalles venta = new frmTipoVentaDetalles();
            venta.usuario = usuario;
            venta.Show();
        }

        private void BtnClientes_Click(object sender, EventArgs e)
        {
            frmTipoCliente cliente = new frmTipoCliente();
            cliente.usuario = usuario;
            cliente.Show();
        }

        private void BtnDeposito_Click(object sender, EventArgs e)
        {
            bool abierto = false;
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(frmEntradas))
                {
                    MessageBox.Show("Ya existe un modulo de entradas abierto", "Alto!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    abierto = true;
                }
            }
            if (abierto)
            {

            }
            else
            {
                frmEntradas entrada = new frmEntradas();
                entrada.Show();
            } 
            
        }

        private void BtnTipodecambio_Click(object sender, EventArgs e)
        {
            frmTipoCambio cambio = new frmTipoCambio();
            cambio.Show();
        }

        private void BtnApartados_Click(object sender, EventArgs e)
        {
            frmElegirApartado apartado = new frmElegirApartado();
            apartado.usuario = usuario;
            apartado.Show();
        }

        private void BtnRetiro_Click(object sender, EventArgs e)
        {
            bool abierto = false;
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(frmSalidas))
                {
                    MessageBox.Show("Ya existe un modulo de salidas abierto", "Alto!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    abierto = true;
                }
            }
            if (abierto)
            {

            }
            else
            {
                frmSalidas salida = new frmSalidas();
                salida.Show();
                //this.Hide();
            }
        }

        private void BtnCotizar_Click(object sender, EventArgs e)
        {
            bool abierto = false;
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(frmProveedores))
                {
                    MessageBox.Show("Ya existe un modulo de proveedores abierto", "Alto!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    abierto = true;
                }
            }
            if (abierto)
            {

            }
            else
            {
                frmProveedores salida = new frmProveedores();
                salida.Show();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (usuario == "Invitado")
            {
                frmCorte cor = new frmCorte();
                cor.usuario = usuario;
                cor.Show();
            }
            else
            {
                frmTipoCorte corte = new frmTipoCorte();
                corte.Show();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            frmProductoMasVendido mas = new frmProductoMasVendido();
            mas.Show();
        }

        private void frmPrincipal_Load(object sender, EventArgs e)
        {
            try
            {
                this.BackgroundImage = Image.FromFile("C:\\Jaeger Soft\\w2.jpg");
                pictureBox1.Image = Image.FromFile("C:\\Jaeger Soft\\logo.png");
            }catch(Exception ex)
            { 
            }
            if (usuario == "Invitado")
            {
                BtnTipodecambio.Hide();
                button3.Hide();
               
                button5.Hide();
            }
            if (Conexion.lugar == "LEO")
            {
                button7.Visible = true;
            }
        }

        private void frmPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            frmDinasti dinasti = new frmDinasti();
            dinasti.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            frmConfiguracion confi = new frmConfiguracion();
            confi.Show();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {

            bool abierto = false;
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(frmUsuarios))
                {
                    MessageBox.Show("Ya existe un modulo de usuarios abierto", "Alto!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    abierto = true;
                }
            }
            if (abierto)
            {

            }
            else
            {
                frmUsuarios USER = new frmUsuarios();
                USER.usuario = usuario;
                USER.Show();
            } 
            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("firefox.exe", "https://www.cfdi.com.mx/login/"); 
        }
    }
}
