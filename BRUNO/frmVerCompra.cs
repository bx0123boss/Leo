using Microsoft.Office.Interop.Excel;
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
    public partial class frmVerCompra : Form
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbDataAdapter da;
        public string id;
        public string usuario = "";

        public frmVerCompra()
        {
            InitializeComponent();
        }

        private void frmVerCompra_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from ComprasMovimientos where Folio='" + id + "';", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.Columns[8].Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application xla = new Microsoft.Office.Interop.Excel.Application();
            Workbook wb = xla.Workbooks.Add(XlSheetType.xlWorksheet);
            Worksheet ws = (Worksheet)xla.ActiveSheet;

            xla.Visible = true;

            ws.Cells[1, 1] = "Movimiento";
            ws.Cells[1, 2] = lblMovimiento.Text;
            ws.Cells[1, 3] = "Folio: " + id;
            ws.Cells[1, 3] = "Fecha: " + lblFecha.Text;
            ws.Cells[2, 1] = "Documento relacionado";
            ws.Cells[2, 2] = lblDocRel.Text;
            ws.Cells[2, 3] = "Usuario: " + lblUser.Text;
            ws.Cells[3, 1] = "Id Producto";
            ws.Cells[3, 2] = "Nombre";
            ws.Cells[3, 3] = "Existencias Antiguas";
            ws.Cells[3, 4] = "Movimiento";
            ws.Cells[3, 5] = "Existencias Actuales";
            ws.Cells[3, 6] = "Tipo";
            int cont = 3;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                cont++;
                ws.Cells[cont, 1] = dataGridView1[2, i].Value.ToString();
                ws.Cells[cont, 2] = dataGridView1[3, i].Value.ToString();
                ws.Cells[cont, 3] = dataGridView1[4, i].Value.ToString();
                ws.Cells[cont, 4] = dataGridView1[5, i].Value.ToString();
                ws.Cells[cont, 5] = dataGridView1[6, i].Value.ToString();
                ws.Cells[cont, 6] = dataGridView1[7, i].Value.ToString();
            }
        }
    }
}
