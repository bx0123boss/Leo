using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace BRUNO
{
    public partial class frmSerial : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbCommand cmd;
        bool frase = false;
        string palabra;
        string key;
        string clave;

        public frmSerial()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (frase)
            {
                clave = textBox1.Text;
                string result = Regex.Replace(textBox1.Text, @".{5}(?!$)", "$0-");
                textBox1.Text = result;
                panel1.Visible = true;
                timer1.Enabled = true;
            }
            else
            {
                frase = true;
                palabra = textBox1.Text;
                MessageBox.Show("Ahora introduzca la clave Serial de su punto de venta:", "Correcto!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Text = "Introduzca la clave Serial de su punto de venta JaegerSoft";
                //MessageBox.Show(Hash(palabra));
                key = Hash(palabra);
                textBox1.Clear();
                textBox1.Focus();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value < 600)
                progressBar1.Value++;
            else
            {
                if (clave == key)
                {
                    timer1.Enabled = false;
                    conectar.Open();
                    DateTime Fecha = DateTime.Now;
                    Fecha = Fecha.AddDays(15);
                    Fecha = Fecha.AddYears(1);
                    cmd = new OleDbCommand("UPDATE Fech set Caja='1', Fecha='" + Fecha + "' where Id=2;", conectar);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Clave verificada correctamente, contacte a soporte tecnico para auxiliarle con la instalación de sus bases de datos", "Exito!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MessageBox.Show("La licencia expira el dia: " + Fecha, "Exito!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                {
                    timer1.Enabled = false;
                    MessageBox.Show("Clave incorrecta, favor de verificar", "Error de verificacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.Clear();
                    panel1.Visible = false;
                    progressBar1.Value = 0;
                    textBox1.Focus();
                }

            }
        }
        static string Hash(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }

        private void frmSerial_Load(object sender, EventArgs e)
        {
        }
    }
}
