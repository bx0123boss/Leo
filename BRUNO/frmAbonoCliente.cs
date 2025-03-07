using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using LibPrintTicket;
using System.Globalization;

namespace BRUNO
{
    public partial class frmAbonoCliente : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbCommand cmd;
        public string folio = "";
        public double saldo = 0;
        public double adeudo = 0;
        public double adeudoFact = 0;
        public frmAbonoCliente()
        {
            InitializeComponent();
        }

        private void txtAbono_Leave(object sender, EventArgs e)
        {
            try
            {
                adeudoFact = 0;
                adeudoFact -= Convert.ToDouble(txtAbono.Text);
                double restante = (Convert.ToDouble(txtAdeudo.Text) - Convert.ToDouble(txtAbono.Text));
                if (restante >= 0)
                {
                    txtRestante.Text = restante.ToString();
                    adeudo += adeudoFact;
                }
                else
                {
                    txtAbono.Text = "";
                    txtAbono.Focus();
                    MessageBox.Show("Ingrese una cantidad valida", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                txtAbono.Text = "";
                txtAbono.Focus();
                MessageBox.Show("No se ha introducido correctamente un dato, favor de verificar", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
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
            ticket.AddHeaderLine("CLIENTE: " + lblCliente.Text);
            ticket.AddSubHeaderLine("FECHA Y HORA:");
            ticket.AddSubHeaderLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            ticket.AddItem("", "ADEUDO", "$" + txtAdeudo.Text);
            ticket.AddItem("", "ABONO", "$" + txtAbono.Text);
            ticket.AddItem("", "RESTANTE", "$" + txtRestante.Text);
            cmd = new OleDbCommand("UPDATE Clientes set Adeudo=" + adeudo + ", UltimoPago='" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "' where Id=" + lblID.Text + ";", conectar);
            cmd.ExecuteNonQuery();
            cmd = new OleDbCommand("UPDATE Ventas2 set Saldo='" + txtRestante.Text + "' where Folio='" + folio+ "';", conectar);
            cmd.ExecuteNonQuery();
            cmd = new OleDbCommand("insert into Corte(Concepto,Monto,FechaHora,Pago) Values('Abono a cuenta de cliente " + lblCliente.Text + "','" + txtAbono.Text + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + cmbPago.Text + "');", conectar);
            cmd.ExecuteNonQuery();
            cmd = new OleDbCommand("insert into Abonos(Abono,idCliente,Fecha,Nombre,Folio,Estatus) Values('" + txtAbono.Text + "','" + lblID.Text + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','"+lblCliente.Text+"','"+folio+ "','PAGADO');", conectar);
            cmd.ExecuteNonQuery();
            //ticket.PrintTicket(Conexion.impresora);
            MessageBox.Show("Se ha abonado con exito", "EXITO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void frmAbonoCliente_Load(object sender, EventArgs e)
        {
            cmbPago.SelectedIndex = 0; 
            conectar.Open();
            if (saldo > 0)            
                txtAdeudo.Text = saldo.ToString();
            else
                txtAdeudo.Text = adeudo.ToString();
        }

        private void frmAbonoCliente_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmClientesCredito cliente = new frmClientesCredito();
            cliente.Show();
        }

        private void txtAbono_KeyPress(object sender, KeyPressEventArgs e)
        {
            CultureInfo cc = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (char.IsNumber(e.KeyChar) || e.KeyChar.ToString() == cc.NumberFormat.NumberDecimalSeparator)
                e.Handled = false;
            else
                e.Handled = true;
        }
    }
}
