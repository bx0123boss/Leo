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
    public partial class frmCapturaInicial : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbConnection conn;
        OleDbDataAdapter MyDataAdapter;
        DataTable dt;
        OleDbCommand cmd;
        public frmCapturaInicial()
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
            OpenFileDialog openfile1 = new OpenFileDialog();
            openfile1.Filter = "Excel Files |*.xlsx";
            openfile1.Title = "Seleccione el archivo de Excel";
            if (openfile1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (openfile1.FileName.Equals("") == false)
                {
                    ruta = openfile1.FileName;
                }
            }

            conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;data source=" + ruta + ";Extended Properties='Excel 12.0 Xml;HDR=Yes'");
            MyDataAdapter = new OleDbDataAdapter("Select Id, Nombre, PrecioVentaMayoreo, PrecioVenta, Existencia, Limite, Categoria, Especial, IVA, Unidad from [" + nombreHoja + "$]", conn);
            dt = new DataTable();
            MyDataAdapter.Fill(dt);

            dgv.DataSource = dt;

            // Paso 1: Configurar columnas editables
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                column.ReadOnly = !(column.Name == "Existencia" || column.Name == "Limite" || column.Name == "IVA");
            }

            // Paso 2: Convertir columna IVA a CheckBox (fuera del foreach)
            if (dgv.Columns.Contains("IVA"))
            {
                int columnIndex = dgv.Columns["IVA"].Index;
                var checkBoxColumn = new DataGridViewCheckBoxColumn
                {
                    Name = "IVA",
                    HeaderText = "IVA",
                    DataPropertyName = "IVA", // Vincula con el campo del DataSource
                    Width = 50
                };

                dgv.Columns.RemoveAt(columnIndex);
                dgv.Columns.Insert(columnIndex, checkBoxColumn);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // Obtener valores de cada columna por nombre
                string id = row.Cells["Id"].Value?.ToString();
                string nombre = row.Cells["Nombre"].Value?.ToString();
                decimal precioMayoreo = Convert.ToDecimal(row.Cells["PrecioVentaMayoreo"].Value);
                decimal precioVenta = Convert.ToDecimal(row.Cells["PrecioVenta"].Value);
                int existencia = Convert.ToInt32(row.Cells["Existencia"].Value);
                int limite = Convert.ToInt32(row.Cells["Limite"].Value);
                string categoria = row.Cells["Categoria"].Value?.ToString();
                bool especial = Convert.ToBoolean(row.Cells["Especial"].Value);
                bool IVA = Convert.ToBoolean(row.Cells["IVA"].Value);
                string unidad = row.Cells["Unidad"].Value?.ToString();
                string iva = IVA ? "IVA(16)" : "NO INCLUYE IVA";
                conectar.Open();
                cmd = new OleDbCommand("insert into Inventario values('" + id + "','" + nombre + "','" + precioMayoreo + "','" + precioVenta + "'," + existencia + "," + limite + ",'" + categoria + "'," + especial + ",'"+ iva+"','0','"+ unidad+"');", conectar);
                cmd.ExecuteNonQuery();
                conectar.Close();
            }
            MessageBox.Show("El inventario inicial se ha capturado con exito", "Exito!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
    }
}
