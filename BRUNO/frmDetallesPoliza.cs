using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmDetallesPoliza : Form
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbDataAdapter da;
        OleDbCommand cmd;
        double existenciasTotales = 0;
        public string Id;
        public int que;
        public string usuario = "";
        public frmDetallesPoliza()
        {
            InitializeComponent();
        }

        private void frmDetallesPoliza_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            if (que == 2)
            {
                da = new OleDbDataAdapter("select * from productosPoliza2 where IdPoliza='" + Id + "';", conectar);
            }
            else
                da = new OleDbDataAdapter("select * from productosPoliza where FolioPoliza='" + lblFolio.Text + "';", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[7].Visible = false;
            if (usuario == "Invitado")
            {
                button1.Hide();
            }
            if (lblExtra.Text==".00")
            {
                lblExtra.Text = "0.00";
            }
            string[] fecha = lblFechaPoli.Text.Split(' ');
            lblFechaPoli.Text = fecha[0];
            //string extra = lblExtra.Text;
            //lblExtra.Text = extra.ToString();
            //lblTotal.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
             DialogResult dialogResult = MessageBox.Show("¿Estas seguro de cancelar la poliza?", "Alto!", MessageBoxButtons.YesNo);
             if (dialogResult == DialogResult.Yes)
             {
                 for (int i = 0; i < dataGridView1.RowCount; i++)
                 {
                     cmd = new OleDbCommand("insert into productosPoliza2 Values('" + dataGridView1[0, i].Value + "','" + dataGridView1[1, i].Value + "','" + dataGridView1[2, i].Value + "','" + dataGridView1[3, i].Value + "','" + dataGridView1[4, i].Value + "','" + dataGridView1[5, i].Value + "','" + dataGridView1[6, i].Value + "','" + Id + "');", conectar);
                     cmd.ExecuteNonQuery();

                     cmd = new OleDbCommand("select * from Inventario where Id='" + dataGridView1[0, i].Value.ToString() + "';", conectar);
                     OleDbDataReader reader = cmd.ExecuteReader();
                     if (reader.Read())
                     {
                         existenciasTotales = Convert.ToDouble(Convert.ToString(reader[4].ToString())) - Convert.ToDouble(dataGridView1[2, i].Value.ToString());
                         cmd = new OleDbCommand("UPDATE Inventario set Existencia='" + existenciasTotales + "' Where Id='" + dataGridView1[0, i].Value.ToString() + "';", conectar);
                         cmd.ExecuteNonQuery();
                         cmd = new OleDbCommand("insert into Kardex (IdProducto,Tipo,Descripcion,ExistenciaAntes,ExistenciaDespues,Fecha) values('" + dataGridView1[2, i].Value.ToString() + "','SALIDA','CANCELACION DE COMPRA FOLIO: " + lblFolio.Text + "'," + Convert.ToString(reader[4].ToString()) + ",'" + existenciasTotales + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "');", conectar);
                         cmd.ExecuteNonQuery();
                     }
                 }
                 cmd = new OleDbCommand("insert into Poliza2 Values('" + lblFolio.Text + "','" + lblFechaPoli.Text + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "', '" + lblMonto.Text + "','" + lblExtra.Text + "','" + Id + "');", conectar);
                 cmd.ExecuteNonQuery();
                 //cmd = new OleDbCommand("delete from productosPoliza where FolioPoliza='" + dataGridView1[7, 0].Value.ToString() + "';", conectar);
                 //cmd.ExecuteNonQuery();
                 cmd = new OleDbCommand("delete from Poliza where Folio='" + lblFolio.Text + "';", conectar);
                 cmd.ExecuteNonQuery();
                 cmd = new OleDbCommand("delete from productosPoliza where FolioPoliza='" + lblFolio.Text + "';", conectar);
                 cmd.ExecuteNonQuery();
                 MessageBox.Show("Poliza eliminada correctamente!", "Poliza", MessageBoxButtons.OK, MessageBoxIcon.Information);
                 this.Close();
             }
        }
    }
}
