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
    public partial class frmGastos : Form
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbDataAdapter da;
        OleDbCommand cmd;
        public String usuario = "";

        public frmGastos()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmAgregarGasto gas = new frmAgregarGasto();
            gas.Show();
            this.Close();
        }

        private void frmGastos_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from GastosGeneral;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            DialogResult dialogResult = MessageBox.Show("¿Estas seguro de cancelar la venta?", "Alto!", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {                
                 cmd = new OleDbCommand("delete from GastosGeneral where Id=" + dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString()+ ";", conectar);
                 cmd.ExecuteNonQuery();  
                 cmd = new OleDbCommand("delete from GastosDetallados where idGasto='" + dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString()+ "';", conectar);
                 cmd.ExecuteNonQuery();  
                MessageBox.Show("Listo");
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from GastosGeneral;", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
            }
        }
    }
}
