using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JaegerSoft
{
    public partial class frmEditarProducto : Form
    {
        SqlConnection conectar = new SqlConnection(Conexion.CadSQL); 
        //SqlConnection conectar = new SqlConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        SqlCommand cmd;
        public string usuario = "";
        public string inventario = "";
        double precio;
        private string rutaImagenSeleccionada = "";
        public string categoriaSeleccionada = ""; // Variable para almacenar la categoría seleccionada

        // New custom controls for expanded fields
        public System.Windows.Forms.Label lblLimiteReal;
        public System.Windows.Forms.TextBox txtLimiteReal;
        public System.Windows.Forms.Label lblStockMaximo;
        public System.Windows.Forms.TextBox txtStockMaximo;
        public System.Windows.Forms.Label lblDiasReposicion;
        public System.Windows.Forms.TextBox txtDiasReposicion;
        public System.Windows.Forms.Label lblProveedor;
        public System.Windows.Forms.ComboBox cmbProveedor;
        public System.Windows.Forms.CheckBox chkManejaInventario;

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
            this.chkManejaInventario = new System.Windows.Forms.CheckBox();
            this.chkEsInsumo = new System.Windows.Forms.CheckBox();
            this.chkEsReceta = new System.Windows.Forms.CheckBox();

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

            // chkManejaInventario
            this.chkManejaInventario.AutoSize = true;
            this.chkManejaInventario.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkManejaInventario.ForeColor = System.Drawing.Color.White;
            this.chkManejaInventario.Location = new System.Drawing.Point(166, 335);
            this.chkManejaInventario.Name = "chkManejaInventario";
            this.chkManejaInventario.Size = new System.Drawing.Size(164, 22);
            this.chkManejaInventario.Text = "Maneja Inventario";
            this.chkManejaInventario.Checked = true;
            this.chkManejaInventario.UseVisualStyleBackColor = true;
            // chkEsInsumo
            this.chkEsInsumo.AutoSize = true;
            this.chkEsInsumo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.chkEsInsumo.ForeColor = System.Drawing.Color.White;
            this.chkEsInsumo.Location = new System.Drawing.Point(45, 365);
            this.chkEsInsumo.Name = "chkEsInsumo";
            this.chkEsInsumo.Size = new System.Drawing.Size(120, 21);
            this.chkEsInsumo.Text = "Es Insumo";
            this.chkEsInsumo.UseVisualStyleBackColor = true;

            // chkEsReceta
            this.chkEsReceta.AutoSize = true;
            this.chkEsReceta.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.chkEsReceta.ForeColor = System.Drawing.Color.White;
            this.chkEsReceta.Location = new System.Drawing.Point(180, 365);
            this.chkEsReceta.Name = "chkEsReceta";
            this.chkEsReceta.Size = new System.Drawing.Size(125, 21);
            this.chkEsReceta.Text = "Es Receta";
            this.chkEsReceta.UseVisualStyleBackColor = true;

            // Add to controls list
            this.Controls.Add(this.lblLimiteReal);
            this.Controls.Add(this.txtLimiteReal);
            this.Controls.Add(this.lblStockMaximo);
            this.Controls.Add(this.txtStockMaximo);
            this.Controls.Add(this.lblDiasReposicion);
            this.Controls.Add(this.txtDiasReposicion);
            this.Controls.Add(this.lblProveedor);
            this.Controls.Add(this.cmbProveedor);
            this.Controls.Add(this.chkManejaInventario);
            this.Controls.Add(this.chkEsInsumo);
            this.Controls.Add(this.chkEsReceta);

            // Move button1 (Editar) down
            this.button1.Location = new System.Drawing.Point(166, 400);
            //this.ClientSize = new System.Drawing.Size(953, 323);
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
                             "Nombre = @Nombre, " +
                             "PrecioVenta = @PrecioVenta, " +
                             "PrecioVentaMayoreo = @PrecioVentaMayoreo, " +
                             "Categoria = @Categoria, " +
                             "Especial = @Especial, " +
                             "IVA = @IVA, " +
                             "Unidad = @Unidad, " +
                             "Uni = @Uni, " +
                             "ProveedorPrincipal = @ProveedorPrincipal, " +
                             "StockMaximo = @StockMaximo, " +
                             "DiasReposicion = @DiasReposicion, " +
                             "Limite = @Limite, " +
                             "ManejaInventario = @ManejaInventario, " +
                             "EsInsumo = @EsInsumo, " +
                             "EsReceta = @EsReceta ," +
                             "ImagenUrl = @ImagenUrl " +
                             "WHERE Id = @Id;";

                using (SqlCommand cmdUpdate = new SqlCommand(sql, conectar))
                {
                    cmdUpdate.Parameters.AddWithValue("@Nombre", txtProducto.Text);
                    cmdUpdate.Parameters.AddWithValue("@PrecioVenta", ParseDecimal(txtVenta.Text));         // PrecioVenta
                    cmdUpdate.Parameters.AddWithValue("@PrecioVentaMayoreo", ParseDecimal(txtCompra.Text));        // PrecioVentaMayoreo
                    cmdUpdate.Parameters.AddWithValue("@Categoria", comboBox1.Text);                      // Categoria
                    cmdUpdate.Parameters.AddWithValue("@Especial", ParseDouble(txtLimite.Text));         // Especial (Precio Compra)
                    cmdUpdate.Parameters.AddWithValue("@IVA", cmbCategoria.Text);                   // IVA (Impuesto)
                    cmdUpdate.Parameters.AddWithValue("@Unidad", cmbUnidad.SelectedValue != null ? cmbUnidad.SelectedValue.ToString() : "");
                    cmdUpdate.Parameters.AddWithValue("@Uni", cmbUnidad.Text);
                    cmdUpdate.Parameters.AddWithValue("@ProveedorPrincipal", cmbProveedor.Text);                   // ProveedorPrincipal
                    cmdUpdate.Parameters.AddWithValue("@StockMaximo", ParseDouble(txtStockMaximo.Text));    // StockMaximo
                    cmdUpdate.Parameters.AddWithValue("@DiasReposicion", ParseInt(txtDiasReposicion.Text));    // DiasReposicion
                    cmdUpdate.Parameters.AddWithValue("@Limite", ParseInt(txtLimiteReal.Text));        // Limite (Stock Minimo)
                    cmdUpdate.Parameters.AddWithValue("@ManejaInventario", chkManejaInventario.Checked);         // ManejaInventario
                    cmdUpdate.Parameters.AddWithValue("@EsInsumo", chkEsInsumo.Checked);                 // EsInsumo
                    cmdUpdate.Parameters.AddWithValue("@EsReceta", chkEsReceta.Checked);
                    string imagenUrl = GuardarImagenProducto(txtID.Text);
                    cmdUpdate.Parameters.AddWithValue("@ImagenUrl", (object)imagenUrl ?? DBNull.Value);
                    cmdUpdate.Parameters.AddWithValue("@Id", txtID.Text);                          // WHERE Id

                    cmdUpdate.ExecuteNonQuery();
                }

                if (precio != venta)
                {
                    string sqlKardex = "insert into Kardex (IdProducto, Tipo, Descripcion, ExistenciaAntes, ExistenciaDespues, Fecha, Precio) " +
                                       "values (@IdProducto, 'PRECIO', 'MODIFICACION DE PRECIO', 0, '0', @Fecha, @Precio);";
                    using (SqlCommand cmdKardex = new SqlCommand(sqlKardex, conectar))
                    {
                        cmdKardex.Parameters.AddWithValue("@IdProducto", txtID.Text);
                        cmdKardex.Parameters.AddWithValue("@Fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmdKardex.Parameters.AddWithValue("@Precio", txtVenta.Text);
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
        private string GuardarImagenProducto(string idProducto)
        {
            if (string.IsNullOrWhiteSpace(rutaImagenSeleccionada))
                return null;

            string carpetaDestino = @"C:\Jaeger Soft\ModuloWeb\wwwroot\images\productos";
            if (!Directory.Exists(carpetaDestino))
                Directory.CreateDirectory(carpetaDestino);

            string extension = Path.GetExtension(rutaImagenSeleccionada);
            string nombreArchivo = idProducto + extension;
            string destino = Path.Combine(carpetaDestino, nombreArchivo);

            // Si el origen y el destino son el mismo archivo, no hagas nada
            if (string.Equals(
                    Path.GetFullPath(rutaImagenSeleccionada),
                    Path.GetFullPath(destino),
                    StringComparison.OrdinalIgnoreCase))
            {
                return $"images/productos/{nombreArchivo}";
            }

            // Liberar imagen antes de sobrescribir
            if (picProducto.Image != null)
            {
                picProducto.Image.Dispose();
                picProducto.Image = null;
            }

            File.Copy(rutaImagenSeleccionada, destino, true);

            return $"images/productos/{nombreArchivo}";
        }
        /// <summary>
        /// Carga en el PictureBox la imagen guardada, si el archivo existe físicamente.
        /// </summary>
        private void CargarImagenProducto(string imagenUrl)
        {
            if (string.IsNullOrWhiteSpace(imagenUrl))
                return;
            string raizWeb = @"C:\Jaeger Soft\ModuloWeb\wwwroot";
            string rutaFisica = Path.Combine(raizWeb, imagenUrl.Replace('/', '\\'));

            if (!File.Exists(rutaFisica))
                return; // No existe el archivo, dejamos el PictureBox vacío

            try
            {
                // Liberar imagen previa para no bloquear el archivo
                if (picProducto.Image != null)
                {
                    picProducto.Image.Dispose();
                    picProducto.Image = null;
                }

                // Cargamos desde un MemoryStream para NO dejar el archivo bloqueado
                byte[] bytes = File.ReadAllBytes(rutaFisica);
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    picProducto.Image = Image.FromStream(ms);
                }

                // Guardamos la ruta por si el usuario no cambia la imagen al editar
                rutaImagenSeleccionada = rutaFisica;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cargar la imagen: " + ex.Message);
            }
        }
        private void frmEditarProducto_Load(object sender, EventArgs e)
        {
            string categoria = comboBox1.Text;
            string proveedor = cmbProveedor.Text;
            conectar.Open();
            
            // Populate Categorias
            DataTable dtCat = new DataTable();
            using (cmd = new SqlCommand("Select Id,Nombre from Categorias;", conectar))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dtCat);
                }
            }
            comboBox1.DisplayMember = "Nombre";
            comboBox1.ValueMember = "Id";
            comboBox1.DataSource = dtCat;
            if(categoriaSeleccionada == "")
                comboBox1.Text = categoria;
            else
                comboBox1.Text = categoriaSeleccionada;
            // Populate Proveedores
            DataTable dtProv = new DataTable();
            using (cmd = new SqlCommand("Select Id,Nombre from Proveedores ORDER BY Nombre;", conectar))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dtProv);
                }
            }
            cmbProveedor.DisplayMember = "Nombre";
            cmbProveedor.ValueMember = "Nombre";
            cmbProveedor.DataSource = dtProv;
            cmbProveedor.Text = proveedor;

         

            try
            {
                precio = Convert.ToDouble(txtVenta.Text);
                using (SqlCommand cmdFlags = new SqlCommand(
                    "SELECT ManejaInventario, EsInsumo, EsReceta, ImagenUrl FROM Inventario WHERE Id = @Id", conectar))
                {
                    cmdFlags.Parameters.AddWithValue("@Id", txtID.Text);

                    using (SqlDataReader dr = cmdFlags.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            if (dr["ManejaInventario"] != DBNull.Value)
                                chkManejaInventario.Checked = Convert.ToBoolean(dr["ManejaInventario"]);

                            if (dr["EsInsumo"] != DBNull.Value)
                                chkEsInsumo.Checked = Convert.ToBoolean(dr["EsInsumo"]);

                            if (dr["EsReceta"] != DBNull.Value)
                                chkEsReceta.Checked = Convert.ToBoolean(dr["EsReceta"]);

                            // âœ… Cargar la imagen si existe
                            if (dr["ImagenUrl"] != DBNull.Value)
                            {
                                string imagenUrl = dr["ImagenUrl"].ToString();
                                CargarImagenProducto(imagenUrl);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // No reventar el formulario si algo falla al cargar la imagen
                Console.WriteLine("Error al cargar flags/imagen: " + ex.Message);
            }
            AbrirTeclado();
        }
        private void AbrirTeclado()
        {
            string RutaTeclado = @"C:\Jaeger Soft\FreeVK.exe";
            try
            {
                if (File.Exists(RutaTeclado))
                {
                    // Verifica si el proceso YA está corriendo para evitar abrir múltiples instancias
                    if (Process.GetProcessesByName("FreeVK").Length == 0)
                    {
                        Process.Start(RutaTeclado);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al abrir el teclado: " + ex.Message);
            }
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

        private void txtLimite_KeyPress(object sender, KeyPressEventArgs e)
        {
            CultureInfo cc = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (char.IsNumber(e.KeyChar) || e.KeyChar.ToString() == cc.NumberFormat.NumberDecimalSeparator || Convert.ToInt32(e.KeyChar) == 8)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void btnSeleccionarImagen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Seleccionar imagen";
                ofd.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    rutaImagenSeleccionada = ofd.FileName;
                    try
                    {
                        // Liberamos la imagen anterior para no bloquear el archivo
                        if (picProducto.Image != null)
                        {
                            picProducto.Image.Dispose();
                            picProducto.Image = null;
                        }
                        picProducto.Image = Image.FromFile(rutaImagenSeleccionada);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
        }
    }
}

