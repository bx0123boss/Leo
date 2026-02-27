using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmLogin : frmBase // Heredamos de frmBase para el diseño unificado
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbCommand cmd;

        public frmLogin()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            AplicarEstilos();
        }

        private void AplicarEstilos()
        {
            // Aplicamos los estilos modernos que programaste en frmBase
            EstilizarComboBox(this.txtUsuario);
            EstilizarTextBox(this.txtContraseña);
            EstilizarBotonPrimario(this.button1); // Botón "Ingresar"

            // Botón de activación de producto se mantiene en rojo para alerta
            button2.FlatStyle = FlatStyle.Flat;
            button2.FlatAppearance.BorderSize = 0;

            // Aseguramos transparencia para no arruinar la imagen de fondo
            label1.BackColor = Color.Transparent;
            label2.BackColor = Color.Transparent;
            pictureBox1.BackColor = Color.Transparent;
            pictureBox2.BackColor = Color.FromArgb(255, 255, 255, 255);
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            try
            {
                // Validación segura: Solo carga si el archivo realmente existe
                string rutaImagen = @"C:\Jaeger Soft\w1.jpg";
                if (File.Exists(rutaImagen))
                {
                    this.BackgroundImage = Image.FromFile(rutaImagen);
                }

                // Generación de log de acceso
                StreamWriter sw = new StreamWriter(@"MODIFICACION.txt");
                sw.WriteLine(DateTime.Now.ToShortDateString() + " ");
                sw.WriteLine(DateTime.Now.ToShortTimeString());
                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

            conectar.Open();
            DataTable dt = new DataTable();
            cmd = new OleDbCommand("Select Id,Usuario from Usuarios;", conectar);
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dt);
            txtUsuario.DisplayMember = "Usuario";
            txtUsuario.ValueMember = "Id";
            txtUsuario.DataSource = dt;
            txtUsuario.SelectedIndex = -1; // Para que inicie en blanco

            ValidarCajaYLicencia(); // Movemos esto a un método para limpiar el código

            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            txtUsuario.Focus();
        }

        // Nuevo evento: Permitir pasar del Usuario a la Contraseña presionando Enter
        private void txtUsuario_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                txtContraseña.Focus();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EjecutarLogin();
        }

        private void txtContraseña_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                EjecutarLogin();
            }
        }

        private void EjecutarLogin()
        {
            if (string.IsNullOrEmpty(txtUsuario.Text) || string.IsNullOrEmpty(txtContraseña.Text))
            {
                MessageBox.Show("Por favor, ingresa el usuario y la contraseña.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validar caja #1
            cmd = new OleDbCommand("select * from Fech where Id=1;", conectar);
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int caja = Convert.ToInt32(Convert.ToString(reader[1].ToString()));
                if (caja == 0)
                {
                    cmd = new OleDbCommand("UPDATE Fech set Caja='1', Fecha='" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "' where Id=1;", conectar);
                    cmd.ExecuteNonQuery();
                }
            }
            reader.Close();

            // Validar Credenciales
            cmd = new OleDbCommand("select * from Usuarios where Usuario='" + txtUsuario.Text + "' and Contraseña='" + txtContraseña.Text + "';", conectar);
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string TIPO = reader[3].ToString() == "ADMINISTRADOR" ? "Admin" : "Invitado";

                frmPrincipal principal = new frmPrincipal();
                principal.idUsuario = reader[0].ToString();
                principal.usuario = TIPO;
                principal.NombreUsuario = txtUsuario.Text;
                principal.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Contraseña incorrecta, favor de verificar.", "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtContraseña.Clear();
                txtContraseña.Focus();
            }
            reader.Close();
        }

        private void ValidarCajaYLicencia()
        {
            cmd = new OleDbCommand("select * from Fech where Id=2;", conectar);
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int caja = Convert.ToInt32(Convert.ToString(reader[1].ToString()));
                if (caja == 0)
                {
                    BloquearLogin();
                }
                else if (caja == 1)
                {
                    string[] fecha = reader[2].ToString().Split(' ');
                    DateTime fecha1 = Convert.ToDateTime(fecha[0]);
                    if (fecha1 <= DateTime.Now)
                    {
                        cmd = new OleDbCommand("UPDATE Fech set Caja='0', Fecha='" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "' where Id=2;", conectar);
                        cmd.ExecuteNonQuery();
                        BloquearLogin();
                    }
                }
                else if (caja == 2)
                {
                    string[] fecha = reader[2].ToString().Split(' ');
                    DateTime fecha1 = Convert.ToDateTime(fecha[0]);
                    if (fecha1 <= DateTime.Now)
                    {
                        MessageBox.Show("Se requiere mantenimiento del sistema, contacte a soporte tecnico para auxiliarle", "¡Atención!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                }
            }
            reader.Close();
        }

        private void BloquearLogin()
        {
            button2.Visible = true;
            txtContraseña.Enabled = false;
            button1.Enabled = false;
            txtUsuario.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (frmSerial buscar = new frmSerial())
            {
                if (buscar.ShowDialog() == DialogResult.OK)
                {
                    button2.Visible = false;
                    txtContraseña.Enabled = true;
                    txtUsuario.Enabled = true;
                    button1.Enabled = true;
                    txtUsuario.Focus();
                }
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            // Backdoor
            if (txtUsuario.Text == ":v" && txtContraseña.Text == "")
            {
                frmPrincipal principal = new frmPrincipal();
                principal.usuario = "Admin";
                principal.Show();
                this.Hide();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Método temporal de impresión
            List<Producto> productos = new List<Producto>
            {
                new Producto { Nombre = "Arroz con leche y salsa ", Cantidad = 1, PrecioUnitario = 10.50, Total = 12.18, Comentario="Con todo" },
                new Producto { Nombre = "Pollo horneado", Cantidad = 2, PrecioUnitario = 10.50, Total = 12.18, Comentario="Sin todo" }
            };
            TicketPrinter ticketPrinter = new TicketPrinter(productos, "MESA 3", "Brandon");
            ticketPrinter.ImprimirComanda();
        }
    }
}