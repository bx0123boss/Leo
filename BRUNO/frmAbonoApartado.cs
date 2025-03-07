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
    public partial class frmAbonoApartado : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);             
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbCommand cmd;
        public double abonado;
        public int idCliente;
        public frmAbonoApartado()
        {
            InitializeComponent();
        }

        private void frmAbonoApartado_Load(object sender, EventArgs e)
        {
            cmbPago.SelectedIndex = 0; 
            conectar.Open();
        }

        private void txtAbono_Layout(object sender, LayoutEventArgs e)
        {
            

        }

        private void txtAbono_Leave(object sender, EventArgs e)
        {
            if ((Convert.ToDouble(txtAdeudo.Text) - Convert.ToDouble(txtAbono.Text)) < 0)
            {
                MessageBox.Show("Ingrese una cantidad valida", "ALTO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtAbono.Clear();
                txtAbono.Focus();
            }
            else
            {
                txtRestante.Text = "" + (Convert.ToDouble(txtAdeudo.Text) - Convert.ToDouble(txtAbono.Text));
                abonado = abonado + Convert.ToDouble(txtAbono.Text);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //aver
            Ticket ticket = new Ticket();
            ticket.MaxChar = 34;
            ticket.FontSize = 9;
            ticket.HeaderImage = Image.FromFile("C:\\Jaeger Soft\\logo.jpg");
            ticket.AddHeaderLine("********  NOTA DE ABONO  *******");
            //jalar datos de ticket
            for (int i = 0; i < Conexion.datosTicket.Length; i++)
            {
                ticket.AddHeaderLine(Conexion.datosTicket[i]);
            }
            ticket.AddHeaderLine("METODO DE PAGO:");
            ticket.AddHeaderLine(cmbPago.Text);
            ticket.AddHeaderLine("FOLIO: " + lblFolio.Text);
            ticket.AddSubHeaderLine("FECHA Y HORA:");
            ticket.AddSubHeaderLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            ticket.AddItem("", "ADEUDO", "$" + txtAdeudo.Text);
            ticket.AddItem("", "ABONO", "$" + txtAbono.Text);
            ticket.AddItem("", "RESTANTE", "$" + txtRestante.Text);
            //ticket.PrintTicket("print");
            cmd = new OleDbCommand("UPDATE Apartados set Restante='" + txtRestante.Text + "', Abono='"+abonado+"' where Id=" + lblID.Text + ";", conectar);
            cmd.ExecuteNonQuery();
            cmd = new OleDbCommand("insert into Corte(Concepto,Monto,FechaHora,Pago) Values('Abono a cuenta de Apartado con Folio:  " + lblFolio.Text + "','" + txtAbono.Text + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','"+cmbPago.Text+"');", conectar);
            cmd.ExecuteNonQuery();
            cmd = new OleDbCommand("insert into AbonosApartado(Abono,idCliente,Fecha,Folio) Values('" + txtAbono.Text + "','" + idCliente + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','"+lblFolio.Text+"');", conectar);
            cmd.ExecuteNonQuery();
            //ticket.PrintTicket(Conexion.impresora);
            MessageBox.Show("Se ha abonado con exito", "EXITO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            frmApartados apart = new frmApartados();
            apart.Show();
            this.Close();
        }

        private void txtAbono_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Solo se permiten numeros", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
        }
    }
}
