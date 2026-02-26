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
    public partial class frmAgregarClientes : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbCommand cmd;
        public bool buscar = false;
        public string Nombre { get; set; }
        public frmAgregarClientes()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.Text == "Agregar")
            {
                if (txtAdeudo.Text!="" && textBox1.Text!="")
                {
                    cmd = new OleDbCommand("insert into Clientes(Nombre, Telefono, Direccion, Referencia, RFC,Correo, Adeudo, Limite, UltimoPago,Estatus, CP) values('" + txtNombre.Text + "','" + txtTelefono.Text + "','" + txtDireccion.Text + "','" + txtReferencia.Text + "','" + txtRFC.Text + "','" + txtCorreo.Text + "','" + txtAdeudo.Text + "','" + textBox1.Text + "','" + dateTimePicker1.Value.ToString() + "','ACTIVO','" + txtCP.Text + "');", conectar);
                    cmd.ExecuteNonQuery();
                    cmd = new OleDbCommand("SELECT @@Identity;", conectar);
                    OleDbDataReader reader = cmd.ExecuteReader();
                    string idCliente = "";
                    if (reader.Read())
                    {
                        idCliente = reader[0].ToString();
                    }
                    cmd = new OleDbCommand("insert into VentasCredito(FolioVenta,IdProducto,Cantidad,Producto,MontoTotal,IdCliente,Fecha) values('001','0','1','ADEUDO INICIAL','" + txtAdeudo.Text + "','" + idCliente + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "');", conectar);
                    cmd.ExecuteNonQuery();
                    if (txtAdeudo.Text != "" || Convert.ToDouble(txtAdeudo.Text) > 0)
                    {
                        cmd = new OleDbCommand("insert into Ventas2(Monto,Fecha,Folio,IdCliente,Saldo) values('" + txtAdeudo.Text + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','INI" + idCliente + "','" + idCliente + "','" + txtAdeudo.Text + "');", conectar);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Se ha agregado el cliente con exito", "AGREGADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (buscar)
                    {
                        Nombre = txtNombre.Text;
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    }
                    else
                    {
                        this.Close();
                        frmClientes cliente = new frmClientes();
                        cliente.Show();
                    }
                }
                else
                {
                    MessageBox.Show("El limite de credito y el adeudo actual no pueden ir vacios", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
            else
            {
                cmd = new OleDbCommand("UPDATE Clientes set Nombre='" + txtNombre.Text + "', Telefono='" + txtTelefono.Text + "', Direccion='" + txtDireccion.Text + "', Referencia='" + txtReferencia.Text + "', RFC='" + txtRFC.Text + "', Correo='" + txtCorreo.Text + "', Limite='" + textBox1.Text + "', CP='"+txtCP.Text+"' where Id=" + lblID.Text + ";", conectar);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Se ha actualizado el cliente con exito", "AGREGADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
                frmClientes cliente = new frmClientes();
                cliente.Show();
            }
        }

        private void frmAgregarClientes_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void frmAgregarClientes_Load(object sender, EventArgs e)
        {
            conectar.Open();
            if (this.Text == "Agregar")
            {

            }
            else
            {
                txtAdeudo.Hide();
                lblAdeudo.Hide();
                label8.Hide();
                dateTimePicker1.Hide();

            }                      
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            CultureInfo cc = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (char.IsNumber(e.KeyChar) || e.KeyChar.ToString() == cc.NumberFormat.NumberDecimalSeparator || e.KeyChar == Convert.ToChar(Keys.Back))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void txtAdeudo_KeyPress(object sender, KeyPressEventArgs e)
        {
            CultureInfo cc = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (char.IsNumber(e.KeyChar) || e.KeyChar.ToString() == cc.NumberFormat.NumberDecimalSeparator || e.KeyChar == Convert.ToChar(Keys.Back))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void txtTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }
    }
}
