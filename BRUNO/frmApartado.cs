using LibPrintTicket;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmApartado : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbCommand cmd;
        double total = 0;
        string origen = "";
        double exis = 0.0;
        public string direccion = "", tel = "", correo = "";
        public string idCliente = "0";
        double descuento;
        public string usuario = "";
        double existencia = 0;
        int foli;

        public frmApartado()
        {
            InitializeComponent();
        }

        private void frmApartado_Load(object sender, EventArgs e)
        {
            conectar.Open();
            cmd = new OleDbCommand("select Numero from Folios where Folio='FolioApartado';", conectar);
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                foli = Convert.ToInt32(Convert.ToString(reader[0].ToString()));
            }
            cmbPago.SelectedIndex = 0;
            lblFolio.Text = "A" + String.Format("{0:0000}", foli);
            if (Conexion.lugar == "SANJUAN" && usuario=="Admin")
            {
                dataGridView1.Columns[2].ReadOnly = false;
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
                    direccion = cliente.direccion;
                    tel = cliente.tel;
                    correo = cliente.correo;
                    //lblAdeudo.Text = String.Format("{0:0.00}", cliente.Adeudo);
                    //lblLimite.Text = cliente.Limite;
                }

                cliente.Show();
            }
        }
        

        private void label1_Click(object sender, EventArgs e)
        {
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (frmBuscarProductos buscar = new frmBuscarProductos())
            {
                if (buscar.ShowDialog() == DialogResult.OK)
                {
                    dataGridView1.Rows.Add("1", buscar.producto, buscar.precio, buscar.monto, buscar.existencia, buscar.ID, origen);

                }
            }
            total = 0;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                total += Convert.ToDouble(dataGridView1[3, i].Value.ToString());
            }
            lblTotal.Text = String.Format("{0:0.00}", total);
            lblFinal.Text = String.Format("{0:0.00}", total);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (e.KeyChar == Convert.ToChar(Keys.Enter))
            //{
            //    if (textBox1.Text == "")
            //    {
            //    }
            //    else
            //    {

            //        cmd = new OleDbCommand("select count(*) from Inventario where Id='" + textBox1.Text + "';", conectar);
            //        int valor = int.Parse(cmd.ExecuteScalar().ToString());
            //        if (valor == 1)
            //        {
            //            total = Convert.ToDouble(lblTotal.Text);
            //            cmd = new OleDbCommand("select * from Inventario where Id='" + textBox1.Text + "';", conectar);
            //            OleDbDataReader reader = cmd.ExecuteReader();
            //            if (reader.Read())
            //            {
            //                //int existen = Convert.ToInt32(Convert.ToString(reader[4].ToString()));
            //                //if (existen > 0)
            //                //{
            //                    dataGridView1.Rows.Add("1", Convert.ToString(reader[1].ToString()), Convert.ToString(reader[3].ToString()), Convert.ToString(reader[3].ToString()), Convert.ToString(reader[4].ToString()), Convert.ToString(reader[0].ToString()), origen);
            //                //}
            //                //else
            //                //{
            //                //  MessageBox.Show("El producto no cuenta con existencias, verifique el almacen", "Alto", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //                //}
            //            }
            //            total += Convert.ToDouble(Convert.ToString(reader[3].ToString()));
            //            lblTotal.Text = String.Format("{0:0.00}", total);
            //            textBox1.Text = "";
            //        }
            //        else
            //        {
            //            MessageBox.Show("El producto no existe, verifique el almacen", "Alto", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            textBox1.Clear();
            //            textBox1.Focus();
            //        }
            //    }
            //}

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (textBox1.Text == "")
                {
                }
                else
                {

                    cmd = new OleDbCommand("select count(*) from Inventario where Id='" + textBox1.Text + "';", conectar);
                    int valor = int.Parse(cmd.ExecuteScalar().ToString());
                    if (valor == 1)
                    {
                        total = Convert.ToDouble(lblTotal.Text);
                        cmd = new OleDbCommand("select * from Inventario where Id='" + textBox1.Text + "';", conectar);
                        OleDbDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            //int existen = Convert.ToInt32(Convert.ToString(reader[4].ToString()));
                            //if (existen > 0)
                            //{
                            using (frmPrecio buscar = new frmPrecio())
                            {
                                if (buscar.ShowDialog() == DialogResult.OK)
                                {
                                    if (buscar.tipo == "GEN")
                                    {
                                        double preci = Convert.ToDouble(reader[3].ToString());
                                        dataGridView1.Rows.Add("1", Convert.ToString(reader[1].ToString()), String.Format("{0:0.00}", preci), String.Format("{0:0.00}", preci), Convert.ToString(reader[4].ToString()), Convert.ToString(reader[0].ToString()), origen, Convert.ToString(reader[8].ToString()), Convert.ToString(reader[7].ToString()));
                                    }
                                    else
                                    {
                                        double preci = Convert.ToDouble(reader[2].ToString());
                                        dataGridView1.Rows.Add("1", Convert.ToString(reader[1].ToString()), String.Format("{0:0.00}", preci), String.Format("{0:0.00}", preci), Convert.ToString(reader[4].ToString()), Convert.ToString(reader[0].ToString()), origen, Convert.ToString(reader[8].ToString()), Convert.ToString(reader[7].ToString()));
                                    }
                                }
                            }
                            //}
                            //else
                            //{
                            //    MessageBox.Show("El producto no cuenta con existencias, verifique el almacen", "Alto", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //}
                        }
                        total += Convert.ToDouble(Convert.ToString(reader[3].ToString()));
                        lblTotal.Text = String.Format("{0:0.00}", total);
                        textBox1.Text = "";
                    }
                    else
                    {

                        using (frmBuscarProductos buscar = new frmBuscarProductos())
                        {
                            buscar.textBox1.Text = textBox1.Text;
                            if (buscar.ShowDialog() == DialogResult.OK)
                            {
                                dataGridView1.Rows.Add("1", buscar.producto, buscar.precio, buscar.monto, buscar.existencia, buscar.ID, origen, buscar.IVA, buscar.compra);

                            }
                        }
                        total = 0;
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            total += Convert.ToDouble(dataGridView1[3, i].Value.ToString());
                        }
                        lblTotal.Text = String.Format("{0:0.00}", total);
                        textBox1.Text = "";
                        //}
                        //}
                    }
                }
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            txtDescuento.Focus();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            txtDescuento.Focus();
        }

        private void txtDescuento_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (radioButton1.Checked)
                {
                    total = 0;
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        total += Convert.ToDouble(dataGridView1[3, i].Value.ToString());
                    }
                    descuento = ((Convert.ToDouble(txtDescuento.Text) / 100)) * total;
                    total = total - descuento;
                    MessageBox.Show("DESCUENTO REALIZADO POR LA CANTIDAD DE: $" + descuento, "DESCUENTO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblTotal.Text = String.Format("{0:0.00}", total);
                    lblFinal.Text = String.Format("{0:0.00}", total);
                }
                else if (radioButton2.Checked)
                {
                    total = 0;
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        total += Convert.ToDouble(dataGridView1[3, i].Value.ToString());
                    }
                    descuento = Convert.ToDouble(txtDescuento.Text);
                    total = total - descuento;
                    MessageBox.Show("DESCUENTO REALIZADO POR LA CANTIDAD DE: $" + descuento, "DESCUENTO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblTotal.Text = String.Format("{0:0.00}", total);
                    lblFinal.Text = String.Format("{0:0.00}", total);
                }
                txtAbono.Focus();
            }
            else
            {
                CultureInfo cc = System.Threading.Thread.CurrentThread.CurrentCulture;
                if (char.IsNumber(e.KeyChar) || e.KeyChar.ToString() == cc.NumberFormat.NumberDecimalSeparator || Convert.ToInt32(e.KeyChar) == 8)
                    e.Handled = false;
                else
                    e.Handled = true;
            }
        }

        private void txtAbono_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (txtAbono.Text == "")
                {
                    txtAbono.Text = "0";
                }
                lblFinal.Text = "" + (Convert.ToDouble(lblTotal.Text) - float.Parse(txtAbono.Text, System.Globalization.CultureInfo.InvariantCulture));
                button2.Focus();
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            total = 0;
            double cantidad = Convert.ToDouble(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString());
            double precio = Convert.ToDouble(dataGridView1[2, dataGridView1.CurrentRow.Index].Value.ToString());
            double monto = cantidad * precio;
            dataGridView1.Rows[e.RowIndex].Cells[3].Value = monto;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                total += Convert.ToDouble(dataGridView1[3, i].Value.ToString());
            }
            lblTotal.Text = String.Format("{0:0.00}", total);
            lblFinal.Text = String.Format("{0:0.00}", total);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (lblCliente.Text == "NO SELECCIONADO")
            {
                MessageBox.Show("Ingrese el cliente para poder hacer la venta", "Alto!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
                {

                    //int existencia = 0;
                    //bool mamo = false;
                    //string productos = "En los siguientes productos se encuentra un error al vender las existencias: ";
                    //for (int i = 0; i < dataGridView1.RowCount; i++)
                    //{
                    //    existencia = Convert.ToInt32(dataGridView1[4, i].Value.ToString()) - Convert.ToInt32(dataGridView1[0, i].Value.ToString());
                    //    if (existencia < 0)
                    //    {
                    //        mamo = true;
                    //        productos = productos + "\n" + dataGridView1[1, i].Value.ToString();
                    //    }
                    //}
                    //if (mamo)
                    //{
                    //    MessageBox.Show(productos + "\nVerifique sus almacenes", "Alto", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //}
                    //else
                    //{

                    Ticket ticket = new Ticket();
                    ticket.MaxChar = Conexion.MaxChar;
                    ticket.FontSize = Conexion.FontSize;
                    ticket.MaxCharDescription = Conexion.MaxCharDescription;
                    ticket.HeaderImage = Image.FromFile("C:\\Jaeger Soft\\logo.jpg");
                    ticket.AddHeaderLine("*******  NOTA DE APARTADO  ******");
                    //jalar datos de ticket
                    for (int i = 0; i < Conexion.datosTicket.Length; i++)
                    {
                        ticket.AddHeaderLine(Conexion.datosTicket[i]);
                    }
                    ticket.AddHeaderLine("METODO DE PAGO:");
                    ticket.AddHeaderLine(cmbPago.Text);
                    ticket.AddHeaderLine("FOLIO DE APARTADO: " + lblFolio.Text);
                    ticket.AddHeaderLine("CLIENTE: " + lblCliente.Text);
                    ticket.AddSubHeaderLine("FECHA Y HORA:");
                    ticket.AddSubHeaderLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                        int totalgrid = dataGridView1.RowCount;
                        existencia = 0;
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            string unidad = "0";
                            //Obtenemos existencias del articulo
                            cmd = new OleDbCommand("select * from Inventario where Id='" + dataGridView1[5, i].Value.ToString() + "';", conectar);
                            OleDbDataReader reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                exis = Convert.ToDouble(Convert.ToString(reader[4].ToString()));
                                unidad = Convert.ToString(reader[9].ToString());
                                if (unidad == "")
                                {
                                    unidad = "0";
                                }

                            }

                            cmd = new OleDbCommand("select * from Unidades where Id=" + unidad + ";", conectar);
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                unidad = Convert.ToString(reader[2].ToString());
                            }
                            else
                                unidad = "";   
                            //Obtenemos existencias del articulo
                            existencia = exis - Convert.ToDouble(dataGridView1[0, i].Value.ToString());
                            if (existencia < 0)
                            {
                                cmd = new OleDbCommand("insert into VentasApartados(FolioVenta,idProducto,Cantidad,Producto,MontoTotal,IdCliente,Fecha,Estatus) values('" + lblFolio.Text + "','" + dataGridView1[5, i].Value.ToString() + "','" + dataGridView1[0, i].Value.ToString() + "','" + dataGridView1[1, i].Value.ToString() + "','" + dataGridView1[3, i].Value.ToString() + "','" + idCliente + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','PENDIENTE');", conectar);
                                cmd.ExecuteNonQuery();
                            }
                            else
                            {

                                //Actualizamos existencias
                                cmd = new OleDbCommand("UPDATE Inventario set Existencia='" + existencia + "' Where Id='" + dataGridView1[5, i].Value.ToString() + "';", conectar);
                                cmd.ExecuteNonQuery();
                                //Insertamos en la venta apartados
                                cmd = new OleDbCommand("insert into VentasApartados(FolioVenta,idProducto,Cantidad,Producto,MontoTotal,IdCliente,Fecha,Estatus) values('" + lblFolio.Text + "','" + dataGridView1[5, i].Value.ToString() + "','" + dataGridView1[0, i].Value.ToString() + "','" + dataGridView1[1, i].Value.ToString() + "','" + dataGridView1[3, i].Value.ToString() + "','" + idCliente + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','DESCONTADO DE INVENTARIO');", conectar);
                                cmd.ExecuteNonQuery();

                                cmd = new OleDbCommand("insert into Kardex (IdProducto,Tipo,Descripcion,ExistenciaAntes,ExistenciaDespues,Fecha) values('" + dataGridView1[5, i].Value.ToString() + "','SALIDA','APARTADO DE ARTICULO FOLIO: " + lblFolio.Text + "'," + exis + ",'" + existencia + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "');", conectar);
                                cmd.ExecuteNonQuery();

                                ticket.AddItem(dataGridView1[0, i].Value.ToString() + " " + unidad, dataGridView1[1, i].Value.ToString(), dataGridView1[3, i].Value.ToString());
                                //MessageBox.Show("Se vendera el numero:"+i+"\nCantidad: "+dataGridView1[0, i].Value.ToString()+"\nProducto: "+ dataGridView1[1, i].Value.ToString()+"\nPrecio: "+dataGridView1[2, i].Value.ToString()+"\nMonto :" + dataGridView1[3, i].Value.ToString() +"\nExistencias :"+ dataGridView1[4, i].Value.ToString()+"\nID :"+dataGridView1[5, i].Value.ToString());

                            }
                        }
                        total = total + descuento;
                        cmd = new OleDbCommand("insert into Apartados (MontoTotal,Fecha,Folio,IdCliente,NombreCliente,Abono,Restante) values('" + total + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + lblFolio.Text + "','" + idCliente + "','"+lblCliente.Text+"','"+txtAbono.Text+"','"+lblFinal.Text+"');", conectar);
                        cmd.ExecuteNonQuery();
                        cmd = new OleDbCommand("insert into Corte(Concepto,Monto,FechaHora,Pago) Values('Abono del apartado con folio " + lblFolio.Text + "','" + txtAbono.Text + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + cmbPago.Text + "');", conectar);
                        cmd.ExecuteNonQuery();
                        double subtotal = total - descuento;  

                        ticket.AddTotal("Suma", total + "");
                        ticket.AddTotal("Descuento", descuento + "");
                        ticket.AddTotal("SubTotal", Math.Round(subtotal / 1.16, 2) + "");
                        ticket.AddTotal("I.V.A.", Math.Round(subtotal - (subtotal / 1.16) ,2) + "");
                        ticket.AddTotal("Total", (total - descuento) + "");
                        ticket.AddTotal("Abono", txtAbono.Text);
                        ticket.AddTotal("Restante", lblFinal.Text);
                        //jalar pie de ticket
                        for (int i = 0; i < Conexion.pieDeTicket.Length; i++)
                        {
                            ticket.AddFooterLine(Conexion.pieDeTicket[i]);
                        }    
                        foli = foli + 1;
                        cmd = new OleDbCommand("UPDATE Folios set Numero=" + foli + " where Folio='FolioApartado';", conectar);
                        cmd.ExecuteNonQuery();
                        //ticket.PrintTicket(Conexion.impresora);
                        MessageBox.Show("Apartado realizada con exito", "APARTADO REALIZADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        frmApartado credito = new frmApartado();
                        credito.Show();
                        this.Close();

                    }
                //}
            }

        private void button1_Click(object sender, EventArgs e)
        {
            total = 0;
            dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                total += Convert.ToDouble(dataGridView1[3, i].Value.ToString());
            }
            lblTotal.Text = String.Format("{0:0.00}", total);
            lblFinal.Text = String.Format("{0:0.00}", total);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

