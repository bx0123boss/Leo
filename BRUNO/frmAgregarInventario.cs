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
using System.Globalization;

namespace BRUNO
{
    public partial class frmAgregarInventario : Form
    {
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        OleDbCommand cmd;
        double entre24, Kilataje, Resultado, Maquila, Total, Peso, PrecioCompra, Treinta, por2, Diez; 

        public frmAgregarInventario()
        {
            InitializeComponent();
            conectar.Open();
        }

        private void txtID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                SendKeys.Send("+{TAB}");
            }
            if (!char.IsLetter(e.KeyChar) && !char.IsNumber(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        private void txtID_Leave(object sender, EventArgs e)
        {
            cmd = new OleDbCommand("select count(*) from " + lblOrigen.Text + " where Id='" + txtID.Text + "';", conectar);
            int valor = int.Parse(cmd.ExecuteScalar().ToString());
            if (valor == 1)
            {
                MessageBox.Show("El producto ya existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtID.Clear();
                txtID.Focus();
                //groupBox2.Enabled = true;
                //cmd = new OleDbCommand("select * from " + lblOrigen.Text + " where Id='" + txtID.Text + "';", conectar);
                //OleDbDataReader reader = cmd.ExecuteReader();
                //if (reader.Read())
                //{

                //    txtActuales.Text = Convert.ToString(reader[4].ToString());
                //    lblNombre.Text = Convert.ToString(reader[1].ToString());
                //    lblPrecioCompra.Text = Convert.ToString(reader[2].ToString());
                //    lblPrecioVenta.Text = Convert.ToString(reader[3].ToString());
                //    lblExistencias.Text = Convert.ToString(reader[4].ToString());
                //    lblLimite.Text = Convert.ToString(reader[5].ToString());
                //}
                //groupBox3.Visible = true;
                //groupBox3.Enabled = true;
                //groupBox1.Visible = false;
                //textBox1.Focus();
            }
            else
            {
                cmd = new OleDbCommand("select count(*) from InventarioSusp where Id='" + txtID.Text + "';", conectar);
                    valor = int.Parse(cmd.ExecuteScalar().ToString());
                    if (valor == 1)
                    {
                        MessageBox.Show("El producto que desea registrar debe ser reactivado", "Alto", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        groupBox1.Enabled = false;
                    }
                    else
                    {
                        groupBox1.Enabled = true;
                        txtProducto.Focus();
                    }
            }
        }

        private void frmAgregarInventario_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            cmd = new OleDbCommand("Select Id,Nombre from Unidades;", conectar);
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dt);
            cmbUnidad.DisplayMember = "Nombre";
            cmbUnidad.ValueMember = "Id";
            cmbUnidad.DataSource = dt;
            cmbUnidad.Text = "";


            dt = new DataTable();
            cmd = new OleDbCommand("Select Id,Nombre from Categorias;", conectar);
            da = new OleDbDataAdapter(cmd);
            da.Fill(dt);
            cmbCategoria.DisplayMember = "Nombre";
            cmbCategoria.ValueMember = "Id";
            cmbCategoria.DataSource = dt;
            cmbCategoria.Text = "";
            //cmd = new OleDbCommand("select * from Cambio where Id=1;", conectar);
            //OleDbDataReader reader = cmd.ExecuteReader();
            //if (reader.Read())
            //{
            //    PrecioOro = Convert.ToDouble(reader[1].ToString());
            //    Dolar = Convert.ToDouble(reader[2].ToString());
            //    lblOroFino.Text = "" + PrecioOro;
            //}
            //cmbCategoria.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(textBox1.Text) < 0)
            {
                MessageBox.Show("Numero incorrecto en las existencias a añadir, favor de introducir un numero valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                cmd = new OleDbCommand("UPDATE " + lblOrigen.Text + " set Existencia='" + txtTotales.Text + "' Where Id='" + txtID.Text + "';", conectar);
                cmd.ExecuteNonQuery();
                cmd = new OleDbCommand("insert into Kardex (IdProducto,Tipo,Descripcion,ExistenciaAntes,ExistenciaDespues,Fecha) values('" + txtID.Text + "','ENTRADA','ENTRADA DE ARTICULO',"+txtActuales.Text+",'" + txtTotales.Text + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "');", conectar);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Se han añadido existencias al producto correctamente", "Correcto", MessageBoxButtons.OK, MessageBoxIcon.Information);
                groupBox2.Enabled = false;
                txtActuales.Clear();
                textBox1.Clear();
                groupBox1.Visible = true;
                groupBox3.Visible = false;
                groupBox3.Enabled = false;
                txtTotales.Clear();
                txtID.Clear();
                txtID.Focus();
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            txtTotales.Text = "" + (Convert.ToInt32(txtActuales.Text) + Convert.ToInt32(textBox1.Text));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Convert.ToDouble(txtVenta.Text) < 0 || Convert.ToDouble(txtCompra.Text) < 0 || Convert.ToDouble(txtEspecial.Text) < 0)
            {
                
            }
            try
            {
                
                        cmd = new OleDbCommand("insert into " + lblOrigen.Text + "(Id,Nombre,PrecioVentaMayoreo,PrecioVenta,Existencia,Limite,Categoria,Especial,IVA,Unidad,Uni) values('" + txtID.Text + "','" + txtProducto.Text + "','" + Convert.ToDouble(txtCompra.Text) + "','" + Convert.ToDouble(txtVenta.Text) + "','" + txtCantidad.Text + "','" + txtLimite.Text + "','" + cmbCategoria.Text + "','" + txtEspecial.Text + "','" + comboBox1.Text + "','" + cmbUnidad.SelectedValue.ToString() + "','" + cmbUnidad.Text + "');", conectar);
                        cmd.ExecuteNonQuery();
                        cmd = new OleDbCommand("insert into Kardex (IdProducto,Tipo,Descripcion,ExistenciaAntes,ExistenciaDespues,Fecha) values('" + txtID.Text + "','ENTRADA','CREACION DE ARTICULO',0,'" + txtCantidad.Text + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "');", conectar);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Se ha agregado el producto con exito", "AGREGADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        groupBox1.Enabled = false;
                        txtProducto.Clear();
                        txtCompra.Clear();
                        txtVenta.Clear();
                        txtCantidad.Clear();
                        txtID.Clear();
                        txtPeso.Clear();
                        checkBox1.Checked = false;
                        txtKilataje.Clear();
                        txtMaquila.Clear();
                        cmbSub.Text = "";
                        comboBox1.Text = "";
                        txtID.Focus();
                        txtEspecial.Clear();
                        txtLimite.Clear();
                        cmbUnidad.Text = "";
                    
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Algun dato ingresado no es el correcto, favor de verificar" + ex.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void txtProducto_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.ToString() == "'")
            {

                txtProducto.SelectedText = "´";
                e.Handled = true;
            }
        }

        private void cmbSub_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void txtMaquila_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (cmbProveedor.Text == "GRUPO VILLALPANDO SA DE CV")
                {
                    //PrecioOro = 762;
                    Kilataje = Convert.ToDouble(txtKilataje.Text);
                    Resultado = entre24 * Kilataje;
                    Maquila = Convert.ToDouble(txtMaquila.Text);
                    Total = Resultado + Maquila;
                    Peso = Convert.ToDouble(txtPeso.Text);
                    PrecioCompra = Total * Peso;
                    txtCompra.Text = PrecioCompra + "";
                    Treinta = PrecioCompra * 1.3;
                    por2 = Treinta * 2;
                    Diez = por2 * 1.1;
                    txtVenta.Text = Diez + "";
                    //MessageBox.Show(entre24 + "\n" + Kilataje + "\n" + Resultado + "\n" + Maquila + "\n" + Total + "\n" + Peso + "\n" + PrecioCompra + "\n" + Treinta + "\n" + por2 + "\n" + Diez);
                }
                else
                {
                    PrecioCompra = Convert.ToDouble(txtCompra.Text);
                    por2 = PrecioCompra * 2;
                    Diez = por2 * 1.1;
                    txtVenta.Text = Diez+"";
                }
            }            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                txtMaquila.Text = "N/A";
                txtMaquila.Enabled = false;
            }
            else
            {
                txtMaquila.Text = "";
                txtMaquila.Enabled = true;
            }

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void label21_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtCompra_KeyPress(object sender, KeyPressEventArgs e)
        {
            CultureInfo cc = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (char.IsNumber(e.KeyChar) || e.KeyChar.ToString() == cc.NumberFormat.NumberDecimalSeparator || Convert.ToInt32(e.KeyChar) == 8)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void txtVenta_KeyPress(object sender, KeyPressEventArgs e)
        {
            CultureInfo cc = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (char.IsNumber(e.KeyChar) || e.KeyChar.ToString() == cc.NumberFormat.NumberDecimalSeparator || Convert.ToInt32(e.KeyChar) == 8)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            CultureInfo cc = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (char.IsNumber(e.KeyChar) || e.KeyChar.ToString() == cc.NumberFormat.NumberDecimalSeparator || Convert.ToInt32(e.KeyChar) == 8)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void txtLimite_KeyPress(object sender, KeyPressEventArgs e)
        {
            CultureInfo cc = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (char.IsNumber(e.KeyChar) || e.KeyChar.ToString() == cc.NumberFormat.NumberDecimalSeparator || Convert.ToInt32(e.KeyChar) == 8)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void txtEspecial_KeyPress(object sender, KeyPressEventArgs e)
        {
            CultureInfo cc = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (char.IsNumber(e.KeyChar) || e.KeyChar.ToString() == cc.NumberFormat.NumberDecimalSeparator || Convert.ToInt32(e.KeyChar) == 8)
                e.Handled = false;
            else
                e.Handled = true;
        }
    }
}
