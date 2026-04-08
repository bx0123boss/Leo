using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmLiquidarConsignacion : frmBase
    {
        public int IdConsignacion { get; set; }
        public string EstadoConsignacion { get; set; }
        public string Folio { get; set; }

        public frmLiquidarConsignacion()
        {
            InitializeComponent();
            this.MinimumSize = new Size(900, 500);
        }

        private void frmLiquidarConsignacion_Load(object sender, EventArgs e)
        {
            this.Text = "Detalle / Liquidación - " + Folio;
            lblFolio.Text = "Folio: " + Folio;
            lblEstado.Text = "Estado: " + EstadoConsignacion;

            EstilizarDataGridView(dataGridView1);
            EstilizarBotonPrimario(btnLiquidar);

            // Si ya está liquidada, bloqueamos todo para que sea solo de "Ver"
            if (EstadoConsignacion == "Liquidada")
            {
                btnLiquidar.Visible = false;
                dataGridView1.ReadOnly = true;
                lblAviso.Text = "Esta consignación ya fue liquidada. Modo de solo lectura.";
                lblAviso.ForeColor = Color.LimeGreen;
            }
            else
            {
                lblAviso.Text = "Escriba en la columna 'Devueltos' la cantidad de piezas que regresó el cliente.";
                lblAviso.ForeColor = Color.Gold;
            }

            CargarDetalles();
        }

        private void CargarDetalles()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.CadSQL))
                {
                    con.Open();

                    // Traemos los datos del cliente y totales
                    string queryPadre = "SELECT ClienteNombre, Fecha, Observaciones, Datos FROM Consignaciones WHERE Id = @Id";
                    using (SqlCommand cmd = new SqlCommand(queryPadre, con))
                    {
                        cmd.Parameters.AddWithValue("@Id", IdConsignacion);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblCliente.Text = "Cliente: " + reader["ClienteNombre"].ToString();
                                lblFecha.Text = "Fecha Salida: " + Convert.ToDateTime(reader["Fecha"]).ToString("dd/MM/yyyy HH:mm");
                            }
                        }
                    }

                    // Traemos el detalle de productos. 
                    // Si es la primera vez, Devueltos y Vendidos serán 0 o null en BD, nosotros lo manejamos en el Grid.
                    string queryDetalle = @"SELECT Id, ProductoCodigo AS Codigo, Descripcion, PrecioUnitario AS Precio, 
                                            CantidadLlevada AS Llevados, 
                                            ISNULL(CantidadDevuelta, 0) AS Devueltos, 
                                            ISNULL(CantidadVendida, 0) AS Vendidos, 
                                            ISNULL(ImporteTotalLlevado, 0) AS Subtotal 
                                            FROM DetalleConsignacion 
                                            WHERE ConsignacionId = @Id";

                    using (SqlCommand cmdDet = new SqlCommand(queryDetalle, con))
                    {
                        cmdDet.Parameters.AddWithValue("@Id", IdConsignacion);
                        SqlDataAdapter da = new SqlDataAdapter(cmdDet);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dataGridView1.DataSource = dt;

                        // Configuramos las columnas visualmente
                        dataGridView1.Columns["Id"].Visible = false;

                        // Solo permitimos editar la columna de "Devueltos" si no está liquidada
                        foreach (DataGridViewColumn col in dataGridView1.Columns)
                        {
                            if (col.Name != "Devueltos")
                            {
                                col.ReadOnly = true;
                            }
                        }
                    }
                }

                RecalcularTotales();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los detalles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Magia del Grid: Cuando el usuario escribe cuántos devolvieron, calculamos automáticamente los vendidos
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Devueltos")
            {
                try
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                    double llevados = Convert.ToDouble(row.Cells["Llevados"].Value);
                    double devueltos = Convert.ToDouble(row.Cells["Devueltos"].Value);

                    // Validación: No pueden devolver más de los que se llevaron
                    if (devueltos > llevados || devueltos < 0)
                    {
                        MessageBox.Show("Cantidad de devolución inválida.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        row.Cells["Devueltos"].Value = 0;
                        devueltos = 0;
                    }

                    double vendidos = llevados - devueltos;
                    double precio = Convert.ToDouble(row.Cells["Precio"].Value);
                    double subtotal = vendidos * precio;

                    row.Cells["Vendidos"].Value = vendidos;
                    row.Cells["Subtotal"].Value = subtotal;

                    RecalcularTotales();
                }
                catch
                {
                    MessageBox.Show("Solo ingrese números.", "Aviso");
                    dataGridView1.Rows[e.RowIndex].Cells["Devueltos"].Value = 0;
                    RecalcularTotales();
                }
            }
        }

        private void RecalcularTotales()
        {
            double granTotal = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                granTotal += Convert.ToDouble(row.Cells["Subtotal"].Value);
            }
            lblTotalPagar.Text = granTotal.ToString("C2");
        }

        private void btnLiquidar_Click(object sender, EventArgs e)
        {
            // Aquí irá la lógica para guardar la liquidación, cobrar (abrir frmPago), descontar de Consignaciones y retornar inventario
            MessageBox.Show("Próximo paso: Cobrar y afectar el inventario.", "En construcción");
        }

        // Formatear columnas de moneda
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Precio" || dataGridView1.Columns[e.ColumnIndex].Name == "Subtotal")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("C2");
                    e.FormattingApplied = true;
                }
            }
        }
    }
}