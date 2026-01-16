using LibPrintTicket;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmCorte : frmBase
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
        private decimal porcentaje;

        public frmCorte()
        {
            InitializeComponent();
            this.MinimumSize = new Size(969, 772);
        }

        private void dgvCorte_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        { 
            if (dgvCorte.Columns[e.ColumnIndex].Name == "Monto")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("C2"); // Formato moneda con 2 decimales
                    e.FormattingApplied = true;
                }
            }
        }

        private void dataGridView4_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView4.Columns[e.ColumnIndex].Name == "Monto")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("C2"); // Formato moneda con 2 decimales
                    e.FormattingApplied = true;
                }
            }
        }

        private void dataGridView3_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView3.Columns[e.ColumnIndex].Name == "Monto")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("C2"); // Formato moneda con 2 decimales
                    e.FormattingApplied = true;
                }
            }
        }

        private void dgvFolios_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvFolios.Columns[e.ColumnIndex].Name == "Monto" || dgvFolios.Columns[e.ColumnIndex].Name == "Descuento" || dgvFolios.Columns[e.ColumnIndex].Name == "Monto sin IVA" || dgvFolios.Columns[e.ColumnIndex].Name == "IVA")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("C2"); // Formato moneda con 2 decimales
                    e.FormattingApplied = true;
                }
            }
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
            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from Corte where Pago='01=EFECTIVO';", conectar);
            da.Fill(ds, "Id");
            dgvCorte.DataSource = ds.Tables["Id"];
            dgvCorte.Columns[0].Visible = false;

            cmd = new OleDbCommand("select Porcentaje from Tarjeta where Id=1;", conectar);
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                porcentaje = Convert.ToDecimal(reader[0].ToString());
            }
            lblComision.Text = $"{porcentaje:F2}% Comisión:";

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
            reader = cmd.ExecuteReader();
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
                    if(Convert.ToDouble(dataGridView1[1, i].Value.ToString())>0)
                    utilidad += Convert.ToDouble(dataGridView1[1, i].Value.ToString());
                }

            }
            ds = new DataSet();
            da = new OleDbDataAdapter("select * from GastosDetallados Where Fecha>=#" + subcadena + " 00:00:00# and Fecha <=#" + subcadena + " 23:59:59#;", conectar);
            da.Fill(ds, "Id");
            dataGridView2.DataSource = ds.Tables["Id"];

            // 1. Llenar el DataGridView como lo tenías
            ds = new DataSet();
            da = new OleDbDataAdapter("Select Id,Folio,    Monto / (1 + (16 / 100)) AS [Monto sin IVA], Monto - (Monto / (1 + (16 / 100))) AS [IVA], Monto,Descuento, Fecha, Estatus, Pago from Ventas where Estatus ='COBRADO' AND Fecha>=#" + subcadena + " 00:00:00# and Fecha <=#" + subcadena + " 23:59:59#;", conectar);
            da.Fill(ds, "Id");
            dgvFolios.DataSource = ds.Tables["Id"];

            // 2. Ocultar la columna que no quieres mostrar (si es necesario)
            dgvFolios.Columns[0].Visible = false;

            // 3. Agregar la columna CheckBox (si no está en el DataSource)
            DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
            checkColumn.Name = "Seleccionar";
            checkColumn.HeaderText = "Seleccionar";
            checkColumn.Width = 80;
            checkColumn.ReadOnly = false;  // Solo esta columna será editable
            dgvFolios.Columns.Insert(0, checkColumn);

            // 4. Marcar todos los CheckBoxes como true inicialmente
            foreach (DataGridViewRow row in dgvFolios.Rows)
            {
                row.Cells["Seleccionar"].Value = true;
            }

            // 5. Hacer que TODAS las demás columnas sean de solo lectura
            foreach (DataGridViewColumn column in dgvFolios.Columns)
            {
                if (column.Name != "Seleccionar") // Excluir solo el CheckBox
                {
                    column.ReadOnly = true;
                }
            }

            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                    gastos += Convert.ToDouble(dataGridView2[2, i].Value.ToString());                
            }
            lblCorte.Text =  $"{mas:C}";
            lblECaja.Text = $"{mas + menos:C}";
            lblBruta.Text = $"{utilidad - gastos:C}";
            lblGastos.Text = $"{gastos:C}";
            inversion = total - utilidad;
            lblInversion.Text = $"{inversion:C}";
            lblUtilidad.Text = $"{utilidad:C}";
            lblEntrada.Text = $"{mas + tarjeta + trans:C}";
            lblSalida.Text = $"{menos * -1:C}";
            lblCredito.Text = $"{tarjeta:F2}";
            decimal factorPorcentaje = porcentaje / 100m; // Convertir a factor (0.027)
            decimal factorRestante = 1m - factorPorcentaje; // Factor restante (0.973)
            lblCredito.Text = $"{(decimal)tarjeta * factorRestante:F2}";
            lbl5por.Text = $"{(decimal)tarjeta * factorPorcentaje:F2}";
            lblTrans.Text = $"{trans:C}";
            // Usar decimal para cálculos financieros
            decimal monto = Convert.ToDecimal(tarjeta + mas + menos + trans);
            decimal tasaIVA = 0.16m; // 16% en formato decimal
            // Calcular monto sin IVA e IVA
            decimal montoSinIVA = monto / (1 + tasaIVA);
            decimal iva = monto - montoSinIVA;
            lblNoIva.Text = $"{montoSinIVA:C2}"; 
            lblIVA.Text = $"{iva:C2}"; 
            lblTotal.Text = $"{monto:C2}";
            EstilizarDataGridView(this.dgvFolios);
            EstilizarDataGridView(this.dgvCorte);
            EstilizarDataGridView(this.dataGridView5);
            EstilizarDataGridView(this.dataGridView1);
            EstilizarDataGridView(this.dataGridView2);
            EstilizarDataGridView(this.dataGridView3);
            EstilizarDataGridView(this.dataGridView4);

            EstilizarBotonPrimario(this.button1);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            #region folios nuevos
            int foli = 0;
            
            List<Producto> productos = new List<Producto>();
            cmd = new OleDbCommand("select Numero from Folios where Folio='FolioVenta';", conectar);
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                foli = Convert.ToInt32(Convert.ToString(reader[0].ToString()));
            }
            bool detallado = false;
            DialogResult dialogResult = MessageBox.Show("¿Requiere el corte impreso detallado?", "Alto!", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                detallado=true;
            }
                foreach (DataGridViewRow row in dgvFolios.Rows)
                {
                if(detallado)
                    productos.Add(new Producto
                    {
                        Cantidad = 0,
                        Nombre = row.Cells["Folio"].Value.ToString(),
                        PrecioUnitario = Convert.ToDouble(row.Cells["Monto sin IVA"].Value.ToString()),
                        Total = Convert.ToDouble(row.Cells["Monto"].Value.ToString()),
                    });
                
                if (row.Cells["Seleccionar"].Value != null && Convert.ToBoolean(row.Cells["Seleccionar"].Value))
                    {
                        string folio = row.Cells["Folio"].Value.ToString();
                       
                        //insertar datos
                        //MessageBox.Show($"Fila seleccionada: {folio}");

                        foli++;
                    }
                }
            cmd = new OleDbCommand("UPDATE Folios set Numero=" + foli + " where Folio='FolioVenta';", conectar);
            cmd.ExecuteNonQuery();
            #endregion 

            string fecha = "";
            cmd = new OleDbCommand("select * from Fech where Id=1;", conectar);
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int caja = Convert.ToInt32(Convert.ToString(reader[1].ToString()));
                fecha = Convert.ToString(reader[2].ToString());
            }
            button1.Visible = false;
            folio++;
            string[] encabezados = new string[] { "**********  CORTE DE CAJA  ********", "             Apertura de caja:", fecha, "               Corte de caja:", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() };
            for (int i = 0; i < dgvCorte.RowCount; i++)
            {                
                //MessageBox.Show("insert into Cortes(Concepto,Monto,idCorte,Tipo) Values('" + dgvCorte[1, i].Value.ToString() + "','" + dgvCorte[2, i].Value.ToString() + "','" + dgvCorte[3, i].Value.ToString() + "','" + folio + "','PAGO CONTADO');");
                cmd = new OleDbCommand("insert into Cortes(Concepto,Monto,idCorte,Tipo) Values('" + dgvCorte[1, i].Value.ToString() + "','" + dgvCorte[2, i].Value.ToString() + "','" + folio + "','" + dgvCorte[4, i].Value.ToString() + "');", conectar);                
                cmd.ExecuteNonQuery();
                

            }
                for (int i = 0; i < dataGridView3.RowCount; i++)
            {
                cmd = new OleDbCommand("insert into Cortes(Concepto,Monto,idCorte,Tipo) Values('" + dataGridView3[1, i].Value.ToString() + "','" + dataGridView3[2, i].Value.ToString() + "','" + folio + "','" + dataGridView3[4, i].Value.ToString() + "');", conectar);
                cmd.ExecuteNonQuery();
            }
            for (int i = 0; i < dataGridView4.RowCount; i++)
            {
                cmd = new OleDbCommand("insert into Cortes(Concepto,Monto,idCorte,Tipo) Values('" + dataGridView4[1, i].Value.ToString() + "','" + dataGridView4[2, i].Value.ToString() + "','" + folio + "','" + dataGridView4[4, i].Value.ToString() + "');", conectar);
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
            // Elimina todo excepto números, punto y signo negativo (si aplica)
            string GetNumericValue(string input)
            {
                return Regex.Replace(input, @"[^\d.-]", "");
            }

            Dictionary<string, double> totales = new Dictionary<string, double>();
            totales.Add("Efectivo", Convert.ToDouble(GetNumericValue(lblCorte.Text)));
            totales.Add("Tarjetas", Convert.ToDouble(GetNumericValue(lblCredito.Text)));
            totales.Add("Transferencias", Convert.ToDouble(GetNumericValue(lblTrans.Text)));
            totales.Add("Sin IVA", Convert.ToDouble(GetNumericValue(lblNoIva.Text)));
            totales.Add("IVA", Convert.ToDouble(GetNumericValue(lblIVA.Text)));
            /*
            totales.Add("Entradas", Convert.ToDouble(GetNumericValue(lblEntrada.Text)));
            totales.Add("Salidas", Convert.ToDouble(GetNumericValue(lblSalida.Text)));
            */
            totales.Add("Total", Convert.ToDouble(GetNumericValue(lblTotal.Text)));
            string[] pieDePagina = new string[] { "" };
            TicketPrinter ticketPrinter = new TicketPrinter(encabezados, Conexion.pieDeTicket, Conexion.logoPath, productos, "", "", "", total, true, totales);

            ticketPrinter.ImprimirTicket();
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
