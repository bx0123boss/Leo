using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace JaegerSoft
{
    public partial class frmConfiguracionTicket : Form
    {
        private string CadCon = Conexion.CadSQL;
        private int idConfiguracion = 1;
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
                using (var conexion = new SqlConnection(CadCon))
                {
                    conexion.Open();
                    string query = "SELECT * FROM Configuracion WHERE Id = @id";
                    using (var cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@id", idConfiguracion);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string datos = reader["DatosTicket"].ToString();
                                string pie = reader["PieDeTicket"].ToString();
                                string logo = reader["LogoPath"].ToString();
                                string whatsapp = reader["Whatsapp"].ToString();
                                bool mediaCarta = Convert.ToBoolean(reader["MediaCarta"]);
                                bool bascula = Convert.ToBoolean(reader["Bascula"]);
                                bool ticket = Convert.ToBoolean(reader["Ticket"]);

                                // Convertimos el texto con | a líneas separadas
                                txtEncabezado.Text = datos.Replace("|", Environment.NewLine);
                                txtPie.Text = pie.Replace("|", Environment.NewLine);
                                txtLogoPath.Text = logo;
                                txtWhatsapp.Text = whatsapp;
                                ckbBascula.Checked = bascula;
                                ckbMediaCarta.Checked = mediaCarta;
                                ckbTicket.Checked = ticket;
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
                using (var conexion = new SqlConnection(CadCon))
                {
                    conexion.Open();

                    string query = "UPDATE Configuracion SET DatosTicket = @datos, PieDeTicket = @pie, LogoPath = @logo, Whatsapp = @whats, Bascula = @bascula, MediaCarta = @mediaCarta, Ticket = @ticket WHERE Id = @id";

                    using (var cmd = new SqlCommand(query, conexion))
                    {
                        string datos = txtEncabezado.Text.Replace(Environment.NewLine, "|");
                        string pie = txtPie.Text.Replace(Environment.NewLine, "|");
                        string logo = txtLogoPath.Text;
                        string whatsapp = txtWhatsapp.Text;
                        bool bascula = ckbBascula.Checked;
                        bool mediaCarta = ckbMediaCarta.Checked;
                        bool ticket = ckbTicket.Checked;

                        cmd.Parameters.AddWithValue("@datos", datos);
                        cmd.Parameters.AddWithValue("@pie", pie);
                        cmd.Parameters.AddWithValue("@logo", logo);
                        cmd.Parameters.AddWithValue("@whats", whatsapp);
                        cmd.Parameters.AddWithValue("@bascula", bascula);
                        cmd.Parameters.AddWithValue("@mediaCarta", mediaCarta);
                        cmd.Parameters.AddWithValue("@ticket", mediaCarta);
                        cmd.Parameters.AddWithValue("@id", ticket);

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

        private void ckbMediaCarta_CheckedChanged(object sender, EventArgs e)
        {
            ckbTicket.Checked = ckbMediaCarta.Checked;
        }
    }
}
