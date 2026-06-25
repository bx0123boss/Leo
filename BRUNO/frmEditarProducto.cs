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

namespace JaegerSoft
{
    public partial class frmEditarProducto : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbCommand cmd;
        public string usuario = "";
        public string inventario = "";
        double precio;

        // New custom controls for expanded fields
        public System.Windows.Forms.Label lblLimiteReal;
        public System.Windows.Forms.TextBox txtLimiteReal;
        public System.Windows.Forms.Label lblStockMaximo;
        public System.Windows.Forms.TextBox txtStockMaximo;
        public System.Windows.Forms.Label lblDiasReposicion;
        public System.Windows.Forms.TextBox txtDiasReposicion;
        public System.Windows.Forms.Label lblProveedor;
        public System.Windows.Forms.ComboBox cmbProveedor;

        public frmEditarProducto()
        {
            InitializeComponent();
            InitializeCustomControls();
        }

        private void InitializeCustomControls()
        {
            this.lblLimiteReal = new System.Windows.Forms.Label();
            this.txtLimiteReal = new System.Windows.Forms.TextBox();
            this.lblStockMaximo = new System.Windows.Forms.Label();
            this.txtStockMaximo = new System.Windows.Forms.TextBox();
            this.lblDiasReposicion = new System.Windows.Forms.Label();
            this.txtDiasReposicion = new System.Windows.Forms.TextBox();
            this.lblProveedor = new System.Windows.Forms.Label();
            this.cmbProveedor = new System.Windows.Forms.ComboBox();

            // Form client size adjustment
            this.ClientSize = new System.Drawing.Size(410, 400);

            // lblLimiteReal
            this.lblLimiteReal.AutoSize = true;
            this.lblLimiteReal.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLimiteReal.ForeColor = System.Drawing.Color.White;
            this.lblLimiteReal.Location = new System.Drawing.Point(46, 225);
            this.lblLimiteReal.Name = "lblLimiteReal";
            this.lblLimiteReal.Size = new System.Drawing.Size(113, 18);
            this.lblLimiteReal.Text = "Stock Mínimo:";

            // txtLimiteReal
            this.txtLimiteReal.Location = new System.Drawing.Point(166, 224);
            this.txtLimiteReal.Name = "txtLimiteReal";
            this.txtLimiteReal.Size = new System.Drawing.Size(234, 20);
            this.txtLimiteReal.Text = "0";
            this.txtLimiteReal.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtLimiteReal_KeyPress);

            // lblStockMaximo
            this.lblStockMaximo.AutoSize = true;
            this.lblStockMaximo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStockMaximo.ForeColor = System.Drawing.Color.White;
            this.lblStockMaximo.Location = new System.Drawing.Point(42, 252);
            this.lblStockMaximo.Name = "lblStockMaximo";
            this.lblStockMaximo.Size = new System.Drawing.Size(117, 18);
            this.lblStockMaximo.Text = "Stock Máximo:";

            // txtStockMaximo
            this.txtStockMaximo.Location = new System.Drawing.Point(166, 251);
            this.txtStockMaximo.Name = "txtStockMaximo";
            this.txtStockMaximo.Size = new System.Drawing.Size(234, 20);
            this.txtStockMaximo.Text = "0";
            this.txtStockMaximo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtStockMaximo_KeyPress);

            // lblDiasReposicion
            this.lblDiasReposicion.AutoSize = true;
            this.lblDiasReposicion.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDiasReposicion.ForeColor = System.Drawing.Color.White;
            this.lblDiasReposicion.Location = new System.Drawing.Point(25, 279);
            this.lblDiasReposicion.Name = "lblDiasReposicion";
            this.lblDiasReposicion.Size = new System.Drawing.Size(134, 18);
            this.lblDiasReposicion.Text = "Días Reposición:";

            // txtDiasReposicion
            this.txtDiasReposicion.Location = new System.Drawing.Point(166, 278);
            this.txtDiasReposicion.Name = "txtDiasReposicion";
            this.txtDiasReposicion.Size = new System.Drawing.Size(234, 20);
            this.txtDiasReposicion.Text = "0";
            this.txtDiasReposicion.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDiasReposicion_KeyPress);

            // lblProveedor
            this.lblProveedor.AutoSize = true;
            this.lblProveedor.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProveedor.ForeColor = System.Drawing.Color.White;
            this.lblProveedor.Location = new System.Drawing.Point(66, 306);
            this.lblProveedor.Name = "lblProveedor";
            this.lblProveedor.Size = new System.Drawing.Size(91, 18);
            this.lblProveedor.Text = "Proveedor:";

            // cmbProveedor
            this.cmbProveedor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProveedor.FormattingEnabled = true;
            this.cmbProveedor.Location = new System.Drawing.Point(166, 305);
            this.cmbProveedor.Name = "cmbProveedor";
            this.cmbProveedor.Size = new System.Drawing.Size(234, 21);

            // Add to controls list
            this.Controls.Add(this.lblLimiteReal);
            this.Controls.Add(this.txtLimiteReal);
            this.Controls.Add(this.lblStockMaximo);
            this.Controls.Add(this.txtStockMaximo);
            this.Controls.Add(this.lblDiasReposicion);
            this.Controls.Add(this.txtDiasReposicion);
            this.Controls.Add(this.lblProveedor);
            this.Controls.Add(this.cmbProveedor);

            // Move button1 (Editar) down
            this.button1.Location = new System.Drawing.Point(166, 345);
        }

        private void txtLimiteReal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsNumber(e.KeyChar) || Convert.ToInt32(e.KeyChar) == 8)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void txtStockMaximo_KeyPress(object sender, KeyPressEventArgs e)
        {
            CultureInfo cc = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (char.IsNumber(e.KeyChar) || e.KeyChar.ToString() == cc.NumberFormat.NumberDecimalSeparator || Convert.ToInt32(e.KeyChar) == 8)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void txtDiasReposicion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsNumber(e.KeyChar) || Convert.ToInt32(e.KeyChar) == 8)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private decimal ParseDecimal(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;
            decimal val;
            if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out val)) return val;
            if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out val)) return val;
            return 0;
        }

        private double ParseDouble(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;
            double val;
            if (double.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out val)) return val;
            if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out val)) return val;
            return 0;
        }

        private int ParseInt(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;
            int val;
            if (int.TryParse(text, out val)) return val;
            return 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtID.Text) || string.IsNullOrWhiteSpace(txtProducto.Text))
            {
                MessageBox.Show("El ID y el Nombre del producto son obligatorios.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                double venta = ParseDouble(txtVenta.Text);
                double compra = ParseDouble(txtCompra.Text);
                double especial = ParseDouble(txtLimite.Text);

                if (venta < 0 || compra < 0 || especial < 0)
                {
                    MessageBox.Show("Los precios no pueden ser negativos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string sql = "UPDATE Inventario SET " +
                             "Nombre = ?, " +
                             "PrecioVenta = ?, " +
                             "PrecioVentaMayoreo = ?, " +
                             "Categoria = ?, " +
                             "Especial = ?, " +
                             "IVA = ?, " +
                             "Unidad = ?, " +
                             "Uni = ?, " +
                             "ProveedorPrincipal = ?, " +
                             "StockMaximo = ?, " +
                             "DiasReposicion = ?, " +
                             "Limite = ? " +
                             "WHERE Id = ?;";

                using (OleDbCommand cmdUpdate = new OleDbCommand(sql, conectar))
                {
                    cmdUpdate.Parameters.AddWithValue("?", txtProducto.Text);
                    cmdUpdate.Parameters.AddWithValue("?", ParseDecimal(txtVenta.Text));         // PrecioVenta
                    cmdUpdate.Parameters.AddWithValue("?", ParseDecimal(txtCompra.Text));        // PrecioVentaMayoreo
                    cmdUpdate.Parameters.AddWithValue("?", comboBox1.Text);                      // Categoria
                    cmdUpdate.Parameters.AddWithValue("?", ParseDouble(txtLimite.Text));         // Especial (Precio Compra)
                    cmdUpdate.Parameters.AddWithValue("?", cmbCategoria.Text);                   // IVA (Impuesto)
                    cmdUpdate.Parameters.AddWithValue("?", cmbUnidad.SelectedValue != null ? cmbUnidad.SelectedValue.ToString() : "");
                    cmdUpdate.Parameters.AddWithValue("?", cmbUnidad.Text);
                    cmdUpdate.Parameters.AddWithValue("?", cmbProveedor.Text);                   // ProveedorPrincipal
                    cmdUpdate.Parameters.AddWithValue("?", ParseDouble(txtStockMaximo.Text));    // StockMaximo
                    cmdUpdate.Parameters.AddWithValue("?", ParseInt(txtDiasReposicion.Text));    // DiasReposicion
                    cmdUpdate.Parameters.AddWithValue("?", ParseInt(txtLimiteReal.Text));        // Limite (Stock Minimo)
                    cmdUpdate.Parameters.AddWithValue("?", txtID.Text);                          // WHERE Id

                    cmdUpdate.ExecuteNonQuery();
                }

                if (precio != venta)
                {
                    string sqlKardex = "insert into Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha, Precio) " +
                                       "values (?, 'PRECIO', 'MODIFICACION DE PRECIO', 0, '0', ?, ?);";
                    using (OleDbCommand cmdKardex = new OleDbCommand(sqlKardex, conectar))
                    {
                        cmdKardex.Parameters.AddWithValue("?", txtID.Text);
                        cmdKardex.Parameters.AddWithValue("?", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                        cmdKardex.Parameters.AddWithValue("?", txtVenta.Text);
                        cmdKardex.ExecuteNonQuery();
                    }
                }

                conectar.Close();
                MessageBox.Show("Se ha actualizado el producto con éxito!", "Editar Oro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar el producto: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmEditarProducto_Load(object sender, EventArgs e)
        {
            string categoria = comboBox1.Text;
            string proveedor = cmbProveedor.Text;
            conectar.Open();
            
            // Populate Categorias
            DataTable dtCat = new DataTable();
            using (cmd = new OleDbCommand("Select Id,Nombre from Categorias;", conectar))
            {
                using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                {
                    da.Fill(dtCat);
                }
            }
            comboBox1.DisplayMember = "Nombre";
            comboBox1.ValueMember = "Id";
            comboBox1.DataSource = dtCat;
            comboBox1.Text = categoria;

            // Populate Proveedores
            DataTable dtProv = new DataTable();
            using (cmd = new OleDbCommand("Select Id,Nombre from Proveedores ORDER BY Nombre;", conectar))
            {
                using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                {
                    da.Fill(dtProv);
                }
            }
            cmbProveedor.DisplayMember = "Nombre";
            cmbProveedor.ValueMember = "Nombre";
            cmbProveedor.DataSource = dtProv;
            cmbProveedor.Text = proveedor;

            precio = Convert.ToDouble(txtVenta.Text);
        }

        private void frmEditarProducto_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (inventario == "PERFUME")
            {
                frmPerfumeria invent = new frmPerfumeria();
                invent.usuario = usuario;
                invent.Show();
            }
            else if (inventario == "INVENT")
            {
              
            }
            else if (inventario == "RELOJ")
            {
                frmRelojeria invent = new frmRelojeria();
                invent.usuario = usuario;
                invent.Show();
            }

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

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void cmbSub_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void cmbCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void txtLimite_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void txtVenta_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCompra_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void txtID_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void txtProducto_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtLimite_KeyPress(object sender, KeyPressEventArgs e)
        {
            CultureInfo cc = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (char.IsNumber(e.KeyChar) || e.KeyChar.ToString() == cc.NumberFormat.NumberDecimalSeparator || Convert.ToInt32(e.KeyChar) == 8)
                e.Handled = false;
            else
                e.Handled = true;
        }
    }
}
