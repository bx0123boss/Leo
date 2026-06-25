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

namespace JaegerSoft
{
    public partial class frmAgregarInventario : Form
    {
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        OleDbCommand cmd;

        // Custom fields for new database columns
        private System.Windows.Forms.Label lblStockMaximo;
        private System.Windows.Forms.TextBox txtStockMaximo;
        private System.Windows.Forms.Label lblDiasReposicion;
        private System.Windows.Forms.TextBox txtDiasReposicion;
        private System.Windows.Forms.Label lblProveedor;
        private System.Windows.Forms.ComboBox cmbProveedor;

        public frmAgregarInventario()
        {
            InitializeComponent();
            InitializeCustomControls();
            conectar.Open();
        }

        private void InitializeCustomControls()
        {
            this.lblStockMaximo = new System.Windows.Forms.Label();
            this.txtStockMaximo = new System.Windows.Forms.TextBox();
            this.lblDiasReposicion = new System.Windows.Forms.Label();
            this.txtDiasReposicion = new System.Windows.Forms.TextBox();
            this.lblProveedor = new System.Windows.Forms.Label();
            this.cmbProveedor = new System.Windows.Forms.ComboBox();

            // lblStockMaximo
            this.lblStockMaximo.AutoSize = true;
            this.lblStockMaximo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStockMaximo.ForeColor = System.Drawing.Color.White;
            this.lblStockMaximo.Location = new System.Drawing.Point(34, 325);
            this.lblStockMaximo.Name = "lblStockMaximo";
            this.lblStockMaximo.Size = new System.Drawing.Size(120, 18);
            this.lblStockMaximo.Text = "Stock Máximo:";

            // txtStockMaximo
            this.txtStockMaximo.Location = new System.Drawing.Point(162, 321);
            this.txtStockMaximo.Name = "txtStockMaximo";
            this.txtStockMaximo.Size = new System.Drawing.Size(379, 26);
            this.txtStockMaximo.Text = "0";
            this.txtStockMaximo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtStockMaximo_KeyPress);

            // lblDiasReposicion
            this.lblDiasReposicion.AutoSize = true;
            this.lblDiasReposicion.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDiasReposicion.ForeColor = System.Drawing.Color.White;
            this.lblDiasReposicion.Location = new System.Drawing.Point(18, 357);
            this.lblDiasReposicion.Name = "lblDiasReposicion";
            this.lblDiasReposicion.Size = new System.Drawing.Size(139, 18);
            this.lblDiasReposicion.Text = "Días Reposición:";

            // txtDiasReposicion
            this.txtDiasReposicion.Location = new System.Drawing.Point(162, 353);
            this.txtDiasReposicion.Name = "txtDiasReposicion";
            this.txtDiasReposicion.Size = new System.Drawing.Size(379, 26);
            this.txtDiasReposicion.Text = "0";
            this.txtDiasReposicion.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDiasReposicion_KeyPress);

            // lblProveedor
            this.lblProveedor.AutoSize = true;
            this.lblProveedor.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProveedor.ForeColor = System.Drawing.Color.White;
            this.lblProveedor.Location = new System.Drawing.Point(66, 389);
            this.lblProveedor.Name = "lblProveedor";
            this.lblProveedor.Size = new System.Drawing.Size(91, 18);
            this.lblProveedor.Text = "Proveedor:";

            // cmbProveedor
            this.cmbProveedor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProveedor.FormattingEnabled = true;
            this.cmbProveedor.Location = new System.Drawing.Point(162, 385);
            this.cmbProveedor.Name = "cmbProveedor";
            this.cmbProveedor.Size = new System.Drawing.Size(379, 28);

            // Add controls to groupBox1
            this.groupBox1.Controls.Add(this.lblStockMaximo);
            this.groupBox1.Controls.Add(this.txtStockMaximo);
            this.groupBox1.Controls.Add(this.lblDiasReposicion);
            this.groupBox1.Controls.Add(this.txtDiasReposicion);
            this.groupBox1.Controls.Add(this.lblProveedor);
            this.groupBox1.Controls.Add(this.cmbProveedor);

            // Adjust layout of parent container
            this.groupBox1.Height = 480;
            this.button1.Location = new System.Drawing.Point(245, 430);
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
        }        private void txtID_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtID.Text)) return;

            int valor = 0;
            using (OleDbCommand cmdCheck = new OleDbCommand("select count(*) from [" + lblOrigen.Text + "] where Id = ?;", conectar))
            {
                cmdCheck.Parameters.AddWithValue("?", txtID.Text);
                valor = Convert.ToInt32(cmdCheck.ExecuteScalar());
            }

            if (valor == 1)
            {
                MessageBox.Show("El producto ya existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtID.Clear();
                txtID.Focus();
            }
            else
            {
                using (OleDbCommand cmdCheckSusp = new OleDbCommand("select count(*) from InventarioSusp where Id = ?;", conectar))
                {
                    cmdCheckSusp.Parameters.AddWithValue("?", txtID.Text);
                    valor = Convert.ToInt32(cmdCheckSusp.ExecuteScalar());
                }

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
            comboBox1.SelectedIndex = 0;
            LlenarProveedores();
        }

        private void LlenarProveedores()
        {
            try
            {
                DataTable dt = new DataTable();
                using (OleDbCommand cmdProv = new OleDbCommand("Select Id, Nombre from Proveedores ORDER BY Nombre;", conectar))
                {
                    using (OleDbDataAdapter da = new OleDbDataAdapter(cmdProv))
                    {
                        da.Fill(dt);
                    }
                }
                cmbProveedor.DisplayMember = "Nombre";
                cmbProveedor.ValueMember = "Nombre";
                cmbProveedor.DataSource = dt;
                cmbProveedor.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar proveedores: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || ParseInt(textBox1.Text) < 0)
            {
                MessageBox.Show("Número incorrecto en las existencias a añadir, favor de introducir un número válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    using (OleDbCommand cmdUpdate = new OleDbCommand("UPDATE [" + lblOrigen.Text + "] set Existencia = ? Where Id = ?;", conectar))
                    {
                        cmdUpdate.Parameters.AddWithValue("?", ParseDouble(txtTotales.Text));
                        cmdUpdate.Parameters.AddWithValue("?", txtID.Text);
                        cmdUpdate.ExecuteNonQuery();
                    }

                    using (OleDbCommand cmdKardex = new OleDbCommand("insert into Kardex (IdProducto,Tipo,Descripcion,ExistenciaAntes,ExistenciaDespues,Fecha) values (?, 'ENTRADA', 'ENTRADA DE ARTICULO', ?, ?, ?);", conectar))
                    {
                        cmdKardex.Parameters.AddWithValue("?", txtID.Text);
                        cmdKardex.Parameters.AddWithValue("?", ParseDouble(txtActuales.Text));
                        cmdKardex.Parameters.AddWithValue("?", ParseDouble(txtTotales.Text));
                        cmdKardex.Parameters.AddWithValue("?", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                        cmdKardex.ExecuteNonQuery();
                    }

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
                catch (Exception ex)
                {
                    MessageBox.Show("Error al añadir existencias: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            txtTotales.Text = "" + (ParseInt(txtActuales.Text) + ParseInt(textBox1.Text));
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
                double especial = ParseDouble(txtEspecial.Text);

                if (venta < 0 || compra < 0 || especial < 0)
                {
                    MessageBox.Show("Los precios no pueden ser negativos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string sql = "insert into [" + lblOrigen.Text + "] (Id, Nombre, PrecioVentaMayoreo, PrecioVenta, Existencia, Limite, Categoria, Especial, IVA, Unidad, Uni, ProveedorPrincipal, StockMaximo, DiasReposicion) " +
                             "values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);";

                using (OleDbCommand cmdInsert = new OleDbCommand(sql, conectar))
                {
                    cmdInsert.Parameters.AddWithValue("?", txtID.Text);
                    cmdInsert.Parameters.AddWithValue("?", txtProducto.Text);
                    cmdInsert.Parameters.AddWithValue("?", ParseDecimal(txtCompra.Text)); // PrecioVentaMayoreo
                    cmdInsert.Parameters.AddWithValue("?", ParseDecimal(txtVenta.Text));  // PrecioVenta
                    cmdInsert.Parameters.AddWithValue("?", ParseDouble(txtCantidad.Text)); // Existencia
                    cmdInsert.Parameters.AddWithValue("?", ParseInt(txtLimite.Text));      // Limite (Stock Minimo)
                    cmdInsert.Parameters.AddWithValue("?", cmbCategoria.Text);
                    cmdInsert.Parameters.AddWithValue("?", ParseDouble(txtEspecial.Text)); // Especial (Precio Compra)
                    cmdInsert.Parameters.AddWithValue("?", comboBox1.Text);               // IVA (Impuesto)
                    cmdInsert.Parameters.AddWithValue("?", cmbUnidad.SelectedValue != null ? cmbUnidad.SelectedValue.ToString() : "");
                    cmdInsert.Parameters.AddWithValue("?", cmbUnidad.Text);
                    cmdInsert.Parameters.AddWithValue("?", cmbProveedor.Text);            // ProveedorPrincipal
                    cmdInsert.Parameters.AddWithValue("?", ParseDouble(txtStockMaximo.Text)); // StockMaximo
                    cmdInsert.Parameters.AddWithValue("?", ParseInt(txtDiasReposicion.Text)); // DiasReposicion

                    cmdInsert.ExecuteNonQuery();
                }

                using (OleDbCommand cmdKardex = new OleDbCommand("insert into Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha) values (?, 'ENTRADA', 'CREACION DE ARTICULO', 0, ?, ?);", conectar))
                {
                    cmdKardex.Parameters.AddWithValue("?", txtID.Text);
                    cmdKardex.Parameters.AddWithValue("?", ParseDouble(txtCantidad.Text));
                    cmdKardex.Parameters.AddWithValue("?", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                    cmdKardex.ExecuteNonQuery();
                }

                MessageBox.Show("Se ha agregado el producto con éxito", "AGREGADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                groupBox1.Enabled = false;
                txtProducto.Clear();
                txtCompra.Clear();
                txtVenta.Clear();
                txtCantidad.Clear();
                txtID.Clear();
                comboBox1.Text = "";
                txtID.Focus();
                txtEspecial.Clear();
                txtLimite.Clear();
                cmbUnidad.Text = "";
                if (txtStockMaximo != null) txtStockMaximo.Text = "0";
                if (txtDiasReposicion != null) txtDiasReposicion.Text = "0";
                if (cmbProveedor != null) cmbProveedor.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar el producto: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
