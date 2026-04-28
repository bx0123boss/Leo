using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmListadoConsignaciones : frmBase
    {
        public frmListadoConsignaciones()
        {
            InitializeComponent();
        }
        private void frmListadoConsignaciones_Load(object sender, EventArgs e)
        {
            EstilizarDataGridView(this.dataGridView1);
            EstilizarBotonPrimario(this.btnVerDetalle);
            EstilizarBotonPrimario(this.button1);
            EstilizarTextBox(this.txtBuscar);

            // 1. SELECCIONAMOS "TODAS" POR DEFECTO PRIMERO
            cmbEstado.SelectedIndex = 0;

            // 2. LUEGO CONFIGURAMOS LAS FECHAS
            dtpInicio.Value = DateTime.Now.AddDays(-30);
            dtpFin.Value = DateTime.Now;

            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.CadSQL))
                {
                    con.Open();

                    string query = @"SELECT Id, Folio, ClienteNombre AS Cliente, Fecha, Total, Estado 
                                     FROM Consignaciones 
                                     WHERE Fecha >= @Inicio AND Fecha <= @Fin ";

                    // Filtro por Estado
                    if (cmbEstado.SelectedItem.ToString() != "TODAS")
                    {
                        query += " AND Estado = @Estado ";
                    }

                    // Filtro por Texto (Folio o Cliente)
                    if (!string.IsNullOrWhiteSpace(txtBuscar.Text))
                    {
                        query += " AND (Folio LIKE @Filtro OR ClienteNombre LIKE @Filtro) ";
                    }

                    query += " ORDER BY Fecha DESC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Aseguramos que tome desde las 00:00:00 hasta las 23:59:59
                        cmd.Parameters.AddWithValue("@Inicio", dtpInicio.Value.Date);
                        cmd.Parameters.AddWithValue("@Fin", dtpFin.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59));

                        if (cmbEstado.SelectedItem.ToString() != "TODAS")
                        {
                            cmd.Parameters.AddWithValue("@Estado", cmbEstado.SelectedItem.ToString());
                        }

                        if (!string.IsNullOrWhiteSpace(txtBuscar.Text))
                        {
                            cmd.Parameters.AddWithValue("@Filtro", "%" + txtBuscar.Text.Trim() + "%");
                        }

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dataGridView1.DataSource = dt;

                        // Ocultamos la columna del ID interno de SQL Server
                        if (dataGridView1.Columns.Count > 0)
                        {
                            dataGridView1.Columns["Id"].Visible = false;

                            // Ajustamos anchos
                            dataGridView1.Columns["Folio"].Width = 120;
                            dataGridView1.Columns["Cliente"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                            dataGridView1.Columns["Fecha"].Width = 150;
                            dataGridView1.Columns["Total"].Width = 120;
                            dataGridView1.Columns["Estado"].Width = 100;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar consignaciones: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Este evento se disparará cuando cambies la fecha, el texto o el combobox
        private void Filtros_Changed(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void btnVerDetalle_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null || dataGridView1.CurrentRow.Index < 0)
            {
                MessageBox.Show("Por favor, seleccione una consignación de la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idConsignacion = Convert.ToInt32(dataGridView1.CurrentRow.Cells["Id"].Value);
            string folio = dataGridView1.CurrentRow.Cells["Folio"].Value.ToString();
            string estado = dataGridView1.CurrentRow.Cells["Estado"].Value.ToString();

            // Abrimos el formulario de liquidación
            using (frmLiquidarConsignacion liquidar = new frmLiquidarConsignacion())
            {
                liquidar.IdConsignacion = idConsignacion;
                liquidar.EstadoConsignacion = estado;
                liquidar.Folio = folio;
                liquidar.ShowDialog();
            }

            // Al cerrar la ventana, recargamos la tabla para que se actualicen los estados
            CargarDatos();
        }
        // Dar formato de moneda a la columna Total
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Total")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("C2");
                    e.FormattingApplied = true;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmNuevaConsignacion consigna = new frmNuevaConsignacion();
            consigna.ShowDialog();
        }
    }
}