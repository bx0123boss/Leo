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

namespace BRUNO
{
    public partial class frmAgregarLinea : Form
    {

        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbCommand cmd;
        OleDbDataAdapter da;
        private DataSet ds;
        public string line;
        public double valor,Costo,Dolar,PrecioOro, Doble, Veinte, Utilidad;

        public frmAgregarLinea()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.Text == "Agregar")
            {
                conectar.Open();
                cmd = new OleDbCommand("insert into Linea values('" + txtLinea.Text+ "','" + txtNumero.Text + "');", conectar);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Se ha agregado la Linea con exito", "AGREGADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                conectar.Close();
                conectar.Open();
                for (int i = 0; i < dataGridView2.RowCount; i++)
                {
                    string id = dataGridView2[0, i].Value.ToString();
                    double Peso = Convert.ToDouble(dataGridView2[9, i].Value.ToString()), Subio;
                    double PrecioMaquilaDolaresLinea = Convert.ToDouble(txtNumero.Text),oroInicial = Convert.ToDouble(dataGridView2[16, i].Value.ToString());
                    double porcentajeOro = ((PrecioOro * 100) / oroInicial) - 100;
                    double SubioOro=0;
                    Subio = PrecioMaquilaDolaresLinea - valor;
                    Subio = Subio * Peso * 3;
                    Costo = (Peso * PrecioMaquilaDolaresLinea)+ Subio;
                    if (porcentajeOro >= 0)
                    {
                        porcentajeOro = porcentajeOro / 100;
                        double Kilates = Convert.ToDouble(dataGridView2[10, i].Value.ToString());
                        if (Kilates == 18)
                            SubioOro = (Costo * porcentajeOro) * .8;
                        else if (Kilates == 14)
                            SubioOro = (Costo * porcentajeOro) * .616;
                        else if (Kilates == 10)
                            SubioOro = (Costo * porcentajeOro) * .440;
                    }
                    else
                    {
                    }
                    Costo = Costo + SubioOro;
                    Doble = (Costo * 2);
                    Veinte = Math.Truncate(Doble * 1.12);
                    Utilidad = Doble - Veinte;
                    cmd = new OleDbCommand("UPDATE Inventario Set PrecioCompra='" + Math.Truncate(Costo) + "', PrecioVenta='" + Math.Truncate(Veinte) + "', Utilidad='" + Math.Truncate(Utilidad) + "' where Id='" + id + "';", conectar);
                    cmd.ExecuteNonQuery();
                }
                cmd = new OleDbCommand("UPDATE Linea set Linea='" + txtLinea.Text + "', Cantidad='" + txtNumero.Text + "' where Linea='" + line + "';", conectar);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Se ha actualizado la Linea con exito", "Editado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }

        private void frmAgregarLinea_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmLineas lin = new frmLineas();
            lin.Show();
        }

        private void frmAgregarLinea_Load(object sender, EventArgs e)
        {
            if (line != null)
            {
                ds = new DataSet();
                conectar.Open();
                da = new OleDbDataAdapter("select * from Inventario where Maquila='"+line+"';", conectar);
                da.Fill(ds, "Id");
                dataGridView2.DataSource = ds.Tables["Id"];

                cmd = new OleDbCommand("select * from Cambio where Id=1;", conectar);
                OleDbDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Dolar = Convert.ToDouble(reader[2].ToString());
                    PrecioOro = Convert.ToDouble(reader[1].ToString());
                }
            }
        }
    }
}
