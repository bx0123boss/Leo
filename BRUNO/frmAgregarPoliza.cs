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
    public partial class frmAgregarPoliza : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbCommand cmd;
        double totalCostos = 0;
        double costosExtra;
        string idProv = "0", nombreProv="";
        public frmAgregarPoliza()
        {
            InitializeComponent();
        }

        private void txtID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                SendKeys.Send("+{TAB}");
            }
        }

        private void txtID_Leave(object sender, EventArgs e)
        {
            cmd = new OleDbCommand("select count(*) from Inventario where Id='" + txtID.Text + "';", conectar);
            int valor = int.Parse(cmd.ExecuteScalar().ToString());
            if (valor == 1)
            {

                cmd = new OleDbCommand("select * from Inventario where Id='" + txtID.Text + "';", conectar);
                OleDbDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txtNombre.Text = Convert.ToString(reader[1].ToString());
                    txtMenudeo.Text = Convert.ToString(reader[2].ToString());
                }
                txtCantidad.Focus();
            }
            else
            {
                txtNombre.Focus();
            }
        }

        private void frmAgregarPoliza_Load(object sender, EventArgs e)
        {
            conectar.Open();
            rdContado.Checked = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            double iva = 0;
            totalCostos = 0;
            if (textBox1.Text=="")
            {
                textBox1.Text = "0";
            }
             costosExtra = Convert.ToDouble(textBox1.Text);

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                iva += Convert.ToDouble(dataGridView1[6, i].Value.ToString()) * Convert.ToDouble(dataGridView1[2, i].Value.ToString());
                totalCostos += Convert.ToDouble(dataGridView1[3, i].Value.ToString()) * Convert.ToDouble(dataGridView1[2, i].Value.ToString());
            }
            double porcentaje = costosExtra / totalCostos;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                double costo = Convert.ToDouble(dataGridView1[3, i].Value.ToString());
                double iv = Convert.ToDouble(dataGridView1[6, i].Value.ToString());
                double costoE = costo * porcentaje;
                double costoR = costo + costoE+ iv;
                dataGridView1[4, i].Value = Math.Round(costoE, 3);
                dataGridView1[5, i].Value = Math.Round(costoR, 2);

            }
            //MessageBox.Show("Total: " + totalCostos);
            lblTotal.Text = "" + (totalCostos + costosExtra).ToString("#,#.00", CultureInfo.InvariantCulture);
            lblIVA.Text = "" + (iva).ToString("#,#.00", CultureInfo.InvariantCulture);
            lblTotalSi.Text = "" + (totalCostos + costosExtra + iva).ToString("#,#.00", CultureInfo.InvariantCulture);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (lblTotal.Text == "0.00" || lblTotal.Text == ".00")
            {
                MessageBox.Show("Debe calcular antes de guardar la compra", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    cmd = new OleDbCommand("insert into productosPoliza Values('" + dataGridView1[0, i].Value + "','" + dataGridView1[1, i].Value + "','" + dataGridView1[2, i].Value + "','" + dataGridView1[3, i].Value + "','" + dataGridView1[4, i].Value + "','" + dataGridView1[5, i].Value + "','" + dataGridView1[6, i].Value + "','" + txtFolio.Text + "');", conectar);
                    cmd.ExecuteNonQuery();

                    cmd = new OleDbCommand("select count(*) from Inventario where Id='" + dataGridView1[0, i].Value + "';", conectar);
                    int valor = int.Parse(cmd.ExecuteScalar().ToString());
                    if (valor == 1)
                    {
                        cmd = new OleDbCommand("select * from Inventario where Id='" + dataGridView1[0, i].Value + "';", conectar);
                        OleDbDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            double actuales = Convert.ToDouble(reader[4].ToString());
                            double nuevas = actuales + Convert.ToDouble(dataGridView1[2, i].Value);
                            double precio = Convert.ToDouble(dataGridView1[5, i].Value);
                            if (Convert.ToDouble(dataGridView1[5, i].Value) > precio)
                            {
                                precio = Convert.ToDouble(dataGridView1[5, i].Value);
                            }
                            cmd = new OleDbCommand("UPDATE Inventario set Especial='" + precio + "', Existencia='" + nuevas + "', PrecioVenta='" + dataGridView1[7, i].Value + "' Where Id='" + dataGridView1[0, i].Value + "';", conectar);
                            cmd.ExecuteNonQuery();
                            cmd = new OleDbCommand("insert into Kardex (IdProducto,Tipo,Descripcion,ExistenciaAntes,ExistenciaDespues,Fecha,Precio) values('" + dataGridView1[0, i].Value + "','ENTRADA','COMPRA DE ARTICULO FOLIO: " + txtFolio.Text + "'," + actuales + ",'" + nuevas + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + dataGridView1[7, i].Value + "');", conectar);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        cmd = new OleDbCommand("insert into Inventario values('" + dataGridView1[0, i].Value + "','" + dataGridView1[1, i].Value + "','" + dataGridView1[06, i].Value + "','" + dataGridView1[5, i].Value + "','" + dataGridView1[2, i].Value + "','0');", conectar);
                        cmd.ExecuteNonQuery();
                        cmd = new OleDbCommand("insert into Kardex (IdProducto,Tipo,Descripcion,ExistenciaAntes,ExistenciaDespues,Fecha,idProveedor,Proveedor) values('" + dataGridView1[0, i].Value + "','ENTRADA','COMPRA DE ARTICULO FOLIO: " + txtFolio.Text + "',0,'" + dataGridView1[2, i].Value + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "');", conectar);
                        cmd.ExecuteNonQuery();
                    }

                }
                if (rdCredito.Checked)
                {
                    cmd = new OleDbCommand("select * from Proveedores where Id=" + idProv+ ";", conectar);
                        OleDbDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            double adeudoActual = Convert.ToDouble(reader[8].ToString());
                            adeudoActual = adeudoActual + (totalCostos + costosExtra);
                            cmd = new OleDbCommand("UPDATE Proveedores set Adeudo='" + adeudoActual + "' Where Id=" + idProv+ ";", conectar);
                            cmd.ExecuteNonQuery();
                        }
                }
                cmd = new OleDbCommand("insert into Poliza(Folio, Fecha, FechaCaptura, CostoTotal, CostoExtra,IdProv,Proveedor,IVA, Total) Values('" + txtFolio.Text + "','" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 00:00:00','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "', '" + lblTotal.Text + "','" + textBox1.Text + "','" + idProv + "','" + nombreProv + "','"+lblIVA.Text+"','"+lblTotalSi.Text+"');", conectar);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Se han añadido la poliza Correctamente", "Correcto", MessageBoxButtons.OK, MessageBoxIcon.Information);
                frmPolizas inv = new frmPolizas();
                inv.Show();
                this.Close();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (frmBuscarProductos buscar = new frmBuscarProductos())
            {
                buscar.poliza = -1;
                if (buscar.ShowDialog() == DialogResult.OK)
                {
                    txtID.Text = buscar.ID;
                    txtNombre.Text = buscar.producto;
                    txtCantidad.Focus();
                    txtMenudeo.Text = buscar.precio;
                    //dataGridView1.Rows.Add("1", buscar.producto, buscar.precio, buscar.monto, buscar.existencia, buscar.ID, "");

                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtCantidad.Text == "0" || txtCantidad.Text == "")
            {
                MessageBox.Show("INTRODUZCA UNA CANTIDAD VALIDA", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCantidad.Clear();
                txtCantidad.Focus();
            }
            else
            {
                dataGridView1.Rows.Add(txtID.Text, txtNombre.Text, txtCantidad.Text, txtCosto.Text, "0", "0", txtVenta.Text,txtMenudeo.Text);
                txtID.Clear();
                txtNombre.Clear();
                txtCantidad.Text = "0";
                txtCosto.Clear();
                txtVenta.Clear();
                txtMenudeo.Clear();
                txtID.Focus();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (frmBuscarProveedor buscar = new frmBuscarProveedor())
            {
                if (buscar.ShowDialog() == DialogResult.OK)
                {
                    nombreProv= buscar.Nombre;
                    idProv= buscar.ID;
                    lblProveedor.Text = nombreProv;
                    //dataGridView1.Rows.Add("1", buscar.producto, buscar.precio, buscar.monto, buscar.existencia, buscar.ID, "");

                }

            }
        }

        private void txtCosto_Leave(object sender, EventArgs e)
        {
            if (txtCosto.Text != null && txtCosto.Text!="")
            {

                double costo = Convert.ToDouble(txtCosto.Text);
                double iva = costo * 0.16;
                txtVenta.Text = Math.Round(iva, 2).ToString();
            }
            else
            {
                txtCosto.Text = "0";
                double costo = Convert.ToDouble(txtCosto.Text);
                double iva = costo * 0.16;
                txtVenta.Text = Math.Round(iva, 2).ToString();
            }
            
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            CultureInfo cc = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (char.IsNumber(e.KeyChar) || e.KeyChar.ToString() == cc.NumberFormat.NumberDecimalSeparator || Convert.ToInt32(e.KeyChar) == 8)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            CultureInfo cc = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (char.IsNumber(e.KeyChar) || e.KeyChar.ToString() == cc.NumberFormat.NumberDecimalSeparator || Convert.ToInt32(e.KeyChar) == 8)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void txtCosto_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtCosto_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtMenudeo_KeyPress(object sender, KeyPressEventArgs e)
        {
            CultureInfo cc = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (char.IsNumber(e.KeyChar) || e.KeyChar.ToString() == cc.NumberFormat.NumberDecimalSeparator || Convert.ToInt32(e.KeyChar) == 8)
                e.Handled = false;
            else
                e.Handled = true;
        }
    }
}
