using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JaegerSoft
{
    public partial class frmVentasVendedorFechas : frmBase
    {
        private DataSet ds;
        SqlConnection conectar = new SqlConnection(Conexion.CadSQL);
        //SqlConnection conectar = new SqlConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        SqlDataAdapter da;

        public frmVentasVendedorFechas()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            ds = new DataSet();
            da = new SqlDataAdapter("SELECT distinct  usuario AS Vendedor, '0' as Total FROM VentasCajero where Fecha >='" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + " 00:00:00' and Fecha <='" + dateTimePicker2.Value.ToString("yyyy-MM-dd") + " 23:59:59' ;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];

            ds = new DataSet();
            da = new SqlDataAdapter("SELECT usuario, total FROM VentasCajero where Fecha >='" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + " 00:00:00' and Fecha <='" + dateTimePicker2.Value.ToString("yyyy-MM-dd") + " 23:59:59' ;", conectar);
            da.Fill(ds, "Id");
            dataGridView3.DataSource = ds.Tables["Id"];
            //data1 es vendedor
            for (int i = 0; i < dataGridView3.RowCount; i++)
            {
                //data 3 es totales

                for (int x = 0; x < dataGridView1.RowCount; x++)
                {
                    if (dataGridView1[0, x].Value.ToString() == dataGridView3[0, i].Value.ToString())
                    {
                        dataGridView1[1, x].Value = (Convert.ToDouble(dataGridView1[1, x].Value.ToString()) + Convert.ToDouble(dataGridView3[1, i].Value.ToString()));
                    }
                }

            }
            dataGridView1.Columns[1].HeaderText = "Total ($)";
                   
        }

        private void frmVentasVendedorFechas_Load(object sender, EventArgs e)
        {
            EstilizarBotonPeligro(button1);
            EstilizarDataGridView(dataGridView1);
        }

        private void label1_Click(object sender, EventArgs e)
        {

            for (int x = 0; x < dataGridView1.RowCount; x++)
            {
                dataGridView1[1, x].Value = String.Format(CultureInfo.InvariantCulture, "{0:0,0.00}", dataGridView1[1, x].Value.ToString());
            }
        }
    }
}
