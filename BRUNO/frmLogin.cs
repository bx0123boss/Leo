using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace JaegerSoft
{
    public partial class frmLogin : frmBase
    {
        SqlConnection conectar = new SqlConnection(Conexion.CadSQL);
        SqlCommand cmd;

        public frmLogin()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            AplicarEstilos();
        }

        private void AplicarEstilos()
        {
            EstilizarComboBox(this.txtUsuario);
            EstilizarTextBox(this.txtContraseña);
            EstilizarBotonPrimario(this.button1);
            button2.FlatStyle = FlatStyle.Flat;
            button2.FlatAppearance.BorderSize = 0;
            label1.BackColor = Color.Transparent;
            label2.BackColor = Color.Transparent;
            pictureBox1.BackColor = Color.Transparent;
            pictureBox2.BackColor = Color.FromArgb(255, 255, 255, 255);
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            try
            {
                string rutaImagen = @"C:\Jaeger Soft\w1.jpg";
                if (File.Exists(rutaImagen))
                {
                    this.BackgroundImage = Image.FromFile(rutaImagen);
                }

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
            cmd = new SqlCommand("Select Id,Usuario from Usuarios;", conectar);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            txtUsuario.DisplayMember = "Usuario";
            txtUsuario.ValueMember = "Id";
            txtUsuario.DataSource = dt;
            txtUsuario.SelectedIndex = -1;
            ValidarCajaYLicencia();

            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            txtUsuario.Focus();
        }

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
            if ((string.IsNullOrEmpty(txtUsuario.Text) || string.IsNullOrEmpty(txtContraseña.Text)) && !Conexion.PuntoB)
            {
                MessageBox.Show("Por favor, ingresa el usuario y la contraseña.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if ((string.IsNullOrEmpty(txtUsuario.Text) || string.IsNullOrEmpty(txtContraseña.Text)) && Conexion.PuntoB)
            {
                frmPrincipal menu = new frmPrincipal();
                menu.IsPuntoB = true;
                menu.Show();
                this.Hide();
                return;
            }

            // Validar Credenciales
            cmd = new SqlCommand("select * from Usuarios where Usuario='" + txtUsuario.Text + "' and Contraseña='" + txtContraseña.Text + "';", conectar);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                // Guardamos los datos básicos en la Sesión
                Sesion.IdUsuario = reader["Id"].ToString();
                Sesion.NombreUsuario = txtUsuario.Text;

                // 1. Limpiamos cualquier permiso viejo por seguridad
                Sesion.PermisosActuales.Clear();

                // 2. Traemos sus permisos asignados desde Access
                try
                {
                    string queryPermisos = "SELECT Permiso FROM PermisosUsuario WHERE IdUsuario = '" + Sesion.IdUsuario + "'";
                    using (SqlCommand cmdPermisos = new SqlCommand(queryPermisos, conectar))
                    {
                        using (SqlDataReader rdrPermisos = cmdPermisos.ExecuteReader())
                        {
                            while (rdrPermisos.Read())
                            {
                                Sesion.PermisosActuales.Add(rdrPermisos["Permiso"].ToString().ToUpper());
                            }
                        }
                    }
                }
                catch (Exception exPermisos)
                {
                    MessageBox.Show("Error al cargar permisos: " + exPermisos.Message);
                }

                reader.Close(); // Cerramos el lector de usuarios antes de hacer otra consulta

                // 3. Consultar el estado de inicio del sistema (caja abierta o cerrada)
                using (SqlCommand cmdEstado = new SqlCommand("SELECT Caja FROM Fech WHERE Id = 1", conectar))
                {
                    using (SqlDataReader rdrEstado = cmdEstado.ExecuteReader())
                    {
                        if (rdrEstado.Read())
                        {
                            string estadoInicio = rdrEstado["Caja"].ToString();

                            if (estadoInicio == "0")
                            {
                                // Abrir caja
                                frmAbrirCaja caja = new frmAbrirCaja();
                                caja.usuario = Sesion.NombreUsuario;
                                // caja.id = idMesero; // Descomenta y asigna si tu formulario de caja lo requiere
                                // caja.nombre = txtUsuario.Text; // Descomenta si lo requiere
                                caja.ShowDialog();
                                this.Hide();
                            }
                            else
                            {
                                // Ir al Form principal
                                frmPrincipal principal = new frmPrincipal();
                                principal.usuario = Sesion.NombreUsuario;
                                // principal.id = idMesero; // Descomenta si lo requiere
                                // principal.lblUser.Text = txtUsuario.Text; // Descomenta si lo requiere
                                principal.Show();
                                this.Hide();
                            }
                        }
                        else
                        {
                            MessageBox.Show("No se encontró información del estado del sistema.",
                                          "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

                CerrarTeclado();
            }
            else
            {
                MessageBox.Show("Contraseña incorrecta, favor de verificar.", "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtContraseña.Clear();
                txtContraseña.Focus();
                reader.Close();
            }
        }

        private void ValidarCajaYLicencia()
        {
            cmd = new SqlCommand("select * from Fech where Id=2;", conectar);
            SqlDataReader reader = cmd.ExecuteReader();
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
                        cmd = new SqlCommand("UPDATE Fech set Caja='0', Fecha='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where Id=2;", conectar);
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

      
    }
}