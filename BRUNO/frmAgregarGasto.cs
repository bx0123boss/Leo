using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JaegerSoft
{
    public partial class frmAgregarGasto : frmBase
    {
        SqlConnection conectar = new SqlConnection(Conexion.CadSQL);
        //SqlConnection conectar = new SqlConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        SqlCommand cmd;
        string id;
        public frmAgregarGasto()
        {
            InitializeComponent();
            conectar.Open();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value.AddHours(-1) <= dateTimePicker2.Value)
            {
                string[] fecha = dateTimePicker1.Value.ToString().Split(' ');
                string fecha1 = fecha[0];
                fecha = dateTimePicker2.Value.ToString().Split(' ');
                string fecha2 = fecha[0];
                cmd = new SqlCommand("insert into GastosGeneral(Nombre,Lapso,Total) values('" + txtNombre.Text + "','" + fecha1 + " - " + fecha2 + "','" + txtTotal.Text + "');", conectar);
                cmd.ExecuteNonQuery();
                cmd = new SqlCommand("SELECT @@Identity;", conectar);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    id = reader[0].ToString();
                }
                DateTime dia = dateTimePicker1.Value.Date;
                TimeSpan tSpan = dateTimePicker2.Value.Date - dateTimePicker1.Value.Date;
                int dias = tSpan.Days;
                double pagoTotal = Convert.ToDouble(txtTotal.Text);
                double pagoParcial = pagoTotal / (dias + 1);
                pagoParcial = Math.Round(pagoParcial, 2);
                for (int i = 0; i <= dias; i++)
                {
                    cmd = new SqlCommand("insert into GastosDetallados(idGasto,Total,Fecha) values('" + id + "','" + pagoParcial + "','" + dia + "');", conectar);
                    cmd.ExecuteNonQuery();
                    dia = dia.AddDays(1);
                }
                MessageBox.Show("¡Gasto agregado con exito!", "Agregar Gasto", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {

            }


        }

        private void txtTotal_KeyPress(object sender, KeyPressEventArgs e)
        {
            CultureInfo cc = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (char.IsNumber(e.KeyChar) || e.KeyChar.ToString() == cc.NumberFormat.NumberDecimalSeparator)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void frmAgregarGasto_Load(object sender, EventArgs e)
        {
            EstilizarTextBox(txtNombre);
            EstilizarTextBox(txtTotal);
            EstilizarBotonPrimario(button1);
        }
    }
}
