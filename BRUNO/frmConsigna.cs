using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Windows.Forms;
// using System.Data.OleDb; // o SqlClient, dependiendo de dónde crees las 2 tablas nuevas.

namespace BRUNO
{
    public partial class frmConsigna : frmBase
    {
        private int _idCliente;
        private string _nombreCliente;

        public frmConsigna(int idCliente, string nombreCliente)
        {
            InitializeComponent();
            _idCliente = idCliente;
            _nombreCliente = nombreCliente;
        }

        private void frmConsigna_Load(object sender, EventArgs e)
        {
            lblCliente.Text = "Cliente: " + _nombreCliente;
            CargarBalance();
            CargarKardex();
            EstilizarDataGridView(dgvBalance);
            EstilizarDataGridView(dgvKardex);
            btnEntregar.Click += new EventHandler(btnEntregar_Click);
            btnDevolver.Click += new EventHandler(btnDevolver_Click);
            btnCobrar.Click += new EventHandler(btnCobrar_Click);
        }

        #region MÉTODOS DE DATOS
        private void CargarBalance()
        {
            try
            {
                using (OleDbConnection con = new OleDbConnection(Conexion.CadCon))
                {
                    con.Open();

                    string query = @"
                        SELECT 
                        c.ProductoId, 
                        i.Nombre AS Producto, 
                        c.PrecioCongelado AS Precio, 
                        c.EnConsigna AS [En Consigna], 
                        c.Vendidos, 
                        c.Devueltos 
                    FROM
                        ConsignaCliente c
                        INNER JOIN Inventario i ON c.ProductoId = i.Id
                    WHERE c.ClienteId = ?";

                    using (OleDbCommand cmd = new OleDbCommand(query, con))
                    {
                        // En Access/OleDb, los parámetros van con '?' y se asignan en orden
                        cmd.Parameters.AddWithValue("?", _idCliente);

                        OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dgvBalance.DataSource = dt;

                        // Configuramos las columnas visualmente
                        if (dgvBalance.Columns.Contains("ProductoId"))
                        {
                            dgvBalance.Columns["ProductoId"].Visible = false; // Ocultamos el ID
                        }

                        // Formateamos como moneda
                        if (dgvBalance.Columns.Contains("Precio"))
                        {
                            dgvBalance.Columns["Precio"].DefaultCellStyle.Format = "C2";
                        }

                        // Bloquear grid
                        foreach (DataGridViewColumn col in dgvBalance.Columns)
                        {
                            col.ReadOnly = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el balance de la consigna: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CargarKardex()
        {
            try
            {
                using (OleDbConnection con = new OleDbConnection(Conexion.CadCon))
                {
                    con.Open();

                    string query = @"
                        SELECT 
                            m.Fecha, 
                            m.TipoMovimiento AS Movimiento, 
                            i.Nombre AS Producto, 
                            m.Cantidad, 
                            m.PrecioVigente AS Precio
                        FROM ConsignaMovimientos m
                        INNER JOIN Inventario i ON m.ProductoId = i.Id
                        WHERE m.ClienteId = ? 
                        ORDER BY m.Fecha DESC";

                    using (OleDbCommand cmd = new OleDbCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("?", _idCliente);

                        OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dgvKardex.DataSource = dt;

                        if (dgvKardex.Columns.Contains("Precio"))
                            dgvKardex.Columns["Precio"].DefaultCellStyle.Format = "C2";

                        if (dgvKardex.Columns.Contains("Fecha"))
                            dgvKardex.Columns["Fecha"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";

                        foreach (DataGridViewColumn col in dgvKardex.Columns)
                            col.ReadOnly = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el historial de movimientos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region EVENTOS DE LOS BOTONES

        private void btnEntregar_Click(object sender, EventArgs e)
        {
            // 1. Abrimos el modal para entregar nueva mercancía
            // frmConsignaEntregar frm = new frmConsignaEntregar(_idCliente, _nombreCliente);
            // frm.ShowDialog();

            // 2. Al cerrar el modal, recargamos las tablas para que el usuario vea el cambio al instante
            CargarBalance();
            CargarKardex();
        }

        private void btnDevolver_Click(object sender, EventArgs e)
        {
            // Validamos que haya algo en consigna para devolver
            if (dgvBalance.Rows.Count == 0)
            {
                MessageBox.Show("El cliente no tiene mercancía en consigna para devolver.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Validamos que haya seleccionado un renglón
            if (dgvBalance.CurrentRow != null)
            {
                string idProducto = dgvBalance.CurrentRow.Cells["ProductoId"].Value.ToString();
                string nombreProd = dgvBalance.CurrentRow.Cells["Producto"].Value.ToString();
                decimal precioCongelado = Convert.ToDecimal(dgvBalance.CurrentRow.Cells["PrecioCongelado"].Value);
                int cantEnConsigna = Convert.ToInt32(dgvBalance.CurrentRow.Cells["EnConsigna"].Value);

                // Abrir el modal
                frmConsignaDevolver frm = new frmConsignaDevolver(_idCliente, idProducto, nombreProd, precioCongelado, cantEnConsigna);
                frm.ShowDialog();
                CargarBalance();
                CargarKardex();
            }
            else
            {
                MessageBox.Show("Por favor seleccione un producto del balance (tabla superior) para devolver.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCobrar_Click(object sender, EventArgs e)
        {
            if (dgvBalance.Rows.Count == 0)
            {
                MessageBox.Show("El cliente no tiene mercancía pendiente por pagar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            frmConsignaCobrar frm = new frmConsignaCobrar(_idCliente, _nombreCliente);
            frm.ShowDialog();

            CargarBalance();
            CargarKardex();
        }

        #endregion
        private void btnEntregar_Click_1(object sender, EventArgs e)
        {
            // Abrimos el modal pasándole los datos del cliente actual
            frmConsignaEntregar frm = new frmConsignaEntregar(_idCliente, _nombreCliente);

            // Si el modal devuelve "OK" (significa que sí se guardó algo y no solo se canceló)
            if (frm.ShowDialog() == DialogResult.OK)
            {
                // Recargamos las tablas automáticamente
                CargarBalance();
                CargarKardex();
            }
        }
    }
    }