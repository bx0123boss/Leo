using System;
using System.Data.OleDb;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmTarjeta : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbDataAdapter da;
        OleDbCommand cmd;
        public frmTarjeta()
        {
            InitializeComponent();
            conectar.Open();
        }

        private void frmTarjeta_Load(object sender, EventArgs e)
        {
            cmd = new OleDbCommand("select Porcentaje from Tarjeta where Id=1;", conectar);
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                txtPorcentaje.Text =reader[0].ToString();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            // Validar que el TextBox no esté vacío
            if (string.IsNullOrWhiteSpace(txtPorcentaje.Text))
            {
                MessageBox.Show("Por favor ingrese un valor porcentual", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPorcentaje.Focus();
                return;
            }

            // Intentar convertir el texto a decimal
            if (!decimal.TryParse(txtPorcentaje.Text, out decimal porcentaje))
            {
                MessageBox.Show("El valor ingresado no es un número válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPorcentaje.Focus();
                return;
            }

            // Validar rango (0.01 a 99.99)
            if (porcentaje < 0.01m || porcentaje > 99.99m)
            {
                MessageBox.Show("El porcentaje debe estar entre 0.01 y 99.99", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPorcentaje.Focus();
                return;
            }

            // Validar que no tenga más de 2 decimales
            if (decimal.Round(porcentaje, 2) != porcentaje)
            {
                MessageBox.Show("El porcentaje no puede tener más de 2 decimales", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPorcentaje.Focus();
                return;
            }

            // --- AQUÍ VA TU CÓDIGO PARA ALMACENAR EN BASE DE DATOS ---
            try
            {
                cmd = new OleDbCommand("UPDATE Tarjeta set Porcentaje=" + porcentaje + " where Id=1;", conectar);
                cmd.ExecuteNonQuery();

                MessageBox.Show($"Porcentaje {porcentaje}% almacenado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar en base de datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
