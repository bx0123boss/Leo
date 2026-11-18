using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JaegerSoft
{
    public partial class frmElegirApartado : frmBase
    {
        public String usuario = "";
        public bool IsServidorActivo { get; set; } = false;
        public frmElegirApartado()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            bool abierto = false;
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(frmApartado))
                {
                    MessageBox.Show("Ya existe un modulo de apartado abierto", "Alto!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    abierto = true;
                }
            }
            if (abierto)
            {

            }
            else
            {
                frmApartado apart = new frmApartado();
                apart.usuario = usuario;
                apart.Show();
                this.Close();
            } 
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool abierto = false;
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(frmApartados))
                {
                    MessageBox.Show("Ya existe un modulo de apartados abierto", "Alto!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    abierto = true;
                }
            }
            if (abierto)
            {

            }
            else
            {
                frmApartados apart = new frmApartados();
                apart.Show();
                this.Close();
            } 
        }

        private void frmElegirApartado_Load(object sender, EventArgs e)
        {
            EstilizarBotonPrimario(button1);
            EstilizarBotonAdvertencia(button2);
            EstilizarBotonPeligro(button3);
        }

        private void button3_Click(object sender, EventArgs e)
        {


            string url = "http://localhost:5000/agenda";
            if (IsServidorActivo)
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo abrir el navegador: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Servidor inactivo.\n\nContacte a soporte técnico para actualizar su sistema o verificar la instalación.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
