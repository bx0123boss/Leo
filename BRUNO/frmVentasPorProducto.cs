using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace BRUNO
{
    public partial class frmVentasPorProducto : frmBase
    {
        private DataSet ds;
        // Se asume que usas la cadena de conexión centralizada
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbDataAdapter da;
        public String usuario = "";

        public frmVentasPorProducto()
        {
            InitializeComponent();
            this.MinimumSize = new Size(1011, 629);
        }

        private void frmVentasPorProducto_Load(object sender, EventArgs e)
        {
            EstilizarDataGridView(this.dataGridView1);
            EstilizarComboBox(cmbCategoria); // Si tienes este método en tu frmBase
            EstilizarBotonPrimario(this.button1);

            CargarCategorias();
            CargarDatos();
        }

        private void CargarCategorias()
        {
            try
            {
                conectar.Open();
                // Obtenemos las categorías únicas del inventario para llenar el ComboBox
                string queryCat = "SELECT DISTINCT Categoria FROM Inventario WHERE Categoria IS NOT NULL AND Categoria <> ''";
                using (OleDbCommand cmd = new OleDbCommand(queryCat, conectar))
                {
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        cmbCategoria.Items.Clear();
                        cmbCategoria.Items.Add("TODAS"); // Opción para ver todas las categorías

                        while (reader.Read())
                        {
                            cmbCategoria.Items.Add(reader["Categoria"].ToString());
                        }
                    }
                }

                if (cmbCategoria.Items.Count > 0)
                {
                    cmbCategoria.SelectedIndex = 0; // Selecciona "TODAS" por defecto
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar categorías: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conectar.Close();
            }
        }

        private void CargarDatos()
        {
            // Evita consultas si el combobox aún no carga
            if (cmbCategoria.SelectedIndex == -1) return;

            try
            {
                DateTime fechaInicio = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day, 0, 0, 0);
                DateTime fechaFin = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day, 23, 59, 59);

                ds = new DataSet();

                // Hacemos el cruce (INNER JOIN) entre VentasContado (VC) y Ventas (V)
                // Se relaciona FolioVenta con Folio para traer el Estatus de la tabla Ventas
                string query = @"SELECT VC.FolioVenta, VC.IdProducto, VC.Cantidad, VC.Producto, VC.MontoTotal, VC.Fecha, V.Estatus, VC.Categoria 
                                 FROM VentasContado VC
                                 INNER JOIN Ventas V ON VC.FolioVenta = V.Folio
                                 WHERE VC.Fecha >= @fechaInicio AND VC.Fecha <= @fechaFin ";

                // Si no seleccionó "TODAS", filtramos por la categoría específica
                if (cmbCategoria.Text != "TODAS")
                {
                    query += " AND VC.Categoria = @categoria ";
                }

                query += " ORDER BY VC.Fecha DESC;";

                using (OleDbCommand cmd = new OleDbCommand(query, conectar))
                {
                    // En Access (OleDb), el ORDEN de los parámetros es vital.
                    cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@fechaFin", fechaFin);

                    if (cmbCategoria.Text != "TODAS")
                    {
                        cmd.Parameters.AddWithValue("@categoria", cmbCategoria.Text);
                    }

                    da = new OleDbDataAdapter(cmd);
                    da.Fill(ds, "VentasProd");
                }

                dataGridView1.DataSource = ds.Tables["VentasProd"];

                this.dataGridView1.ReadOnly = true;
                this.dataGridView1.AllowUserToAddRows = false;
                this.dataGridView1.AllowUserToDeleteRows = false;

                if (dataGridView1.Rows.Count > 0)
                {
                    dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];
                    dataGridView1.Rows[0].Selected = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void cmbCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
                {
                    frmVentaDetallada detalles = new frmVentaDetallada();

                    // Asumiendo que la columna 0 es FolioVenta
                    detalles.lblFolio.Text = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["FolioVenta"].Value.ToString();
                    detalles.lblFecha.Text = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Fecha"].Value.ToString();

                    // Verificamos el Estatus que trajimos con el INNER JOIN (columna "Estatus")
                    string estatus = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Estatus"].Value.ToString();

                    if (estatus == "CANCELADO")
                    {
                        detalles.button1.Visible = false;
                        detalles.button2.Visible = false;
                    }
                    else
                    {
                        detalles.button1.Visible = true;
                        detalles.button2.Visible = true;
                    }

                    detalles.usuario = usuario;
                    detalles.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Por favor, seleccione un registro de la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir los detalles: " + ex.Message);
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Formatear la columna de MontoTotal a formato de moneda
            if (dataGridView1.Columns[e.ColumnIndex].Name == "MontoTotal")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("C2"); // Formato moneda con 2 decimales
                    e.FormattingApplied = true;
                }
            }

            // Opcional: Pintar de rojo la fila si está cancelada
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Estatus")
            {
                if (e.Value != null && e.Value.ToString() == "CANCELADO")
                {
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Red;
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Verificamos que haya una fila seleccionada
                if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
                {
                    frmVentaDetallada detalles = new frmVentaDetallada();

                    // Obtenemos los valores de la fila actual basándonos en el nombre de la columna devuelta por el SELECT
                    detalles.lblFolio.Text = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["FolioVenta"].Value.ToString();
                    detalles.lblFecha.Text = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Fecha"].Value.ToString();

                    // Verificamos el Estatus de la Venta
                    string estatus = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Estatus"].Value.ToString();

                    if (estatus == "CANCELADO")
                    {
                        // Si la venta está cancelada, ocultamos los botones (Cancelar e Imprimir ticket, etc.)
                        detalles.button1.Visible = false;
                        detalles.button2.Visible = false;
                    }
                    else
                    {
                        detalles.button1.Visible = true;
                        detalles.button2.Visible = true;
                    }

                    detalles.usuario = usuario;
                    detalles.Show();

                    // Cierra el reporte actual. 
                    // Si prefieres que no se cierre, puedes cambiar this.Close() por detalles.ShowDialog() y quitar this.Close()
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Por favor, seleccione un registro de la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir los detalles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbCategoria_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            // Si aún no hay nada seleccionado, no hace nada
            if (cmbCategoria.SelectedIndex == -1) return;

            try
            {
                // Tomamos el inicio y fin del día seleccionado en el calendario
                DateTime fechaInicio = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day, 0, 0, 0);
                DateTime fechaFin = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day, 23, 59, 59);

                ds = new DataSet();

                // Hacemos el cruce (INNER JOIN) para traer productos y el estatus de la venta
                string query = @"SELECT VC.FolioVenta, VC.IdProducto, VC.Cantidad, VC.Producto, VC.MontoTotal, VC.Fecha, V.Estatus, VC.Categoria 
                         FROM VentasContado VC
                         INNER JOIN Ventas V ON VC.FolioVenta = V.Folio
                         WHERE VC.Fecha >= @fechaInicio AND VC.Fecha <= @fechaFin ";

                // Si el usuario no eligió "TODAS", agregamos el filtro de categoría
                if (cmbCategoria.Text != "TODAS")
                {
                    query += " AND VC.Categoria = @categoria ";
                }

                query += " ORDER BY VC.Fecha DESC;";

                using (OleDbCommand cmd = new OleDbCommand(query, conectar))
                {
                    // Agregamos los parámetros en orden estricto
                    cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@fechaFin", fechaFin);

                    if (cmbCategoria.Text != "TODAS")
                    {
                        cmd.Parameters.AddWithValue("@categoria", cmbCategoria.Text);
                    }

                    da = new OleDbDataAdapter(cmd);
                    da.Fill(ds, "VentasProd");
                }

                // Refrescamos el DataGridView
                dataGridView1.DataSource = ds.Tables["VentasProd"];
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al filtrar por categoría: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dateTimePicker1_ValueChanged_1(object sender, EventArgs e)
        {
            if (cmbCategoria.SelectedIndex == -1) return;

            try
            {
                // Tomamos el inicio y fin del día seleccionado en el calendario
                DateTime fechaInicio = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day, 0, 0, 0);
                DateTime fechaFin = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day, 23, 59, 59);

                ds = new DataSet();

                // Hacemos el cruce (INNER JOIN) para traer productos y el estatus de la venta
                string query = @"SELECT VC.FolioVenta, VC.IdProducto, VC.Cantidad, VC.Producto, VC.MontoTotal, VC.Fecha, V.Estatus, VC.Categoria 
                         FROM VentasContado VC
                         INNER JOIN Ventas V ON VC.FolioVenta = V.Folio
                         WHERE VC.Fecha >= @fechaInicio AND VC.Fecha <= @fechaFin ";

                // Si el usuario no eligió "TODAS", agregamos el filtro de categoría
                if (cmbCategoria.Text != "TODAS")
                {
                    query += " AND VC.Categoria = @categoria ";
                }

                query += " ORDER BY VC.Fecha DESC;";

                using (OleDbCommand cmd = new OleDbCommand(query, conectar))
                {
                    // Agregamos los parámetros en orden estricto
                    cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@fechaFin", fechaFin);

                    if (cmbCategoria.Text != "TODAS")
                    {
                        cmd.Parameters.AddWithValue("@categoria", cmbCategoria.Text);
                    }

                    da = new OleDbDataAdapter(cmd);
                    da.Fill(ds, "VentasProd");
                }

                // Refrescamos el DataGridView
                dataGridView1.DataSource = ds.Tables["VentasProd"];
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al filtrar por categoría: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    }