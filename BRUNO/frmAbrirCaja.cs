using JaegerSoft;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace JaegerSoft
{
    public partial class frmAbrirCaja : frmBase
    {
        SqlConnection conectar = new SqlConnection(Conexion.CadSQL);
        SqlCommand cmd;
        public string usuario, nombre;
        public int id = 0;

        public frmAbrirCaja()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            aceptar();
        }

        private void txtIngreso_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) // Enter
            {
                aceptar();
            }
            else if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != '.'))
            {
                // Agregué la validación del punto decimal por si el fondo de caja lleva centavos
                MessageBox.Show("Solo se permiten numeros", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
        }

        private void frmAbrirCaja_Load(object sender, EventArgs e)
        {
            EstilizarTextBox(txtIngreso);
            EstilizarBotonPrimario(button1);
        }

        private void frmAbrirCaja_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (conectar.State == ConnectionState.Closed)
                conectar.Open();
            using (SqlCommand cmdUpdate = new SqlCommand("UPDATE Fech SET Caja='1', Fecha=@Fecha WHERE Id=1;", conectar))
            {
                cmdUpdate.Parameters.AddWithValue("@Fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmdUpdate.ExecuteNonQuery();
            }

            frmPrincipal principal = new frmPrincipal();
            principal.usuario = Sesion.NombreUsuario;
            principal.Show();
        }

        public void aceptar()
        {
            try
            {
                conectar.Open();
                if (!string.IsNullOrEmpty(txtIngreso.Text) && txtIngreso.Text != "0")
                {
                    string query = @"INSERT INTO Corte (Concepto, Monto, FechaHora, Pago) VALUES
                                    ('APERTURA DE CAJA', @Total, @Fecha, 'Efectivo')";

                    using (SqlCommand cmd2 = new SqlCommand(query, conectar))
                    {
                        cmd2.Parameters.AddWithValue("@Total", Convert.ToDouble(txtIngreso.Text));
                        cmd2.Parameters.AddWithValue("@Fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd2.ExecuteNonQuery();
                    }
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir la caja: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Asegurarnos de cerrar la conexión siempre
                if (conectar.State == ConnectionState.Open)
                {
                    conectar.Close();
                }
            }
        }
    }
}