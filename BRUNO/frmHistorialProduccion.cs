using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace JaegerSoft
{
    public partial class frmHistorialProduccion : frmBase
    {
        public string IdProductoSeleccionado { get; set; } = "";

        public frmHistorialProduccion()
        {
            InitializeComponent();
            AplicarEstilosPersonalizados();
        }

        private void AplicarEstilosPersonalizados()
        {
            EstilizarDataGridView(dgvLotes);
            EstilizarBotonPrimario(btnBuscar);
            EstilizarBotonPrimario(btnVerDetalle);
            EstilizarBotonPeligro(btnCancelarLote);
            EstilizarTextBox(txtBuscar);
            EstilizarComboBox(cmbUsuario);
            EstilizarComboBox(cmbEstado);
            EstilizarCheckBox(chkFiltrarFecha);
        }

        private void frmHistorialProduccion_Load(object sender, EventArgs e)
        {
            // Inicializar rango de fechas (últimos 30 días hasta hoy)
            dtpDesde.Value = DateTime.Today.AddDays(-30);
            dtpHasta.Value = DateTime.Today;

            CargarComboUsuarios();

            cmbEstado.SelectedIndex = 0; // "Todos"

            if (!string.IsNullOrWhiteSpace(IdProductoSeleccionado))
            {
                txtBuscar.Text = IdProductoSeleccionado;
            }

            CargarLotes();
        }

        private void CargarComboUsuarios()
        {
            try
            {
                cmbUsuario.Items.Clear();
                cmbUsuario.Items.Add("Todos los usuarios");

                using (SqlConnection con = new SqlConnection(Conexion.CadSQL))
                {
                    con.Open();
                    // Obtener usuarios de la tabla de usuarios y de los lotes ya registrados
                    string query = @"
                        SELECT DISTINCT Usuario FROM (
                            SELECT Usuario FROM Usuarios WHERE Usuario IS NOT NULL AND Usuario <> ''
                            UNION
                            SELECT Usuario FROM ProduccionLotes WHERE Usuario IS NOT NULL AND Usuario <> ''
                        ) U ORDER BY Usuario ASC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string usu = reader["Usuario"]?.ToString() ?? "";
                            if (!string.IsNullOrEmpty(usu))
                                cmbUsuario.Items.Add(usu);
                        }
                    }
                }

                cmbUsuario.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar lista de usuarios: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CargarLotes()
        {
            try
            {
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(Conexion.CadSQL))
                {
                    con.Open();

                    string query = @"
                        SELECT L.Id, L.Folio, L.Fecha, L.IdProductoFinal, 
                               ISNULL(I.Nombre, 'Producto No Encontrado') AS ProductoFinal, 
                               ISNULL(L.CantidadProducida, 0) AS CantidadProducida, 
                               L.Usuario, 
                               ISNULL(L.Cancelado, 0) AS Cancelado,
                               CASE WHEN ISNULL(L.Cancelado, 0) = 1 THEN 'CANCELADO' ELSE 'ACTIVO' END AS Estado,
                               L.FechaCancelado, 
                               L.UsuarioCancelado
                        FROM ProduccionLotes L
                        LEFT JOIN Inventario I ON L.IdProductoFinal = I.Id
                        WHERE 1=1 ";

                    if (chkFiltrarFecha.Checked)
                    {
                        query += " AND L.Fecha >= @FechaDesde AND L.Fecha <= @FechaHasta ";
                    }

                    if (!string.IsNullOrWhiteSpace(txtBuscar.Text))
                    {
                        query += " AND (L.Folio LIKE @Filtro OR L.IdProductoFinal LIKE @Filtro OR I.Nombre LIKE @Filtro) ";
                    }

                    if (cmbUsuario.SelectedIndex > 0 && cmbUsuario.SelectedItem != null)
                    {
                        query += " AND L.Usuario = @Usuario ";
                    }

                    if (cmbEstado.SelectedIndex == 1) // Solo Activos
                    {
                        query += " AND ISNULL(L.Cancelado, 0) = 0 ";
                    }
                    else if (cmbEstado.SelectedIndex == 2) // Solo Cancelados
                    {
                        query += " AND ISNULL(L.Cancelado, 0) = 1 ";
                    }

                    query += " ORDER BY L.Fecha DESC, L.Id DESC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (chkFiltrarFecha.Checked)
                        {
                            DateTime fDesde = dtpDesde.Value.Date;
                            DateTime fHasta = dtpHasta.Value.Date.AddDays(1).AddTicks(-1);
                            cmd.Parameters.AddWithValue("@FechaDesde", fDesde);
                            cmd.Parameters.AddWithValue("@FechaHasta", fHasta);
                        }

                        if (!string.IsNullOrWhiteSpace(txtBuscar.Text))
                        {
                            cmd.Parameters.AddWithValue("@Filtro", "%" + txtBuscar.Text.Trim() + "%");
                        }

                        if (cmbUsuario.SelectedIndex > 0 && cmbUsuario.SelectedItem != null)
                        {
                            cmd.Parameters.AddWithValue("@Usuario", cmbUsuario.SelectedItem.ToString());
                        }

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                }

                dgvLotes.DataSource = dt;
                FormatearGrid();
                CalcularResumen(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar lotes de producción: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatearGrid()
        {
            if (dgvLotes.Columns.Count == 0) return;

            if (dgvLotes.Columns.Contains("Id")) dgvLotes.Columns["Id"].Visible = false;
            if (dgvLotes.Columns.Contains("Cancelado")) dgvLotes.Columns["Cancelado"].Visible = false;

            if (dgvLotes.Columns.Contains("Folio"))
            {
                dgvLotes.Columns["Folio"].HeaderText = "Folio del Lote";
                dgvLotes.Columns["Folio"].Width = 145;
            }

            if (dgvLotes.Columns.Contains("Fecha"))
            {
                dgvLotes.Columns["Fecha"].HeaderText = "Fecha de Producción";
                dgvLotes.Columns["Fecha"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
                dgvLotes.Columns["Fecha"].Width = 140;
            }

            if (dgvLotes.Columns.Contains("IdProductoFinal"))
            {
                dgvLotes.Columns["IdProductoFinal"].HeaderText = "Código";
                dgvLotes.Columns["IdProductoFinal"].Width = 90;
            }

            if (dgvLotes.Columns.Contains("ProductoFinal"))
            {
                dgvLotes.Columns["ProductoFinal"].HeaderText = "Producto Elaborado";
            }

            if (dgvLotes.Columns.Contains("CantidadProducida"))
            {
                dgvLotes.Columns["CantidadProducida"].HeaderText = "Piezas";
                dgvLotes.Columns["CantidadProducida"].DefaultCellStyle.Format = "N0";
                dgvLotes.Columns["CantidadProducida"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvLotes.Columns["CantidadProducida"].Width = 85;
            }

            if (dgvLotes.Columns.Contains("Usuario"))
            {
                dgvLotes.Columns["Usuario"].HeaderText = "Productor / Panadero";
                dgvLotes.Columns["Usuario"].Width = 140;
            }

            if (dgvLotes.Columns.Contains("Estado"))
            {
                dgvLotes.Columns["Estado"].HeaderText = "Estado";
                dgvLotes.Columns["Estado"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvLotes.Columns["Estado"].Width = 100;
            }

            if (dgvLotes.Columns.Contains("FechaCancelado"))
            {
                dgvLotes.Columns["FechaCancelado"].HeaderText = "Fecha Cancelado";
                dgvLotes.Columns["FechaCancelado"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
                dgvLotes.Columns["FechaCancelado"].Width = 130;
            }

            if (dgvLotes.Columns.Contains("UsuarioCancelado"))
            {
                dgvLotes.Columns["UsuarioCancelado"].HeaderText = "Cancelado Por";
                dgvLotes.Columns["UsuarioCancelado"].Width = 120;
            }

            dgvLotes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void CalcularResumen(DataTable dt)
        {
            int totalLotes = dt.Rows.Count;
            double totalPiezas = 0;
            int lotesActivos = 0;
            int lotesCancelados = 0;

            foreach (DataRow row in dt.Rows)
            {
                if (row["CantidadProducida"] != DBNull.Value)
                    totalPiezas += Convert.ToDouble(row["CantidadProducida"]);

                bool canc = false;
                if (row["Cancelado"] != DBNull.Value)
                    canc = Convert.ToBoolean(row["Cancelado"]);

                if (canc)
                    lotesCancelados++;
                else
                    lotesActivos++;
            }

            lblTotalLotes.Text = totalLotes.ToString("N0");
            lblTotalPiezas.Text = $"{totalPiezas:N0} pz";
            lblLotesActivos.Text = lotesActivos.ToString("N0");
            lblLotesCancelados.Text = lotesCancelados.ToString("N0");
        }

        private void dgvLotes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvLotes.Rows.Count) return;

            var row = dgvLotes.Rows[e.RowIndex];
            bool cancelado = false;
            if (row.Cells["Cancelado"].Value != null && row.Cells["Cancelado"].Value != DBNull.Value)
            {
                bool.TryParse(row.Cells["Cancelado"].Value.ToString(), out cancelado);
            }

            if (dgvLotes.Columns[e.ColumnIndex].Name == "Estado")
            {
                if (cancelado)
                {
                    e.CellStyle.ForeColor = Color.FromArgb(231, 76, 60);
                    e.CellStyle.Font = new Font(dgvLotes.Font, FontStyle.Bold);
                }
                else
                {
                    e.CellStyle.ForeColor = Color.FromArgb(46, 204, 113);
                    e.CellStyle.Font = new Font(dgvLotes.Font, FontStyle.Bold);
                }
            }
            else if (cancelado)
            {
                e.CellStyle.ForeColor = Color.FromArgb(160, 160, 160);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarLotes();
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                CargarLotes();
            }
        }

        private void chkFiltrarFecha_CheckedChanged(object sender, EventArgs e)
        {
            dtpDesde.Enabled = chkFiltrarFecha.Checked;
            dtpHasta.Enabled = chkFiltrarFecha.Checked;
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscar.Clear();
            if (cmbUsuario.Items.Count > 0) cmbUsuario.SelectedIndex = 0;
            cmbEstado.SelectedIndex = 0;
            chkFiltrarFecha.Checked = true;
            dtpDesde.Value = DateTime.Today.AddDays(-30);
            dtpHasta.Value = DateTime.Today;
            CargarLotes();
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            ExportarGridAExcel(dgvLotes);
        }

        private void btnVerDetalle_Click(object sender, EventArgs e)
        {
            AbrirDetalleSeleccionado();
        }

        private void dgvLotes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                AbrirDetalleSeleccionado();
            }
        }

        private void AbrirDetalleSeleccionado()
        {
            if (dgvLotes.CurrentRow == null || dgvLotes.CurrentRow.Index < 0)
            {
                MessageBox.Show("Por favor, seleccione un lote de producción de la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int id = Convert.ToInt32(dgvLotes.CurrentRow.Cells["Id"].Value);
            string folio = dgvLotes.CurrentRow.Cells["Folio"].Value?.ToString() ?? "";

            using (frmDetalleProduccion frm = new frmDetalleProduccion(id, folio))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    CargarLotes();
                }
            }
        }

        private void btnCancelarLote_Click(object sender, EventArgs e)
        {
            if (dgvLotes.CurrentRow == null || dgvLotes.CurrentRow.Index < 0)
            {
                MessageBox.Show("Por favor, seleccione un lote de producción para cancelar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            bool cancelado = Convert.ToBoolean(dgvLotes.CurrentRow.Cells["Cancelado"].Value);
            if (cancelado)
            {
                MessageBox.Show("El lote seleccionado ya fue cancelado previamente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Abre el detalle donde está el botón de cancelación con la vista desglosada
            AbrirDetalleSeleccionado();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
