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
using Microsoft.Office.Interop.Excel;

namespace BRUNO
{
    public partial class frmInventario : Form
    {

        private DataSet ds;
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        //public static string CadCon = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Jaeger Soft\Joyeria.accdb";
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        OleDbCommand cmd;
        OleDbDataAdapter da;
        public String usuario = "Admin";
        bool sinllenar = true;
        public frmInventario()
        {
            InitializeComponent();
            
        }

        private void BtnApartados_Click(object sender, EventArgs e)
        {
            frmAgregarInventario add = new frmAgregarInventario();
            add.Show();
            this.Close();
        }

        private void frmInventario_Load(object sender, EventArgs e)
        {
            //ds = new DataSet();
            conectar.Open();
            System.Data.DataTable dt = new System.Data.DataTable();
            cmd = new OleDbCommand("Select Id,Nombre from Categorias;", conectar);
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dt);
            comboBox2.DisplayMember = "Nombre";
            comboBox2.ValueMember = "Id";
            comboBox2.DataSource = dt;
           
                if (usuario != "Admin")
                {
                    panel6.Hide();
                    ds = new DataSet();
                    da = new OleDbDataAdapter("select * from Inventario order by Nombre;", conectar);
                    da.Fill(ds, "Id");
                    dataGridView2.DataSource = ds.Tables["Id"];
                    dataGridView2.Columns[7].HeaderText = "Precio Compra";
                    dataGridView2.Columns[10].HeaderText = "Unidad";
                    dataGridView2.Columns[9].Visible = false;
                }
                if (Conexion.lugar=="LEO")
                {
                    button9.Visible = true;
                }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView2[6, dataGridView2.CurrentRow.Index].Value.ToString() == "JOYERIA")
                {
                    frmEditarOro oro = new frmEditarOro();
                    oro.usuario = usuario;
                    oro.txtID.Text = dataGridView2[0, dataGridView2.CurrentRow.Index].Value.ToString();
                    oro.txtProducto.Text = dataGridView2[1, dataGridView2.CurrentRow.Index].Value.ToString();
                    oro.txtCompra.Text = dataGridView2[2, dataGridView2.CurrentRow.Index].Value.ToString();
                    oro.lugar = "INVENT";
                    oro.txtVenta.Text = dataGridView2[3, dataGridView2.CurrentRow.Index].Value.ToString();
                    oro.txtLimite.Text = dataGridView2[5, dataGridView2.CurrentRow.Index].Value.ToString();
                    oro.cmbCategoria.Text = dataGridView2[6, dataGridView2.CurrentRow.Index].Value.ToString();
                    oro.cmbSub.Text = dataGridView2[7, dataGridView2.CurrentRow.Index].Value.ToString();
                    oro.cmbProveedor.Text = dataGridView2[12, dataGridView2.CurrentRow.Index].Value.ToString();
                    oro.txtPeso.Text = dataGridView2[9, dataGridView2.CurrentRow.Index].Value.ToString();
                    oro.txtKilataje.Text = dataGridView2[10, dataGridView2.CurrentRow.Index].Value.ToString();
                    oro.txtMaquila.Text = dataGridView2[11, dataGridView2.CurrentRow.Index].Value.ToString();
                    oro.textBox1.Text = dataGridView2[13, dataGridView2.CurrentRow.Index].Value.ToString();
                    oro.Show();
                    this.Close();
                }
                else
                {

                    frmEditarProducto editar = new frmEditarProducto();
                    editar.usuario = usuario;
                    editar.inventario = "INVENT";
                    editar.txtID.Text = dataGridView2[0, dataGridView2.CurrentRow.Index].Value.ToString();
                    editar.txtProducto.Text = dataGridView2[1, dataGridView2.CurrentRow.Index].Value.ToString();
                    editar.txtCompra.Text = dataGridView2[2, dataGridView2.CurrentRow.Index].Value.ToString();
                    editar.txtVenta.Text = dataGridView2[3, dataGridView2.CurrentRow.Index].Value.ToString();
                    editar.cmbCategoria.Text = dataGridView2[8, dataGridView2.CurrentRow.Index].Value.ToString();
                    editar.txtLimite.Text = dataGridView2[7, dataGridView2.CurrentRow.Index].Value.ToString();
                    editar.comboBox1.Text = dataGridView2[6, dataGridView2.CurrentRow.Index].Value.ToString();
                    System.Data.DataTable dt = new System.Data.DataTable();
                    cmd = new OleDbCommand("Select Id,Nombre from Unidades;", conectar);
                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    da.Fill(dt);
                    editar.cmbUnidad.DisplayMember = "Nombre";
                    editar.cmbUnidad.ValueMember = "Id";
                    editar.cmbUnidad.DataSource = dt;
                    editar.cmbUnidad.SelectedValue = dataGridView2[9, dataGridView2.CurrentRow.Index].Value.ToString();
                    editar.cmbUnidad.Text = dataGridView2[10, dataGridView2.CurrentRow.Index].Value.ToString();
                    editar.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                DialogResult dialogResult = MessageBox.Show("¿Estas seguro de eliminar el producto?", "Alto!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    cmd = new OleDbCommand("insert into InventarioSusp(Id,Nombre,PrecioVentaMayoreo,PrecioVenta,Existencia,Limite,Categoria,Especial,IVA,Unidad,Uni) values('" + dataGridView2[0, dataGridView2.CurrentRow.Index].Value.ToString() + "','" + dataGridView2[1, dataGridView2.CurrentRow.Index].Value.ToString() + "','" + dataGridView2[2, dataGridView2.CurrentRow.Index].Value.ToString() + "','" + dataGridView2[3, dataGridView2.CurrentRow.Index].Value.ToString() + "','" + dataGridView2[4, dataGridView2.CurrentRow.Index].Value.ToString() + "','" + dataGridView2[5, dataGridView2.CurrentRow.Index].Value.ToString() + "','" + dataGridView2[6, dataGridView2.CurrentRow.Index].Value.ToString() + "','" + dataGridView2[7, dataGridView2.CurrentRow.Index].Value.ToString() + "','" + dataGridView2[8, dataGridView2.CurrentRow.Index].Value.ToString() + "','" + dataGridView2[9, dataGridView2.CurrentRow.Index].Value.ToString() + "','" + dataGridView2[10, dataGridView2.CurrentRow.Index].Value.ToString() + "');", conectar);
                    cmd.ExecuteNonQuery();
                    cmd = new OleDbCommand("delete from Inventario where Id='" + dataGridView2[0, dataGridView2.CurrentRow.Index].Value.ToString() + "';", conectar);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("PRODUCTO ELIMINADO CON EXITO", "ELIMINADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ds = new DataSet();
                    da = new OleDbDataAdapter("select * from Inventario order by Nombre;", conectar);
                    da.Fill(ds, "Id");
                    dataGridView2.DataSource = ds.Tables["Id"];
                    dataGridView2.Columns[7].HeaderText = "Precio Compra";
                    dataGridView2.Columns[10].HeaderText = "Unidad";
                    dataGridView2.Columns[9].Visible = false;
                }
            }
            catch (Exception ex)
            {

            }
            
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where Categoria='" + comboBox2.Text + "'order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                dataGridView2.Columns[7].HeaderText = "Precio Compra";
                dataGridView2.Columns[10].HeaderText = "Unidad";
                dataGridView2.Columns[9].Visible = false;
                //dataGridView2.Columns[0].Visible = false;
            }
            else
            {
                
                //dataGridView2.Columns[0].Visible = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                textBox1.Enabled = false;
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where Categoria='" + comboBox2.Text + "' and not Categoria='ACCESORIOS' order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                dataGridView2.Columns[7].HeaderText = "Precio Compra";
                dataGridView2.Columns[10].HeaderText = "Unidad";
                dataGridView2.Columns[9].Visible = false;
                //dataGridView2.Columns[0].Visible = false;
            }
            else
            {
                textBox1.Enabled = true;
                comboBox2.SelectedIndex = 0;
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario where not Categoria='ACCESORIOS' order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                dataGridView2.Columns[7].HeaderText = "Precio Compra";
                dataGridView2.Columns[10].HeaderText = "Unidad";
                dataGridView2.Columns[9].Visible = false;
                //dataGridView2.Columns[0].Visible = false;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //if (textBox1.Text == "")
            //{
            //    ds = new DataSet();
            //    da = new OleDbDataAdapter("select * from Inventario where not Categoria='ACCESORIOS' and Existencia > 0 order by Nombre;", conectar);
            //    da.Fill(ds, "Id");
            //    dataGridView2.DataSource = ds.Tables["Id"];
            //    //dataGridView2.Columns[0].Visible = false;
            //}
            //else
            //{
            //    ds = new DataSet();
            //    da = new OleDbDataAdapter("select * from Inventario where not Categoria='ACCESORIOS' and Existencia > 0 and Nombre LIKE '%" + textBox1.Text + "%';", conectar);
            //    da.Fill(ds, "Id");
            //    dataGridView2.DataSource = ds.Tables["Id"];
            //    //dataGridView2.Columns[0].Visible = false;
            //}
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            //if (textBox2.Text == "")
            //{
            //    ds = new DataSet();
            //    da = new OleDbDataAdapter("select * from Inventario where not Categoria='ACCESORIOS' and Existencia > 0  order by Nombre;", conectar);
            //    da.Fill(ds, "Id");
            //    dataGridView2.DataSource = ds.Tables["Id"];
            //    //dataGridView2.Columns[0].Visible = false;
            //}
            //else
            //{
            //    ds = new DataSet();
            //    da = new OleDbDataAdapter("select * from Inventario where not Categoria='ACCESORIOS'  and Existencia > 0 and Id LIKE '%" + textBox2.Text + "%';", conectar);
            //    da.Fill(ds, "Id");
            //    dataGridView2.DataSource = ds.Tables["Id"];
            //    //dataGridView2.Columns[0].Visible = false;
            //}
        }

        private void button3_Click(object sender, EventArgs e)
        {
            frmLineas Linea = new frmLineas();
            Linea.Show();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            frmDescuentoPorCategoria desc = new frmDescuentoPorCategoria();
            desc.Show();
            this.Close();
        }

        private void label5_Click(object sender, EventArgs e)
        {
           
        }

        private void label3_Click_1(object sender, EventArgs e)
        {
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            frmAccesorios acc = new frmAccesorios();
            acc.Show();
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (sinllenar)
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Inventario order by Nombre;", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];
                dataGridView2.Columns[7].HeaderText = "Precio Compra";
                dataGridView2.Columns[10].HeaderText = "Unidad";
                dataGridView2.Columns[9].Visible = false;
                button13.Visible = true;
                sinllenar = false;
            }
            else if (Conexion.lugar == "LEO")
            {

            }
            else 
            {
                double piezasM = 0;
                double ventaM = 0;
                double inversionM = 0;

                double piezasF = 0;
                double ventaF = 0;
                double inversionF = 0;

                for (int i = 0; i < dataGridView2.RowCount; i++)
                {
                    if (dataGridView2[6, i].Value.ToString() == "Materiales")
                    {
                        piezasM = Convert.ToDouble(dataGridView2[4, i].Value.ToString());
                        ventaM += piezasM * Convert.ToDouble(dataGridView2[3, i].Value.ToString());
                        inversionM += piezasM * Convert.ToDouble(dataGridView2[7, i].Value.ToString());
                    }
                    else if (dataGridView2[6, i].Value.ToString() == "Ferreteria")
                    {
                        piezasF= Convert.ToDouble(dataGridView2[4, i].Value.ToString());
                        ventaF += piezasF * Convert.ToDouble(dataGridView2[3, i].Value.ToString());
                        inversionF += piezasF * Convert.ToDouble(dataGridView2[7, i].Value.ToString());
                    }
                    
                }
                frmDatos dat = new frmDatos();
                dat.lblInversionM.Text = "$" + inversionM.ToString("#,#.00", CultureInfo.InvariantCulture);
                dat.lblVentaM.Text = "$" + ventaM.ToString("#,#.00", CultureInfo.InvariantCulture);
                dat.lblUtilidadM.Text = "$" + (ventaM - inversionM).ToString("#,#.00", CultureInfo.InvariantCulture);

                dat.lblInversionF.Text = "$" + inversionF.ToString("#,#.00", CultureInfo.InvariantCulture);
                dat.lblVentaF.Text = "$" + ventaF.ToString("#,#.00", CultureInfo.InvariantCulture);
                dat.lblUtilidadF.Text = "$" + (ventaF - inversionF).ToString("#,#.00", CultureInfo.InvariantCulture);

                dat.Show();
            }
                //button6.Visible = false;
            
            //else if (usuario == "Admin")
            //{
            //    for (int i = 0; i < dataGridView2.RowCount; i++)
            //    {
            //        try
            //        {
            //            if (dataGridView2[6, i].Value.ToString() == "ARMAZONES")
            //            {
            //                piezasArmazones += Convert.ToInt32(dataGridView2[4, i].Value.ToString());
            //            }
            //            if (dataGridView2[6, i].Value.ToString() == "LENTE DE CONTACTO")
            //            {
            //                piezasLenteContacto += Convert.ToInt32(dataGridView2[4, i].Value.ToString());
            //            }
            //            if (dataGridView2[6, i].Value.ToString() == "ESTUCHES DE LENTE DE CONTACTO")
            //            {
            //                piezasEstucheLenteContacto += Convert.ToInt32(dataGridView2[4, i].Value.ToString());
            //            }
            //            if (dataGridView2[6, i].Value.ToString() == "SOLUCION DE LENTE DE CONTACTO")
            //            {
            //                piezasSolucionLenteContacto += Convert.ToInt32(dataGridView2[4, i].Value.ToString());
            //            }
            //            if (dataGridView2[6, i].Value.ToString() == "MICAS")
            //            {
            //                piezasMicas += Convert.ToInt32(dataGridView2[4, i].Value.ToString());
            //            }
            //            if (dataGridView2[6, i].Value.ToString() == "60 ml")
            //            {
            //                piezas60 += Convert.ToInt32(dataGridView2[4, i].Value.ToString());
            //            }
            //            if (dataGridView2[6, i].Value.ToString() == "90ml")
            //            {
            //                piezas90 += Convert.ToInt32(dataGridView2[4, i].Value.ToString());
            //            }
            //            if (dataGridView2[6, i].Value.ToString() == "120ml")
            //            {
            //                piezas120 += Convert.ToInt32(dataGridView2[4, i].Value.ToString());
            //            }
            //            if (dataGridView2[6, i].Value.ToString() == "500ml")
            //            {
            //                piezas500 += Convert.ToInt32(dataGridView2[4, i].Value.ToString());
            //            }
            //            if (dataGridView2[6, i].Value.ToString() == "LENTE DE LECTURA")
            //            {
            //                piezasLectura += Convert.ToInt32(dataGridView2[4, i].Value.ToString());
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            //MessageBox.Show("ERROR EN PRODUCTO: " + dataGridView2[1, i].Value.ToString() + "\n" + ex);
            //        }
            //    }

            //    lblPiezasR.Text = "" + piezasArmazones;
            //    lblLenteContacto.Text = "" + piezasLenteContacto;
            //    lblEstuLenteContacto.Text = "" + piezasEstucheLenteContacto;
            //    lblSoluLenteContac.Text = "" + piezasSolucionLenteContacto;
            //    lblMicas.Text = "" + piezasMicas;
            //    lbl60.Text = "" + piezas60;
            //    lbl90.Text = "" + piezas90;
            //    lbl120.Text = "" + piezas120;
            //    lbl500.Text = "" + piezas500;
            //    lblLectura.Text =""+ piezasLectura;
            //    panel5.Visible = true;
            //    button6.Visible = false;
            //}
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (textBox1.Text == "")
                {
                    ds = new DataSet();
                    da = new OleDbDataAdapter("select * from Inventario order by Nombre;", conectar);
                    da.Fill(ds, "Id");
                    dataGridView2.DataSource = ds.Tables["Id"];
                    dataGridView2.Columns[7].HeaderText = "Precio Compra";
                    dataGridView2.Columns[10].HeaderText = "Unidad";
                    dataGridView2.Columns[9].Visible = false;
                }
                else
                {
                    ds = new DataSet();
                    da = new OleDbDataAdapter("select * from Inventario where Nombre LIKE '%" + textBox1.Text + "%';", conectar);
                    da.Fill(ds, "Id");
                    dataGridView2.DataSource = ds.Tables["Id"];
                    dataGridView2.Columns[7].HeaderText = "Precio Compra";
                    dataGridView2.Columns[10].HeaderText = "Unidad";
                    dataGridView2.Columns[9].Visible = false;
                }
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (textBox2.Text == "")
                {
                    ds = new DataSet();
                    da = new OleDbDataAdapter("select * from Inventario order by Nombre;", conectar);
                    da.Fill(ds, "Id");
                    dataGridView2.DataSource = ds.Tables["Id"];
                    dataGridView2.Columns[7].HeaderText = "Precio Compra";
                    dataGridView2.Columns[10].HeaderText = "Unidad";
                    dataGridView2.Columns[9].Visible = false;
                }
                else
                {
                    ds = new DataSet();
                    da = new OleDbDataAdapter("select * from Inventario where Id LIKE '%" + textBox2.Text + "%';", conectar);
                    da.Fill(ds, "Id");
                    dataGridView2.DataSource = ds.Tables["Id"];
                    dataGridView2.Columns[7].HeaderText = "Precio Compra";
                    dataGridView2.Columns[10].HeaderText = "Unidad";
                    dataGridView2.Columns[9].Visible = false;
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                frmKardex kar = new frmKardex();
                kar.lblProducto.Text = dataGridView2[1, dataGridView2.CurrentRow.Index].Value.ToString();
                kar.idProducto = dataGridView2[0, dataGridView2.CurrentRow.Index].Value.ToString();
                kar.Show();
                this.Close();
            }
            catch (Exception ex)
            {

            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
            frmPolizas comp = new frmPolizas();
            comp.usuario = usuario;
            comp.Show();
            this.Close();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            frmGastos gas = new frmGastos();
            gas.Show();
            this.Close();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                cmd = new OleDbCommand("UPDATE Inventario set Existencia='" + dataGridView2[4, i].Value.ToString() + "' where Id='" + dataGridView2[0, i].Value.ToString() + "';", conectar);
                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("Se han guardado los cambios correctamente!", "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            frmUnidades UN = new frmUnidades();
            UN.Show();
            this.Close();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            frmCategorias es = new frmCategorias();
            es.Show();
            this.Close();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            frmSuspendidos susp = new frmSuspendidos();
            susp.Show();
            this.Close();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application xla = new Microsoft.Office.Interop.Excel.Application();
            Workbook wb = xla.Workbooks.Add(XlSheetType.xlWorksheet);
            Worksheet ws = (Worksheet)xla.ActiveSheet;

            xla.Visible = true;

            ws.Cells[1, 1] = "ID";
            ws.Cells[1, 2] = "Nombre";
            ws.Cells[1, 3] = "Existencia";
            ws.Cells[1, 4] = "Limite";
            ws.Cells[1, 5] = "Fecha: " + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            int cont = 1;
            for (int i = 0; i < dataGridView2.RowCount; i++)
            {

                if (Convert.ToDouble(dataGridView2[4, i].Value.ToString()) <= Convert.ToDouble(dataGridView2[5, i].Value.ToString()))
                {
                    cont++;
                    ws.Cells[cont, 1] = dataGridView2[0, i].Value.ToString();
                    ws.Cells[cont, 2] = dataGridView2[1, i].Value.ToString();
                    ws.Cells[cont, 3] = dataGridView2[4, i].Value.ToString();
                    ws.Cells[cont, 4] = dataGridView2[5, i].Value.ToString();

                }
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
             DialogResult dialogResult = MessageBox.Show("¿Desea imprimir un reporte para capturar el inventario fisico?", "Alto!", MessageBoxButtons.YesNo);
             if (dialogResult == DialogResult.Yes)
             {
                 Microsoft.Office.Interop.Excel.Application xla = new Microsoft.Office.Interop.Excel.Application();
                 Workbook wb = xla.Workbooks.Add(XlSheetType.xlWorksheet);
                 Worksheet ws = (Worksheet)xla.ActiveSheet;

                 xla.Visible = true;

                 ws.Cells[1, 1] = "ID";
                 ws.Cells[1, 2] = "Nombre";
                 ws.Cells[1, 3] = "Existen";
                 ws.Cells[1, 4] = "Fisico";
                 int cont = 1;
                 for (int i = 0; i < dataGridView2.RowCount; i++)
                 {
                         cont++;
                         ws.Cells[cont, 1] = dataGridView2[0, i].Value.ToString();
                         ws.Cells[cont, 2] = dataGridView2[1, i].Value.ToString();
                         ws.Cells[cont, 3] = dataGridView2[4, i].Value.ToString();
                 }
                 frmInventariosFisicos fis = new frmInventariosFisicos();
                 fis.Show();
                 this.Close();
             }
             else
             {
                 frmInventariosFisicos fis = new frmInventariosFisicos();
                 fis.Show();
                 this.Close();

             }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application xla = new Microsoft.Office.Interop.Excel.Application();
            Workbook wb = xla.Workbooks.Add(XlSheetType.xlWorksheet);
            Worksheet ws = (Worksheet)xla.ActiveSheet;
            
            xla.Visible = true;
            Microsoft.Office.Interop.Excel.Range formatRange;
            formatRange = ws.get_Range("A:A",System.Type.Missing);
            formatRange.NumberFormat = "####";
            formatRange.EntireColumn.ColumnWidth = 17;
            
            formatRange = ws.get_Range("B:B",System.Type.Missing);
            formatRange.EntireColumn.ColumnWidth = 30;

            formatRange = ws.get_Range("a1", "j1"); 
            formatRange.Interior.Color = System.Drawing.
            ColorTranslator.ToOle(System.Drawing.Color.Yellow);
            formatRange.EntireRow.Font.Bold = true;

            ws.Cells[1, 1] = "ID";
            ws.Cells[1, 2] = "Nombre";
            ws.Cells[1, 3] = "Precio de Venta";
            ws.Cells[1, 4] = "Existencia";
            ws.Cells[1, 5] = "Limite";
            ws.Cells[1, 6] = "Categoria";
            ws.Cells[1, 7] = "Precio de compra";
            ws.Cells[1, 8] = "Unidad";
            ws.Cells[1, 9] = "Fecha: " + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            int cont = 1;
            for (int i = 0; i < dataGridView2.RowCount; i++)
            {

                if (Convert.ToDouble(dataGridView2[4, i].Value.ToString()) <= Convert.ToDouble(dataGridView2[5, i].Value.ToString()))
                {
                    cont++;
                    ws.Cells[cont, 1] = dataGridView2[0, i].Value.ToString();
                    ws.Cells[cont, 2] = dataGridView2[1, i].Value.ToString();
                    ws.Cells[cont, 3] = dataGridView2[3, i].Value.ToString();
                    ws.Cells[cont, 4] = dataGridView2[4, i].Value.ToString();
                    ws.Cells[cont, 5] = dataGridView2[5, i].Value.ToString();
                    ws.Cells[cont, 6] = dataGridView2[6, i].Value.ToString();
                    ws.Cells[cont, 7] = dataGridView2[7, i].Value.ToString();
                    ws.Cells[cont, 8] = dataGridView2[10, i].Value.ToString();

                }
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            frmCaptura cap = new frmCaptura();
            cap.Show();
            this.Close();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            frmCompras2 COM = new frmCompras2();
            COM.Show();
            this.Close();
        }
    }
}
