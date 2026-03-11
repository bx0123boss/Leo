using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmProductoMasVendido : frmBase
    {
        public frmProductoMasVendido()
        {
            InitializeComponent();
        }

        private void frmProductoMasVendido_Load(object sender, EventArgs e)
        {
            EstilizarDataGridView(dataGridView1); // Crédito
            EstilizarDataGridView(dataGridView2); // Contado
            EstilizarBotonPrimario(button1);
            EstilizarTextBox(txtTop);

            dateTimePicker1.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dateTimePicker2.Value = DateTime.Now;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtTop.Text, out int topNum) || topNum <= 0)
            {
                MessageBox.Show("Proporciona una cantidad de productos válida (Ej: 10, 20, 50).", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTop.Focus();
                return;
            }

            DateTime fechaInicio = dateTimePicker1.Value.Date;
            DateTime fechaFin = dateTimePicker2.Value.Date.AddDays(1).AddSeconds(-1);

            try
            {
                using (OleDbConnection conectar = new OleDbConnection(Conexion.CadCon))
                {
                    conectar.Open();

                    // 1. PRODUCTOS ESTRELLA - CONTADO
                    DataSet dsContado = new DataSet();
                    string queryContado = $"SELECT TOP {topNum} Producto, SUM(Cantidad) AS CantidadVendidos " +
                                          "FROM VentasContado WHERE Fecha >= ? AND Fecha <= ? " +
                                          "GROUP BY Producto ORDER BY SUM(Cantidad) DESC";

                    using (OleDbCommand cmd1 = new OleDbCommand(queryContado, conectar))
                    {
                        cmd1.Parameters.AddWithValue("?", fechaInicio);
                        cmd1.Parameters.AddWithValue("?", fechaFin);
                        using (OleDbDataAdapter da1 = new OleDbDataAdapter(cmd1))
                        {
                            da1.Fill(dsContado, "Contado");
                        }
                    }
                    dataGridView2.DataSource = dsContado.Tables["Contado"];

                    // 2. PRODUCTOS ESTRELLA - CREDITO
                    DataSet dsCredito = new DataSet();
                    string queryCredito = $"SELECT TOP {topNum} Producto, SUM(Cantidad) AS CantidadVendidos " +
                                          "FROM VentasCredito WHERE Fecha >= ? AND Fecha <= ? " +
                                          "GROUP BY Producto ORDER BY SUM(Cantidad) DESC";

                    using (OleDbCommand cmd2 = new OleDbCommand(queryCredito, conectar))
                    {
                        cmd2.Parameters.AddWithValue("?", fechaInicio);
                        cmd2.Parameters.AddWithValue("?", fechaFin);
                        using (OleDbDataAdapter da2 = new OleDbDataAdapter(cmd2))
                        {
                            da2.Fill(dsCredito, "Credito");
                        }
                    }
                    dataGridView1.DataSource = dsCredito.Tables["Credito"];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar productos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            if (e.KeyChar == (char)Keys.Enter)
            {
                button1.PerformClick();
            }
        }
    }
}