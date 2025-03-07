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
    public partial class frmDatos : Form
    {
        private DataSet ds;
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Restaurante.accdb");
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbDataAdapter da;
        OleDbCommand cmd;
        public frmDatos()
        {
            InitializeComponent();
        }

        private void BtnApartados_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmDatos_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("SELECT SUM(Existencia) AS Cantidad, SUM(Existencia * Especial) AS PrecioCompra,  SUM(Existencia * PrecioVenta) AS PrecioVenta,  SUM(Existencia * (PrecioVenta - Especial)) AS Utilidad, Categoria FROM Inventario  GROUP BY Categoria ORDER BY Categoria ;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "PrecioCompra" ||
               dataGridView1.Columns[e.ColumnIndex].Name == "PrecioVenta" ||
               dataGridView1.Columns[e.ColumnIndex].Name == "Utilidad")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("C2"); // Formato moneda con 2 decimales
                    e.FormattingApplied = true;
                }
            }
        }
    }
}
