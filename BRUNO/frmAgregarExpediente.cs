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
    public partial class frmAgregarExpediente : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbCommand cmd;
        public string idCliente = "0";
        public string direccion = "", tel = "", correo = "";
        public string precio, existencia, ID, origen="";
        public string idExpe = "0";
        public frmAgregarExpediente()
        {
            InitializeComponent();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            conectar.Open();
            string QUE = "";
            if (rdbCristal.Checked)
            {
                QUE = "Cristal";
                txtCristal.Text = "SI";
            }
            if (rdbPoli.Checked)
            {
                QUE = "Policarbonato";
                txtPolicarbonato.Text = "SI";
            }
            if (rdbPlastico.Checked)
            {
                QUE = "Plastico";
                txtPlastico.Text = "SI";
            }            
            cmd = new OleDbCommand("insert into Expediente(idCliente,Fecha,Laboratorio,Nombre,ArmazonMod,OD,OI,ODIG,OIIG,ODX,OIX,ODADD,OIADD,Bifocal,Progresivo,Monofocal,Cristal,Policarbonato,Plastico,Color,ALTBIF,Tratamiento,H,V,D,P,DI,DIEN,FechaEstimada,Observaciones) values('"+idCliente+"','" + dtpFecha.Value.ToString() + "','"+txtLab.Text+"','"+txtNombre.Text+"','"+txtArmazonMod.Text+"','"+txtOD.Text+"','"+txtOI.Text+"','"+txtODIG.Text+"','"+txtOIIG.Text+"','"+txtODX.Text+"','"+txtOIX.Text+"','"+txtODADD.Text+"','"+txtOIADD.Text+"','"+txtBifocal.Text+"','"+txtProgresivo.Text+"','"+txtMonofocal.Text+"','"+txtCristal.Text+"','"+txtPolicarbonato.Text+"','"+txtPlastico.Text+"','"+txtColor.Text+"','"+txtALTBIF.Text+"','"+txtTratamiento.Text+"','"+txtH.Text+"','"+txtV.Text+"','"+txtD.Text+"','"+txtP.Text+"','"+txtDI.Text+"','"+txtDIEN.Text+"','"+dtpEstimado.Value.ToString("dd/MM/yyy")+"','"+txtObservaciones.Text+"');", conectar);
            cmd.ExecuteNonQuery();

            MessageBox.Show("Se ha agregado el expediente con exito", "AGREGADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //frmClientes ser = new frmClientes();
            //ser.idCliente = idCliente;
            //ser.lblCliente.Text = txtNombre.Text;
            //ser.dataGridView1.Rows.Add("1", txtArmazonMod.Text, precio, precio, existencia,ID, origen);
            //ser.lblFinal.Text = String.Format("{0:0.00}", precio);
            //ser.lblTotal.Text = String.Format("{0:0.00}", precio);
            //ser.Show();
            this.Close();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {

            using (frmBuscarProductos buscar = new frmBuscarProductos())
            {
                if (buscar.ShowDialog() == DialogResult.OK)
                {
                    //dataGridView1.Rows.Add("1", buscar.producto, buscar.precio, buscar.monto, buscar.existencia, buscar.ID, origen);
                    txtArmazonMod.Text = buscar.producto;
                    precio = buscar.precio;
                    existencia = buscar.existencia;
                    ID = buscar.ID;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string QUE = "";
            if (rdbCristal.Checked)
            {
                QUE = "Cristal";
                txtCristal.Text = "SI";
            }
            if (rdbPoli.Checked)
            {
                QUE = "Policarbonato";
                txtPolicarbonato.Text = "SI";
            }
            if (rdbPlastico.Checked)
            {
                QUE = "Plastico";
                txtPlastico.Text = "SI";
            }
            QUE = "";
            if (txtCristal.Text=="SI")
            {
                QUE = "Cristal";
                txtCristal.Text = "SI";
            }
            if (txtPolicarbonato.Text == "SI")
            {
                QUE = "Policarbonato";
                txtPolicarbonato.Text = "SI";
            }
            if (txtPlastico.Text == "SI")
            {
                QUE = "Plastico";
                txtPlastico.Text = "SI";
            }
            Ticket ticket = new Ticket();
            ticket.MaxChar = 34;
            ticket.FontSize = 9;
            //ticket.HeaderImage = Image.FromFile("C:\\Jaeger Soft\\logo.jpg");
            ticket.AddSubHeaderLine("FECHA Y HORA:");
            ticket.AddSubHeaderLine(lblFecha1.Text);
            ticket.AddHeaderLine("CLIENTE: " + txtNombre.Text);
            ticket.AddHeaderLine("DIRECCION: " + direccion);
            ticket.AddHeaderLine("TELEFONO: " + tel);
            ticket.AddHeaderLine("E-mail: " + correo);
            ticket.AddHeaderLine("Laboratorio: " + txtLab.Text);
            ticket.AddHeaderLine("O.D.: " + txtOD.Text+"=.: " + txtODIG.Text+"X: " + txtODX.Text+"ADD.: " + txtODADD.Text);
            ticket.AddHeaderLine("O.I.: " + txtOI.Text+"=.: " + txtOIIG.Text+"X: " + txtOIX.Text+"ADD.: " + txtOIADD.Text);
            ticket.AddHeaderLine("Bifocal: " + txtBifocal.Text);
            ticket.AddHeaderLine("Progresivo: " + txtProgresivo.Text);
            ticket.AddHeaderLine("Monofocal: " + txtMonofocal.Text);
            ticket.AddHeaderLine("Tipo: " + QUE);
            ticket.AddHeaderLine("Color: " + txtColor.Text);
            ticket.AddHeaderLine("ALT BIF: " + txtALTBIF.Text);
            ticket.AddHeaderLine("Tratamiento: " + txtTratamiento.Text);
            ticket.AddHeaderLine("H.: " + txtH.Text);
            ticket.AddHeaderLine("V.: " + txtV.Text);
            ticket.AddHeaderLine("D.: " + txtD.Text);
            ticket.AddHeaderLine("P.: " + txtP.Text);
            ticket.AddHeaderLine("D.I.: " + txtDI.Text+"/: " + txtDIEN.Text);
            ticket.AddHeaderLine("Prometidos para el dia: ");
            ticket.AddHeaderLine(dtpEstimado.Value.ToString("dd/MM/yyy"));
            ticket.AddHeaderLine("Observaciones: " + txtObservaciones.Text);
            ticket.AddItem("1", txtArmazonMod.Text,"");
            //ticket.PrintTicket(Conexion.impresora);
        }

        private void frmAgregarExpediente_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string QUE = "";
            if (rdbCristal.Checked)
            {
                QUE = "Cristal";
                txtCristal.Text = "SI";
            }
            if (rdbPoli.Checked)
            {
                QUE = "Policarbonato";
                txtPolicarbonato.Text = "SI";
            }
            if (rdbPlastico.Checked)
            {
                QUE = "Plastico";
                txtPlastico.Text = "SI";
            }
            conectar.Open();
            cmd = new OleDbCommand("UPDATE Expediente Set idCliente='" + idCliente + "', Laboratorio='" + txtLab.Text + "', Nombre='" + txtNombre.Text + "', ArmazonMod='" + txtArmazonMod.Text + "', OD='" + txtOD.Text + "', OI='" + txtOI.Text + "', ODIG='" + txtODIG.Text + "', OIIG='" + txtOIIG.Text + "', ODX='" + txtODX.Text + "', OIX='" + txtOIX.Text + "', ODADD='" + txtODADD.Text + "', OIADD='" + txtOIADD.Text + "', Bifocal='" + txtBifocal.Text + "', Progresivo='" + txtProgresivo.Text + "', Monofocal='" + txtMonofocal.Text + "',Cristal='" + txtCristal.Text + "', Policarbonato='" + txtPolicarbonato.Text + "', Plastico='" + txtPlastico.Text + "', Color='" + txtColor.Text + "', ALTBIF='" + txtALTBIF.Text + "', Tratamiento='" + txtTratamiento.Text + "', H='" + txtH.Text + "', V='" + txtV.Text + "', D='" + txtD.Text + "', DI='" + txtDI.Text + "', DIEN='" + txtDIEN.Text + "', FechaEstimada='" + dtpEstimado.Value.ToString("dd/MM/yyy") + "', Observaciones='" + txtObservaciones.Text + "' where Id=" + idExpe + ";", conectar);
            cmd.ExecuteNonQuery();
            MessageBox.Show("Se ha editado el expediente con exito", "EDITADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string QUE = "";
            if (rdbCristal.Checked)
            {
                QUE = "Cristal";
                txtCristal.Text = "SI";
            }
            if (rdbPoli.Checked)
            {
                QUE = "Policarbonato";
                txtPolicarbonato.Text = "SI";
            }
            if (rdbPlastico.Checked)
            {
                QUE = "Plastico";
                txtPlastico.Text = "SI";
            }
            Ticket ticket = new Ticket();
            ticket.MaxChar = 34;

            ticket.FontSize = 9;
            //ticket.HeaderImage = Image.FromFile("C:\\Jaeger Soft\\logo.jpg");
            ticket.AddSubHeaderLine("FECHA Y HORA:");
            ticket.AddSubHeaderLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            ticket.AddHeaderLine("CLIENTE: " + txtNombre.Text);
            ticket.AddHeaderLine("DIRECCION: " + direccion);
            ticket.AddHeaderLine("TELEFONO: " + tel);
            ticket.AddHeaderLine("E-mail: " + correo);
            ticket.AddHeaderLine("Laboratorio: " + txtLab.Text);
            ticket.AddHeaderLine("O.D.: " + txtOD.Text + "=.: " + txtODIG.Text + "X: " + txtODX.Text + "ADD.: " + txtODADD.Text);
            ticket.AddHeaderLine("O.I.: " + txtOI.Text + "=.: " + txtOIIG.Text + "X: " + txtOIX.Text + "ADD.: " + txtOIADD.Text);
            ticket.AddHeaderLine("Bifocal: " + txtBifocal.Text);
            ticket.AddHeaderLine("Progresivo: " + txtProgresivo.Text);
            ticket.AddHeaderLine("Monofocal: " + txtMonofocal.Text);
            ticket.AddHeaderLine("Tipo: " + QUE);
            ticket.AddHeaderLine("Color: " + txtColor.Text);
            ticket.AddHeaderLine("ALT BIF: " + txtALTBIF.Text);
            ticket.AddHeaderLine("Tratamiento: " + txtTratamiento.Text);
            ticket.AddHeaderLine("H.: " + txtH.Text + " V.: " + txtV.Text + " D.: " + txtD.Text + " P.: " + txtP.Text);
            ticket.AddHeaderLine("D.I.: " + txtDI.Text + "/: " + txtDIEN.Text);
            ticket.AddHeaderLine("Prometidos para el dia: ");
            ticket.AddHeaderLine(dtpEstimado.Value.ToString("dd/MM/yyy"));
            ticket.AddHeaderLine("Observaciones: " + txtObservaciones.Text);
            ticket.AddItem("1", txtArmazonMod.Text, "$" + precio);
            //ticket.PrintTicket(Conexion.impresora);
        }
    }
}
