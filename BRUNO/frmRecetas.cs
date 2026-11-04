using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace JaegerSoft
{
    public partial class frmRecetas : Form
    {
        private string idProductoFinal;
        private string nombreProductoFinal;
        private SqlConnection conectar = new SqlConnection(Conexion.CadSQL);

        public frmRecetas(string idProd, string nombreProd)
        {
            InitializeComponent();
            this.idProductoFinal = idProd;
            this.nombreProductoFinal = nombreProd;
        }

        private void frmRecetas_Load(object sender, EventArgs e)
        {
            lblProducto.Text = $"Receta para: {nombreProductoFinal} (ID: {idProductoFinal})";
            CargarComboInsumos();
            CargarInsumosReceta();
        }

        private void CargarComboInsumos()
        {
            try
            {
                DataTable dt = new DataTable();
                // Solo cargamos los productos marcados como Insumo (EsInsumo = 1)
                string query = "SELECT Id, Nombre, Especial, Uni FROM Inventario WHERE EsInsumo = 1 ORDER BY Nombre";
                SqlDataAdapter da = new SqlDataAdapter(query, conectar);
                da.Fill(dt);

                cmbInsumos.DataSource = dt;
                cmbInsumos.DisplayMember = "Nombre";
                cmbInsumos.ValueMember = "Id";
                cmbInsumos.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar lista de insumos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarInsumosReceta()
        {
            try
            {
                DataTable dt = new DataTable();
                string query = @"SELECT R.Id, R.IdInsumo, I.Nombre AS Insumo, R.CantidadRequerida, I.Uni AS Unidad, 
                                        I.Especial AS CostoUnitario, (R.CantidadRequerida * I.Especial) AS CostoSubtotal
                                 FROM ProductoReceta R
                                 INNER JOIN Inventario I ON R.IdInsumo = I.Id
                                 WHERE R.IdProductoFinal = @IdPan";

                using (SqlCommand cmd = new SqlCommand(query, conectar))
                {
                    cmd.Parameters.AddWithValue("@IdPan", idProductoFinal);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                dgvReceta.DataSource = dt;
                FormatearGrid();
                CalcularCostoTotal();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la receta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatearGrid()
        {
            if (dgvReceta.Columns.Count == 0) return;

            if (dgvReceta.Columns.Contains("Id")) dgvReceta.Columns["Id"].Visible = false;
            if (dgvReceta.Columns.Contains("IdInsumo")) dgvReceta.Columns["IdInsumo"].Visible = false;

            if (dgvReceta.Columns.Contains("Insumo")) dgvReceta.Columns["Insumo"].HeaderText = "Insumo / Materia Prima";
            if (dgvReceta.Columns.Contains("CantidadRequerida")) dgvReceta.Columns["CantidadRequerida"].HeaderText = "Cant. por Pieza";
            if (dgvReceta.Columns.Contains("Unidad")) dgvReceta.Columns["Unidad"].HeaderText = "Unidad";
            if (dgvReceta.Columns.Contains("CostoUnitario")) dgvReceta.Columns["CostoUnitario"].HeaderText = "Costo Insumo";
            if (dgvReceta.Columns.Contains("CostoSubtotal")) dgvReceta.Columns["CostoSubtotal"].HeaderText = "Subtotal";

            dgvReceta.Columns["CostoUnitario"].DefaultCellStyle.Format = "C4";
            dgvReceta.Columns["CostoSubtotal"].DefaultCellStyle.Format = "C4";
            dgvReceta.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void CalcularCostoTotal()
        {
            decimal total = 0;
            foreach (DataGridViewRow row in dgvReceta.Rows)
            {
                if (row.Cells["CostoSubtotal"].Value != DBNull.Value && row.Cells["CostoSubtotal"].Value != null)
                {
                    total += Convert.ToDecimal(row.Cells["CostoSubtotal"].Value);
                }
            }
            lblCostoTotal.Text = $"Costo de Insumos por Pieza: {total:C4}";
        }

        private void btnAgregarInsumo_Click(object sender, EventArgs e)
        {
            if (cmbInsumos.SelectedValue == null)
            {
                MessageBox.Show("Seleccione un insumo de la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtCantidad.Text, out decimal cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Ingrese una cantidad válida mayor a cero (ej. 0.050 para 50 gr).", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string idInsumo = cmbInsumos.SelectedValue.ToString();

            try
            {
                if (conectar.State == ConnectionState.Closed) conectar.Open();

                string sqlInsert = "INSERT INTO ProductoReceta (IdProductoFinal, IdInsumo, CantidadRequerida) VALUES (@IdPan, @IdInsumo, @Cant)";
                using (SqlCommand cmd = new SqlCommand(sqlInsert, conectar))
                {
                    cmd.Parameters.AddWithValue("@IdPan", idProductoFinal);
                    cmd.Parameters.AddWithValue("@IdInsumo", idInsumo);
                    cmd.Parameters.AddWithValue("@Cant", cantidad);
                    cmd.ExecuteNonQuery();
                }

                txtCantidad.Clear();
                cmbInsumos.SelectedIndex = -1;
                CargarInsumosReceta();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar insumo a la receta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminarInsumo_Click(object sender, EventArgs e)
        {
            if (dgvReceta.CurrentRow == null) return;

            string idRegistro = dgvReceta.CurrentRow.Cells["Id"].Value.ToString();

            try
            {
                if (conectar.State == ConnectionState.Closed) conectar.Open();

                string sqlDelete = "DELETE FROM ProductoReceta WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(sqlDelete, conectar))
                {
                    cmd.Parameters.AddWithValue("@Id", idRegistro);
                    cmd.ExecuteNonQuery();
                }

                CargarInsumosReceta();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar el insumo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}