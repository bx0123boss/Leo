using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using LibPrintTicket;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using System.Drawing.Printing;
using System.Globalization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace BRUNO
{
    public partial class frmVentas : Form
    {
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        OleDbCommand cmd;
        double total = 0;
        double iva;
        string origen = "";
        double exis = 0.0;
        string idCliente = "0";
        double descuento;
        int foli;
        string direccion = "";
        public string usuario = "", idUsuario="";
        public frmVentas()
        {
            InitializeComponent();
            conectar.Open();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            using (frmBuscarProductos buscar = new frmBuscarProductos())
            {
                if (buscar.ShowDialog() == DialogResult.OK)
                {
                    dataGridView1.Rows.Add("1", buscar.producto, buscar.precio, buscar.monto, buscar.existencia, buscar.ID, origen,buscar.IVA,buscar.compra);

                }
            }
            total = 0;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                total += Convert.ToDouble(dataGridView1[3, i].Value.ToString());
            }
            lblTotal.Text = String.Format("{0:0.00}", total);
            
            
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
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
                                        dataGridView1.Rows.Add("1", Convert.ToString(reader[1].ToString()), String.Format("{0:0.00}", preci), String.Format("{0:0.00}", preci), Convert.ToString(reader[4].ToString()), Convert.ToString(reader[0].ToString()), origen, Convert.ToString(reader[8].ToString()), Convert.ToString(reader[7].ToString()),"");
                                    }
                                    else
                                    {
                                        double preci = Convert.ToDouble(reader[2].ToString());
                                        dataGridView1.Rows.Add("1", Convert.ToString(reader[1].ToString()), String.Format("{0:0.00}", preci), String.Format("{0:0.00}", preci), Convert.ToString(reader[4].ToString()), Convert.ToString(reader[0].ToString()), origen, Convert.ToString(reader[8].ToString()), Convert.ToString(reader[7].ToString()),"");
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
                                dataGridView1.Rows.Add("1", buscar.producto, buscar.precio, buscar.monto, buscar.existencia, buscar.ID, origen, buscar.IVA, buscar.compra,"");

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

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                total = 0;
                dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    total += Convert.ToDouble(dataGridView1[3, i].Value.ToString());
                }
                lblTotal.Text = String.Format("{0:0.00}", total);
                textBox1.Text = "";
            }
            catch (Exception Ex)
            {
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            total = 0;
            try
            {
                double cantidad = Convert.ToDouble(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString());
                double precio = Convert.ToDouble(dataGridView1[2, dataGridView1.CurrentRow.Index].Value.ToString());
                double monto = cantidad * precio;
                dataGridView1.Rows[e.RowIndex].Cells[3].Value = String.Format("{0:0.00}", monto);
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    total += Convert.ToDouble(dataGridView1[3, i].Value.ToString());
                }
                lblTotal.Text = String.Format("{0:0.00}", total);
                textBox1.Focus();
            }
            catch 
            {
                MessageBox.Show("Solo puedes introducir números", "Alto", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataGridView1.Rows[e.RowIndex].Cells[0].Value = "1";
                double cantidad = 1;
                double precio = Convert.ToDouble(dataGridView1[2, dataGridView1.CurrentRow.Index].Value.ToString());
                double monto = cantidad * precio;
                dataGridView1.Rows[e.RowIndex].Cells[3].Value = String.Format("{0:0.00}", monto);
                total = 0;
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    total += Convert.ToDouble(dataGridView1[3, i].Value.ToString());
                }
                lblTotal.Text = String.Format("{0:0.00}", total);
                textBox1.Focus();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Venta();
        }

        private void frmVentas_Load(object sender, EventArgs e)
        {
            
            cmbPago.SelectedIndex = 0;
            if (Conexion.lugar == "LEO")
            {
                dataGridView1.Columns[2].ReadOnly = false;
            }
            else if (Conexion.lugar == "SANJUAN" && usuario=="Admin")
            {
                dataGridView1.Columns[2].ReadOnly = false;
            }
        }
       

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            txtDescuento.Focus();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            txtDescuento.Focus();
        }

        private void txtDescuento_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter))
            {
                MessageBox.Show("Solo se permiten numeros", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
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
                    total =Math.Round(total - descuento,2);
                    MessageBox.Show("DESCUENTO REALIZADO POR LA CANTIDAD DE: $"+descuento,"DESCUENTO",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    lblTotal.Text = String.Format("{0:0.00}", total);
                }
                else if (radioButton2.Checked)
                {
                    total = 0;
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        total += Convert.ToDouble(dataGridView1[3, i].Value.ToString());
                    }
                    descuento=Convert.ToDouble(txtDescuento.Text);
                    total = total - descuento ;
                    MessageBox.Show("DESCUENTO REALIZADO POR LA CANTIDAD DE: $" + descuento, "DESCUENTO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblTotal.Text = String.Format("{0:0.00}", total);
                }
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
                }

                cliente.Show();
            }
        }

        static double CalcularImpuesto(double baseValue, double percentage)
        {
            return baseValue * (percentage / 100.0);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                using (frmCredito ori = new frmCredito())
                {
                    if (ori.ShowDialog() == DialogResult.OK)
                    {
                        iva = ori.iva;
                        double lol = total;
                        // Cálculo del impuesto
                        double impuesto = CalcularImpuesto(lol, iva);

                        // Valor total después del impuesto
                        double valorConImpuesto = lol + impuesto;
                        lol = Math.Round(valorConImpuesto, 0);
                        total = lol;
                        lblTotal.Text = total.ToString("N2");

                        checkBox1.Checked = true;
                        cmbPago.Items.Clear();
                        cmbPago.Items.Add("04=TARJETA DE CREDITO");
                        cmbPago.Items.Add("28=TARJETA DE DEBITO");
                        cmbPago.SelectedIndex = 0;
                    }

                }

            }
            else
            {
                // Lista de opciones para el ComboBox
                string[] opcionesPago = {
                    "01=EFECTIVO",
                    "02=CHEQUE NOMINATIVO",
                    "03=TRANFERENCIA ELECTRONICA DE FONDOS",
                    "05=MONEDERO ELECTRONICO",
                    "06=DINERO ELECTRONICO",
                    "08=VALES DE DESPENSA",
                    "12=DACION EN PAGO",
                    "13=PAGO POR SUBROGACION",
                    "14=PAGO POR CONSIGNACION",
                    "15=CONDONACION",
                    "17=COMPENSACION",
                    "23=NOVACION",
                    "24=CONFUSION",
                    "25=REMISION DE DEUDA",
                    "26=PRESCRIPCION O CADUCIDAD",
                    "27=A SATISFACCION DEL ACREEDOR",
                    "29=TARJETA DE SERVICIOS",
                    "30=APLICACION DE ANTICIPOS",
                    "31=INTERMEDIARIO PAGOS",
                    "99=POR DEFINIR"
                };
                cmbPago.Items.Clear();
                cmbPago.Items.AddRange(opcionesPago);
                if (cmbPago.Items.Count > 0)
                {
                    cmbPago.SelectedIndex = 0;
                }
                total = 0;
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    total += Convert.ToDouble(dataGridView1[3, i].Value.ToString());
                }
                lblTotal.Text = String.Format("{0:0.00}", total);
                checkBox1.Checked = false;
            }
        }
        public void Venta() 
        {
            cmd = new OleDbCommand("select Numero from Folios where Folio='FolioContado';", conectar);
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                foli = Convert.ToInt32(Convert.ToString(reader[0].ToString()));
            }
            lblFolio.Text = "VR" + String.Format("{0:0000}", foli);
            lblFolio.Visible = true;
            label2.Visible = true;
            double IVA = 0;
            double efectivo = 0, cambio = 0;
            double existencia = 0;
                using (frmPago ori = new frmPago())
                {

                    ori.txtTotal.Text = "" + String.Format("{0:0.00}", total);
                    if (ori.ShowDialog() == DialogResult.OK)
                    {
                        efectivo = ori.efectivo;
                        cambio = ori.cambio;
                    }
                }
            /*
                printDocument1.PrinterSettings.DefaultPageSettings.PaperSize = new PaperSize("", width, height);
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += new PrintPageEventHandler(this.printDocument1_PrintPage_1);
                PrintDialog printdlg = new PrintDialog();
                PrintPreviewDialog printPrvDlg = new PrintPreviewDialog();
                // preview the assigned document or you can create a different previewButton for it
                printPrvDlg.Document = pd;
                printdlg.Document = pd;
                pd.Print(); 
            */
                Ticket ticket = new Ticket();
                ticket.MaxChar = Conexion.MaxChar;
                ticket.FontSize = Conexion.FontSize;
                ticket.MaxCharDescription = Conexion.MaxCharDescription;
                if (Conexion.Font == "")
                {

                }
                else
                    ticket.FontName = Conexion.Font;
                //ticket.HeaderImage = Image.FromFile("C:\\Jaeger Soft\\logo.jpg");
                //jalar datos de ticket
                for (int i = 0; i < Conexion.datosTicket.Length; i++)
                {
                    ticket.AddHeaderLine(Conexion.datosTicket[i]);
                }
                ticket.AddHeaderLine("METODO DE PAGO:");
                ticket.AddHeaderLine(cmbPago.Text);                
                ticket.AddHeaderLine("FOLIO DE VENTA A CONTADO:");
                ticket.AddHeaderLine(lblFolio.Text);
                ticket.AddHeaderLine("CLIENTE: " + lblCliente.Text);
                ticket.AddHeaderLine("DIRECCION: " + direccion);
                ticket.AddSubHeaderLine("FECHA Y HORA:");
                ticket.AddSubHeaderLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                existencia = 0;
            //sacar total de utilidad
            double totalUtilidad = 0;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                double venta = Convert.ToDouble(dataGridView1[2, i].Value.ToString()) * Convert.ToDouble(dataGridView1[0, i].Value.ToString());
                double compra = Convert.ToDouble(dataGridView1[8, i].Value.ToString()) * Convert.ToDouble(dataGridView1[0, i].Value.ToString());
                double utilidad = venta - compra;

                totalUtilidad += utilidad; // Suma a la utilidad total
            }
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    string unidad = "0";
                    //Obtenemos existencias del articulo
                    cmd = new OleDbCommand("select * from Inventario where Id='" + dataGridView1[5, i].Value.ToString() + "';", conectar);
                    reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        exis = Convert.ToDouble(Convert.ToString(reader[4].ToString()));
                        unidad = Convert.ToString(reader[9].ToString());
                        unidad = "pz";
                        if (unidad=="")
                        {
                            unidad = "0";
                        }
                        
                    }      
                    existencia = exis - Convert.ToDouble(dataGridView1[0, i].Value.ToString());
                    //Actualizamos existencias
                    cmd = new OleDbCommand("UPDATE Inventario set Existencia='" + existencia + "' Where Id='" + dataGridView1[5, i].Value.ToString() + "';", conectar);
                    cmd.ExecuteNonQuery();
                    cmd = new OleDbCommand("insert into Kardex (IdProducto,Tipo,Descripcion,ExistenciaAntes,ExistenciaDespues,Fecha) values('" + dataGridView1[5, i].Value.ToString() + "','SALIDA','VENTA DE ARTICULO FOLIO: " + lblFolio.Text + "'," + exis + ",'" + existencia + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "');", conectar);
                    cmd.ExecuteNonQuery();
                    //Insertamos en la venta a credito
                    double venta = Convert.ToDouble(dataGridView1[2, i].Value.ToString()) * Convert.ToDouble(dataGridView1[0, i].Value.ToString());
                    double compra = Convert.ToDouble(dataGridView1[8, i].Value.ToString()) * Convert.ToDouble(dataGridView1[0, i].Value.ToString());
                    double utilidad = venta - compra;
                    // Calcula el descuento proporcional para este producto
                    double descuentoProporcional = (utilidad / totalUtilidad) * descuento;
                double nuevaUtilidad = utilidad - descuentoProporcional;
                //MessageBox.Show("Utilidad" + nuevaUtilidad);
                cmd = new OleDbCommand("insert into VentasContado(FolioVenta,IdProducto,Cantidad,Producto,MontoTotal,idCliente,Fecha,Utilidad) values('" + lblFolio.Text + "','" + dataGridView1[5, i].Value.ToString() + "','" + dataGridView1[0, i].Value.ToString() + "','" + dataGridView1[1, i].Value.ToString() + "','" + dataGridView1[3, i].Value.ToString() + "','" + idCliente + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','"+ nuevaUtilidad + "');", conectar);
                    cmd.ExecuteNonQuery();
                    string precio = "" + Math.Round(Convert.ToDouble(dataGridView1[3, i].Value.ToString()), 2);
                    if (dataGridView1[7, i].Value.ToString()=="IVA(16)")
                    {
                        IVA += Convert.ToDouble(precio)-(Convert.ToDouble(precio) / 1.16);
                    }
                    ticket.AddItem(dataGridView1[0, i].Value.ToString()+" "+ unidad,"      "+ dataGridView1[1, i].Value.ToString() , "   $" + dataGridView1[3, i].Value.ToString());
                    //MessageBox.Show("Se vendera el numero:"+i+"\nCantidad: "+dataGridView1[0, i].Value.ToString()+"\nProducto: "+ dataGridView1[1, i].Value.ToString()+"\nPrecio: "+dataGridView1[2, i].Value.ToString()+"\nMonto :" + dataGridView1[3, i].Value.ToString() +"\nExistencias :"+ dataGridView1[4, i].Value.ToString()+"\nID :"+dataGridView1[5, i].Value.ToString());

                }
            double UtilidadTotal = 0;
                total = 0;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                total += Convert.ToDouble(dataGridView1[3, i].Value.ToString());
                double venta = Convert.ToDouble(dataGridView1[2, i].Value.ToString()) * Convert.ToDouble(dataGridView1[0, i].Value.ToString());
                double compra = Convert.ToDouble(dataGridView1[8, i].Value.ToString()) * Convert.ToDouble(dataGridView1[0, i].Value.ToString());
                UtilidadTotal = UtilidadTotal + (venta - compra);
            }
                //double total = Convert.ToDouble(lblTotal.Text);
                cmd = new OleDbCommand("insert into Ventas(Monto,Fecha,Folio,Estatus, Descuento, Pago) values('" + (total - descuento) + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + lblFolio.Text + "','COBRADO','"+descuento+ "','"+cmbPago.Text+"');", conectar);
                cmd.ExecuteNonQuery();               
                cmd = new OleDbCommand("insert into Corte(Concepto,Monto,FechaHora,Pago) Values('Venta a contado folio " + lblFolio.Text + "','" + (total - descuento) + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','"+cmbPago.Text+"');", conectar);
                cmd.ExecuteNonQuery();
                cmd = new OleDbCommand("insert into VentasCajero(IdUsuario,Usuario,FolioVenta,Total,Fecha,Cajero) values('" + idUsuario + "','" + lblUsuario.Text + "','" + lblFolio.Text + "','" + (total - descuento) + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','"+lblCajero.Text+"');", conectar);
                cmd.ExecuteNonQuery();    

                ticket.AddTotal("Suma", "$" + String.Format("{0:0.00}", total) + "");
                ticket.AddTotal("Descuento", "$" + String.Format("{0:0.00}", descuento) + "");
                ticket.AddTotal("SubTotal", "$" + String.Format("{0:0.00}", Math.Round(total - descuento - IVA, 2)) + "");
                ticket.AddTotal("I.V.A.", "$" + String.Format("{0:0.00}", Math.Round(IVA, 2)) + "");
                ticket.AddTotal("Total", "$" + String.Format("{0:0.00}", (total - descuento)) + "");
                if (cmbPago.Text == "04=TARJETA DE CREDITO" || cmbPago.Text == "28=TARJETA DE DEBITO")
                {
                }
                else
                {
                    ticket.AddTotal("Efectivo", "$" + String.Format("{0:0.00}", efectivo));
                    ticket.AddTotal("Cambio", "$" + String.Format("{0:0.00}", cambio));
                }
                //jalar pie de ticket
                for (int i = 0; i < Conexion.pieDeTicket.Length; i++)
                {
                    ticket.AddFooterLine(Conexion.pieDeTicket[i]);
                }    
                
                foli = foli + 1;
                cmd = new OleDbCommand("UPDATE Folios set Numero=" + foli + " where Folio='FolioContado';", conectar);
                cmd.ExecuteNonQuery();
                //ticket.PrintTicket(Conexion.impresora);
                MessageBox.Show("Venta realizada con exito", "VENTA REALIZADA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                frmVentas vent = new frmVentas();
                vent.usuario = usuario;
                vent.lblUsuario.Text = lblUsuario.Text;
                vent.idUsuario = idUsuario;
                vent.lblCajero.Text = lblCajero.Text;
                vent.Show();
                this.Close();
                
                //}            
        }
        private void frmVentas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F5)
            {
                Venta();
            }
            if (e.KeyCode == Keys.F1)
            {
                checkBox1.Checked = !checkBox1.Checked; // Alterna el estado
            }
        }

        private void lblUsuario_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            using (frmClaveVendedor ori = new frmClaveVendedor())
            {
                if (ori.ShowDialog() == DialogResult.OK)
                {
                    lblUsuario.Text = ori.Usuario;
                    idUsuario = ori.Id.ToString();
                }
            }
        }

        private void txtDescuento_Leave(object sender, EventArgs e)
        {
            // Verifica si el campo está vacío o no es un número
            if (string.IsNullOrWhiteSpace(txtDescuento.Text) || !decimal.TryParse(txtDescuento.Text, out _))
            {
                // Muestra un mensaje de error o maneja el caso como desees
                MessageBox.Show("Por favor, ingrese un valor numérico válido.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            else
            {
                if (radioButton1.Checked)
                {
                    total = 0;
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        total += Convert.ToDouble(dataGridView1[3, i].Value.ToString());
                    }
                    descuento = ((Convert.ToDouble(txtDescuento.Text) / 100)) * total;
                    total = Math.Round(total - descuento, 2);
                    MessageBox.Show("DESCUENTO REALIZADO POR LA CANTIDAD DE: $" + descuento, "DESCUENTO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblTotal.Text = String.Format("{0:0.00}", total);
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
                }
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

        }

        private void printDocument1_PrintPage_1(object sender, PrintPageEventArgs e)
        {
            int posicion = 10;
            //RESIZE
            Image logo = null;//= Image.FromFile("C:\\Jaeger Soft\\logo.jpg");

            //Image logo = Image.FromFile(@"C:\Jaeger Soft\logo.jpg");
            //e.Graphics.DrawImage(logo, new PointF(1, 10));
            //LOGO
            posicion += 200;
            e.Graphics.DrawString("********  NOTA DE CONSUMO  ********", new Font("Arial", 12, FontStyle.Bold), Brushes.Black, new Point(1, posicion));
            posicion += 20;
            e.Graphics.DrawString("FOLIO DE VENTA: " + lblFolio.Text, new Font("Arial", 12, FontStyle.Bold), Brushes.Black, new Point(1, posicion));
            posicion += 20;
            e.Graphics.DrawString("FECHA: " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString(), new Font("Arial", 12, FontStyle.Bold), Brushes.Black, new Point(1, posicion));
            posicion += 50;
            //Titulo Columna
            e.Graphics.DrawString("Cant   Producto        P.Unit  Importe", new Font("Arial", 12, FontStyle.Bold), Brushes.Black, new Point(1, posicion));
            posicion += 20;
            e.Graphics.DrawLine(new Pen(Color.Black), 1, posicion, 420, posicion);
            posicion += 10;
            //Lista de Productos
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Far;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {

                double precio = Convert.ToDouble(dataGridView1[2, i].Value.ToString());
                string producto = dataGridView1[1, i].Value.ToString();
                double cant = Convert.ToDouble(dataGridView1[0, i].Value.ToString());
                string item = cant.ToString("0.00", CultureInfo.InvariantCulture);
                string pre = precio.ToString("00.00", CultureInfo.InvariantCulture);
                double precioUni = Convert.ToDouble(dataGridView1[2, i].Value.ToString());
                string uni = precioUni.ToString("00.00", CultureInfo.InvariantCulture);
                if (producto.Length > 15)
                {
                    producto = producto.Substring(0, 15);
                }

                e.Graphics.DrawString(item, new Font("Arial", 8, FontStyle.Regular), Brushes.Black, new Point(1, posicion));
                e.Graphics.DrawString(producto, new Font("Arial", 8, FontStyle.Regular), Brushes.Black, new Point(40, posicion));
                e.Graphics.DrawString(uni, new Font("Arial", 8, FontStyle.Regular), Brushes.Black, new Point(210, posicion), sf);
                e.Graphics.DrawString(String.Format(CultureInfo.InvariantCulture, "{0:0,0.00}", precio), new Font("Arial", 8, FontStyle.Regular), Brushes.Black, new Point(280, posicion), sf);
                posicion += 20;
                //ticket.AddItem(item, producto, uni + "|" + String.Format(CultureInfo.InvariantCulture, "{0:0,0.00}", precio));
            }
            double to = Convert.ToDouble(lblTotal.Text);
            string toty = String.Format(CultureInfo.InvariantCulture, "{0:0,0.00}", to);
            e.Graphics.DrawLine(new Pen(Color.Black), 210, posicion + 10, 420, posicion + 10);
            posicion += 15;
            e.Graphics.DrawString("TOTAL: $" + toty, new Font("Arial", 10, FontStyle.Bold), Brushes.Black, new Point(280, posicion), sf);
            posicion += 50;

            for (int i = 0; i < Conexion.pieDeTicket.Length; i++)
            {
                e.Graphics.DrawString(Conexion.pieDeTicket[i], new Font("Arial", 10, FontStyle.Bold), Brushes.Black, new Point(1, posicion));
                posicion += 20;
            }
            posicion += 20;
            e.Graphics.DrawLine(new Pen(Color.Black), 1, posicion, 2, posicion);

        }
    }
}
