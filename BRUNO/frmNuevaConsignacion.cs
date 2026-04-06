using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmNuevaConsignacion : frmBase
    {
        OleDbConnection conectarAccess = new OleDbConnection(Conexion.CadCon);
        OleDbCommand cmd;
        double total = 0;
        string origen = "";
        string idCliente = "0";
        string observaciones = "", datos = "";
        public string usuario = "", idUsuario = "";

        public frmNuevaConsignacion()
        {
            InitializeComponent();
            this.MinimumSize = new Size(1066, 418);
        }

        private void frmNuevaConsignacion_Load(object sender, EventArgs e)
        {
            EstilizarDataGridView(this.dataGridView1);
            EstilizarBotonPrimario(this.btnGenerarConsigna);
            EstilizarBotonAdvertencia(this.button3); // Botón Buscar Producto
            EstilizarBotonPeligro(this.button1); // Botón Eliminar
            EstilizarTextBox(this.textBox1);

            lblFolio.Visible = false;
            label2.Visible = false;

            // Configurar columnas editables
            dataGridView1.Columns[0].ReadOnly = false; // Cantidad
            if (Conexion.lugar == "DEPORTES LEO")
            {
                dataGridView1.Columns[2].ReadOnly = false; // Precio
            }
            if (Conexion.lugar == "TURBO LLANTAS")
            {
                dataGridView1.Columns[2].ReadOnly = false;
                dataGridView1.Columns[1].ReadOnly = false; // Descripción
            }
        }

        public void ReiniciarForm()
        {
            total = 0;
            observaciones = "";
            datos = "";
            origen = "";
            idCliente = "0";
            lblCliente.Text = "PUBLICO EN GENERAL";
            lblDatosCotizacion.Text = "Sin datos extra";
            dataGridView1.Rows.Clear();
            lblTotal.Text = $"{RecalcularTotal:C}";
            textBox1.Text = "";
            textBox1.Focus();
        }

        private double RecalcularTotal
        {
            get
            {
                total = 0;
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    total += Convert.ToDouble(dataGridView1.Rows[i].Cells[3].Value); // Columna Monto
                }
                return total;
            }
        }

        // =========================================================================
        // EVENTOS DE AGREGAR PRODUCTOS (Báscula, Lector de código y Buscador)
        // =========================================================================

        private double ObtenerPesoLocal()
        {
            if (!frmPrincipal.IsAgenteBasculaActivo)
            {
                return 1;
            }
            try
            {
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("http://localhost:8080/peso/");
                request.Timeout = 300;
                request.Method = "GET";

                using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse())
                using (System.IO.StreamReader stream = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    string respuesta = stream.ReadToEnd();
                    if (respuesta != "ERROR")
                    {
                        return double.Parse(respuesta, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
            }
            catch (Exception) { }
            return 0;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text)) return;
                string input = textBox1.Text.Trim();

                try
                {
                    conectarAccess.Open();
                    cmd = new OleDbCommand("select count(*) from Inventario where Id='" + input + "';", conectarAccess);
                    int valor = int.Parse(cmd.ExecuteScalar().ToString());

                    if (valor == 1)
                    {
                        cmd = new OleDbCommand("select * from Inventario where Id='" + input + "';", conectarAccess);
                        OleDbDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            using (frmPrecio buscar = new frmPrecio())
                            {
                                if (buscar.ShowDialog() == DialogResult.OK)
                                {
                                    double preci = (buscar.tipo == "GEN") ? Convert.ToDouble(reader[3]) : Convert.ToDouble(reader[2]);
                                    double cantidad = ObtenerPesoLocal();
                                    if (cantidad <= 0) cantidad = 1;
                                    double importe = preci * cantidad;

                                    dataGridView1.Rows.Add(
                                        cantidad.ToString(),
                                        reader[1].ToString(),
                                        String.Format("{0:0.00}", preci),
                                        String.Format("{0:0.00}", importe),
                                        reader[4].ToString(), reader[0].ToString(), origen, reader[8].ToString(), reader[7].ToString(), "", "X");
                                }
                            }
                        }
                        reader.Close();
                        lblTotal.Text = $"{RecalcularTotal:C}";
                        textBox1.Text = "";
                    }
                    else
                    {
                        using (frmBuscarProductos buscar = new frmBuscarProductos())
                        {
                            buscar.textBox1.Text = input;
                            if (buscar.ShowDialog() == DialogResult.OK)
                            {
                                double cantidad = ObtenerPesoLocal();
                                if (cantidad <= 0) cantidad = 1;

                                double precioUnitario = Convert.ToDouble(buscar.precio);
                                double importeCalculado = precioUnitario * cantidad;

                                dataGridView1.Rows.Add(
                                    cantidad.ToString(),
                                    buscar.producto,
                                    String.Format("{0:0.00}", precioUnitario),
                                    String.Format("{0:0.00}", importeCalculado),
                                    buscar.existencia, buscar.ID, origen, buscar.IVA, buscar.compra, "", "X"
                                );
                                lblTotal.Text = $"{RecalcularTotal:C}";
                            }
                        }
                        textBox1.Text = "";
                    }
                }
                finally
                {
                    if (conectarAccess.State == ConnectionState.Open) conectarAccess.Close();
                }
                e.Handled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (frmBuscarProductos buscar = new frmBuscarProductos())
            {
                if (buscar.ShowDialog() == DialogResult.OK)
                {
                    double cantidad = ObtenerPesoLocal();
                    if (cantidad <= 0) cantidad = 1;

                    double precio = Convert.ToDouble(buscar.precio);
                    double importe = precio * cantidad;

                    dataGridView1.Rows.Add(
                        cantidad.ToString(),
                        buscar.producto,
                        String.Format("{0:0.00}", precio),
                        String.Format("{0:0.00}", importe),
                        buscar.existencia, buscar.ID, origen, buscar.IVA, buscar.compra, "", "X");
                }
            }
            lblTotal.Text = $"{RecalcularTotal:C}";
            textBox1.Focus();
        }

        // =========================================================================
        // EVENTOS DE EDICIÓN Y ELIMINACIÓN EN EL GRID
        // =========================================================================

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                double cantidad = Convert.ToDouble(dataGridView1[0, e.RowIndex].Value.ToString());
                double precio = Convert.ToDouble(dataGridView1[2, e.RowIndex].Value.ToString());
                double monto = cantidad * precio;
                dataGridView1.Rows[e.RowIndex].Cells[3].Value = String.Format("{0:0.00}", monto);
            }
            catch
            {
                MessageBox.Show("Solo puedes introducir números válidos.", "Alto", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataGridView1.Rows[e.RowIndex].Cells[0].Value = "1";
                double cantidad = 1;
                double precio = Convert.ToDouble(dataGridView1[2, e.RowIndex].Value.ToString());
                double monto = cantidad * precio;
                dataGridView1.Rows[e.RowIndex].Cells[3].Value = String.Format("{0:0.00}", monto);
            }
            lblTotal.Text = $"{RecalcularTotal:C}";
            textBox1.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EliminarProductos();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Índice 10 es el botón 'btnEliminar' con la X
            if (e.ColumnIndex == 10 && e.RowIndex >= 0)
            {
                dataGridView1.Rows.RemoveAt(e.RowIndex);
                lblTotal.Text = $"{RecalcularTotal:C}";
                textBox1.Focus();
            }
        }

        private void EliminarProductos()
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione al menos una fila para eliminar.", "Alto!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    if (row.DataBoundItem != null)
                    {
                        ((DataRowView)row.DataBoundItem).Row.Delete();
                    }
                    dataGridView1.Rows.Remove(row);
                }
                lblTotal.Text = $"{RecalcularTotal:C}";
                textBox1.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar filas: {ex.Message}");
            }
        }

        // =========================================================================
        // CLIENTE, EXTRAS Y GUARDADO DE CONSIGNA
        // =========================================================================

        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            using (frmBuscaCliente cliente = new frmBuscaCliente())
            {
                if (cliente.ShowDialog() == DialogResult.OK)
                {
                    idCliente = cliente.ID;
                    lblCliente.Text = cliente.Nombre;
                }
            }
        }

        private void btnGenerarConsigna_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount == 0)
            {
                MessageBox.Show("No hay productos para consignar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (idCliente == "0" || string.IsNullOrEmpty(idCliente))
            {
                MessageBox.Show("Debe seleccionar un cliente obligatoriamente para una consignación.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult respuesta = MessageBox.Show("¿Desea generar la salida a consignación? Se descontará el inventario físico.", "Confirmar Consigna", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (respuesta != DialogResult.Yes) return;

            GuardarConsignacion();
        }

        private void GuardarConsignacion()
        {
            try
            {
                // 1. GENERAR FOLIO
                string folioConsigna = "CON-" + DateTime.Now.ToString("yyMMddHHmmss");
                int idConsignacionInsertada = 0;

                // 2. GUARDAR EN SQL SERVER
                using (SqlConnection conSql = new SqlConnection(Conexion.CadSQL))
                {
                    conSql.Open();
                    SqlTransaction transaccionSql = conSql.BeginTransaction();

                    try
                    {
                        string queryPadre = @"INSERT INTO Consignaciones (Folio, ClienteId, ClienteNombre, Fecha, Total, Estado, Observaciones, Datos) 
                                              OUTPUT INSERTED.Id 
                                              VALUES (@Folio, @CId, @CNom, GETDATE(), @Total, 'Pendiente', @Obs, @Dat)";
                        using (SqlCommand cmdSql = new SqlCommand(queryPadre, conSql, transaccionSql))
                        {
                            cmdSql.Parameters.AddWithValue("@Folio", folioConsigna);
                            cmdSql.Parameters.AddWithValue("@CId", idCliente);
                            cmdSql.Parameters.AddWithValue("@CNom", lblCliente.Text);
                            cmdSql.Parameters.AddWithValue("@Total", total);
                            cmdSql.Parameters.AddWithValue("@Obs", observaciones);
                            cmdSql.Parameters.AddWithValue("@Dat", datos);

                            idConsignacionInsertada = (int)cmdSql.ExecuteScalar();
                        }

                        string queryDetalle = @"INSERT INTO DetalleConsignacion (ConsignacionId, ProductoCodigo, Descripcion, PrecioUnitario, CantidadLlevada, ImporteTotalLlevado) 
                                                VALUES (@CId, @ProdCod, @Desc, @Pre, @Cant, @Imp)";

                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            using (SqlCommand cmdDet = new SqlCommand(queryDetalle, conSql, transaccionSql))
                            {
                                cmdDet.Parameters.AddWithValue("@CId", idConsignacionInsertada);
                                cmdDet.Parameters.AddWithValue("@Cant", Convert.ToDouble(row.Cells[0].Value));
                                cmdDet.Parameters.AddWithValue("@Desc", row.Cells[1].Value.ToString());
                                cmdDet.Parameters.AddWithValue("@Pre", Convert.ToDecimal(row.Cells[2].Value));
                                cmdDet.Parameters.AddWithValue("@Imp", Convert.ToDecimal(row.Cells[3].Value));
                                cmdDet.Parameters.AddWithValue("@ProdCod", row.Cells[5].Value.ToString());
                                cmdDet.ExecuteNonQuery();
                            }
                        }
                        transaccionSql.Commit();
                    }
                    catch (Exception exSql)
                    {
                        transaccionSql.Rollback();
                        throw new Exception("Error al guardar en SQL: " + exSql.Message);
                    }
                }

                // 3. DESCONTAR INVENTARIO EN ACCESS
                conectarAccess.Open();
                OleDbTransaction transaccionAccess = conectarAccess.BeginTransaction();
                try
                {
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        string idProd = dataGridView1.Rows[i].Cells[5].Value.ToString();
                        double cantidad = Convert.ToDouble(dataGridView1.Rows[i].Cells[0].Value);
                        double existenciaActual = Convert.ToDouble(dataGridView1.Rows[i].Cells[4].Value);
                        double nuevaExistencia = existenciaActual - cantidad;

                        using (OleDbCommand cmdAcc = new OleDbCommand("UPDATE Inventario SET Existencia='" + nuevaExistencia + "' WHERE Id='" + idProd + "';", conectarAccess, transaccionAccess))
                        {
                            cmdAcc.ExecuteNonQuery();
                        }

                        string descKardex = "SALIDA A CONSIGNACION FOLIO: " + folioConsigna;
                        string queryKardex = "INSERT INTO Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha) VALUES('" + idProd + "', 'SALIDA', '" + descKardex + "', " + existenciaActual + ", '" + nuevaExistencia + "', '" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "');";
                        using (OleDbCommand cmdKar = new OleDbCommand(queryKardex, conectarAccess, transaccionAccess))
                        {
                            cmdKar.ExecuteNonQuery();
                        }
                    }
                    transaccionAccess.Commit();
                }
                catch (Exception exAcc)
                {
                    transaccionAccess.Rollback();
                    throw new Exception("Error al descontar inventario en Access: " + exAcc.Message);
                }
                finally
                {
                    conectarAccess.Close();
                }

                MessageBox.Show("Consignación generada exitosamente con Folio: " + folioConsigna, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Imprimir Ticket 
                // Aqui ira tu lógica de impresión adaptada a salida de consigna (sin desglosar cobro/cambio)

                ReiniciarForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Cierre del Formulario y Hotkeys
        private void frmNuevaConsignacion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F5)
            {
                btnGenerarConsigna_Click(sender, e);
            }
            if (e.KeyCode == Keys.Delete)
            {
                EliminarProductos();
            }
        }
    }
}