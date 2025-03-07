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

namespace BRUNO
{
    public partial class frmBuscarProductos : Form
    {
        bool selec = false;
        private DataSet ds;
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        OleDbDataAdapter da;
        public string producto { get; set; }
        public string precio { get; set; }
        public string monto { get; set; }
        public string existencia { get; set; }
        public string ID { get; set; }
        public string IVA { get; set; }
        public string compra { get; set; }
        public bool compras = false;
        public int poliza = 0;
        public frmBuscarProductos()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (selec)
            {
                if (compras)
                {
                    double preci = Convert.ToDouble(dataGridView1[3, dataGridView1.CurrentRow.Index].Value.ToString());
                    producto = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
                    precio = String.Format("{0:0.00}", preci);
                    monto = String.Format("{0:0.00}", preci);
                    existencia = dataGridView1[4, dataGridView1.CurrentRow.Index].Value.ToString();
                    ID = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
                    IVA = dataGridView1[6, dataGridView1.CurrentRow.Index].Value.ToString();
                    compra = dataGridView1[5, dataGridView1.CurrentRow.Index].Value.ToString();
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                {
                    using (frmPrecio buscar = new frmPrecio())
                    {

                        if (buscar.ShowDialog() == DialogResult.OK)
                        {
                            if (buscar.tipo == "GEN")
                            {
                                double preci = Convert.ToDouble(dataGridView1[3, dataGridView1.CurrentRow.Index].Value.ToString());
                                producto = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
                                precio = String.Format("{0:0.00}", preci);
                                monto = String.Format("{0:0.00}", preci);
                                existencia = dataGridView1[4, dataGridView1.CurrentRow.Index].Value.ToString();
                                ID = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
                                IVA = dataGridView1[6, dataGridView1.CurrentRow.Index].Value.ToString();
                                compra = dataGridView1[5, dataGridView1.CurrentRow.Index].Value.ToString();
                                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                            }

                            else if (buscar.tipo == "MAY")
                            {
                                double preci = Convert.ToDouble(dataGridView1[2, dataGridView1.CurrentRow.Index].Value.ToString());
                                producto = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
                                precio = String.Format("{0:0.00}", preci);
                                monto = String.Format("{0:0.00}", preci);
                                existencia = dataGridView1[4, dataGridView1.CurrentRow.Index].Value.ToString();
                                ID = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
                                IVA = dataGridView1[6, dataGridView1.CurrentRow.Index].Value.ToString();
                                compra = dataGridView1[5, dataGridView1.CurrentRow.Index].Value.ToString();
                                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No se ha seleccionado ningun producto", "¡ALTO!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void frmBuscarProductos_Load(object sender, EventArgs e)
        {
            //ds = new DataSet();
            this.KeyPreview = true;
            conectar.Open();
            if(textBox1.Text!="")
            {
                if (textBox1.Text == "")
                {
                    ds = new DataSet();
                    da = new OleDbDataAdapter("select Id,Nombre,PrecioVentaMayoreo,PrecioVenta,Existencia,Especial,IVA from Inventario;", conectar);
                    da.Fill(ds, "Id");
                    dataGridView1.DataSource = ds.Tables["Id"];
                    dataGridView1.Columns[5].Visible = false;
                    dataGridView1.Columns[6].Visible = false;
                }
                else
                {
                    ds = new DataSet();
                    da = new OleDbDataAdapter("select Id,Nombre,PrecioVentaMayoreo,PrecioVenta,Existencia,Especial,IVA from Inventario where Nombre LIKE '%" + textBox1.Text + "%' ORDER BY Nombre ;", conectar);
                    da.Fill(ds, "Id");
                    dataGridView1.DataSource = ds.Tables["Id"];
                    dataGridView1.Columns[5].Visible = false;
                    dataGridView1.Columns[6].Visible = false;
                }
               
            }
            textBox2.Focus();
            //da = new OleDbDataAdapter("select Id,Nombre,PrecioCompra,PrecioVenta,Existencia from Inventario where Existencia > 0;", conectar);
            //da.Fill(ds, "Id");
            //dataGridView1.DataSource = ds.Tables["Id"];
            //dataGridView1.Columns[0].Visible = false;
            //dataGridView1.Columns[2].Visible = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //if (textBox1.Text == "")
            //{
            //    ds = new DataSet();
            //    da = new OleDbDataAdapter("select * from Inventario ORDER BY Nombre;", conectar);
            //    da.Fill(ds, "Id");
            //    dataGridView1.DataSource = ds.Tables["Id"];
            //    dataGridView1.Columns[0].Visible = false;
            //    dataGridView1.Columns[2].Visible = false;
            //}
            //else
            //{

            //    ds = new DataSet();
            //    da = new OleDbDataAdapter("select * from Inventario where Nombre LIKE '%" + textBox1.Text + "%' ORDER BY Nombre ;", conectar);
            //    da.Fill(ds, "Id");
            //    dataGridView1.DataSource = ds.Tables["Id"];
            //    dataGridView1.Columns[0].Visible = false;
            //    dataGridView1.Columns[2].Visible = false;
            //}
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            //if (textBox2.Text == "")
            //{
            //    ds = new DataSet();
            //    da = new OleDbDataAdapter("select * from Inventario ORDER BY Nombre;", conectar);
            //    da.Fill(ds, "Id");
            //    dataGridView1.DataSource = ds.Tables["Id"];
            //    dataGridView1.Columns[0].Visible = false;
            //    dataGridView1.Columns[2].Visible = false;
            //}
            //else
            //{

            //    ds = new DataSet();
            //    da = new OleDbDataAdapter("select * from Inventario where Id LIKE '%" + textBox2.Text + "%' ORDER BY Nombre ;", conectar);
            //    da.Fill(ds, "Id");
            //    dataGridView1.DataSource = ds.Tables["Id"];
            //    dataGridView1.Columns[0].Visible = false;
            //    dataGridView1.Columns[2].Visible = false;
            //}
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (textBox2.Text == "")
                {
                    ds = new DataSet();
                    da = new OleDbDataAdapter("select Id,Nombre,PrecioVentaMayoreo,PrecioVenta,Existencia,Especial,IVA from Inventario ;", conectar);
                    da.Fill(ds, "Id");
                    dataGridView1.DataSource = ds.Tables["Id"];
                    dataGridView1.Columns[5].Visible = false;
                    dataGridView1.Columns[6].Visible = false;
                }
                else
                {
                    ds = new DataSet();
                    da = new OleDbDataAdapter("select Id,Nombre,PrecioVentaMayoreo,PrecioVenta,Existencia,Especial,IVA from Inventario where Id LIKE '%" + textBox2.Text + "%' ORDER BY Nombre ;", conectar);
                    da.Fill(ds, "Id");
                    dataGridView1.DataSource = ds.Tables["Id"];
                    dataGridView1.Columns[5].Visible = false;
                    dataGridView1.Columns[6].Visible = false;
                }
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (textBox1.Text == "")
                {
                    ds = new DataSet();
                    da = new OleDbDataAdapter("select Id,Nombre,PrecioVentaMayoreo,PrecioVenta,Existencia,Especial,IVA from Inventario;", conectar);
                    da.Fill(ds, "Id");
                    dataGridView1.DataSource = ds.Tables["Id"];
                    dataGridView1.Columns[5].Visible = false;
                    dataGridView1.Columns[6].Visible = false;
                }
                else
                {
                    ds = new DataSet();
                    da = new OleDbDataAdapter("select Id,Nombre,PrecioVentaMayoreo,PrecioVenta,Existencia,Especial,IVA from Inventario where Nombre LIKE '%" + textBox1.Text + "%'ORDER BY Nombre ;", conectar);
                    da.Fill(ds, "Id");
                    dataGridView1.DataSource = ds.Tables["Id"];
                    dataGridView1.Columns[5].Visible = false;
                    dataGridView1.Columns[6].Visible = false;
                }
            }
        }

        private void frmBuscarProductos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (selec)
                {                                    
                if (compras)
                {
                    double preci = Convert.ToDouble(dataGridView1[3, dataGridView1.CurrentRow.Index].Value.ToString());
                    producto = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
                    precio = String.Format("{0:0.00}", preci);
                    monto = String.Format("{0:0.00}", preci);
                    existencia = dataGridView1[4, dataGridView1.CurrentRow.Index].Value.ToString();
                    ID = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
                    IVA = dataGridView1[6, dataGridView1.CurrentRow.Index].Value.ToString();
                    compra = dataGridView1[5, dataGridView1.CurrentRow.Index].Value.ToString();
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                {
                    using (frmPrecio buscar = new frmPrecio())
                    {
                        if (buscar.ShowDialog() == DialogResult.OK)
                        {
                            if (buscar.tipo == "GEN")
                            {
                    try
                    {
                        double preci = Convert.ToDouble(dataGridView1[3, dataGridView1.CurrentRow.Index].Value.ToString());
                        producto = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
                        precio = String.Format("{0:0.00}", preci);
                        monto = String.Format("{0:0.00}", preci);
                        existencia = dataGridView1[4, dataGridView1.CurrentRow.Index].Value.ToString();
                        ID = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
                        IVA = dataGridView1[6, dataGridView1.CurrentRow.Index].Value.ToString();
                        compra = dataGridView1[5, dataGridView1.CurrentRow.Index].Value.ToString();
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    }
                    catch (Exception ex)
                    {

                    }
                            }
                            else if (buscar.tipo == "MAY")
                            {
                                double preci = Convert.ToDouble(dataGridView1[2, dataGridView1.CurrentRow.Index].Value.ToString());
                                producto = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
                                precio = String.Format("{0:0.00}", preci);
                                monto = String.Format("{0:0.00}", preci);
                                existencia = dataGridView1[4, dataGridView1.CurrentRow.Index].Value.ToString();
                                ID = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
                                IVA = dataGridView1[6, dataGridView1.CurrentRow.Index].Value.ToString();
                                compra = dataGridView1[5, dataGridView1.CurrentRow.Index].Value.ToString();
                                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                            }
                            //else
                            //{
                            //    double preci = Convert.ToDouble(dataGridView1[2, dataGridView1.CurrentRow.Index].Value.ToString());
                            //    producto = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
                            //    precio = String.Format("{0:0.00}", preci);
                            //    monto = String.Format("{0:0.00}", preci);
                            //    existencia = dataGridView1[4, dataGridView1.CurrentRow.Index].Value.ToString();
                            //    ID = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
                            //    IVA = dataGridView1[6, dataGridView1.CurrentRow.Index].Value.ToString();
                            //    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                            //}
                        }
                    }
                }
                }
                else
                {
                    //MessageBox.Show("No se ha seleccionado ningun producto", "¡ALTO!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
            else if (e.KeyData == Keys.F1)
            {
                textBox1.Focus();
            }
            else if (e.KeyData == Keys.F2)
            {
                textBox2.Focus();
            }
            else if (e.KeyData == Keys.F3)
            {
                dataGridView1.Focus();
            }
            else if (e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            selec = true;
        }

        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "PrecioVenta" ||
             dataGridView1.Columns[e.ColumnIndex].Name == "PrecioVentaMayoreo")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("C2"); // Formato moneda con 2 decimales
                    e.FormattingApplied = true;
                }
            }
        }
    }
}
