using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static BRUNO.frmInventario;

namespace BRUNO
{
    public partial class frmDatos : Form
    {
        private List<ResumenCategoria> datosResumen;
        public frmDatos(List<ResumenCategoria> datos)
        {
            InitializeComponent();
            this.datosResumen = datos;
        }

        private void BtnApartados_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmDatos_Load(object sender, EventArgs e)
        {
            dataGridViewResumen.DataSource = datosResumen;
            FormatearGrid();
        }
        private void FormatearGrid()
        {
            try
            {
                // 'c' es para formato de Moneda (Currency)
                dataGridViewResumen.Columns["TotalInversion"].DefaultCellStyle.Format = "c";
                dataGridViewResumen.Columns["TotalVenta"].DefaultCellStyle.Format = "c";
                dataGridViewResumen.Columns["Utilidad"].DefaultCellStyle.Format = "c";

                // 'n0' es para número sin decimales
                dataGridViewResumen.Columns["TotalExistencia"].DefaultCellStyle.Format = "n0";

                // Ajustar columnas al espacio disponible
                dataGridViewResumen.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                // Manejo simple por si una columna no existe
                Console.WriteLine("Error al formatear grid: " + ex.Message);
            }
        }
    }
}
