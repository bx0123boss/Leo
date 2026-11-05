using LibPrintTicket;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JaegerSoft
{
    public partial class frmAgregarServicio : Form
    {
        SqlConnection conectar = new SqlConnection(Conexion.CadSQL); 
        //SqlConnection conectar = new SqlConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        SqlCommand cmd;
        string idCliente = "0", lblFolio = "";
        int foli;

        public frmAgregarServicio()
        {
            InitializeComponent();
        }

        private void frmAgregarServicio_Load(object sender, EventArgs e)
        {
            conectar.Open();
            cmd = new SqlCommand("select Numero from Folios where Folio='FolioServicio';", conectar);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                foli = Convert.ToInt32(Convert.ToString(reader[0].ToString()));
            }

            lblFolio = "S" + String.Format("{0:0000}", foli);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (idCliente != "0")
            {
                Ticket ticket = new Ticket();
                ticket.MaxChar = 34;

                ticket.FontSize = 9;
                ticket.HeaderImage = Image.FromFile("C:\\Jaeger Soft\\logo.jpg");
                ticket.AddHeaderLine("********  NOTA DE SERVICIO  ******");                
                ticket.AddHeaderLine("FOLIO DE SERVICIO: " + lblFolio);
                ticket.AddHeaderLine("CLIENTE: " + lblCliente.Text);
                ticket.AddSubHeaderLine("FECHA Y HORA:");
                ticket.AddSubHeaderLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                ticket.AddSubHeaderLine("PRODUCTO: " + txtProducto.Text);
                ticket.AddItem("1", txtServicio.Text, "$" + txtPresupuesto.Text);
                ticket.AddTotal("Abono", "$" + txtReal.Text);
                cmd = new SqlCommand("insert into Servicios(Producto,Peso, Descripcion, Servicio, Fecha,FechaEntrega, Presupuesto,idCliente,NombreCliente,Folio,CostoReal,Abono) values('" + txtProducto.Text + "','" + txtServicio.Text + "','" + txtConcepto.Text + "','" + txtServicio.Text + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + dateTimePicker1.Value.ToString() + "','" + txtPresupuesto.Text + "','" + idCliente + "','" + lblCliente.Text + "','" + lblFolio + "','" + txtPresupuesto.Text + "','" + txtReal.Text + "');", conectar);
                cmd.ExecuteNonQuery();
                cmd = new SqlCommand("insert into Corte(Concepto,Monto,FechaHora) Values('ANTICIPO DE SERVICIO FOLIO " + lblFolio + "','" + txtReal.Text + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');", conectar);
                cmd.ExecuteNonQuery();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 
                foli++;
                cmd = new SqlCommand("UPDATE Folios set Numero=" + foli + " where Folio='FolioServicio';", conectar);
                cmd.ExecuteNonQuery();
                ticket.AddFooterLine("1.-TODO SERVICIO DE REPARACION REQUIERE UN 50%.");
                ticket.AddFooterLine("2.-NO SE ACEPTAN CANCELACIONES, NI SE HARAN DEVOLUCIONES DESPUES DE ESTAR PROCESADO EL TRABAJO SOLICITADO.");
                ticket.AddFooterLine("3.-NO SERAN SUJETO DE RECLAMACION AQUELLOS TRABAJO DE REPARACION Y/O SOLDADURAS POR DECOLORACION DEL ARMAZON.");
                ticket.AddFooterLine("4.-EN CAMBIOS DE PARTES NO ORIGINALES O DE MARCA NO HAY GARANTIA POR MAL USO.");
                ticket.AddFooterLine("5.-ARMAZONES DE USO DE MALA CALIDAD, RESECO O CON FALLAS DE MONTURASON ANEJADOS A RESPONSABILIDAD DEL CLIENTE.");
                ticket.AddFooterLine("6.-DESPUES DE 15 DIAS NO NOS HACEMOS RESPONSABLES DE NO RECOGER LAS REPARACIONES.");
                //ticket.PrintTicket(Conexion.impresora);
                MessageBox.Show("Se ha agregado el servicio con exito", "AGREGADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                frmServicios ser = new frmServicios();
                ser.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("NO SE HA SELECCIONADO EL CLIENTE, FAVOR DE SELECCIONAR", "CLIENTE NO SELECICONADO", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (frmBuscaCliente cliente = new frmBuscaCliente())
            {

                if (cliente.ShowDialog() == DialogResult.OK)
                {
                    idCliente = cliente.ID;
                    lblCliente.Text = cliente.Nombre;
                    txtProducto.Focus();

                }

                cliente.Show();
            }
        }
    }
}
