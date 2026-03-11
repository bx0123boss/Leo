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
                da = new OleDbDataAdapter("select top 100 Id,Nombre,PrecioVenta,PrecioVentaMayoreo,Existencia,Especial,IVA from Inventario where Nombre LIKE '%" + textBox1.Text.Replace("'", "''") + "%' ORDER BY Nombre ;", conectar);
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
            SeleccionarProducto();
        }

        // ---------- NUEVOS MÉTODOS PARA SELECCIONAR PRODUCTO ----------

        private void SeleccionarProducto()
        {
            if (selec && dataGridView1.CurrentRow != null)
            {
                var filaActual = dataGridView1.Rows[dataGridView1.CurrentRow.Index];

                if (compras)
                {
                    // Si estamos en compras, toma el Precio Venta por defecto sin preguntar
                    double preci = Convert.ToDouble(filaActual.Cells["PrecioVenta"].Value.ToString());
                    AsignarValoresYSalir(filaActual, preci);
                }
                else
                {
                    // Módulo de ventas: abrimos frmPrecio para elegir GEN o MAYOREO
                    using (frmPrecio buscar = new frmPrecio())
                    {
                        if (buscar.ShowDialog() == DialogResult.OK)
                        {
                            double preci = 0;
                            if (buscar.tipo == "GEN")
                            {
                                // Toma el Precio Venta Normal
                                preci = Convert.ToDouble(filaActual.Cells["PrecioVenta"].Value.ToString());
                            }
                            else
                            {
                                // Toma el Precio Venta Mayoreo
                                preci = Convert.ToDouble(filaActual.Cells["PrecioVentaMayoreo"].Value.ToString());
                            }

                            AsignarValoresYSalir(filaActual, preci);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No se ha seleccionado ningun producto", "¡ALTO!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void AsignarValoresYSalir(DataGridViewRow filaActual, double preci)
        {
            try
            {
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

        // --------------------------------------------------------------

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
            // 1. SOLUCIÓN: Si el control activo es un TextBox, ignoramos este evento
            // para que el Enter lo procese el evento KeyPress del TextBox y haga la búsqueda.
            if (e.KeyData == Keys.Enter && (this.ActiveControl == textBox1 || this.ActiveControl == textBox2))
            {
                return;
            }

            // 2. Si NO estamos en un TextBox y presionamos Enter, seleccionamos el producto
            if (e.KeyData == Keys.Enter)
            {
                SeleccionarProducto();
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