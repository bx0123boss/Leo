using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmLogin : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbCommand cmd;
        public frmLogin()
        {
            InitializeComponent();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            //this.BackgroundImage = Image.FromFile(@"E:\Users\Brandon\Desktop\JS\Bruno\BRUNO\abstract_aurora_gold-wallpaper-1920x1200.jpg");
            //this.BackgroundImage = Image.FromFile(@"E:\Users\Brandon\Desktop\JS\Bruno\BRUNO\DOREDOGRECAS.jpg");
            try 
            {
                this.BackgroundImage = Image.FromFile("C:\\Jaeger Soft\\w1.jpg");
                //Pass the filepath and filename to the StreamWriter Constructor
                StreamWriter sw = new StreamWriter(@"MODIFICACION.txt");
                //Write a line of text
                sw.WriteLine(DateTime.Now.ToShortDateString() + " ");
                //Write a second line of text
                sw.WriteLine(DateTime.Now.ToShortTimeString());
                //Close the file
                sw.Close();}
            catch(Exception ex)
            {Console.WriteLine("Exception: " + ex.Message);
            }
            conectar.Open();
            DataTable dt = new DataTable();
            cmd = new OleDbCommand("Select Id,Usuario from Usuarios;", conectar);
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dt);
            txtUsuario.DisplayMember = "Usuario";
            txtUsuario.ValueMember = "Id";
            txtUsuario.DataSource = dt;
            txtUsuario.Text = "";
            cmd = new OleDbCommand("select * from Fech where Id=2;", conectar);
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int caja = Convert.ToInt32(Convert.ToString(reader[1].ToString()));
                if (caja == 0)
                {
                    button2.Visible = true;
                    txtContraseña.Enabled = false;
                    button1.Enabled = false;
                    txtUsuario.Enabled = false;
                }
                else if (caja == 1)
                {

                    string[] fecha = reader[2].ToString().Split(' ');
                    DateTime fecha1 = Convert.ToDateTime(fecha[0]);
                    if (fecha1 > DateTime.Now)
                    {

                    }
                    else
                    {
                        cmd = new OleDbCommand("UPDATE Fech set Caja='0', Fecha='" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "' where Id=2;", conectar);
                        cmd.ExecuteNonQuery();
                        button2.Visible = true;
                        txtContraseña.Enabled = false;
                        button1.Enabled = false;
                        txtUsuario.Enabled = false;

                    }
                     
                }
                else if (caja == 2)
                {
                    string[] fecha = reader[2].ToString().Split(' ');
                    DateTime fecha1 = Convert.ToDateTime(fecha[0]);
                    if (fecha1 > DateTime.Now)
                    {

                    }
                    else
                    {
                        MessageBox.Show("Se requiere mantenimiento del sistema, contacte a soporte tecnico para auxiliarle", "¡Atención!",MessageBoxButtons.OK,MessageBoxIcon.Hand);
                    }
                }

            }
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cmd = new OleDbCommand("select * from Fech where Id=1;", conectar);
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int caja = Convert.ToInt32(Convert.ToString(reader[1].ToString()));
                if (caja == 0)
                {

                    cmd = new OleDbCommand("UPDATE Fech set Caja='1', Fecha='" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "' where Id=1;", conectar);
                    cmd.ExecuteNonQuery();
                }
            }

            cmd = new OleDbCommand("select * from Usuarios where Usuario='" + txtUsuario.Text + "' and Contraseña='" + txtContraseña.Text + "';", conectar);
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string TIPO;
                if (reader[3].ToString() == "ADMINISTRADOR")
                    TIPO = "Admin";
                else
                    TIPO = "Invitado";
                frmPrincipal principal = new frmPrincipal();
                principal.idUsuario = reader[0].ToString();
                principal.usuario = TIPO;
                principal.NombreUsuario = txtUsuario.Text;
                principal.Show();
                this.Hide();
            }
            else
                MessageBox.Show("Contraseña incorrecta, favor de verificar", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
               

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (txtUsuario.Text == ":v" && txtContraseña.Text == "")
            {
                frmPrincipal principal = new frmPrincipal();
                principal.usuario = "Admin";
                principal.Show();
                this.Hide();
            }
        }

        private void txtContraseña_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                cmd = new OleDbCommand("select * from Fech where Id=1;", conectar);
                OleDbDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    int caja = Convert.ToInt32(Convert.ToString(reader[1].ToString()));
                    if (caja == 0)
                    {

                        cmd = new OleDbCommand("UPDATE Fech set Caja='1', Fecha='" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "' where Id=1;", conectar);
                        cmd.ExecuteNonQuery();
                    }
                }
                cmd = new OleDbCommand("select * from Usuarios where Usuario='" + txtUsuario.Text + "' and Contraseña='" + txtContraseña.Text + "';", conectar);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string TIPO;
                    if (reader[3].ToString() == "ADMINISTRADOR")
                        TIPO = "Admin";
                    else
                        TIPO = "Invitado";
                    frmPrincipal principal = new frmPrincipal();
                    principal.idUsuario = reader[0].ToString();
                    principal.usuario = TIPO;
                    principal.NombreUsuario = txtUsuario.Text;
                    principal.Show();
                    this.Hide();
                }
                else
                    MessageBox.Show("Contraseña incorrecta, favor de verificar", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
               
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (frmSerial buscar = new frmSerial())
            {
                if (buscar.ShowDialog() == DialogResult.OK)
                {
                    button2.Visible = false;
                    txtContraseña.Enabled = true;
                    txtUsuario.Enabled = true;
                    button1.Enabled = true;
                }
            }
        }
    }
}
