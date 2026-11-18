using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace JaegerSoft
{
    public partial class frmGruposCategorias : Form
    {
        private int idGrupoSeleccionado = 0;

        private class ItemCategoria
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public int? IdGrupoAsignado { get; set; }
            public string NombreGrupoAsignado { get; set; }

            public override string ToString()
            {
                if (IdGrupoAsignado.HasValue)
                    return $"{Id} - {Nombre}  [En: {NombreGrupoAsignado}]";
                return $"{Id} - {Nombre}";
            }
        }

      

        public frmGruposCategorias()
        {
            InitializeComponent();
        }

        private void frmGruposCategorias_Load(object sender, EventArgs e)
        {
            CargarCategoriasDisponibles();
            CargarGruposExistentes();
        }
        private void CargarCategoriasDisponibles()
        {
            clbCategorias.Items.Clear();
            string query = @"
            SELECT 
                c.Id, 
                c.Nombre,
                g.IdGrupo,
                g.Nombre AS NombreGrupo
            FROM Categorias c
            LEFT JOIN GruposCategoriasDetalle d ON c.Id = d.IdCategoria
            LEFT JOIN GruposCategorias g ON d.IdGrupo = g.IdGrupo AND g.Activo = 1
            ORDER BY c.Nombre ASC";

                using (SqlConnection con = new SqlConnection(Conexion.CadSQL))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            clbCategorias.Items.Add(new ItemCategoria
                            {
                                Id = Convert.ToInt32(dr["Id"]),
                                Nombre = dr["Nombre"].ToString(),
                                IdGrupoAsignado = dr["IdGrupo"] != DBNull.Value ? Convert.ToInt32(dr["IdGrupo"]) : (int?)null,
                                NombreGrupoAsignado = dr["NombreGrupo"]?.ToString()
                            });
                        }
                    }
                }
            }

        private void CargarGruposExistentes()
        {
            // STRING_AGG requiere SQL Server 2017+. Muestra en una sola celda las categorías englobadas.
            string query = @"
                SELECT 
                    g.IdGrupo,
                    g.Nombre AS [Grupo],
                    ISNULL(STRING_AGG(c.Nombre, ', '), 'Sin categorías') AS [Categorías Englobadas]
                FROM GruposCategorias g
                LEFT JOIN GruposCategoriasDetalle d ON g.IdGrupo = d.IdGrupo
                LEFT JOIN Categorias c ON d.IdCategoria = c.Id
                WHERE g.Activo = 1
                GROUP BY g.IdGrupo, g.Nombre
                ORDER BY g.Nombre ASC";

            using (SqlConnection con = new SqlConnection(Conexion.CadSQL))
            using (SqlDataAdapter da = new SqlDataAdapter(query, con))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvGrupos.DataSource = dt;

                if (dgvGrupos.Columns.Contains("IdGrupo"))
                    dgvGrupos.Columns["IdGrupo"].Visible = false;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Ingresa un nombre para el grupo (ej. Bebidas).", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return;
            }

            if (clbCategorias.CheckedItems.Count == 0)
            {
                MessageBox.Show("Selecciona al menos una categoría para englobar.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection con = new SqlConnection(Conexion.CadSQL))
            {
                foreach (ItemCategoria item in clbCategorias.CheckedItems)
                {
                    if (item.IdGrupoAsignado.HasValue && item.IdGrupoAsignado.Value != idGrupoSeleccionado)
                    {
                        MessageBox.Show(
                            $"La categoría '{item.Nombre}' ya está asignada al grupo '{item.NombreGrupoAsignado}'. Quítala de ese grupo primero para evitar duplicados en el reporte.",
                            "Categoría ocupada",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }
                }
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    int idGrupoActual = idGrupoSeleccionado;

                    if (idGrupoActual == 0)
                    {
                        // Insertar nuevo grupo
                        string sqlInsertGrupo = "INSERT INTO GruposCategorias (Nombre) OUTPUT INSERTED.IdGrupo VALUES (@Nombre)";
                        using (SqlCommand cmd = new SqlCommand(sqlInsertGrupo, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                            idGrupoActual = (int)cmd.ExecuteScalar();
                        }
                    }
                    else
                    {
                        // Actualizar nombre del grupo existente y limpiar su detalle anterior
                        string sqlUpdateGrupo = "UPDATE GruposCategorias SET Nombre = @Nombre WHERE IdGrupo = @IdGrupo";
                        using (SqlCommand cmd = new SqlCommand(sqlUpdateGrupo, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                            cmd.Parameters.AddWithValue("@IdGrupo", idGrupoActual);
                            cmd.ExecuteNonQuery();
                        }

                        string sqlDeleteDet = "DELETE FROM GruposCategoriasDetalle WHERE IdGrupo = @IdGrupo";
                        using (SqlCommand cmd = new SqlCommand(sqlDeleteDet, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@IdGrupo", idGrupoActual);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Insertar las categorías marcadas
                    string sqlInsertDet = "INSERT INTO GruposCategoriasDetalle (IdGrupo, IdCategoria) VALUES (@IdGrupo, @IdCategoria)";
                    foreach (ItemCategoria item in clbCategorias.CheckedItems)
                    {
                        using (SqlCommand cmd = new SqlCommand(sqlInsertDet, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@IdGrupo", idGrupoActual);
                            cmd.Parameters.AddWithValue("@IdCategoria", item.Id);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    tran.Commit();
                    MessageBox.Show("Grupo guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    CargarGruposExistentes();
                }
                catch (SqlException ex)
                {
                    tran.Rollback();
                    if (ex.Number == 2627) // Violación de restricción UNIQUE
                        MessageBox.Show("Una de las categorías seleccionadas ya pertenece a otro grupo.", "Error de duplicidad", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show("Error en base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvGrupos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            idGrupoSeleccionado = Convert.ToInt32(dgvGrupos.Rows[e.RowIndex].Cells["IdGrupo"].Value);
            txtNombre.Text = dgvGrupos.Rows[e.RowIndex].Cells["Grupo"].Value.ToString();
            btnGuardar.Text = "Actualizar Grupo";

            // Desmarcar todos primero
            for (int i = 0; i < clbCategorias.Items.Count; i++)
                clbCategorias.SetItemChecked(i, false);

            // Consultar categorías asociadas al grupo
            List<int> idsAsociados = new List<int>();
            string query = "SELECT IdCategoria FROM GruposCategoriasDetalle WHERE IdGrupo = @IdGrupo";

            using (SqlConnection con = new SqlConnection(Conexion.CadSQL))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@IdGrupo", idGrupoSeleccionado);
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                        idsAsociados.Add(Convert.ToInt32(dr["IdCategoria"]));
                }
            }

            // Marcar en el CheckedListBox las categorías correspondientes
            for (int i = 0; i < clbCategorias.Items.Count; i++)
            {
                var item = (ItemCategoria)clbCategorias.Items[i];
                if (idsAsociados.Contains(item.Id))
                    clbCategorias.SetItemChecked(i, true);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvGrupos.CurrentRow == null) return;

            int idEliminar = Convert.ToInt32(dgvGrupos.CurrentRow.Cells["IdGrupo"].Value);
            string nombre = dgvGrupos.CurrentRow.Cells["Grupo"].Value.ToString();

            if (MessageBox.Show($"¿Seguro que deseas eliminar el grupo '{nombre}'?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Borrado lógico (o físico con DELETE FROM GruposCategorias WHERE IdGrupo = @IdGrupo)
                string query = "UPDATE GruposCategorias SET Activo = 0 WHERE IdGrupo = @IdGrupo";
                using (SqlConnection con = new SqlConnection(Conexion.CadSQL))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdGrupo", idEliminar);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                LimpiarFormulario();
                CargarGruposExistentes();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void LimpiarFormulario()
        {
            idGrupoSeleccionado = 0;
            txtNombre.Clear();
            btnGuardar.Text = "Guardar Grupo";
            for (int i = 0; i < clbCategorias.Items.Count; i++)
                clbCategorias.SetItemChecked(i, false);
            txtNombre.Focus();
        }
    }
}