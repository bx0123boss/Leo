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
    public partial class frmCaptura : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbCommand cmd;

        public frmCaptura()
        {
            InitializeComponent();
        }

        private void frmCaptura_Load(object sender, EventArgs e)
        {

        }

        private void txtID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (txtID.Text == "")
                {
                }
                else
                {
                    bool esta = false;
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                            if (dataGridView1[0, i].Value.ToString()==txtID.Text)
                            {
                                esta=true;
                                dataGridView1[1, i].Value = Convert.ToInt32(dataGridView1[1, i].Value) + 1;
                            }                        
                    }
                    if (!esta)
                    {
                        dataGridView1.Rows.Add(txtID.Text, 1);
                        
                    }
                    txtID.Clear();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            conectar.Open();
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                cmd = new OleDbCommand("insert into Captura(IdProducto,Cantidad,Fecha) values('" + dataGridView1[0, i].Value.ToString() + "'," + dataGridView1[1, i].Value.ToString() + ",'" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "');", conectar);
                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("Captura realizada con exito", "CAPTURA", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
            frmInventario inv = new frmInventario();
            inv.Show();
        }
    }
}
