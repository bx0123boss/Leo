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
    public partial class frmAdeudoProveedor : Form
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbDataAdapter da;
        public string id;
        OleDbCommand cmd;
        public double pesos, dolar, oro;
        public frmAdeudoProveedor()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.Text == "PESOS (MX)")
            {
                lblPesos.Text ="" + (Convert.ToDouble(lblPesos.Text) + Convert.ToDouble(txtMonto.Text));
                txtMonto.Clear();
                comboBox2.SelectedIndex = 0;
                txtMonto.Focus();
            }
            else if (comboBox2.Text == "DOLAR (EU)")
            {
                lblDolares.Text = "" + (Convert.ToDouble(lblDolares.Text) + Convert.ToDouble(txtMonto.Text));
                txtMonto.Clear();
                comboBox2.SelectedIndex = 0;
                txtMonto.Focus();
            }
            else if (comboBox2.Text == "ORO (GRAMOS)")
            {
                lblOro.Text = "" + (Convert.ToDouble(lblOro.Text) + Convert.ToDouble(txtMonto.Text));
                txtMonto.Clear();
                comboBox2.SelectedIndex = 0;
                txtMonto.Focus();
            }
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void txtValor_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void txtValor_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void txtPesos_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void txtPesos_TextChanged(object sender, EventArgs e)
        {

        }

        private void frmAdeudoProveedor_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from AbonosProveedores where idProveedor='"+id+"' order by Id desc;", conectar);
            da.Fill(ds, "Id");
            dataGridView2.DataSource = ds.Tables["Id"];
            dataGridView2.Columns[0].Visible = false;
            dataGridView2.Columns[1].Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Convert.ToDouble(lblPesos.Text) != pesos)
            {
                //PESOS
                cmd = new OleDbCommand("insert into AbonosProveedores(idProveedor,Fecha,Monto,Unidad,ValorUnidad,ValorPesos,AdeudoActual) values('" + id + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + (pesos - Convert.ToDouble(lblPesos.Text)) + "','PESOS (MX)','1','" + (pesos - Convert.ToDouble(lblPesos.Text)) + "','" + lblActual.Text + "');", conectar);
                cmd.ExecuteNonQuery();
            }
            if (Convert.ToDouble(lblDolares.Text) != dolar)
            {
                //DOLAR
                cmd = new OleDbCommand("insert into AbonosProveedores(idProveedor,Fecha,Monto,Unidad,ValorUnidad,ValorPesos,AdeudoActual) values('" + id + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + (dolar - Convert.ToDouble(lblDolares.Text)) + "','DOLAR (EU)','" + textBox1.Text + "','" + ((dolar - Convert.ToDouble(lblDolares.Text)) * Convert.ToDouble(textBox1.Text)) + "','" + lblActual.Text + "');", conectar);
                cmd.ExecuteNonQuery();
            }
            if (Convert.ToDouble(lblOro.Text) != oro)
            {
                //ORO
                cmd = new OleDbCommand("insert into AbonosProveedores(idProveedor,Fecha,Monto,Unidad,ValorUnidad,ValorPesos,AdeudoActual) values('" + id + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + (oro - Convert.ToDouble(lblOro.Text)) + "','ORO (GRAMOS)','" + textBox2.Text + "','" + ((oro - Convert.ToDouble(lblOro.Text)) * Convert.ToDouble(textBox2.Text)) + "','" + lblActual.Text + "');", conectar);
                cmd.ExecuteNonQuery();
            }
            cmd = new OleDbCommand("UPDATE Proveedores set AdeudoActual='" + lblActual.Text + "', AdeudoPesos='" + lblPesos.Text + "', AdeudoDolares='" + lblDolares.Text + "', AdeudoOro='"+lblOro.Text+"' where Id=" + id + ";", conectar);
            cmd.ExecuteNonQuery();
            MessageBox.Show("Se ha agregado el adeudo con exito", "AGREGADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            conectar.Close();
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from AbonosProveedores where idProveedor='" + id + "' order by Id desc;", conectar);
            da.Fill(ds, "Id");
            dataGridView2.DataSource = ds.Tables["Id"];
            dataGridView2.Columns[0].Visible = false;
            dataGridView2.Columns[1].Visible = false;
            button1.Enabled = false;
        }

        private void frmAdeudoProveedor_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmProveedores prov = new frmProveedores();
            prov.Show();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                textBox2.Focus();
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                lblActual.Text = "" + (Convert.ToDouble(lblPesos.Text) + (Convert.ToDouble(lblOro.Text) * Convert.ToDouble(textBox2.Text)) + (Convert.ToDouble(lblDolares.Text) * Convert.ToDouble(textBox1.Text)));
            } 
        }
    }
}
