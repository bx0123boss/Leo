using LibPrintTicket;
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
    public partial class frmCorte : Form
    {
        double mas = 0;
        double menos = 0;
        double tarjeta = 0;
        double trans = 0;
        public string usuario = "";
        private DataSet ds;
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Restaurante.accdb");
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        OleDbDataAdapter da;
        OleDbCommand cmd;
        double total = 0, utilidad = 0, inversion = 0, gastos=0;
        int folio;

        public frmCorte()
        {
            InitializeComponent();
        }

        private void frmCorte_Load(object sender, EventArgs e)
        {
            if (usuario=="Invitado")
            {
                //button2.Hide();
                label9.Hide();
                lblInversion.Hide();
                label14.Hide();
                lblUtilidad.Hide();
                label10.Hide();
                lblGastos.Hide();
                label11.Hide();
                lblBruta.Hide();
            }
            if (Conexion.lugar == "LEO")
            {
                label9.Hide();
                lblInversion.Hide();
                label14.Hide();
                lblUtilidad.Hide();
                label10.Hide();
                lblGastos.Hide();
                label11.Hide();
                lblBruta.Hide();
            }
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from Corte where Pago='01=EFECTIVO';", conectar);
            da.Fill(ds, "Id");
            dgvCorte.DataSource = ds.Tables["Id"];
            dgvCorte.Columns[0].Visible = false;

            ds = new DataSet();
            da = new OleDbDataAdapter("select * from Corte where Pago='04=TARJETA DE CREDITO' or Pago='28=TARJETA DE DEBITO';", conectar);
            da.Fill(ds, "Id");
            dataGridView3.DataSource = ds.Tables["Id"];
            dataGridView3.Columns[0].Visible = false;

            ds = new DataSet();
            da = new OleDbDataAdapter("select * from Corte where Pago='03=TRANFERENCIA ELECTRONICA DE FONDOS';", conectar);
            da.Fill(ds, "Id");
            dataGridView4.DataSource = ds.Tables["Id"];
            dataGridView4.Columns[0].Visible = false;

            for (int i = 0; i < dgvCorte.RowCount; i++)
            {
                if (Convert.ToDouble(dgvCorte[2, i].Value.ToString()) < 0)
                {
                    menos += Convert.ToSingle(dgvCorte[2, i].Value.ToString(), CultureInfo.CreateSpecificCulture("es-ES"));

                }
                else if (Convert.ToDouble(dgvCorte[2, i].Value.ToString()) > 0)
                    mas += Convert.ToSingle(dgvCorte[2, i].Value.ToString(), CultureInfo.CreateSpecificCulture("es-ES"));
            }

            for (int i = 0; i < dataGridView3.RowCount; i++)
            {
                tarjeta += Convert.ToSingle(dataGridView3[2, i].Value.ToString(), CultureInfo.CreateSpecificCulture("es-ES"));
            }

            for (int i = 0; i < dataGridView4.RowCount; i++)
            {
                trans += Convert.ToSingle(dataGridView4[2, i].Value.ToString(), CultureInfo.CreateSpecificCulture("es-ES"));
            }
            cmd = new OleDbCommand("select Numero from Folios where Folio='Corte';", conectar);
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                folio = Convert.ToInt32(Convert.ToString(reader[0].ToString()));
            }

            ds = new DataSet();
            string[] fecha = DateTime.Now.Date.ToString().Split(' ');

            string subcadena = DateTime.Now.Date.Month.ToString() + "/" + DateTime.Now.Date.Day.ToString() + "/" + DateTime.Now.Date.Year.ToString() ;
            da = new OleDbDataAdapter("select MontoTotal,Utilidad,Id from VentasContado Where Fecha>=#" + subcadena + " 00:00:00# and Fecha <=#" + subcadena + " 23:59:59#;", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (Convert.ToDouble(dataGridView1[1, i].Value.ToString()) > 0)
                {
                    total += Convert.ToDouble(dataGridView1[0, i].Value.ToString());
                    utilidad += Convert.ToDouble(dataGridView1[1, i].Value.ToString());
                }

            }
            ds = new DataSet();
            da = new OleDbDataAdapter("select * from GastosDetallados Where Fecha>=#" + subcadena + " 00:00:00# and Fecha <=#" + subcadena + " 23:59:59#;", conectar);
            da.Fill(ds, "Id");
            dataGridView2.DataSource = ds.Tables["Id"];


            ds = new DataSet();
            da = new OleDbDataAdapter("select * from Abonos Where Fecha>=#" + subcadena + " 00:00:00# and Fecha <=#" + subcadena + " 23:59:59#;", conectar);
            da.Fill(ds, "Id");
            dataGridView5.DataSource = ds.Tables["Id"];
            dataGridView5.Columns[0].Visible = false;
            dataGridView5.Columns[2].Visible = false;

            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                    gastos += Convert.ToDouble(dataGridView2[2, i].Value.ToString());                
            }
            lblBruta.Text = "$" + (utilidad - gastos);
            lblGastos.Text = "$"+ gastos.ToString(); ;
            inversion = total - utilidad;
            lblInversion.Text = "$"+ inversion;
            lblUtilidad.Text = "$"+utilidad;
            lblEntrada.Text = "$" + (mas + tarjeta + trans);
            lblSalida.Text = "$" + (menos * -1);
            lblCorte.Text = "$" + mas;
            lblCredito.Text = "$" + Math.Round(tarjeta * .95,2);
            lbl5por.Text = "$" + Math.Round(tarjeta * .05);
            lblTrans.Text = "$" + trans;
            lblTotal.Text = "$" + (tarjeta + mas + menos + trans);
            
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string fecha = "";
            cmd = new OleDbCommand("select * from Fech where Id=1;", conectar);
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int caja = Convert.ToInt32(Convert.ToString(reader[1].ToString()));
                fecha = Convert.ToString(reader[2].ToString());
            }
            button1.Visible = false;
            folio++;
            Ticket ticket = new Ticket();
            if (Conexion.lugar == "LEO")
                ticket.MaxChar = 50;
            else
                ticket.MaxChar = Conexion.MaxChar;
            ticket.FontSize = Conexion.FontSize;
            ticket.MaxCharDescription = Conexion.MaxCharDescription;
            if (Conexion.Font == "")
            {

            }
            else
                ticket.FontName = Conexion.Font;
            ticket.HeaderImage = Image.FromFile("C:\\Jaeger Soft\\logo.jpg");
            ticket.AddHeaderLine("*****  CORTE DE CAJA  ****");
            for (int i = 0; i < Conexion.datosTicket.Length; i++)
            {
                ticket.AddHeaderLine(Conexion.datosTicket[i]);
            }
            ticket.AddSubHeaderLine("     Apertura de caja:");
            ticket.AddSubHeaderLine(fecha);
            ticket.AddSubHeaderLine("        Corte de caja:");
            ticket.AddSubHeaderLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            for (int i = 0; i < dgvCorte.RowCount; i++)
            {                
                //MessageBox.Show("insert into Cortes(Concepto,Monto,idCorte,Tipo) Values('" + dgvCorte[1, i].Value.ToString() + "','" + dgvCorte[2, i].Value.ToString() + "','" + dgvCorte[3, i].Value.ToString() + "','" + folio + "','PAGO CONTADO');");
                cmd = new OleDbCommand("insert into Cortes(Concepto,Monto,idCorte,Tipo) Values('" + dgvCorte[1, i].Value.ToString() + "','" + dgvCorte[2, i].Value.ToString() + "','" + folio + "','PAGO CONTADO');", conectar);                
                cmd.ExecuteNonQuery();
                if (Conexion.lugar == "LEO")
                {
                    
                }
                else
                    ticket.AddItem("1", dgvCorte[1, i].Value.ToString(), "   $" + dgvCorte[2, i].Value.ToString());
            }
            for (int i = 0; i < dataGridView3.RowCount; i++)
            {
                cmd = new OleDbCommand("insert into Cortes(Concepto,Monto,idCorte,Tipo) Values('" + dataGridView3[1, i].Value.ToString() + "','" + dataGridView3[2, i].Value.ToString() + "','" + folio + "','PAGO TARJETA CREDITO');", conectar);
                cmd.ExecuteNonQuery();
            }
                cmd = new OleDbCommand("INSERT INTO histocortes(Id,Monto,Fecha,Mas,Menos,Tarjeta,utilidad,inversion) VALUES ('" + folio + "','" + lblTotal.Text + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','"+mas+"','"+menos+"','"+tarjeta+"','"+utilidad+"','"+inversion+"');", conectar);
                cmd.ExecuteNonQuery();
            
            cmd = new OleDbCommand("UPDATE Folios set Numero=" + folio + " where Folio='Corte';", conectar);
            cmd.ExecuteNonQuery();
            cmd = new OleDbCommand("UPDATE Folios set Numero=0 where Folio='Inicio';", conectar);
            cmd.ExecuteNonQuery();
            cmd = new OleDbCommand("delete from Corte where 1;", conectar);
            cmd.ExecuteNonQuery();
            cmd = new OleDbCommand("UPDATE Fech set Caja='0' where Id=1;", conectar);
            cmd.ExecuteNonQuery();
            cmd = new OleDbCommand("delete from Credito where 1;", conectar);
            cmd.ExecuteNonQuery();
            ticket.AddTotal("Efectivo", lblCorte.Text);
            ticket.AddTotal("Tarjetas", lblCredito.Text);
            ticket.AddTotal("Entradas", lblEntrada.Text);
            ticket.AddTotal("Salidas",lblSalida.Text);
            ticket.AddTotal("Total", lblTotal.Text);
            ticket.PrintTicket(Conexion.impresora);
            MessageBox.Show("CORTE REALIZADO CON EXITO","EXITO",MessageBoxButtons.OK,MessageBoxIcon.Information);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Visible = false;
            folio++;
            Ticket ticket = new Ticket();
            ticket.MaxChar = Conexion.MaxChar;
            ticket.FontSize = Conexion.FontSize;
            ticket.MaxCharDescription = Conexion.MaxCharDescription;
            if (Conexion.Font == "")
            {

            }
            else
                ticket.FontName = Conexion.Font;
            ticket.HeaderImage = Image.FromFile("C:\\Jaeger Soft\\logo.jpg");
            ticket.AddHeaderLine("********  CORTE PARCIAL  *******");
            ticket.AddSubHeaderLine("FECHA Y HORA:");
            ticket.AddSubHeaderLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            for (int i = 0; i < dgvCorte.RowCount; i++)
            {
                ticket.AddItem("1", dgvCorte[1, i].Value.ToString(), "   $" + dgvCorte[2, i].Value.ToString());
            }
            ticket.AddTotal("Efectivo", lblCorte.Text);
            ticket.AddTotal("Tarjetas", lblCredito.Text);
            ticket.AddTotal("Entradas", lblEntrada.Text);
            ticket.AddTotal("Salidas", lblSalida.Text);
            ticket.AddTotal("Total", lblTotal.Text);
            ticket.PrintTicket(Conexion.impresora);
            MessageBox.Show("CORTE REALIZADO CON EXITO", "EXITO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
    }
}
