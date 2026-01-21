using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BRUNO
{
    public partial class frmCotizacion : frmBase
    {
        public string ID { get; set; }
        public string Nombre { get; set; }
        public string Folio { get; set; }

        public frmCotizacion()
        {
            InitializeComponent();
        }

        private void frmCotizacion_Load(object sender, EventArgs e)
        {
            
            EstilizarDataGridView(this.dataGridView1);
            this.dataGridView1.ReadOnly = true;
            EstilizarTextBox(this.textBox1); // Nombre
            EstilizarTextBox(this.textBox2); // ID
            CargarHistorial();
        }

        private void CargarHistorial()
        {
            try
            {
                string query = "SELECT Id, ClienteId, ClienteNombre, Fecha, Total, Observaciones FROM Cotizaciones ORDER BY Fecha DESC";

                using (SqlConnection conn = new SqlConnection(Conexion.CadSQL))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);
                            dataGridView1.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar historial: " + ex.Message);
            }
        }

        // Búsqueda por Nombre
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = string.Format("ClienteNombre LIKE '%{0}%'", textBox1.Text);
        }

        // Búsqueda por ID
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = string.Format("Convert(ClienteId, 'System.String') LIKE '%{0}%'", textBox2.Text);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                this.ID = dataGridView1.Rows[e.RowIndex].Cells["ClienteId"].Value.ToString();
                this.Nombre = dataGridView1.Rows[e.RowIndex].Cells["ClienteNombre"].Value.ToString();
                this.Folio = dataGridView1.Rows[e.RowIndex].Cells["Id"].Value.ToString();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}