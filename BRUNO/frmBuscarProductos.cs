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
    public partial class frmBuscarProductos : frmBase
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

        private void frmBuscarProductos_Load(object sender, EventArgs e)
        {
            EstilizarDataGridView(this.dataGridView1);
            EstilizarBotonPrimario(this.button1);
            EstilizarTextBox(this.textBox1);
            EstilizarTextBox(this.textBox2);

            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.KeyPreview = true;
            conectar.Open();

            if (textBox1.Text != "")
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select top 100 Id,Nombre,PrecioVenta,PrecioVentaMayoreo,Existencia,Especial,IVA from Inventario where Nombre LIKE '%" + textBox1.Text + "%' ORDER BY Nombre ;", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                OcultarColumnasInternas();
            }
            else
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select top 100 Id,Nombre,PrecioVenta, PrecioVentaMayoreo,Existencia,Especial,IVA from Inventario;", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                OcultarColumnasInternas();
            }
            textBox2.Focus();
        }

        // Método auxiliar para ocultar columnas por NOMBRE y no por número de índice
        private void OcultarColumnasInternas()
        {
            if (dataGridView1.Columns.Contains("Especial"))
                dataGridView1.Columns["Especial"].Visible = false;

            if (dataGridView1.Columns.Contains("IVA"))
                dataGridView1.Columns["IVA"].Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (selec)
            {
                var filaActual = dataGridView1.Rows[dataGridView1.CurrentRow.Index];

                if (compras)
                {
                    double preci = Convert.ToDouble(filaActual.Cells["PrecioVenta"].Value.ToString());
                    producto = filaActual.Cells["Nombre"].Value.ToString();
                    precio = String.Format("{0:0.00}", preci);
                    monto = String.Format("{0:0.00}", preci);
                    existencia = filaActual.Cells["Existencia"].Value.ToString();
                    ID = filaActual.Cells["Id"].Value.ToString();
                    IVA = filaActual.Cells["IVA"].Value.ToString();
                    compra = filaActual.Cells["Especial"].Value.ToString();
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                {
                    // Asignación mediante el nombre de la columna para evitar errores si el SQL cambia de orden
                    double preci = Convert.ToDouble(filaActual.Cells["PrecioVenta"].Value.ToString());
                    producto = filaActual.Cells["Nombre"].Value.ToString();
                    precio = String.Format("{0:0.00}", preci);
                    monto = String.Format("{0:0.00}", preci);
                    existencia = filaActual.Cells["Existencia"].Value.ToString();
                    ID = filaActual.Cells["Id"].Value.ToString();
                    IVA = filaActual.Cells["IVA"].Value.ToString();
                    compra = filaActual.Cells["Especial"].Value.ToString();
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
            }
            else
            {
                MessageBox.Show("No se ha seleccionado ningun producto", "¡ALTO!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (textBox2.Text == "")
                {
                    ds = new DataSet();
                    da = new OleDbDataAdapter("select Id,Nombre,PrecioVenta, PrecioVentaMayoreo,Existencia,Especial,IVA from Inventario ;", conectar);
                    da.Fill(ds, "Id");
                    dataGridView1.DataSource = ds.Tables["Id"];
                    OcultarColumnasInternas();
                }
                else
                {
                    ds = new DataSet();
                    da = new OleDbDataAdapter("select Id,Nombre,PrecioVenta, PrecioVentaMayoreo,Existencia,Especial,IVA from Inventario where Id LIKE '%" + textBox2.Text + "%' ORDER BY Nombre ;", conectar);
                    da.Fill(ds, "Id");
                    dataGridView1.DataSource = ds.Tables["Id"];
                    OcultarColumnasInternas();
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
                    da = new OleDbDataAdapter("select Id,Nombre,PrecioVenta,PrecioVentaMayoreo,Existencia,Especial,IVA from Inventario;", conectar);
                    da.Fill(ds, "Id");
                    dataGridView1.DataSource = ds.Tables["Id"];
                    OcultarColumnasInternas();
                }
                else
                {
                    ds = new DataSet();
                    da = new OleDbDataAdapter("select Id,Nombre,PrecioVenta, PrecioVentaMayoreo,Existencia,Especial,IVA from Inventario where Nombre LIKE '%" + textBox1.Text + "%'ORDER BY Nombre ;", conectar);
                    da.Fill(ds, "Id");
                    dataGridView1.DataSource = ds.Tables["Id"];
                    OcultarColumnasInternas();
                }
            }
        }

        private void frmBuscarProductos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (selec)
                {
                    var filaActual = dataGridView1.Rows[dataGridView1.CurrentRow.Index];

                    if (compras)
                    {
                        double preci = Convert.ToDouble(filaActual.Cells["PrecioVenta"].Value.ToString());
                        producto = filaActual.Cells["Nombre"].Value.ToString();
                        precio = String.Format("{0:0.00}", preci);
                        monto = String.Format("{0:0.00}", preci);
                        existencia = filaActual.Cells["Existencia"].Value.ToString();
                        ID = filaActual.Cells["Id"].Value.ToString();
                        IVA = filaActual.Cells["IVA"].Value.ToString();
                        compra = filaActual.Cells["Especial"].Value.ToString();
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    }
                    else
                    {
                        try
                        {
                            double preci = Convert.ToDouble(filaActual.Cells["PrecioVenta"].Value.ToString());
                            producto = filaActual.Cells["Nombre"].Value.ToString();
                            precio = String.Format("{0:0.00}", preci);
                            monto = String.Format("{0:0.00}", preci);
                            existencia = filaActual.Cells["Existencia"].Value.ToString();
                            ID = filaActual.Cells["Id"].Value.ToString();
                            IVA = filaActual.Cells["IVA"].Value.ToString();
                            compra = filaActual.Cells["Especial"].Value.ToString();
                            this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al extraer producto: " + ex.Message);
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