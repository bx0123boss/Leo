using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BRUNO
{
    public partial class frmConfiguracion : Form
    {
        String ruta;
        public frmConfiguracion()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {            
            ChooseFolder();
            this.Close();
        }
        public void ChooseFolder()
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                ruta = folderBrowserDialog1.SelectedPath;
            }
            string fileName = "Optica.accdb";
            string sourcePath = @"C:\Jaeger Soft\";
            string targetPath = @""+ruta;

            // Use Path class to manipulate file and directory paths.
            string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
            string destFile = System.IO.Path.Combine(targetPath, fileName);

            // To copy a folder's contents to a new location:
            // Create a new target folder, if necessary.
            if (!System.IO.Directory.Exists(targetPath))
            {
                System.IO.Directory.CreateDirectory(targetPath);
            }
            try
            {
                System.IO.File.Copy(sourceFile, destFile, true);
                MessageBox.Show("Base de datos exportada de manera correcta!", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.ToString());
            }
            
        }
        private void button2_Click(object sender, EventArgs e)
        {            
                Importar();
                this.Close();
        }
        public void Importar()
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                ruta = folderBrowserDialog1.SelectedPath;
            }
            string fileName = "Optica.accdb";
            string targetPath = @"C:\Jaeger Soft\";
            string targetPath2 = @"C:\Jaeger Soft\Respaldo";
            string sourcePath = @"" + ruta;
            string sourcePath2 = @"C:\Jaeger Soft\";

            string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
            string destFile = System.IO.Path.Combine(targetPath, fileName);

            string sourceFile2 = System.IO.Path.Combine(sourcePath2, fileName);
            string destFile2 = System.IO.Path.Combine(targetPath2, fileName+" Respaldo");
            // To copy a folder's contents to a new location:
            // Create a new target folder, if necessary.
            if (!System.IO.Directory.Exists(targetPath))
            {
                System.IO.Directory.CreateDirectory(targetPath);
            }
            if (!System.IO.Directory.Exists(targetPath2))
            {
                System.IO.Directory.CreateDirectory(targetPath2);
            }
            try
            {
                System.IO.File.Copy(sourceFile2, destFile2, true);
                System.IO.File.Copy(sourceFile, destFile, true);                
                MessageBox.Show("Base de datos importada de manera correcta!", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.ToString());
            }
        }
    }
}
