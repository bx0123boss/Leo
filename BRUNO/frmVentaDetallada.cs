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
    public partial class frmVentaDetallada : Form
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbDataAdapter da;
        OleDbCommand cmd;
        double existenciasTotales = 0;
        public String usuario = "";

        public frmVentaDetallada()
        {
            InitializeComponent();
        }

        private void frmVentaDetallada_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from VentasContado where FolioVenta='" + lblFolio.Text + "';", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.Columns[2].Visible = false;
            if (usuario == "Invitado")
            {
                button1.Hide();
            }
            cmd = new OleDbCommand("select * from Clientes where Id=" + dataGridView1[6, 0].Value.ToString() + ";", conectar);
                    OleDbDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        lblCliente.Text = Convert.ToString(reader[1].ToString());
                        lblDireccion.Text = Convert.ToString(reader[3].ToString());
                    }
                    else
                    {
                        lblCliente.Text = "PUBLICO EN GENERAL";
                    }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            DialogResult dialogResult = MessageBox.Show("¿Estas seguro de cancelar la venta?", "Alto!", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {

                    cmd = new OleDbCommand("select * from Inventario where Id='" + dataGridView1[2, i].Value.ToString() + "';", conectar);
                    OleDbDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        existenciasTotales = Convert.ToDouble(dataGridView1[3, i].Value.ToString()) + Convert.ToDouble(Convert.ToString(reader[4].ToString()));
                        cmd = new OleDbCommand("UPDATE Inventario set Existencia='" + existenciasTotales + "' Where Id='" + dataGridView1[2, i].Value.ToString() + "';", conectar);
                        cmd.ExecuteNonQuery();
                        cmd = new OleDbCommand("UPDATE VentasContado set Utilidad='0' Where Id=" + dataGridView1[0, i].Value.ToString() + ";", conectar);
                        cmd.ExecuteNonQuery();
                        cmd = new OleDbCommand("insert into Kardex (IdProducto,Tipo,Descripcion,ExistenciaAntes,ExistenciaDespues,Fecha) values('" + dataGridView1[2, i].Value.ToString() + "','ENTRADA','CANCELACION DE VENTA FOLIO: " + lblFolio.Text + "'," + Convert.ToString(reader[4].ToString()) + ",'" + existenciasTotales+ "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "');", conectar);
                        cmd.ExecuteNonQuery();
                        //cmd = new OleDbCommand("delete from VentasContado where Id=" + dataGridView1[0, i].Value.ToString() + ";", conectar);
                        //cmd.ExecuteNonQuery();
                    }
                }
                cmd = new OleDbCommand("update Ventas set Estatus='CANCELADO' where Folio='"+lblFolio.Text+"';",conectar);
                cmd.ExecuteNonQuery();
                cmd = new OleDbCommand("insert into Corte(Concepto,Monto,Pago) Values('Cancelacion de la venta a contado folio " + lblFolio.Text + "',-" + lblMonto.Text + ",'01=EFECTIVO');", conectar);
                cmd.ExecuteNonQuery();
                MessageBox.Show("VENTA CANCELADA CON EXITO","CANCELADA!",MessageBoxButtons.OK,MessageBoxIcon.Information);
                this.Close();
            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }
        }

        private void frmVentaDetallada_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmReporteVentas repor = new frmReporteVentas();
            repor.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Ticket ticket = new Ticket();
            ticket.MaxChar = 32;
            ticket.FontSize = 9;
            ticket.MaxCharDescription = 16;
            ticket.HeaderImage = Image.FromFile("C:\\Jaeger Soft\\logo.jpg");
            ticket.AddHeaderLine("******  NOTA DE VENTA  *****"); 
            for (int i = 0; i < Conexion.datosTicket.Length; i++)
            {
                ticket.AddHeaderLine(Conexion.datosTicket[i]);
            }
            ticket.AddHeaderLine("FOLIO DE VENTA: " + lblFolio.Text);
            
            ticket.AddHeaderLine("CLIENTE: " + lblCliente.Text);
            ticket.AddHeaderLine("DIRECCION: " + lblDireccion.Text);
            ticket.AddSubHeaderLine("FECHA Y HORA:");

            ticket.AddSubHeaderLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                ticket.AddItem(dataGridView1[3, i].Value.ToString(), dataGridView1[4, i].Value.ToString(), "$" +dataGridView1[5, i].Value.ToString());
            }
            ticket.AddTotal("Total", "$" + lblMonto.Text);
            //jalar pie de ticket
            for (int i = 0; i < Conexion.pieDeTicket.Length; i++)
            {
                ticket.AddFooterLine(Conexion.pieDeTicket[i]);
            }    
            ticket.PrintTicket(Conexion.impresora);
        }
    }
}
