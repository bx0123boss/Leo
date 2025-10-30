using System;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmConfiguracionTicket : Form
    {
        // Puedes usar la cadena estática que ya tienes en tu clase Conexion
        private string CadCon = Conexion.CadCon;
        private int idConfiguracion = 1; // Id fijo o configurable
        private static string logoPath = @"C:\Jaeger Soft\logo.jpg";
        public frmConfiguracionTicket()
        {
            InitializeComponent();
        }

        // ==========================
        // EVENTO LOAD
        // ==========================
        private void FrmConfiguracion_Load(object sender, EventArgs e)
        {
            CargarConfiguracion();
        }

        // ==========================
        // CARGA DE DATOS
        // ==========================
        private void CargarConfiguracion()
        {
            try
            {
                using (var conexion = new OleDbConnection(CadCon))
                {
                    conexion.Open();
                    string query = "SELECT * FROM Configuracion WHERE Id = @id";
                    using (var cmd = new OleDbCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@id", idConfiguracion);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string datos = reader["DatosTicket"].ToString();
                                string pie = reader["PieDeTicket"].ToString();
                                string logo = reader["LogoPath"].ToString();

                                // Convertimos el texto con | a líneas separadas
                                txtEncabezado.Text = datos.Replace("|", Environment.NewLine);
                                txtPie.Text = pie.Replace("|", Environment.NewLine);
                                txtLogoPath.Text = logo;

                                if (File.Exists(logo))
                                    picLogo.Image = System.Drawing.Image.FromFile(logo);
                            }
                            else
                            {
                                MessageBox.Show("No se encontró configuración con el Id especificado.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar configuración: " + ex.Message);
            }
        }

        // ==========================
        // GUARDADO DE DATOS
        // ==========================
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                using (var conexion = new OleDbConnection(CadCon))
                {
                    conexion.Open();

                    string query = "UPDATE Configuracion SET DatosTicket = @datos, PieDeTicket = @pie, LogoPath = @logo WHERE Id = @id";

                    using (var cmd = new OleDbCommand(query, conexion))
                    {
                        // Convertir las líneas de texto de vuelta a | para la base de datos
                        string datos = txtEncabezado.Text.Replace(Environment.NewLine, "|");
                        string pie = txtPie.Text.Replace(Environment.NewLine, "|");
                        string logo = txtLogoPath.Text;

                        // IMPORTANTE: Los parámetros en el MISMO ORDEN que en la consulta
                        cmd.Parameters.AddWithValue("@datos", datos);
                        cmd.Parameters.AddWithValue("@pie", pie);
                        cmd.Parameters.AddWithValue("@logo", logo);
                        cmd.Parameters.AddWithValue("@id", idConfiguracion);

                        // Ejecutar la actualización sin preocuparse por el resultado
                        cmd.ExecuteNonQuery();
                        Conexion.CargarConfiguracion(idConfiguracion);
                        MessageBox.Show("Configuración guardada correctamente. Los datos surgiran efecto al reinicar el programa.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar configuración: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================
        // BUSCAR LOGO
        // ==========================
        private void btnBuscarLogo_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Seleccionar logo";
                ofd.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtLogoPath.Text = ofd.FileName;
                    picLogo.Image = System.Drawing.Image.FromFile(ofd.FileName);
                }
            }
        }

        // ==========================
        // CANCELAR
        // ==========================
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
