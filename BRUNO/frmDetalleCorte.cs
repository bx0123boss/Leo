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
using LibPrintTicket;

namespace BRUNO
{
    public partial class frmDetalleCorte : Form
    {
        public int ID;
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbDataAdapter da;
        public string mas, menos, tarjeta;
        public frmDetalleCorte()
        {
            InitializeComponent();
        }

        private void frmDetalleCorte_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from Cortes where idCorte='" + ID + "';", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
            //dataGridView1.Columns[1].Visible = false;
            dataGridView1.Columns[3].Visible = false;
            dataGridView1.Columns[4].Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application xla = new Microsoft.Office.Interop.Excel.Application();
            Workbook wb = xla.Workbooks.Add(XlSheetType.xlWorksheet);
            Worksheet ws = (Worksheet)xla.ActiveSheet;

            xla.Visible = true;

            ws.Cells[1, 1] = "Concepto";
            ws.Cells[1, 2] = "Monto Total";
            ws.Cells[1, 3] = "Tipo Pago";
            ws.Cells[1, 4] = "Fecha de Corte: " + lblFecha.Text;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                ws.Cells[i + 2, 1] = dataGridView1[1, i].Value.ToString();
                ws.Cells[i + 2, 2] = dataGridView1[2, i].Value.ToString();
                ws.Cells[i + 2, 3] = dataGridView1[5, i].Value.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Ticket ticket = new Ticket();
            ticket.MaxChar = 35;
            ticket.MaxCharDescription = 22;
            ticket.FontSize = 8;
            ticket.HeaderImage = Image.FromFile("C:\\Jaeger Soft\\logo.jpg");
            ticket.AddHeaderLine("********  CORTE DE CAJA  *******");
            ticket.AddSubHeaderLine("FECHA Y HORA:");
            ticket.AddSubHeaderLine(lblFecha.Text);
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                ticket.AddItem("1", dataGridView1[1, i].Value.ToString(), "   $" + dataGridView1[2, i].Value.ToString());
            }
            string entrada= "$" + (Convert.ToDouble(mas) + Convert.ToDouble(tarjeta));
            string salida= "$" + (Convert.ToDouble(menos) * -1);
            string corte = "$" + mas;
            string credito = "$" + tarjeta;
            string total = "$" + (Convert.ToDouble(tarjeta) + Convert.ToDouble(mas) + Convert.ToDouble(menos));
            ticket.AddTotal("Efectivo", corte);
            ticket.AddTotal("Tarjetas", credito);
            ticket.AddTotal("Entradas", entrada);
            ticket.AddTotal("Salidas", salida);
            ticket.AddTotal("Total", total);
            //ticket.PrintTicket(Conexion.impresora);
        }
    }
}
