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
using System.Windows.Forms.DataVisualization.Charting;

namespace BRUNO
{
    public partial class frmCortePorFechas : Form
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Restaurante.accdb");
        OleDbDataAdapter da;
        double total = 0;
        public frmCortePorFechas()
        {
            InitializeComponent();
            conectar.Open();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ds = new DataSet();
            da = new OleDbDataAdapter("Select * from histocortes where Fecha >=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 00:00:00# and Fecha <=#" + dateTimePicker2.Value.Month.ToString() + "/" + dateTimePicker2.Value.Day.ToString() + "/" + dateTimePicker2.Value.Year.ToString() + " 23:59:59#;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[3].Visible = false;
            dataGridView1.Columns[4].Visible = false;
            dataGridView1.Columns[5].Visible = false;
            dataGridView1.Columns[2].HeaderText = "Fecha de Corte";
            total = 0;

            Series serie;
            chart1.Series.Clear();
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                double descontado = Math.Round((Convert.ToDouble(dataGridView1[1, i].Value.ToString().Substring(1))));

                total += Convert.ToDouble(dataGridView1[1, i].Value.ToString().Substring(1));
                serie = chart1.Series.Add(dataGridView1[2, i].Value.ToString());
                serie.Label = "" + descontado;
                serie.Points.Add(descontado);
            }

            Series serie2;
            chart2.Series.Clear();
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                double descontado = Math.Round((Convert.ToDouble(dataGridView1[6, i].Value.ToString())));

                total += Convert.ToDouble(dataGridView1[1, i].Value.ToString().Substring(1));
                serie2 = chart2.Series.Add(dataGridView1[2, i].Value.ToString());
                serie2.Label = "" + descontado;
                serie2.Points.Add(descontado);
            }

            Series serie3;
            chart3.Series.Clear();
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                double descontado = Math.Round((Convert.ToDouble(dataGridView1[7, i].Value.ToString())));

                total += Convert.ToDouble(dataGridView1[1, i].Value.ToString().Substring(1));
                serie3 = chart3.Series.Add(dataGridView1[2, i].Value.ToString());
                serie3.Label = "" + descontado;
                serie3.Points.Add(descontado);
            }
            
            lblTotal.Text =  Math.Round(total*0.4611) + "";
        }

        private void frmCortePorFechas_Load(object sender, EventArgs e)
        {

        }
    }
}
