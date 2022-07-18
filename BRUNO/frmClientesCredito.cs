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
using Microsoft.Office.Interop.Excel;
using System.Globalization;

namespace BRUNO
{
    public partial class frmClientesCredito : Form
    {
        private DataSet ds;
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbDataAdapter da;
        public String usuario = "";

        public frmClientesCredito()
        {
            InitializeComponent();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            frmHistorialVentas histo = new frmHistorialVentas();
            histo.lblID.Text = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
            histo.lblNombre.Text = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
            histo.adeudo = Convert.ToDouble(dataGridView1[7, dataGridView1.CurrentRow.Index].Value.ToString());
            histo.Show();
            this.Close();
        }

        private void frmClientesCredito_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from Clientes where Adeudo>0 order by Nombre;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;

            double total = 0;
            if (usuario == "Invitado")
            {
                lblAdeudo.Hide();
                label2.Hide();
                button3.Hide();
                dataGridView1.Columns[7].Visible = false;
                dataGridView1.Columns[8].Visible = false;
            }
            else
            {
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    total += Convert.ToDouble(dataGridView1[7, i].Value.ToString());
                }
                lblAdeudo.Text = "$" + total.ToString("#,#.00", CultureInfo.InvariantCulture);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            frmAbonoCliente abonar = new frmAbonoCliente();
            abonar.txtAdeudo.Text = dataGridView1[7, dataGridView1.CurrentRow.Index].Value.ToString();
            abonar.lblID.Text = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
            abonar.lblCliente.Text = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
            abonar.Show();
            this.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {

            frmHistorialAbonos histo = new frmHistorialAbonos();
            histo.lblID.Text = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
            histo.lblNombre.Text = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
            histo.Show();
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {

            Microsoft.Office.Interop.Excel.Application xla = new Microsoft.Office.Interop.Excel.Application();
            Workbook wb = xla.Workbooks.Add(XlSheetType.xlWorksheet);
            Worksheet ws = (Worksheet)xla.ActiveSheet;

            xla.Visible = true;

            ws.Cells[1, 1] = "Nombre";
            ws.Cells[1, 2] = "Dirección";
            ws.Cells[1, 3] = "Telefono";
            ws.Cells[1, 4] = "Adeudo";
            ws.Cells[1, 5] = "Ultimo Abono";
            ws.Cells[1, 6] = "FECHA: " + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            int linea = dataGridView1.RowCount;
            int cont = 0;
            int hoja = 0;
            do
            {
                if (Convert.ToDouble(dataGridView1[7, cont].Value.ToString()) > 0)
                {
                    ws.Cells[(hoja + 2), 1] = dataGridView1[1, cont].Value.ToString();
                    ws.Cells[(hoja + 2), 2] = dataGridView1[3, cont].Value.ToString();
                    ws.Cells[(hoja + 2), 3] = dataGridView1[2, cont].Value.ToString();
                    ws.Cells[(hoja + 2), 4] = "$" + dataGridView1[7, cont].Value.ToString();
                    ws.Cells[(hoja + 2), 5] = dataGridView1[9, cont].Value.ToString();
                    hoja++;
                }

                cont++;
            } while (linea > cont);
               
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Clientes where Adeudo>0 ORDER BY Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
            }
            else
            {

                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Clientes where Nombre LIKE '%" + textBox1.Text + "%' AND Adeudo>0 ORDER BY Nombre ;", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
            }
        }
    }
}
