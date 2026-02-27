using System;
using System.Net.Sockets;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmPrincipal : frmBase
    {
        bool IsServidorActivo = false;
        private Process _procesoWeb;
        public String usuario = "";
        public string NombreUsuario = "";
        public string idUsuario = "";

        // Truco para eliminar el 100% del parpadeo con la imagen de fondo
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Activa WS_EX_COMPOSITED
                return cp;
            }
        }

        public frmPrincipal()
        {
            InitializeComponent();
            this.DoubleBuffered = true; // Refuerza la eliminación de parpadeo
            IsServidorActivo = ArrancarServidorWeb();

            // Hacemos transparentes los contenedores, pero NO tocamos los botones
            pnlMenu.BackColor = Color.Transparent;
            pictureBox1.BackColor = Color.Transparent;
        }

        private void frmPrincipal_Load(object sender, EventArgs e)
        {
            try
            {
                string bgPath = @"C:\Jaeger Soft\w2.jpg";
                if (File.Exists(bgPath)) { this.BackgroundImage = Image.FromFile(bgPath); }

                string logoPath = @"C:\Jaeger Soft\logo.png";
                if (File.Exists(logoPath)) { pictureBox1.Image = Image.FromFile(logoPath); }
            }
            catch (Exception) { }

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

        // === MÉTODO OPTIMIZADO PARA VERIFICAR SI UN FORMULARIO YA ESTÁ ABIERTO ===
        private bool FormularioEstaAbierto(Type tipoFormulario)
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == tipoFormulario)
                {
                    MessageBox.Show("Este módulo ya se encuentra abierto.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    frm.BringToFront(); // Lo trae al frente si estaba escondido
                    return true;
                }
            }
            return false;
        }

        private void BtnInventario_Click(object sender, EventArgs e)
        {
            if (!FormularioEstaAbierto(typeof(frmInventario)))
            {
                frmInventario invent = new frmInventario();
                invent.usuario = usuario;
                invent.Show();
            }
        }

        private void BtnCobrar_Click(object sender, EventArgs e)
        {
            if (!FormularioEstaAbierto(typeof(frmTipoVenta)))
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
            if (!FormularioEstaAbierto(typeof(frmTipoVentaDetalles)))
            {
                frmTipoVentaDetalles venta = new frmTipoVentaDetalles();
                venta.usuario = usuario;
                venta.Show();
            }
        }

        private void BtnClientes_Click(object sender, EventArgs e)
        {
            if (!FormularioEstaAbierto(typeof(frmTipoCliente)))
            {
                frmTipoCliente cliente = new frmTipoCliente();
                cliente.usuario = usuario;
                cliente.Show();
            }
        }

        private void BtnDeposito_Click(object sender, EventArgs e)
        {
            if (!FormularioEstaAbierto(typeof(frmEntradas)))
            {
                frmEntradas entrada = new frmEntradas();
                entrada.Show();
            }
        }

        private void BtnRetiro_Click(object sender, EventArgs e)
        {
            if (!FormularioEstaAbierto(typeof(frmSalidas)))
            {
                frmSalidas salida = new frmSalidas();
                salida.Show();
            }
        }

        private void BtnCotizar_Click(object sender, EventArgs e)
        {
            if (!FormularioEstaAbierto(typeof(frmProveedores)))
            {
                frmProveedores proveedores = new frmProveedores();
                proveedores.Show();
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            if (!FormularioEstaAbierto(typeof(frmUsuarios)))
            {
                frmUsuarios USER = new frmUsuarios();
                USER.usuario = usuario;
                USER.Show();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!FormularioEstaAbierto(typeof(frmConfiguracionTicket)))
            {
                frmConfiguracionTicket config = new frmConfiguracionTicket();
                config.Show();
            }
        }

        private void BtnApartados_Click(object sender, EventArgs e)
        {
            frmElegirApartado apartado = new frmElegirApartado();
            apartado.usuario = usuario;
            apartado.Show();
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

        private void button2_Click(object sender, EventArgs e)
        {
            frmServicios services = new frmServicios();
            services.Show();
        }

        private void BtnTipodecambio_Click(object sender, EventArgs e)
        {
            frmTipoCambio cambio = new frmTipoCambio();
            cambio.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            frmProductoMasVendido mas = new frmProductoMasVendido();
            mas.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            frmConfiguracion confi = new frmConfiguracion();
            confi.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("firefox.exe", "https://www.cfdi.com.mx/login/");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string url = "http://localhost:5000";
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

        private bool ArrancarServidorWeb()
        {
            try
            {
                string rutaWebExe = @"C:\Jaeger Soft\ModuloWeb\PuntoVentaWeb.exe";
                if (!File.Exists(rutaWebExe)) return false;

                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = rutaWebExe;
                info.WindowStyle = ProcessWindowStyle.Hidden;
                info.CreateNoWindow = true;
                info.UseShellExecute = false;
                info.WorkingDirectory = Path.GetDirectoryName(rutaWebExe);

                _procesoWeb = Process.Start(info);
                return _procesoWeb != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error iniciando web: " + ex.Message);
                return false;
            }
        }

        public void DetenerServidorWeb()
        {
            try
            {
                if (_procesoWeb != null && !_procesoWeb.HasExited)
                {
                    _procesoWeb.Kill();
                    _procesoWeb.WaitForExit(1000);
                }
            }
            catch { }

            try
            {
                foreach (var process in System.Diagnostics.Process.GetProcessesByName("PuntoVentaWeb"))
                {
                    process.Kill();
                }
            }
            catch { }
        }

        private void frmPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            DetenerServidorWeb();
            Application.Exit();
        }

        private void pictureBox1_Click(object sender, EventArgs e) { }
    }
}