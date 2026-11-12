using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace JaegerSoft
{
    public partial class frmAbonosFolio : frmBase
    {   
        private DataSet ds;
        SqlConnection conectar = new SqlConnection(Conexion.CadSQL);
        SqlDataAdapter da;

        public frmAbonosFolio()
        {
            InitializeComponent();
        }

        private void frmAbonosFolio_Load(object sender, EventArgs e)
        {
            conectar.Open();
            ds = new DataSet();
            da = new SqlDataAdapter("select Id,Abono,idCliente,Fecha,Folio from Abonos where Folio='" + lblFolio.Text + "';", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[2].Visible = false;
            EstilizarDataGridView(dataGridView1);
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
        }
    }
}
