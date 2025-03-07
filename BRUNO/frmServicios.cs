using LibPrintTicket;
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
    public partial class frmServicios : Form
    {
        OleDbCommand cmd;
        private DataSet ds;
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        OleDbDataAdapter da;

        public frmServicios()
        {
            InitializeComponent();
        }

        private void frmServicios_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from Servicios;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[10].Visible = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Servicios;", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
                //dataGridView1.Columns[4].Visible = false;
            }
            else
            {

                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Servicios where Folio LIKE '%" + textBox1.Text + "%';", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
                //dataGridView1.Columns[4].Visible = false;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Servicios;", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
                //dataGridView1.Columns[4].Visible = false;
            }
            else
            {

                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Servicios where NombreCliente LIKE '%" + textBox2.Text + "%';", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
                //dataGridView1.Columns[4].Visible = false;
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            ds = new DataSet();
            da = new OleDbDataAdapter("Select * from Servicios where Fecha >=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 00:00:00# and Fecha <=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 23:59:59#;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            frmAgregarServicio addd = new frmAgregarServicio();
            addd.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmAgregarPrecioYComentario detalles = new frmAgregarPrecioYComentario();
            detalles.lblFolio.Text = dataGridView1[12, dataGridView1.CurrentRow.Index].Value.ToString();
            detalles.lblPresupuesto.Text = dataGridView1[8, dataGridView1.CurrentRow.Index].Value.ToString();
            detalles.Show();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int restante = (Convert.ToInt32(dataGridView1[9, dataGridView1.CurrentRow.Index].Value.ToString()) - (Convert.ToInt32(dataGridView1[13, dataGridView1.CurrentRow.Index].Value.ToString())));
            DialogResult dialogResult = MessageBox.Show("¿Estas seguro de cobrar el servicio? Se necesita liquidar la cantiad de:$"+restante, "Alto!", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Ticket ticket = new Ticket();
                ticket.MaxChar = 34;

                ticket.FontSize = 9;
                ticket.HeaderImage = Image.FromFile("C:\\Jaeger Soft\\logo.jpg");
                ticket.AddHeaderLine("********  NOTA DE VENTA  *******");
                ticket.AddHeaderLine("CLIENTE: " + dataGridView1[11, dataGridView1.CurrentRow.Index].Value.ToString());
                ticket.AddHeaderLine("FOLIO DE SERVICIO: " + dataGridView1[12, dataGridView1.CurrentRow.Index].Value.ToString());
                ticket.AddSubHeaderLine("FECHA Y HORA:");
                ticket.AddSubHeaderLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                ticket.AddItem("1", dataGridView1[4, dataGridView1.CurrentRow.Index].Value.ToString(), "$" + dataGridView1[9, dataGridView1.CurrentRow.Index].Value.ToString());
                ticket.AddTotal("ABONADO", "$" + dataGridView1[13, dataGridView1.CurrentRow.Index].Value.ToString());
                ticket.AddTotal("RESTANTE","$"+restante);                
                cmd = new OleDbCommand("insert into Corte(Concepto,Monto,FechaHora) Values('COBRO DE SERVICIO FOLIO " + dataGridView1[12, dataGridView1.CurrentRow.Index].Value.ToString() + "','" + restante + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "');", conectar);
                cmd.ExecuteNonQuery();
                cmd = new OleDbCommand("delete from Servicios where Id=" + dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString() + ";", conectar);
                cmd.ExecuteNonQuery();
                ticket.AddFooterLine("1.-TODO SERVICIO DE REPARACION REQUIERE UN 50%.");
                ticket.AddFooterLine("2.-NO SE ACEPTAN CANCELACIONES, NI SE HARAN DEVOLUCIONES DESPUES DE ESTAR PROCESADO EL TRABAJO SOLICITADO.");
                ticket.AddFooterLine("3.-NO SERAN SUJETO DE RECLAMACION AQUELLOS TRABAJO DE REPARACION Y/O SOLDADURAS POR DECOLORACION DEL ARMAZON.");
                ticket.AddFooterLine("4.-EN CAMBIOS DE PARTES NO ORIGINALES O DE MARCA NO HAY GARANTIA POR MAL USO.");
                ticket.AddFooterLine("5.-ARMAZONES DE USO DE MALA CALIDAD, RESECO O CON FALLAS DE MONTURASON ANEJADOS A RESPONSABILIDAD DEL CLIENTE.");
                ticket.AddFooterLine("6.-DESPUES DE 15 DIAS NO NOS HACEMOS RESPONSABLES DE NO RECOGER LAS REPARACIONES.");
                //ticket.PrintTicket(Conexion.impresora);
                MessageBox.Show("Se ha cobrado el servicio con exito", "COBRADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Servicios;", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[10].Visible = false;
            }
        }
    }
}
