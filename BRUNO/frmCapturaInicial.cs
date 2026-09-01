using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace JaegerSoft
{
    public partial class frmCapturaInicial : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbConnection conn;
        OleDbDataAdapter MyDataAdapter;
        DataTable dt;

        public frmCapturaInicial()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            importarExcel(dataGridView1, "Hoja1");
        }

        public void importarExcel(DataGridView dgv, string nombreHoja)
        {
            string ruta = "";
            OpenFileDialog openfile1 = new OpenFileDialog();
            openfile1.Filter = "Excel Files |*.xlsx";
            openfile1.Title = "Seleccione el archivo de Excel";

            if (openfile1.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(openfile1.FileName))
                {
                    ruta = openfile1.FileName;
                }
            }

            if (string.IsNullOrEmpty(ruta)) return;

            conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;data source=" + ruta + ";Extended Properties='Excel 12.0 Xml;HDR=Yes;IMEX=1'");

            // Leemos PrecioCompra desde el Excel y lo proyectamos internamente como Especial
            string queryExcel = "Select Id, Nombre, PrecioVentaMayoreo, PrecioVenta, Existencia, Limite, Categoria, PrecioCompra AS Especial, IVA, Unidad, ManejaInventario from [" + nombreHoja + "$]";

            MyDataAdapter = new OleDbDataAdapter(queryExcel, conn);
            dt = new DataTable();
            MyDataAdapter.Fill(dt);

            // 🔴 PASO 1: FILTRAR Y ELIMINAR FILAS VACÍAS (Traídas por el DataValidation de Excel)
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dr = dt.Rows[i];
                if (dr["Id"] == DBNull.Value || string.IsNullOrWhiteSpace(dr["Id"].ToString()))
                {
                    dr.Delete();
                }
            }
            dt.AcceptChanges();

            // 🔴 PASO 2: CONVERTIR COLUMNAS A BOOLEANO EN EL DATATABLE PARA QUE EL DATAGRIDVIEW
            // CREÉ LOS CHECKBOXES AUTOMÁTICAMENTE Y BIEN VINCULADOS
            DataTable dtClon = dt.Clone();
            dtClon.Columns["IVA"].DataType = typeof(bool);
            dtClon.Columns["ManejaInventario"].DataType = typeof(bool);

            foreach (DataRow row in dt.Rows)
            {
                DataRow newRow = dtClon.NewRow();
                newRow.ItemArray = row.ItemArray;

                // Convertir strings "VERDADERO"/"TRUE"/"1" de Excel a bool real
                newRow["IVA"] = ParseBool(row["IVA"]);
                newRow["ManejaInventario"] = ParseBool(row["ManejaInventario"]);

                dtClon.Rows.Add(newRow);
            }

            dgv.DataSource = dtClon;

            // Configurar columnas editables
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                column.ReadOnly = !(column.Name == "Existencia" || column.Name == "Limite" || column.Name == "IVA" || column.Name == "ManejaInventario");
            }

            // Ajustes visuales de cabecera
            if (dgv.Columns.Contains("ManejaInventario"))
                dgv.Columns["ManejaInventario"].HeaderText = "Maneja Inv.";
            if (dgv.Columns.Contains("Especial"))
                dgv.Columns["Especial"].HeaderText = "Precio Compra";
        }

        // Función auxiliar para parsear valores booleanos de Excel
        private bool ParseBool(object value)
        {
            if (value == null || value == DBNull.Value) return false;
            string valStr = value.ToString().Trim().ToUpper();
            return valStr == "TRUE" || valStr == "VERDADERO" || valStr == "1" || valStr == "SI" || valStr == "SÍ";
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0 || dataGridView1.DataSource == null)
            {
                MessageBox.Show("No hay datos cargados para guardar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                conectar.Open();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    // Evitar la última fila vacía si el DataGridView permite añadir filas
                    if (row.IsNewRow || row.Cells["Id"].Value == null) continue;

                    // Lectura de variables
                    string id = row.Cells["Id"].Value?.ToString() ?? "";
                    string nombre = row.Cells["Nombre"].Value?.ToString() ?? "";
                    decimal precioMayoreo = Convert.ToDecimal(row.Cells["PrecioVentaMayoreo"].Value ?? 0);
                    decimal precioVenta = Convert.ToDecimal(row.Cells["PrecioVenta"].Value ?? 0);
                    int existencia = Convert.ToInt32(row.Cells["Existencia"].Value ?? 0);
                    int limite = Convert.ToInt32(row.Cells["Limite"].Value ?? 0);
                    string categoria = row.Cells["Categoria"].Value?.ToString() ?? "";

                    // Manejado internamente como 'Especial' (Precio de Compra)
                    decimal especial = Convert.ToDecimal(row.Cells["Especial"].Value ?? 0);

                    bool tieneIVA = Convert.ToBoolean(row.Cells["IVA"].Value ?? false);
                    string unidad = row.Cells["Unidad"].Value?.ToString() ?? "";
                    string strIVA = tieneIVA ? "IVA(16)" : "NO INCLUYE IVA";
                    bool manejaInventario = Convert.ToBoolean(row.Cells["ManejaInventario"].Value ?? false);
                    string fechaActual = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                    // -------------------------------------------------------------
                    // 1. INSERCIÓN EN LA TABLA INVENTARIO
                    // -------------------------------------------------------------
                    string sqlInventario = @"INSERT INTO Inventario 
                        (Id, Nombre, PrecioVentaMayoreo, PrecioVenta, Existencia, Limite, Categoria, Especial, IVA, Unidad, ManejaInventario) 
                        VALUES (@Id, @Nombre, @PrecioMayoreo, @PrecioVenta, @Existencia, @Limite, @Categoria, @Especial, @IVA, @Unidad, @ManejaInv)";

                    using (OleDbCommand cmdInventario = new OleDbCommand(sqlInventario, conectar))
                    {
                        cmdInventario.Parameters.AddWithValue("@Id", id);
                        cmdInventario.Parameters.AddWithValue("@Nombre", nombre);
                        cmdInventario.Parameters.AddWithValue("@PrecioMayoreo", precioMayoreo);
                        cmdInventario.Parameters.AddWithValue("@PrecioVenta", precioVenta);
                        cmdInventario.Parameters.AddWithValue("@Existencia", existencia);
                        cmdInventario.Parameters.AddWithValue("@Limite", limite);
                        cmdInventario.Parameters.AddWithValue("@Categoria", categoria);
                        cmdInventario.Parameters.AddWithValue("@Especial", especial);
                        cmdInventario.Parameters.AddWithValue("@IVA", strIVA);
                        cmdInventario.Parameters.AddWithValue("@Unidad", unidad);
                        cmdInventario.Parameters.AddWithValue("@ManejaInv", manejaInventario);

                        cmdInventario.ExecuteNonQuery();
                    }

                    // -------------------------------------------------------------
                    // 2. INSERCIÓN EN LA TABLA KARDEX (Registro de Creación)
                    // -------------------------------------------------------------
                    string sqlKardex = @"INSERT INTO Kardex 
                        (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha, Precio) 
                        VALUES (@IdProducto, @Tipo, @Descripcion, @ExistenciaAntes, @ExistenciaDespues, @Fecha, @Precio)";

                    using (OleDbCommand cmdKardex = new OleDbCommand(sqlKardex, conectar))
                    {
                        cmdKardex.Parameters.AddWithValue("@IdProducto", id.ToString());
                        cmdKardex.Parameters.AddWithValue("@Tipo", "CREACION");
                        cmdKardex.Parameters.AddWithValue("@Descripcion", "CREACIÓN DE PRODUCTO EN INVENTARIO INICIAL");
                        cmdKardex.Parameters.AddWithValue("@ExistenciaAntes", 0);
                        cmdKardex.Parameters.AddWithValue("@ExistenciaDespues", existencia);
                        cmdKardex.Parameters.AddWithValue("@Fecha", fechaActual);
                        cmdKardex.Parameters.AddWithValue("@Precio", precioVenta.ToString("0.00"));

                        cmdKardex.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("El inventario inicial y su historial en Kardex se han capturado con éxito.", "¡Éxito!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al procesar el inventario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conectar.State == ConnectionState.Open)
                {
                    conectar.Close();
                }
            }
        }
    }
}