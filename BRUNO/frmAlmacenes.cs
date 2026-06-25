using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;

namespace JaegerSoft
{
    public partial class frmAlmacenes : frmBase
    {
        private OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        private DataTable dtStock;

        public frmAlmacenes()
        {
            InitializeComponent();
        }

        private void frmAlmacenes_Load(object sender, EventArgs e)
        {
            try
            {
                EnsureDatabaseSchema();
                AplicarEstilos();
                CargarAlmacenes();
                CargarComboAlmacenes();
                RefrescarStock();
                RefrescarHistorial();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el módulo de almacenes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EnsureDatabaseSchema()
        {
            if (conectar.State != ConnectionState.Open)
            {
                conectar.Open();
            }

            // 1. Crear Tabla Almacenes
            bool almacenesExists = false;
            try
            {
                using (var cmd = new OleDbCommand("SELECT COUNT(*) FROM Almacenes", conectar))
                {
                    cmd.ExecuteScalar();
                    almacenesExists = true;
                }
            }
            catch
            {
                almacenesExists = false;
            }

            if (!almacenesExists)
            {
                try
                {
                    using (var cmd = new OleDbCommand("CREATE TABLE Almacenes (Id COUNTER PRIMARY KEY, Nombre VARCHAR(100))", conectar))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error creando tabla Almacenes: " + ex.Message);
                }
            }

            // 2. Crear Tabla InventarioAlmacenes
            bool inventarioAlmacenesExists = false;
            try
            {
                using (var cmd = new OleDbCommand("SELECT COUNT(*) FROM InventarioAlmacenes", conectar))
                {
                    cmd.ExecuteScalar();
                    inventarioAlmacenesExists = true;
                }
            }
            catch
            {
                inventarioAlmacenesExists = false;
            }

            if (!inventarioAlmacenesExists)
            {
                try
                {
                    using (var cmd = new OleDbCommand("CREATE TABLE InventarioAlmacenes (IdAlmacen INT, IdProducto VARCHAR(255), Existencia DOUBLE, CONSTRAINT PK_InventarioAlmacenes PRIMARY KEY (IdAlmacen, IdProducto))", conectar))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error creando tabla InventarioAlmacenes: " + ex.Message);
                }
            }

            // 3. Crear Tabla MovimientosAlmacenes
            bool movimientosAlmacenesExists = false;
            try
            {
                using (var cmd = new OleDbCommand("SELECT COUNT(*) FROM MovimientosAlmacenes", conectar))
                {
                    cmd.ExecuteScalar();
                    movimientosAlmacenesExists = true;
                }
            }
            catch
            {
                movimientosAlmacenesExists = false;
            }

            if (!movimientosAlmacenesExists)
            {
                try
                {
                    using (var cmd = new OleDbCommand("CREATE TABLE MovimientosAlmacenes (Id COUNTER PRIMARY KEY, IdOrigen INT, IdDestino INT, IdProducto VARCHAR(255), Cantidad DOUBLE, Fecha DATETIME, Usuario VARCHAR(100), Cancelado YESNO DEFAULT 0)", conectar))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error creando tabla MovimientosAlmacenes: " + ex.Message);
                }
            }
            else
            {
                // Verificar si existe la columna Cancelado en MovimientosAlmacenes
                try
                {
                    using (var cmd = new OleDbCommand("SELECT Cancelado FROM MovimientosAlmacenes WHERE 1=0", conectar))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch
                {
                    try
                    {
                        using (var cmd = new OleDbCommand("ALTER TABLE MovimientosAlmacenes ADD COLUMN Cancelado YESNO", conectar))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error agregando columna Cancelado a MovimientosAlmacenes: " + ex.Message);
                    }
                }
            }

            // Verificar si existe la columna IdAlmacen en Poliza
            try
            {
                using (var cmd = new OleDbCommand("SELECT IdAlmacen FROM Poliza WHERE 1=0", conectar))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                try
                {
                    using (var cmd = new OleDbCommand("ALTER TABLE Poliza ADD COLUMN IdAlmacen INT", conectar))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error agregando columna IdAlmacen a Poliza: " + ex.Message);
                }
            }
        }

        private void AplicarEstilos()
        {
            // Estilizar DataGridViews
            EstilizarDataGridView(dgvAlmacenes);
            EstilizarDataGridView(dgvStock);
            EstilizarDataGridView(dgvHistorial);

            // Estilizar TextBoxes
            EstilizarTextBox(txtNombreAlmacen);
            EstilizarTextBox(txtBuscarProducto);
            EstilizarTextBox(txtCodigoProducto);
            EstilizarTextBox(txtCantidadTraspaso);

            // Estilizar ComboBoxes
            EstilizarComboBox(cmbAlmacenStock);
            EstilizarComboBox(cmbOrigen);
            EstilizarComboBox(cmbDestino);

            // Estilizar Botones
            EstilizarBotonPrimario(btnCrearAlmacen);
            EstilizarBotonPrimario(btnTraspasar);
            EstilizarBotonAdvertencia(btnEditarAlmacen);
            EstilizarBotonPeligro(btnEliminarAlmacen);
            EstilizarBotonPeligro(btnCancelarMovimiento);
        }

        private void CargarAlmacenes()
        {
            if (conectar.State != ConnectionState.Open) conectar.Open();

            string query = "SELECT Id, Nombre FROM Almacenes ORDER BY Nombre;";
            using (var cmd = new OleDbCommand(query, conectar))
            {
                using (var da = new OleDbDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    dgvAlmacenes.DataSource = dt;
                    if (dgvAlmacenes.Columns.Count > 0)
                    {
                        dgvAlmacenes.Columns["Id"].Visible = false;
                        dgvAlmacenes.Columns["Nombre"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }
                }
            }
        }

        private void CargarComboAlmacenes()
        {
            if (conectar.State != ConnectionState.Open) conectar.Open();

            // Guardamos selección actual para restaurar si es posible
            int prevStockIdx = cmbAlmacenStock.SelectedIndex;
            int prevOrigIdx = cmbOrigen.SelectedIndex;
            int prevDestIdx = cmbDestino.SelectedIndex;

            // Almacén Principal + dinámicos
            var dtStockCmb = new DataTable();
            dtStockCmb.Columns.Add("Id", typeof(int));
            dtStockCmb.Columns.Add("Nombre", typeof(string));

            var dtOrigenCmb = new DataTable();
            dtOrigenCmb.Columns.Add("Id", typeof(int));
            dtOrigenCmb.Columns.Add("Nombre", typeof(string));

            // Para la vista de stock, agregamos la opción Global Desglosada (-1)
            dtStockCmb.Rows.Add(-1, "Todos los Almacenes (Desglose)");
            dtStockCmb.Rows.Add(0, "Almacén Principal / Bodega");

            // Para los combobox de traspaso no agregamos el global
            dtOrigenCmb.Rows.Add(0, "Almacén Principal / Bodega");
            var dtDestinoCmb = dtOrigenCmb.Copy();

            string query = "SELECT Id, Nombre FROM Almacenes ORDER BY Nombre;";
            using (var cmd = new OleDbCommand(query, conectar))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["Id"]);
                        string nombre = reader["Nombre"].ToString();

                        dtStockCmb.Rows.Add(id, nombre);
                        dtOrigenCmb.Rows.Add(id, nombre);
                        dtDestinoCmb.Rows.Add(id, nombre);
                    }
                }
            }

            // Asignar cmbAlmacenStock
            cmbAlmacenStock.DisplayMember = "Nombre";
            cmbAlmacenStock.ValueMember = "Id";
            cmbAlmacenStock.DataSource = dtStockCmb;

            // Asignar cmbOrigen
            cmbOrigen.DisplayMember = "Nombre";
            cmbOrigen.ValueMember = "Id";
            cmbOrigen.DataSource = dtOrigenCmb;

            // Asignar cmbDestino
            cmbDestino.DisplayMember = "Nombre";
            cmbDestino.ValueMember = "Id";
            cmbDestino.DataSource = dtDestinoCmb;

            // Restaurar índices o establecer por defecto
            if (prevStockIdx >= 0 && prevStockIdx < cmbAlmacenStock.Items.Count) cmbAlmacenStock.SelectedIndex = prevStockIdx;
            else if (cmbAlmacenStock.Items.Count > 0) cmbAlmacenStock.SelectedIndex = 0;

            if (prevOrigIdx >= 0 && prevOrigIdx < cmbOrigen.Items.Count) cmbOrigen.SelectedIndex = prevOrigIdx;
            else if (cmbOrigen.Items.Count > 0) cmbOrigen.SelectedIndex = 0;

            if (prevDestIdx >= 0 && prevDestIdx < cmbDestino.Items.Count) cmbDestino.SelectedIndex = prevDestIdx;
            else if (cmbDestino.Items.Count > 1) cmbDestino.SelectedIndex = 1;
            else if (cmbDestino.Items.Count > 0) cmbDestino.SelectedIndex = 0;
        }

        private void RefrescarStock()
        {
            if (cmbAlmacenStock.SelectedValue == null) return;
            if (cmbAlmacenStock.SelectedValue is DataRowView) return;
            if (conectar.State != ConnectionState.Open) conectar.Open();

            int idAlmacen = Convert.ToInt32(cmbAlmacenStock.SelectedValue);

            string query = "";

            if (idAlmacen == -1)
            {
                // TODOS LOS ALMACENES DESGLOSADOS (Consulta Dinámica - Pivot)
                DataTable dtAlms = new DataTable();
                using (var cmdA = new OleDbCommand("SELECT Id, Nombre FROM Almacenes ORDER BY Id", conectar))
                {
                    using (var daA = new OleDbDataAdapter(cmdA))
                    {
                        daA.Fill(dtAlms);
                    }
                }

                // Construimos la sentencia de variables dinámicas
                string selectCols = "I.Id, I.Nombre, I.Categoria, I.Existencia AS [Stock Ventas], ";
                string pivotSums = "";

                foreach (DataRow row in dtAlms.Rows)
                {
                    int aId = Convert.ToInt32(row["Id"]);
                    // Evitar caracteres que rompan SQL
                    string aName = row["Nombre"].ToString().Replace("]", "").Replace("[", "");

                    // MODIFICACIÓN: Ya no concatenamos la palabra "Stock ", solo el nombre del almacén
                    selectCols += $"IIF(IsNull(P.A{aId}), 0, P.A{aId}) AS [{aName}], ";
                    pivotSums += $"SUM(IIF(IdAlmacen = {aId}, Existencia, 0)) AS A{aId}, ";
                }

                selectCols += "(I.Existencia + IIF(IsNull(P.TotalIA), 0, P.TotalIA)) AS [TOTAL EN EXISTENCIA] ";
                pivotSums += "SUM(Existencia) AS TotalIA ";

                query = $"SELECT {selectCols} " +
                        $"FROM Inventario I LEFT JOIN ( " +
                        $"SELECT IdProducto, {pivotSums} FROM InventarioAlmacenes GROUP BY IdProducto " +
                        $") P ON I.Id = P.IdProducto " +
                        $"ORDER BY I.Nombre;";
            }
            else if (idAlmacen == 0)
            {
                // Almacén Principal
                query = "SELECT Id, Nombre, Categoria, Existencia FROM Inventario ORDER BY Nombre;";
            }
            else
            {
                // Almacén Dinámico Individual
                query = "SELECT I.Id, I.Nombre, I.Categoria, " +
                        "IIF(IsNull(IA.Existencia), 0, IA.Existencia) AS Existencia, " +
                        "I.Existencia AS ExistenciaPrincipal, " +
                        "(I.Existencia + IIF(IsNull(Totales.TotalIA), 0, Totales.TotalIA)) AS [TOTAL EN EXISTENCIA] " +
                        "FROM (Inventario I " +
                        "LEFT JOIN (SELECT IdProducto, Existencia FROM InventarioAlmacenes WHERE IdAlmacen = " + idAlmacen + ") IA ON I.Id = IA.IdProducto) " +
                        "LEFT JOIN (SELECT IdProducto, SUM(Existencia) AS TotalIA FROM InventarioAlmacenes GROUP BY IdProducto) Totales ON I.Id = Totales.IdProducto " +
                        "ORDER BY I.Nombre;";
            }

            using (var cmd = new OleDbCommand(query, conectar))
            {
                using (var da = new OleDbDataAdapter(cmd))
                {
                    dtStock = new DataTable();
                    da.Fill(dtStock);
                    dgvStock.DataSource = dtStock;

                    if (dgvStock.Columns.Count > 0)
                    {
                        // 1. Columnas Fijas 
                        dgvStock.Columns["Id"].HeaderText = "Código";
                        dgvStock.Columns["Id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                        dgvStock.Columns["Nombre"].HeaderText = "Descripción";
                        dgvStock.Columns["Nombre"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                        dgvStock.Columns["Categoria"].HeaderText = "Categoría";
                        dgvStock.Columns["Categoria"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                        // 2. Columna base (almacén individual o principal)
                        if (dgvStock.Columns.Contains("Existencia"))
                        {
                            dgvStock.Columns["Existencia"].HeaderText = (idAlmacen == 0) ? "Stock Ventas" : "Stock en Almacén";
                            dgvStock.Columns["Existencia"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                            dgvStock.Columns["Existencia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }

                        // 3. Columna Stock de Ventas (si se está en un almacén individual)
                        if (dgvStock.Columns.Contains("ExistenciaPrincipal"))
                        {
                            dgvStock.Columns["ExistenciaPrincipal"].HeaderText = "Stock Ventas";
                            dgvStock.Columns["ExistenciaPrincipal"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                            dgvStock.Columns["ExistenciaPrincipal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }

                        // 4. Columna Stock de Ventas (Para el desglose total dinámico "-1")
                        if (dgvStock.Columns.Contains("Stock Ventas"))
                        {
                            dgvStock.Columns["Stock Ventas"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                            dgvStock.Columns["Stock Ventas"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }

                        // 5. Configurar la columna final sumatoria
                        if (dgvStock.Columns.Contains("TOTAL EN EXISTENCIA"))
                        {
                            dgvStock.Columns["TOTAL EN EXISTENCIA"].HeaderText = "Stock Total";
                            dgvStock.Columns["TOTAL EN EXISTENCIA"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                            dgvStock.Columns["TOTAL EN EXISTENCIA"].DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                            dgvStock.Columns["TOTAL EN EXISTENCIA"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }

                        // 6. Darle estilo a las N columnas dinámicas (NombreAlmacen directo)
                        // Para detectarlas ahora, simplemente ignoramos las que ya sabemos que son fijas.
                        foreach (DataGridViewColumn col in dgvStock.Columns)
                        {
                            if (col.Name != "Id" &&
                                col.Name != "Nombre" &&
                                col.Name != "Categoria" &&
                                col.Name != "Stock Ventas" &&
                                col.Name != "Existencia" &&
                                col.Name != "ExistenciaPrincipal" &&
                                col.Name != "TOTAL EN EXISTENCIA")
                            {
                                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            }
                        }
                    }
                }
            }

            FiltrarStock();
        }

        private void FiltrarStock()
        {
            if (dtStock == null) return;

            string filtro = txtBuscarProducto.Text.Trim().Replace("'", "''");
            if (string.IsNullOrEmpty(filtro))
            {
                dtStock.DefaultView.RowFilter = "";
            }
            else
            {
                dtStock.DefaultView.RowFilter = string.Format("Id LIKE '%{0}%' OR Nombre LIKE '%{0}%' OR Categoria LIKE '%{0}%'", filtro);
            }
        }

        private void RefrescarHistorial()
        {
            if (conectar.State != ConnectionState.Open) conectar.Open();

            string query = "SELECT M.Id, M.IdOrigen, M.IdDestino, M.Cancelado, IIF(M.Cancelado = True, 'Cancelado', 'Activo') AS Estado, " +
                           "IIF(M.IdOrigen = 0, 'Almacén Principal (Bodega)', A1.Nombre) AS Origen, " +
                           "IIF(M.IdDestino = 0, 'Almacén Principal (Bodega)', A2.Nombre) AS Destino, " +
                           "M.IdProducto AS Código, P.Nombre AS Producto, M.Cantidad, M.Fecha, M.Usuario " +
                           "FROM ((MovimientosAlmacenes M LEFT JOIN Almacenes A1 ON M.IdOrigen = A1.Id) " +
                           "LEFT JOIN Almacenes A2 ON M.IdDestino = A2.Id) " +
                           "LEFT JOIN Inventario P ON M.IdProducto = P.Id " +
                           "ORDER BY M.Fecha DESC;";

            using (var cmd = new OleDbCommand(query, conectar))
            {
                using (var da = new OleDbDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    dgvHistorial.DataSource = dt;
                    if (dgvHistorial.Columns.Count > 0)
                    {
                        dgvHistorial.Columns["Id"].Visible = false;
                        dgvHistorial.Columns["IdOrigen"].Visible = false;
                        dgvHistorial.Columns["IdDestino"].Visible = false;
                        dgvHistorial.Columns["Cancelado"].Visible = false;
                        dgvHistorial.Columns["Producto"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        dgvHistorial.Columns["Fecha"].Width = 150;
                    }
                }
            }
        }

        private void btnCrearAlmacen_Click(object sender, EventArgs e)
        {
            string nombre = txtNombreAlmacen.Text.Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Por favor, ingrese un nombre de almacén.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (conectar.State != ConnectionState.Open) conectar.Open();

                // Verificar si ya existe
                string checkQuery = "SELECT COUNT(*) FROM Almacenes WHERE Nombre = @nombre";
                using (var cmdCheck = new OleDbCommand(checkQuery, conectar))
                {
                    cmdCheck.Parameters.AddWithValue("@nombre", nombre);
                    int count = Convert.ToInt32(cmdCheck.ExecuteScalar());
                    if (count > 0)
                    {
                        MessageBox.Show("Ya existe un almacén con este nombre.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Insertar
                string insertQuery = "INSERT INTO Almacenes (Nombre) VALUES (@nombre)";
                using (var cmdInsert = new OleDbCommand(insertQuery, conectar))
                {
                    cmdInsert.Parameters.AddWithValue("@nombre", nombre);
                    cmdInsert.ExecuteNonQuery();
                }

                txtNombreAlmacen.Clear();
                CargarAlmacenes();
                CargarComboAlmacenes();
                MessageBox.Show("Almacén creado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear almacén: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEditarAlmacen_Click(object sender, EventArgs e)
        {
            if (dgvAlmacenes.CurrentRow == null)
            {
                MessageBox.Show("Por favor, seleccione un almacén de la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(dgvAlmacenes.CurrentRow.Cells["Id"].Value);
            string nombreActual = dgvAlmacenes.CurrentRow.Cells["Nombre"].Value.ToString();

            string nuevoNombre = PromptDialog.ShowDialog("Modificar Nombre del Almacén", "Ingrese el nuevo nombre:", nombreActual);
            if (string.IsNullOrEmpty(nuevoNombre) || nuevoNombre.Trim() == nombreActual) return;

            try
            {
                if (conectar.State != ConnectionState.Open) conectar.Open();

                string updateQuery = "UPDATE Almacenes SET Nombre = @nombre WHERE Id = @id";
                using (var cmd = new OleDbCommand(updateQuery, conectar))
                {
                    cmd.Parameters.AddWithValue("@nombre", nuevoNombre.Trim());
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                CargarAlmacenes();
                CargarComboAlmacenes();
                MessageBox.Show("Nombre modificado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar almacén: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminarAlmacen_Click(object sender, EventArgs e)
        {
            if (dgvAlmacenes.CurrentRow == null)
            {
                MessageBox.Show("Por favor, seleccione un almacén de la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(dgvAlmacenes.CurrentRow.Cells["Id"].Value);
            string nombre = dgvAlmacenes.CurrentRow.Cells["Nombre"].Value.ToString();

            // Verificar si el almacén tiene stock
            try
            {
                if (conectar.State != ConnectionState.Open) conectar.Open();

                string checkQuery = "SELECT SUM(Existencia) FROM InventarioAlmacenes WHERE IdAlmacen = " + id;
                using (var cmd = new OleDbCommand(checkQuery, conectar))
                {
                    object result = cmd.ExecuteScalar();
                    double stockTotal = result != DBNull.Value ? Convert.ToDouble(result) : 0;
                    if (stockTotal > 0)
                    {
                        MessageBox.Show("No se puede eliminar el almacén '" + nombre + "' porque tiene mercancía en existencia (" + stockTotal + " unidades). Primero debe realizar traspasos para vaciarlo.", "No permitido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                var res = MessageBox.Show("¿Está seguro de eliminar el almacén '" + nombre + "'?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    // Eliminar registros de inventario de almacén (deberían ser cero/nulos, pero por seguridad limpiamos)
                    using (var cmdClean = new OleDbCommand("DELETE FROM InventarioAlmacenes WHERE IdAlmacen = " + id, conectar))
                    {
                        cmdClean.ExecuteNonQuery();
                    }

                    // Eliminar almacén
                    using (var cmdDel = new OleDbCommand("DELETE FROM Almacenes WHERE Id = " + id, conectar))
                    {
                        cmdDel.ExecuteNonQuery();
                    }

                    CargarAlmacenes();
                    CargarComboAlmacenes();
                    MessageBox.Show("Almacén eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar almacén: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbAlmacenStock_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefrescarStock();
        }

        private void txtBuscarProducto_TextChanged(object sender, EventArgs e)
        {
            FiltrarStock();
        }

        private void dgvAlmacenes_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAlmacenes.CurrentRow != null)
            {
                string nombre = dgvAlmacenes.CurrentRow.Cells["Nombre"].Value.ToString();
                txtNombreAlmacen.Text = nombre;
            }
        }

        private void cmbOrigen_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarInfoProducto();
        }

        private void cmbDestino_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarInfoProducto();
        }

        private void txtCodigoProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ActualizarInfoProducto();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void ActualizarInfoProducto()
        {
            string codigo = txtCodigoProducto.Text.Trim();
            if (string.IsNullOrEmpty(codigo))
            {
                lblNombreProductoVal.Text = "Ingrese un código";
                lblStockOrigenVal.Text = "0";
                lblStockDestinoVal.Text = "0";
                return;
            }

            if (cmbOrigen.SelectedValue == null || cmbDestino.SelectedValue == null) return;
            if (cmbOrigen.SelectedValue is DataRowView || cmbDestino.SelectedValue is DataRowView) return;

            int idOrigen = Convert.ToInt32(cmbOrigen.SelectedValue);
            int idDestino = Convert.ToInt32(cmbDestino.SelectedValue);

            try
            {
                if (conectar.State != ConnectionState.Open) conectar.Open();

                // Buscar Producto
                string queryProd = "SELECT Nombre, Existencia FROM Inventario WHERE Id = @id";
                string nombreProd = "";
                double stockPrincipal = 0;

                using (var cmd = new OleDbCommand(queryProd, conectar))
                {
                    cmd.Parameters.AddWithValue("@id", codigo);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            nombreProd = reader["Nombre"].ToString();
                            stockPrincipal = Convert.ToDouble(reader["Existencia"]);
                        }
                        else
                        {
                            reader.Close();
                            using (frmBuscarProductos buscar = new frmBuscarProductos())
                            {
                                buscar.compras = true;
                                buscar.textBox1.Text = codigo;
                                if (buscar.ShowDialog() == DialogResult.OK)
                                {
                                    txtCodigoProducto.Text = buscar.ID;
                                    ActualizarInfoProducto();
                                }
                                else
                                {
                                    lblNombreProductoVal.Text = "Producto no encontrado";
                                    lblStockOrigenVal.Text = "0";
                                    lblStockDestinoVal.Text = "0";
                                }
                            }
                            return;
                        }
                    }
                }

                lblNombreProductoVal.Text = nombreProd;

                // Obtener Stock de Origen
                double stockOrigen = 0;
                if (idOrigen == 0)
                {
                    stockOrigen = stockPrincipal;
                }
                else
                {
                    string qStock = "SELECT Existencia FROM InventarioAlmacenes WHERE IdAlmacen = " + idOrigen + " AND IdProducto = @id";
                    using (var cmd = new OleDbCommand(qStock, conectar))
                    {
                        cmd.Parameters.AddWithValue("@id", codigo);
                        object res = cmd.ExecuteScalar();
                        stockOrigen = res != null ? Convert.ToDouble(res) : 0;
                    }
                }
                lblStockOrigenVal.Text = stockOrigen.ToString();

                // Obtener Stock de Destino
                double stockDestino = 0;
                if (idDestino == 0)
                {
                    stockDestino = stockPrincipal;
                }
                else
                {
                    string qStock = "SELECT Existencia FROM InventarioAlmacenes WHERE IdAlmacen = " + idDestino + " AND IdProducto = @id";
                    using (var cmd = new OleDbCommand(qStock, conectar))
                    {
                        cmd.Parameters.AddWithValue("@id", codigo);
                        object res = cmd.ExecuteScalar();
                        stockDestino = res != null ? Convert.ToDouble(res) : 0;
                    }
                }
                lblStockDestinoVal.Text = stockDestino.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar producto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTraspasar_Click(object sender, EventArgs e)
        {
            if (cmbOrigen.SelectedValue == null || cmbDestino.SelectedValue == null) return;

            int idOrigen = Convert.ToInt32(cmbOrigen.SelectedValue);
            int idDestino = Convert.ToInt32(cmbDestino.SelectedValue);
            string codigo = txtCodigoProducto.Text.Trim();
            string nombreOrigenText = cmbOrigen.Text;
            string nombreDestinoText = cmbDestino.Text;

            if (idOrigen == idDestino)
            {
                MessageBox.Show("El almacén origen y destino deben ser diferentes.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(codigo))
            {
                MessageBox.Show("Por favor, ingrese o escanee el código del producto.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            double cantidad = 0;
            if (!double.TryParse(txtCantidadTraspaso.Text, out cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Por favor, ingrese una cantidad válida mayor a cero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            double stockOrigen = Convert.ToDouble(lblStockOrigenVal.Text);
            if (cantidad > stockOrigen)
            {
                MessageBox.Show("Stock insuficiente en el almacén de origen. Stock disponible: " + stockOrigen, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OleDbTransaction trans = null;
            try
            {
                if (conectar.State != ConnectionState.Open) conectar.Open();

                trans = conectar.BeginTransaction();
                string fechaActual = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                // 1. Descontar del origen
                double stockOrigAntes = stockOrigen;
                double stockOrigDespues = stockOrigAntes - cantidad;

                if (idOrigen == 0)
                {
                    // Almacén Principal: UPDATE Inventario
                    string query = "UPDATE Inventario SET Existencia = Existencia - @qty WHERE Id = @id";
                    using (var cmd = new OleDbCommand(query, conectar, trans))
                    {
                        cmd.Parameters.AddWithValue("@qty", cantidad);
                        cmd.Parameters.AddWithValue("@id", codigo);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Almacén Dinámico: UPDATE InventarioAlmacenes
                    string query = "UPDATE InventarioAlmacenes SET Existencia = Existencia - @qty WHERE IdAlmacen = @org AND IdProducto = @id";
                    using (var cmd = new OleDbCommand(query, conectar, trans))
                    {
                        cmd.Parameters.AddWithValue("@qty", cantidad);
                        cmd.Parameters.AddWithValue("@org", idOrigen);
                        cmd.Parameters.AddWithValue("@id", codigo);
                        cmd.ExecuteNonQuery();
                    }
                }

                // 2. Aumentar en el destino
                double stockDestAntes = Convert.ToDouble(lblStockDestinoVal.Text);
                double stockDestDespues = stockDestAntes + cantidad;

                if (idDestino == 0)
                {
                    // Almacén Principal: UPDATE Inventario
                    string query = "UPDATE Inventario SET Existencia = Existencia + @qty WHERE Id = @id";
                    using (var cmd = new OleDbCommand(query, conectar, trans))
                    {
                        cmd.Parameters.AddWithValue("@qty", cantidad);
                        cmd.Parameters.AddWithValue("@id", codigo);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Almacén Dinámico: Verificar si existe registro
                    string checkQuery = "SELECT COUNT(*) FROM InventarioAlmacenes WHERE IdAlmacen = @dst AND IdProducto = @id";
                    bool rowExists = false;
                    using (var cmd = new OleDbCommand(checkQuery, conectar, trans))
                    {
                        cmd.Parameters.AddWithValue("@dst", idDestino);
                        cmd.Parameters.AddWithValue("@id", codigo);
                        rowExists = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }

                    if (rowExists)
                    {
                        string query = "UPDATE InventarioAlmacenes SET Existencia = Existencia + @qty WHERE IdAlmacen = @dst AND IdProducto = @id";
                        using (var cmd = new OleDbCommand(query, conectar, trans))
                        {
                            cmd.Parameters.AddWithValue("@qty", cantidad);
                            cmd.Parameters.AddWithValue("@dst", idDestino);
                            cmd.Parameters.AddWithValue("@id", codigo);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string query = "INSERT INTO InventarioAlmacenes (IdAlmacen, IdProducto, Existencia) VALUES (@dst, @id, @qty)";
                        using (var cmd = new OleDbCommand(query, conectar, trans))
                        {
                            cmd.Parameters.AddWithValue("@dst", idDestino);
                            cmd.Parameters.AddWithValue("@id", codigo);
                            cmd.Parameters.AddWithValue("@qty", cantidad);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                // 3. Registrar Movimiento en MovimientosAlmacenes
                string queryLog = "INSERT INTO MovimientosAlmacenes (IdOrigen, IdDestino, IdProducto, Cantidad, Fecha, Usuario) " +
                                  "VALUES (@org, @dst, @id, @qty, @fecha, @usuario)";
                using (var cmd = new OleDbCommand(queryLog, conectar, trans))
                {
                    cmd.Parameters.AddWithValue("@org", idOrigen);
                    cmd.Parameters.AddWithValue("@dst", idDestino);
                    cmd.Parameters.AddWithValue("@id", codigo);
                    cmd.Parameters.AddWithValue("@qty", cantidad);
                    cmd.Parameters.AddWithValue("@fecha", fechaActual);
                    cmd.Parameters.AddWithValue("@usuario", string.IsNullOrEmpty(Sesion.NombreUsuario) ? "Sistema" : Sesion.NombreUsuario);
                    cmd.ExecuteNonQuery();
                }

                // 4. Registrar en Kardex
                if (idOrigen == 0)
                {
                    // Origen Principal -> Destino Dinámico (Salida del Principal)
                    // Buscamos el stock principal antes del movimiento
                    double stockPrincipalAntes = stockOrigAntes;
                    double stockPrincipalDespues = stockOrigDespues;

                    string queryKardex = "INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha, Precio) " +
                                         "VALUES (@id, 'TRASPASO_SALIDA', @desc, @antes, @despues, @fecha, 'NA')";
                    using (var cmd = new OleDbCommand(queryKardex, conectar, trans))
                    {
                        cmd.Parameters.AddWithValue("@id", codigo);
                        cmd.Parameters.AddWithValue("@desc", "TRASPASO A ALMACEN " + nombreDestinoText);
                        cmd.Parameters.AddWithValue("@antes", stockPrincipalAntes);
                        cmd.Parameters.AddWithValue("@despues", stockPrincipalDespues);
                        cmd.Parameters.AddWithValue("@fecha", fechaActual);
                        cmd.ExecuteNonQuery();
                    }
                }
                else if (idDestino == 0)
                {
                    // Origen Dinámico -> Destino Principal (Entrada al Principal)
                    // Buscamos el stock principal antes del movimiento
                    double stockPrincipalAntes = stockDestAntes;
                    double stockPrincipalDespues = stockDestDespues;

                    string queryKardex = "INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha, Precio) " +
                                         "VALUES (@id, 'TRASPASO_ENTRADA', @desc, @antes, @despues, @fecha, 'NA')";
                    using (var cmd = new OleDbCommand(queryKardex, conectar, trans))
                    {
                        cmd.Parameters.AddWithValue("@id", codigo);
                        cmd.Parameters.AddWithValue("@desc", "TRASPASO DESDE ALMACEN " + nombreOrigenText);
                        cmd.Parameters.AddWithValue("@antes", stockPrincipalAntes);
                        cmd.Parameters.AddWithValue("@despues", stockPrincipalDespues);
                        cmd.Parameters.AddWithValue("@fecha", fechaActual);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Traspaso entre dos Almacenes Dinámicos
                    // Loggeamos 2 registros detallados en Kardex: uno para el Origen y otro para el Destino
                    string queryKardexOrig = "INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha, Precio) " +
                                             "VALUES (@id, 'TRASPASO_SALIDA', @desc, @antes, @despues, @fecha, 'NA')";
                    using (var cmd = new OleDbCommand(queryKardexOrig, conectar, trans))
                    {
                        cmd.Parameters.AddWithValue("@id", codigo);
                        cmd.Parameters.AddWithValue("@desc", "TRASPASO DE " + nombreOrigenText + " A " + nombreDestinoText + " (ORIGEN)");
                        cmd.Parameters.AddWithValue("@antes", stockOrigAntes);
                        cmd.Parameters.AddWithValue("@despues", stockOrigDespues);
                        cmd.Parameters.AddWithValue("@fecha", fechaActual);
                        cmd.ExecuteNonQuery();
                    }

                    string queryKardexDest = "INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha, Precio) " +
                                             "VALUES (@id, 'TRASPASO_ENTRADA', @desc, @antes, @despues, @fecha, 'NA')";
                    using (var cmd = new OleDbCommand(queryKardexDest, conectar, trans))
                    {
                        cmd.Parameters.AddWithValue("@id", codigo);
                        cmd.Parameters.AddWithValue("@desc", "TRASPASO DE " + nombreOrigenText + " A " + nombreDestinoText + " (DESTINO)");
                        cmd.Parameters.AddWithValue("@antes", stockDestAntes);
                        cmd.Parameters.AddWithValue("@despues", stockDestDespues);
                        cmd.Parameters.AddWithValue("@fecha", fechaActual);
                        cmd.ExecuteNonQuery();
                    }
                }

                trans.Commit();

                txtCantidadTraspaso.Clear();
                txtCodigoProducto.Clear();
                lblNombreProductoVal.Text = "Seleccione un producto";
                lblStockOrigenVal.Text = "0";
                lblStockDestinoVal.Text = "0";

                RefrescarStock();
                RefrescarHistorial();

                MessageBox.Show("Traspaso realizado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                if (trans != null) trans.Rollback();
                MessageBox.Show("Error durante el traspaso: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabStock)
            {
                RefrescarStock();
            }
            else if (tabControl1.SelectedTab == tabHistorial)
            {
                RefrescarHistorial();
            }
        }

        private void btnCancelarMovimiento_Click(object sender, EventArgs e)
        {
            if (dgvHistorial.CurrentRow == null)
            {
                MessageBox.Show("Por favor, seleccione un movimiento del historial para cancelar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idMovimiento = Convert.ToInt32(dgvHistorial.CurrentRow.Cells["Id"].Value);
            int idOrigen = Convert.ToInt32(dgvHistorial.CurrentRow.Cells["IdOrigen"].Value);
            int idDestino = Convert.ToInt32(dgvHistorial.CurrentRow.Cells["IdDestino"].Value);
            string codigo = dgvHistorial.CurrentRow.Cells["Código"].Value.ToString();
            double cantidad = Convert.ToDouble(dgvHistorial.CurrentRow.Cells["Cantidad"].Value);
            bool cancelado = Convert.ToBoolean(dgvHistorial.CurrentRow.Cells["Cancelado"].Value);

            if (cancelado)
            {
                MessageBox.Show("Este movimiento ya se encuentra cancelado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validar existencia en el almacén de destino antes de revertir
            double stockDestinoActual = 0;
            if (idDestino == 0)
            {
                // Almacén Principal (Inventario)
                using (var cmd = new OleDbCommand("SELECT Existencia FROM Inventario WHERE Id = @id", conectar))
                {
                    cmd.Parameters.AddWithValue("@id", codigo);
                    object val = cmd.ExecuteScalar();
                    stockDestinoActual = val != null ? Convert.ToDouble(val) : 0;
                }
            }
            else
            {
                // Almacén Dinámico
                using (var cmd = new OleDbCommand("SELECT Existencia FROM InventarioAlmacenes WHERE IdAlmacen = @dst AND IdProducto = @id", conectar))
                {
                    cmd.Parameters.AddWithValue("@dst", idDestino);
                    cmd.Parameters.AddWithValue("@id", codigo);
                    object val = cmd.ExecuteScalar();
                    stockDestinoActual = val != null ? Convert.ToDouble(val) : 0;
                }
            }

            if (stockDestinoActual < cantidad)
            {
                MessageBox.Show("No se puede cancelar el movimiento. El stock actual del almacén destino (" + stockDestinoActual + ") es menor a la cantidad a retornar (" + cantidad + ").", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult confirm = MessageBox.Show("¿Está seguro que desea cancelar este movimiento de traspaso? Se retornarán " + cantidad + " unidades al almacén de origen.", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            OleDbTransaction trans = null;
            try
            {
                if (conectar.State != ConnectionState.Open) conectar.Open();
                trans = conectar.BeginTransaction();
                string fechaActual = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                // 1. Obtener stocks antes de la reversión (para Kardex)
                double stockOrigAntes = 0;
                if (idOrigen == 0)
                {
                    using (var cmd = new OleDbCommand("SELECT Existencia FROM Inventario WHERE Id = @id", conectar, trans))
                    {
                        cmd.Parameters.AddWithValue("@id", codigo);
                        stockOrigAntes = Convert.ToDouble(cmd.ExecuteScalar());
                    }
                }
                else
                {
                    using (var cmd = new OleDbCommand("SELECT Existencia FROM InventarioAlmacenes WHERE IdAlmacen = @org AND IdProducto = @id", conectar, trans))
                    {
                        cmd.Parameters.AddWithValue("@org", idOrigen);
                        cmd.Parameters.AddWithValue("@id", codigo);
                        object val = cmd.ExecuteScalar();
                        stockOrigAntes = val != null ? Convert.ToDouble(val) : 0;
                    }
                }

                double stockDestAntes = stockDestinoActual;

                // 2. Restar de la existencia en el almacén de destino (revertir entrega)
                if (idDestino == 0)
                {
                    using (var cmd = new OleDbCommand("UPDATE Inventario SET Existencia = Existencia - @qty WHERE Id = @id", conectar, trans))
                    {
                        cmd.Parameters.AddWithValue("@qty", cantidad);
                        cmd.Parameters.AddWithValue("@id", codigo);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (var cmd = new OleDbCommand("UPDATE InventarioAlmacenes SET Existencia = Existencia - @qty WHERE IdAlmacen = @dst AND IdProducto = @id", conectar, trans))
                    {
                        cmd.Parameters.AddWithValue("@qty", cantidad);
                        cmd.Parameters.AddWithValue("@dst", idDestino);
                        cmd.Parameters.AddWithValue("@id", codigo);
                        cmd.ExecuteNonQuery();
                    }
                }

                // 3. Aumentar la existencia en el almacén de origen (revertir salida)
                if (idOrigen == 0)
                {
                    using (var cmd = new OleDbCommand("UPDATE Inventario SET Existencia = Existencia + @qty WHERE Id = @id", conectar, trans))
                    {
                        cmd.Parameters.AddWithValue("@qty", cantidad);
                        cmd.Parameters.AddWithValue("@id", codigo);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Verificar si existe fila
                    bool rowExists = false;
                    using (var cmd = new OleDbCommand("SELECT COUNT(*) FROM InventarioAlmacenes WHERE IdAlmacen = @org AND IdProducto = @id", conectar, trans))
                    {
                        cmd.Parameters.AddWithValue("@org", idOrigen);
                        cmd.Parameters.AddWithValue("@id", codigo);
                        rowExists = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }

                    if (rowExists)
                    {
                        using (var cmd = new OleDbCommand("UPDATE InventarioAlmacenes SET Existencia = Existencia + @qty WHERE IdAlmacen = @org AND IdProducto = @id", conectar, trans))
                        {
                            cmd.Parameters.AddWithValue("@qty", cantidad);
                            cmd.Parameters.AddWithValue("@org", idOrigen);
                            cmd.Parameters.AddWithValue("@id", codigo);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        using (var cmd = new OleDbCommand("INSERT INTO InventarioAlmacenes (IdAlmacen, IdProducto, Existencia) VALUES (@org, @id, @qty)", conectar, trans))
                        {
                            cmd.Parameters.AddWithValue("@org", idOrigen);
                            cmd.Parameters.AddWithValue("@id", codigo);
                            cmd.Parameters.AddWithValue("@qty", cantidad);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                // 4. Actualizar estado del movimiento a Cancelado = True
                using (var cmd = new OleDbCommand("UPDATE MovimientosAlmacenes SET Cancelado = True WHERE Id = @id", conectar, trans))
                {
                    cmd.Parameters.AddWithValue("@id", idMovimiento);
                    cmd.ExecuteNonQuery();
                }

                // 5. Registrar cancelación en Kardex
                string nombreOrigenText = idOrigen == 0 ? "Almacén Principal" : GetNombreAlmacen(idOrigen, trans);
                string nombreDestinoText = idDestino == 0 ? "Almacén Principal" : GetNombreAlmacen(idDestino, trans);

                double stockOrigDespues = stockOrigAntes + cantidad;
                double stockDestDespues = stockDestAntes - cantidad;

                if (idOrigen == 0)
                {
                    // Retorno al principal (Entrada)
                    string queryKardex = "INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha, Precio) " +
                                         "VALUES (@id, 'TRASPASO_ENTRADA', @desc, @antes, @despues, @fecha, 'NA')";
                    using (var cmd = new OleDbCommand(queryKardex, conectar, trans))
                    {
                        cmd.Parameters.AddWithValue("@id", codigo);
                        cmd.Parameters.AddWithValue("@desc", "CANCELACION TRASPASO FOLIO: " + idMovimiento + " (RETORNO DESDE " + nombreDestinoText + ")");
                        cmd.Parameters.AddWithValue("@antes", stockOrigAntes);
                        cmd.Parameters.AddWithValue("@despues", stockOrigDespues);
                        cmd.Parameters.AddWithValue("@fecha", fechaActual);
                        cmd.ExecuteNonQuery();
                    }
                }
                else if (idDestino == 0)
                {
                    // Retorno desde el principal (Salida)
                    string queryKardex = "INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha, Precio) " +
                                         "VALUES (@id, 'TRASPASO_SALIDA', @desc, @antes, @despues, @fecha, 'NA')";
                    using (var cmd = new OleDbCommand(queryKardex, conectar, trans))
                    {
                        cmd.Parameters.AddWithValue("@id", codigo);
                        cmd.Parameters.AddWithValue("@desc", "CANCELACION TRASPASO FOLIO: " + idMovimiento + " (RETORNO A " + nombreOrigenText + ")");
                        cmd.Parameters.AddWithValue("@antes", stockDestAntes);
                        cmd.Parameters.AddWithValue("@despues", stockDestDespues);
                        cmd.Parameters.AddWithValue("@fecha", fechaActual);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Traspaso entre dinámicos (Registrar salida y entrada)
                    string queryKardexOrig = "INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha, Precio) " +
                                             "VALUES (@id, 'TRASPASO_ENTRADA', @desc, @antes, @despues, @fecha, 'NA')";
                    using (var cmd = new OleDbCommand(queryKardexOrig, conectar, trans))
                    {
                        cmd.Parameters.AddWithValue("@id", codigo);
                        cmd.Parameters.AddWithValue("@desc", "CANCELACION TRASPASO FOLIO: " + idMovimiento + " (RETORNO A " + nombreOrigenText + ")");
                        cmd.Parameters.AddWithValue("@antes", stockOrigAntes);
                        cmd.Parameters.AddWithValue("@despues", stockOrigDespues);
                        cmd.Parameters.AddWithValue("@fecha", fechaActual);
                        cmd.ExecuteNonQuery();
                    }

                    string queryKardexDest = "INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha, Precio) " +
                                             "VALUES (@id, 'TRASPASO_SALIDA', @desc, @antes, @despues, @fecha, 'NA')";
                    using (var cmd = new OleDbCommand(queryKardexDest, conectar, trans))
                    {
                        cmd.Parameters.AddWithValue("@id", codigo);
                        cmd.Parameters.AddWithValue("@desc", "CANCELACION TRASPASO FOLIO: " + idMovimiento + " (RETORNO DESDE " + nombreDestinoText + ")");
                        cmd.Parameters.AddWithValue("@antes", stockDestAntes);
                        cmd.Parameters.AddWithValue("@despues", stockDestDespues);
                        cmd.Parameters.AddWithValue("@fecha", fechaActual);
                        cmd.ExecuteNonQuery();
                    }
                }

                trans.Commit();

                RefrescarStock();
                RefrescarHistorial();

                MessageBox.Show("Movimiento cancelado con éxito. El stock ha sido retornado al almacén de origen.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                if (trans != null) trans.Rollback();
                MessageBox.Show("Error al cancelar el movimiento: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetNombreAlmacen(int id, OleDbTransaction trans)
        {
            string nombre = "Almacén " + id;
            using (var cmd = new OleDbCommand("SELECT Nombre FROM Almacenes WHERE Id = @id", conectar, trans))
            {
                cmd.Parameters.AddWithValue("@id", id);
                object val = cmd.ExecuteScalar();
                if (val != null) nombre = val.ToString();
            }
            return nombre;
        }
    }

    // Clase auxiliar estática para mostrar un cuadro de entrada de texto sencillo
    public static class PromptDialog
    {
        public static string ShowDialog(string title, string text, string defaultValue = "")
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 180,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = title,
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = Color.FromArgb(25, 25, 25),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.75F)
            };

            Label textLabel = new Label() { Left = 20, Top = 20, Width = 350, Text = text };
            TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 350, Text = defaultValue, BorderStyle = BorderStyle.FixedSingle, BackColor = Color.FromArgb(60, 60, 60), ForeColor = Color.White };
            Button confirmation = new Button() { Text = "Aceptar", Left = 270, Width = 100, Top = 90, Height = 30, FlatStyle = FlatStyle.Flat };
            confirmation.FlatAppearance.BorderSize = 0;
            confirmation.BackColor = Color.FromArgb(52, 152, 219);
            confirmation.DialogResult = DialogResult.OK;

            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}