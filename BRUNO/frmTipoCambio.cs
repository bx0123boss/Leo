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
    public partial class frmTipoCambio : Form
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbCommand cmd;
        OleDbDataAdapter da;
        public double PrecioCompra, PrecioMaquilaDolaresLinea, PrecioPorGramo, ValorLineaGramo, Costo, Doble, Veinte,Utilidad;
        double PrecioOro, entre24, Kilataje, Resultado, Maquila, Total, Peso, Treinta, por2, Diez;
        public double  Dolar, FactorKilataje, PrecioVenta, Gramos, Formula1, Formula2, Formula3, Formula4, Formula5, Formula6;
        public frmTipoCambio()
        {
            InitializeComponent();
            conectar.Open();
        }

        private void frmTipoCambio_Load(object sender, EventArgs e)
        {
                 cmd = new OleDbCommand("select * from Cambio where Id=1;", conectar);
                        OleDbDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            txtOro.Text = Convert.ToString(reader[1].ToString());
                            txtDolar.Text= Convert.ToString(reader[2].ToString());
                            Dolar = Convert.ToDouble(txtDolar.Text);
                            PrecioOro = Convert.ToDouble(txtOro.Text);
                        }
                        ds = new DataSet();
                        da = new OleDbDataAdapter("select * from Inventario WHERE Categoria='JOYERIA' and not Proveedor='DINASTI';", conectar);
                        //da = new OleDbDataAdapter("select * from Inventario WHERE Id='001';", conectar);
                        da.Fill(ds, "Id");
                        dataGridView1.DataSource = ds.Tables["Id"];

                        ds = new DataSet();
                        da = new OleDbDataAdapter("select * from Inventario WHERE Proveedor='DINASTI';", conectar);
                        da.Fill(ds, "Id");
                        dataGridView2.DataSource = ds.Tables["Id"];
                           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (PrecioOro != Convert.ToDouble(txtOro.Text))
            {
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    if ((dataGridView1[11, i].Value.ToString() == "") || (dataGridView1[11, i].Value.ToString() == "N/A") || (dataGridView1[11, i].Value.ToString() == "0"))
                    {
                        try
                        {
                            double oroInicial = Convert.ToDouble(dataGridView1[16, i].Value.ToString());
                            double porcentajeOro = ((PrecioOro * 100) / oroInicial) - 100;
                            double SubioOro = 0;
                            Costo = Convert.ToDouble(dataGridView1[2, i].Value.ToString());
                                if (porcentajeOro >= 0)
                                {
                                    porcentajeOro = porcentajeOro / 100;
                                    double Kilates = Convert.ToDouble(dataGridView1[10, i].Value.ToString());
                                    if (Kilates == 18)
                                        SubioOro = (Costo * porcentajeOro) * .8;
                                    else if (Kilates == 14)
                                        SubioOro = (Costo * porcentajeOro) * .616;
                                    else if (Kilates == 10)
                                        SubioOro = (Costo * porcentajeOro) * .440;
                                }
                                PrecioCompra = Costo + SubioOro;
                                por2 = PrecioCompra * 2;
                                Diez = por2 * 1.1;
                                double Utilidad = Math.Truncate(Diez) - Math.Truncate(PrecioCompra);
                                conectar.Close();
                            conectar.Open();
                                cmd = new OleDbCommand("UPDATE Inventario Set PrecioVenta='" + Math.Truncate(Diez) + "',Utilidad='" + Utilidad + "' where Id='" + dataGridView1[0, i].Value.ToString() + "';", conectar);
                                cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show(ex.ToString());
                        }
                    }
                    else
                    {
                        try
                        {
                            conectar.Close();
                            PrecioOro = Convert.ToDouble(txtOro.Text);
                            entre24 = PrecioOro / 24;
                            Kilataje = Convert.ToDouble(dataGridView1[10, i].Value.ToString());
                            Resultado = entre24 * Kilataje;
                            Maquila = Convert.ToDouble(dataGridView1[11, i].Value.ToString());
                            Total = Resultado + Maquila;
                            Peso = Convert.ToDouble(dataGridView1[9, i].Value.ToString());
                            PrecioCompra = Total * Peso;
                            Treinta = PrecioCompra * 1.3;
                            por2 = Treinta * 2;
                            Diez = por2 * 1.1;
                            double Utilidad = Math.Truncate(Diez) - Math.Truncate(PrecioCompra);
                            conectar.Open();
                            cmd = new OleDbCommand("UPDATE Inventario Set PrecioCompra='" + Math.Truncate(PrecioCompra) + "',PrecioVenta='" + Math.Truncate(Diez) + "',Utilidad='" + Utilidad + "' where Id='" + dataGridView1[0, i].Value.ToString() + "';", conectar);
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                        }
                    }



                }
                 cmd = new OleDbCommand("UPDATE Cambio Set Oro='" + txtOro.Text + "' where Id=1;", conectar);
                cmd.ExecuteNonQuery();
                MessageBox.Show("LOS PRECIOS DE JOYERIA HAN SIDO ACTUALIZADOS", "PRECIOS ACTUALIZADOS", MessageBoxButtons.OK, MessageBoxIcon.Information);
               
            }
            if (Dolar != Convert.ToDouble(txtDolar.Text))
            {
                PrecioOro = Convert.ToDouble(txtOro.Text);
                Dolar = Convert.ToDouble(txtDolar.Text);
                        
                for (int i = 0; i < dataGridView2.RowCount; i++)
                {
                    conectar.Close();
                    conectar.Open();
                    try
                    {
                        cmd = new OleDbCommand("select * from Linea where Linea='" + dataGridView2[11, i].Value.ToString() + "';", conectar);
                        OleDbDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            Gramos = Convert.ToDouble(dataGridView2[9, i].Value.ToString());
                            double dolarucos = Convert.ToDouble(dataGridView2[14, i].Value.ToString());
                            PrecioCompra = Convert.ToDouble(dataGridView2[2, i].Value.ToString());

                            if (dataGridView2[10, i].Value.ToString() == "10")
                            {
                                FactorKilataje = 0.440;
                            }
                            else if (dataGridView2[10, i].Value.ToString() == "14")
                            {
                                FactorKilataje = 0.616;
                            }
                            else if (dataGridView2[10, i].Value.ToString() == "18")
                            {
                                FactorKilataje = 0.8;
                            }
                            double oroInicial = Convert.ToDouble(dataGridView2[16, i].Value.ToString());
                            double porcentajeOro = ((PrecioOro * 100) / oroInicial) - 100;
                            double SubioOro = 0;
                            if (Dolar > Convert.ToDouble(dataGridView2[15, i].Value.ToString()))
                            {
                                Costo = dolarucos * Dolar;
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
                                Costo = Costo + SubioOro;
                                Doble = Costo * 2;
                                Veinte = Math.Truncate(Doble * 1.12);
                                Utilidad = Doble - Costo;
                                cmd = new OleDbCommand("UPDATE Inventario Set PrecioCompra='" + Math.Truncate(Costo) + "', PrecioVenta='" + Math.Truncate(Veinte) + "', Utilidad='" + Math.Truncate(Utilidad) + "' where Id='" + dataGridView2[0, i].Value.ToString() + "';", conectar);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show("ID: " + dataGridView2[0, i].Value.ToString() +"\n");
                    }

                }

                cmd = new OleDbCommand("UPDATE Cambio set Dolar='" + txtDolar.Text + "' where Id=1;", conectar);
                cmd.ExecuteNonQuery();
                MessageBox.Show("LOS PRECIOS DE DINASTI HAN SIDO ACTUALIZADOS", "PRECIOS ACTUALIZADOS", MessageBoxButtons.OK, MessageBoxIcon.Information);
               

            }
            else
            {
               // MessageBox.Show("EL PRECIO DEL ORO NO SE HA MODIFICADO", "ALTO", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("");
        }
    }
}
