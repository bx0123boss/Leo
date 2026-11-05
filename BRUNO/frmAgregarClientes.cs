using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JaegerSoft
{
    public partial class frmAgregarClientes : frmBase
    {
        SqlConnection conectar = new SqlConnection(Conexion.CadSQL); 
        //SqlConnection conectar = new SqlConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        SqlCommand cmd;
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
                    using (var cmd = new SqlCommand(
                            "INSERT INTO Clientes (Nombre, Telefono, Direccion, Referencia, RFC, Correo, Adeudo, Limite, UltimoPago, Estatus, CP, Regimen) " +
                            "VALUES (@Nombre, @Telefono, @Direccion, @Referencia, @RFC, @Correo, @Adeudo, @Limite, @UltimoPago, 'ACTIVO', @CP, @Regimen)", conectar))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                        cmd.Parameters.AddWithValue("@Telefono", txtTelefono.Text.Trim());
                        cmd.Parameters.AddWithValue("@Direccion", txtDireccion.Text.Trim());
                        cmd.Parameters.AddWithValue("@Referencia", txtReferencia.Text.Trim());
                        cmd.Parameters.AddWithValue("@RFC", txtRFC.Text.Trim());
                        cmd.Parameters.AddWithValue("@Correo", txtCorreo.Text.Trim());

                        decimal adeudo = 0;
                        decimal.TryParse(txtAdeudo.Text, out adeudo);
                        cmd.Parameters.AddWithValue("@Adeudo", adeudo);

                        decimal limite = 0;
                        decimal.TryParse(textBox1.Text, out limite);
                        cmd.Parameters.AddWithValue("@Limite", limite);

                        cmd.Parameters.AddWithValue("@UltimoPago", dateTimePicker1.Value);

                        cmd.Parameters.AddWithValue("@CP", txtCP.Text.Trim());
                        cmd.Parameters.AddWithValue("@Regimen", txtRegimen.Text.Trim());

                        cmd.ExecuteNonQuery();
                    }
                    cmd = new SqlCommand("SELECT @@Identity;", conectar);
                    SqlDataReader reader = cmd.ExecuteReader();
                    string idCliente = "";
                    if (reader.Read())
                    {
                        idCliente = reader[0].ToString();
                    }
                    
                    if (txtAdeudo.Text != "" || Convert.ToDouble(txtAdeudo.Text) > 0)
                    {
                        cmd = new SqlCommand("insert into VentasCredito(FolioVenta,IdProducto,Cantidad,Producto,MontoTotal,IdCliente,Fecha, Categoria) values('INI" + idCliente + "','0','1','ADEUDO INICIAL','" + txtAdeudo.Text + "','" + idCliente + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', 'ADEUDO INICIAL');", conectar);
                        cmd.ExecuteNonQuery();
                        cmd = new SqlCommand("insert into Ventas2(Monto,Fecha,Folio,IdCliente,Saldo) values('" + txtAdeudo.Text + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','INI" + idCliente + "','" + idCliente + "','" + txtAdeudo.Text + "');", conectar);
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
                cmd = new SqlCommand("UPDATE Clientes set Nombre='" + txtNombre.Text + "', Telefono='" + txtTelefono.Text + "', Direccion='" + txtDireccion.Text + "', Referencia='" + txtReferencia.Text + "', RFC='" + txtRFC.Text + "', Correo='" + txtCorreo.Text + "', Limite='" + textBox1.Text + "', CP='"+txtCP.Text+ "', Regimen='" + txtRegimen.Text + "' where Id=" + lblID.Text + ";", conectar);
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
            EstilizarTextBox(textBox1);
            EstilizarTextBox(txtNombre);
            EstilizarTextBox(txtTelefono);
            EstilizarTextBox(txtDireccion);
            EstilizarTextBox(txtCP);
            EstilizarTextBox(txtReferencia);
            EstilizarTextBox(txtRFC);
            EstilizarTextBox(txtCorreo);
            EstilizarTextBox(txtRegimen);
            EstilizarTextBox(txtAdeudo);
            EstilizarBotonPrimario(button1);
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
