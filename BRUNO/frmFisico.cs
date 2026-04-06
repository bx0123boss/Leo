using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmFisico : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        OleDbConnection conn;
        OleDbDataAdapter MyDataAdapter;
        DataTable dt;
        OleDbCommand cmd;
        public frmFisico()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            importarExcel(dataGridView1, "Hoja1");
        }
        public void importarExcel(DataGridView dgv, String nombreHoja)
        {
            String ruta = "";
            using (OpenFileDialog openfile1 = new OpenFileDialog())
            {
                openfile1.Filter = "Excel Files |*.xlsx;*.xls";
                openfile1.Title = "Seleccione el archivo de Excel";
                if (openfile1.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(openfile1.FileName))
                {
                    return;
                }
                ruta = openfile1.FileName;
            }
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                string stringConexion = "Provider=Microsoft.ACE.OLEDB.12.0;data source=" + ruta + ";Extended Properties='Excel 12.0 Xml;HDR=Yes'";

                using (conn = new OleDbConnection(stringConexion))
                {
                    conn.Open();
                    DataTable dtEsquema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    string hojaReal = "";

                    if (dtEsquema != null && dtEsquema.Rows.Count > 0)
                    {
                        hojaReal = dtEsquema.Rows[0]["TABLE_NAME"].ToString();
                    }
                    else
                    {
                        hojaReal = nombreHoja + "$";
                    }

                    // 3. ARMAMOS LA CONSULTA CON EL NOMBRE REAL Y LA FÓRMULA CORREGIDA
                    string query = $"Select ID, Nombre, Existen, Fisico, Fisico - Existen as Diferencia from [{hojaReal}]";

                    using (MyDataAdapter = new OleDbDataAdapter(query, conn))
                    {
                        dt = new DataTable();
                        MyDataAdapter.Fill(dt);
                        dgv.DataSource = dt;
                    }
                }
            }
            catch (OleDbException ex)
            {
                // Este catch atrapa errores específicos de Base de Datos o Excel
                MessageBox.Show("No se pudo importar el Excel. Verifica lo siguiente:\n\n" +
                                "1. El archivo NO debe estar abierto en Excel actualmente.\n" +
                                "2. El archivo debe contener las columnas exactas: ID, Nombre, Existen, Fisico.\n\n" +
                                "Detalle técnico: " + ex.Message,
                                "Error de Formato", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                // Este catch atrapa cualquier otro error inesperado (falta de memoria, drivers no instalados, etc)
                MessageBox.Show("Ocurrió un error inesperado al leer el archivo:\n" + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // El bloque finally SIEMPRE se ejecuta, haya error o no.
                // Lo usamos para regresar el cursor a la normalidad.
                Cursor.Current = Cursors.Default;

                // Nos aseguramos de cerrar la conexión si se quedó abierta por un error
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount == 0) return;

            // Usamos el cursor de espera porque esto puede tardar
            this.Cursor = Cursors.WaitCursor;
            OleDbTransaction transaccion = null;

            try
            {
                if (conectar.State == ConnectionState.Closed) conectar.Open();

                // Iniciamos la transacción
                transaccion = conectar.BeginTransaction();

                // 1. Insertar el encabezado
                string queryFisico = "INSERT INTO Fisico(Fecha) VALUES(@fecha)";
                using (OleDbCommand cmdEncabezado = new OleDbCommand(queryFisico, conectar, transaccion))
                {
                    cmdEncabezado.Parameters.AddWithValue("@fecha", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                    cmdEncabezado.ExecuteNonQuery();
                }

                // 2. Obtener el ID recién creado
                string idFisico = "";
                using (OleDbCommand cmdId = new OleDbCommand("SELECT @@Identity;", conectar, transaccion))
                {
                    idFisico = cmdId.ExecuteScalar().ToString();
                }

                // 3. Preparamos los comandos del ciclo (es mucho más rápido prepararlos una vez)
                string queryDetalle = "INSERT INTO FisicoDetallado(Id, Nombre, Existen, Fisico, Diferencia, IdFisico) VALUES(@id, @nombre, @existen, @fisico, @diferencia, @idFisico)";
                string queryUpdate = "UPDATE Inventario SET Existencia=@fisico WHERE Id=@id";

                using (OleDbCommand cmdDetalle = new OleDbCommand(queryDetalle, conectar, transaccion))
                using (OleDbCommand cmdUpd = new OleDbCommand(queryUpdate, conectar, transaccion))
                {
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        // Limpiamos parámetros en cada iteración
                        cmdDetalle.Parameters.Clear();
                        cmdUpd.Parameters.Clear();

                        string idProd = dataGridView1[0, i].Value?.ToString() ?? "";
                        string nombreProd = dataGridView1[1, i].Value?.ToString() ?? "";
                        string existen = dataGridView1[2, i].Value?.ToString() ?? "0";
                        string fisico = dataGridView1[3, i].Value?.ToString() ?? "0";
                        string diferencia = dataGridView1[4, i].Value?.ToString() ?? "0";

                        cmdDetalle.Parameters.AddWithValue("@id", idProd);
                        cmdDetalle.Parameters.AddWithValue("@nombre", nombreProd);
                        cmdDetalle.Parameters.AddWithValue("@existen", existen);
                        cmdDetalle.Parameters.AddWithValue("@fisico", fisico);
                        cmdDetalle.Parameters.AddWithValue("@diferencia", diferencia);
                        cmdDetalle.Parameters.AddWithValue("@idFisico", idFisico);
                        cmdDetalle.ExecuteNonQuery();
                        cmdUpd.Parameters.AddWithValue("@fisico", fisico);
                        cmdUpd.Parameters.AddWithValue("@id", idProd);
                        cmdUpd.ExecuteNonQuery();
                    }
                }
                transaccion.Commit();
                MessageBox.Show("El inventario físico se ha capturado con éxito", "Éxito!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                transaccion?.Rollback();
                MessageBox.Show("Error al guardar el inventario: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                if (conectar.State == ConnectionState.Open) conectar.Close();
            }
        }
    }
}
