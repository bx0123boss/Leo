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
    public partial class frmApartadoDetallado : Form
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbDataAdapter da;
        OleDbCommand cmd;
        int existenciasTotales = 0;
        public String usuario = "";

        public frmApartadoDetallado()
        {
            InitializeComponent();
        }

        private void frmApartadoDetallado_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from VentasApartados where FolioVenta='" + lblFolio.Text + "';", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.Columns[2].Visible = false;
            dataGridView1.Columns[6].Visible = false;
            dataGridView1.Columns[7].Visible = false;
            lblMonto.Text =""+ (Convert.ToDouble(lblRestante.Text) + Convert.ToDouble(lblAbonado.Text));
            if (usuario == "Invitado")
            {
                button1.Hide();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            DialogResult dialogResult = MessageBox.Show("¿Estas seguro de cancelar el apartado?", "Alto!", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    if (dataGridView1[8, i].Value.ToString() == "DESCONTADO DE INVENTARIO")
                    {
                        cmd = new OleDbCommand("select * from Inventario where Id='" + dataGridView1[2, i].Value.ToString() + "';", conectar);
                        OleDbDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            existenciasTotales = Convert.ToInt32(dataGridView1[3, i].Value.ToString()) + Convert.ToInt32(Convert.ToString(reader[4].ToString()));
                            cmd = new OleDbCommand("UPDATE Inventario set Existencia='" + existenciasTotales + "' Where Id='" + dataGridView1[2, i].Value.ToString() + "';", conectar);
                            cmd.ExecuteNonQuery();
                            //cmd = new OleDbCommand("delete from VentasContado where Id=" + dataGridView1[0, i].Value.ToString() + ";", conectar);
                            //cmd.ExecuteNonQuery();
                        }
                    }
                    cmd = new OleDbCommand("UPDATE VentasApartados set Estatus='CANCELADO' where Id=" + dataGridView1[0, i].Value.ToString(), conectar);
                    cmd.ExecuteNonQuery();
                }

                cmd = new OleDbCommand("delete from Apartados where Folio='" + lblFolio.Text + "';", conectar);
                cmd.ExecuteNonQuery();
                cmd = new OleDbCommand("insert into Corte(Concepto,Monto) Values('Cancelacion de la apartado folio " + lblFolio.Text + "',-" + lblAbonado.Text + ");", conectar);
                cmd.ExecuteNonQuery();
                MessageBox.Show("APARTADO CANCELADO CON EXITO", "CANCELADA!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                frmApartados apart = new frmApartados();
                apart.Show();
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Ticket ticket = new Ticket();
            ticket.MaxChar = 34;

            ticket.FontSize = 9;
            ticket.HeaderImage = Image.FromFile("C:\\Jaeger Soft\\logo.jpg");
            ticket.AddHeaderLine("*******  NOTA DE APARTADO  ******");
            //jalar datos de ticket
            for (int i = 0; i < Conexion.datosTicket.Length; i++)
            {
                ticket.AddHeaderLine(Conexion.datosTicket[i]);
            }
            ticket.AddHeaderLine("FOLIO DE APARTADO: " + lblFolio.Text);
            //ticket.AddHeaderLine("CLIENTE: " + lblCliente.Text);
            ticket.AddSubHeaderLine("FECHA Y HORA:");
            ticket.AddSubHeaderLine(lblFecha.Text);
            int totalgrid = dataGridView1.RowCount;
            
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                //Obtenemos existencias del articulo
               

                    
                    ticket.AddItem(dataGridView1[3, i].Value.ToString(), dataGridView1[4, i].Value.ToString(), dataGridView1[5, i].Value.ToString());
                    //MessageBox.Show("Se vendera el numero:"+i+"\nCantidad: "+dataGridView1[0, i].Value.ToString()+"\nProducto: "+ dataGridView1[1, i].Value.ToString()+"\nPrecio: "+dataGridView1[2, i].Value.ToString()+"\nMonto :" + dataGridView1[3, i].Value.ToString() +"\nExistencias :"+ dataGridView1[4, i].Value.ToString()+"\nID :"+dataGridView1[5, i].Value.ToString());

                
            }
           
            
            ticket.AddTotal("Total", "$"+lblMonto.Text);
            ticket.AddTotal("Abonado", "$" + lblAbonado.Text);
            ticket.AddTotal("Restante", "$" + lblRestante.Text);
            //jalar pie de ticket
            for (int i = 0; i < Conexion.pieDeTicket.Length; i++)
            {
                ticket.AddFooterLine(Conexion.pieDeTicket[i]);
            }    
            //ticket.PrintTicket(Conexion.impresora);
        }
    }
}
