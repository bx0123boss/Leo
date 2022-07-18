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
    public partial class frmExpedientes : Form
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbDataAdapter da;
        OleDbCommand cmd;
        public string idCliente = "0";
        public string direccion = "", tel = "", correo = "";
        public frmExpedientes()
        {
            InitializeComponent();
        }

        private void frmExpedientes_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from Clientes where Estatus='SUSPENDIDO';", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("¿Estas seguro de reactivar al cliente?", "Alto!", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                cmd = new OleDbCommand("UPDATE Clientes set Estatus='ACTIVO' where Id=" + dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString() + ";", conectar);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Se ha reactivado el cliente con exito", "ELIMINADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Clientes where Estatus='SUSPENDIDO';", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmAgregarExpediente add = new frmAgregarExpediente();
            add.idCliente = idCliente;
            add.direccion = direccion;
            add.tel = tel;
            add.correo = correo;
            add.txtNombre.Text = lblNombre.Text;
            add.dtpFecha.Visible = false;
            add.button4.Visible = false;
            add.lblFecha1.Text = dataGridView1[2, dataGridView1.CurrentRow.Index].Value.ToString();
            add.txtLab.Text = dataGridView1[3, dataGridView1.CurrentRow.Index].Value.ToString();
            add.txtNombre.Text = dataGridView1[4, dataGridView1.CurrentRow.Index].Value.ToString();
            add.txtArmazonMod.Text = dataGridView1[5, dataGridView1.CurrentRow.Index].Value.ToString();
            add.txtOD.Text = dataGridView1[6, dataGridView1.CurrentRow.Index].Value.ToString();
            add.txtOI.Text = dataGridView1[7, dataGridView1.CurrentRow.Index].Value.ToString();
            add.txtODIG.Text = dataGridView1[8, dataGridView1.CurrentRow.Index].Value.ToString();
            add.txtOIIG.Text = dataGridView1[9, dataGridView1.CurrentRow.Index].Value.ToString();
            add.txtODX.Text = dataGridView1[10, dataGridView1.CurrentRow.Index].Value.ToString();
            add.txtOIX.Text = dataGridView1[11, dataGridView1.CurrentRow.Index].Value.ToString();
            add.txtODADD.Text = dataGridView1[12, dataGridView1.CurrentRow.Index].Value.ToString();
            add.txtOIADD.Text = dataGridView1[13, dataGridView1.CurrentRow.Index].Value.ToString();
            add.txtBifocal.Text = dataGridView1[14, dataGridView1.CurrentRow.Index].Value.ToString();
            add.txtProgresivo.Text = dataGridView1[15, dataGridView1.CurrentRow.Index].Value.ToString();
            add.txtMonofocal.Text = dataGridView1[16, dataGridView1.CurrentRow.Index].Value.ToString();
            add.txtCristal.Text = dataGridView1[17, dataGridView1.CurrentRow.Index].Value.ToString();
            add.txtPolicarbonato.Text = dataGridView1[18, dataGridView1.CurrentRow.Index].Value.ToString();
            add.txtPlastico.Text = dataGridView1[19, dataGridView1.CurrentRow.Index].Value.ToString();
            add.txtColor.Text = dataGridView1[20, dataGridView1.CurrentRow.Index].Value.ToString();
            add.txtALTBIF.Text = dataGridView1[21, dataGridView1.CurrentRow.Index].Value.ToString();
            add.txtTratamiento.Text = dataGridView1[22, dataGridView1.CurrentRow.Index].Value.ToString();
            add.txtH.Text = dataGridView1[23, dataGridView1.CurrentRow.Index].Value.ToString();
            add.txtV.Text = dataGridView1[24, dataGridView1.CurrentRow.Index].Value.ToString();
            add.txtD.Text = dataGridView1[25, dataGridView1.CurrentRow.Index].Value.ToString();
            add.txtP.Text = dataGridView1[26, dataGridView1.CurrentRow.Index].Value.ToString();
            add.txtDI.Text = dataGridView1[27, dataGridView1.CurrentRow.Index].Value.ToString();
            add.txtDIEN.Text = dataGridView1[28, dataGridView1.CurrentRow.Index].Value.ToString();
            //add.dtpEstimado.Visible = false;
            add.lblFecha2.Text = dataGridView1[29, dataGridView1.CurrentRow.Index].Value.ToString();
            add.txtObservaciones.Text = dataGridView1[30, dataGridView1.CurrentRow.Index].Value.ToString();
            add.btnAceptar.Visible = false;
            add.dtpEstimado.Value = Convert.ToDateTime(dataGridView1[29, dataGridView1.CurrentRow.Index].Value.ToString()); 
            //add.rdbCristal.Visible = false;
            //add.rdbPlastico.Visible = false;
            //add.rdbPoli.Visible = false;
            if (add.txtCristal.Text == "SI")
                add.rdbCristal.Checked = true;
            else if (add.txtPlastico.Text == "SI")
                add.rdbPlastico.Checked = true;
            else if (add.txtPolicarbonato.Text == "SI")
                add.rdbPoli.Checked = true;
            //add.txtCristal.Visible = true;
            //add.txtPlastico.Visible = true;
            add.button3.Visible = true;
            //add.txtPolicarbonato.Visible = true;
            add.idExpe = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
            add.Show();
            this.Close();

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Clientes ORDER BY Nombre and Estatus='SUSPENDIDO';", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
            }
            else
            {

                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Clientes where Estatus='SUSPENDIDO' and Nombre LIKE '%" + textBox1.Text + "%' ORDER BY Nombre ;", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
