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
    public partial class frmAgregarCompras2 : Form
    {
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        public string usuario = "";
        OleDbCommand cmd;

        public frmAgregarCompras2()
        {
            InitializeComponent();
        }

        private void frmAgregarCompras2_Load(object sender, EventArgs e)
        {
            conectar.Open();
            DataTable dt = new DataTable();
            cmd = new OleDbCommand("Select Id from Compras2;", conectar);
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dt);
            comboBox1.DisplayMember = "Id";
            comboBox1.ValueMember = "Id";
            comboBox1.DataSource = dt;
            cmd = new OleDbCommand("select Id,Nombre,Existencia from Inventario;", conectar);
            OleDbDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                dataGridView1.Rows.Add(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), "0", reader[2].ToString());
            }
            cmbProveedor.SelectedIndex = 0;
            lblUser.Text = usuario;
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DataGridViewColumn col = dataGridView1.Columns[e.ColumnIndex] as DataGridViewColumn;
            char punto = '.';

            if (col.Name == "Movimiento")
            {
                DataGridViewTextBoxCell cell = dataGridView1[e.ColumnIndex, e.RowIndex] as DataGridViewTextBoxCell;
                if (cell != null)
                {
                    char[] chars = e.FormattedValue.ToString().ToCharArray();
                    foreach (char c in chars)
                    {
                        if (c == punto)
                        {

                        }
                        else if (char.IsDigit(c) == false)
                        {
                            MessageBox.Show("Solo puedes introducir números", "Alto", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            e.Cancel = true;

                            break;
                        }
                    }
                }
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            double valorActual = Convert.ToDouble(dataGridView1[2, e.RowIndex].Value.ToString());
            double valor = Convert.ToDouble(dataGridView1[3, e.RowIndex].Value.ToString());
            double valorNuevo = 0;
            switch (cmbProveedor.SelectedIndex)
            {
                case 0:
                    valorNuevo = valorActual + valor;
                    break;
                case 1:
                    valorNuevo = valorActual - valor;
                    break;
                case 2:
                    valorNuevo = valorActual + valor;
                    break;
            }
            dataGridView1[4, e.RowIndex].Value = valorNuevo;
        }

        private void cmbProveedor_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                double valorActual = Convert.ToDouble(dataGridView1[2, i].Value.ToString());
                double valor = Convert.ToDouble(dataGridView1[3, i].Value.ToString());
                double valorNuevo = 0;
                switch (cmbProveedor.SelectedIndex)
                {
                    case 0:
                        valorNuevo = valorActual + valor;
                        break;
                    case 1:
                        valorNuevo = valorActual - valor;
                        break;
                    case 2:
                        valorNuevo = valorActual + valor;
                        break;
                }
                dataGridView1[4, i].Value = valorNuevo;
            }
            if (cmbProveedor.SelectedIndex == 0)
            {
                comboBox1.Visible = false;
                label1.Visible = false;
            }
            else
            {
                comboBox1.Visible = true;
                label1.Visible = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string movimiento = cmbProveedor.Text;
            string relacion = "";
            if (cmbProveedor.SelectedIndex != 0)
            {
                relacion = comboBox1.Text;
            }
            cmd = new OleDbCommand("INSERT INTO Compras2(Movimiento,Fecha,Capturo,Relacion) VALUES ('" + movimiento + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','" + usuario + "','" + relacion + "');", conectar);
            cmd.ExecuteNonQuery();
            string idInsertado = "";
            cmd = new OleDbCommand("select @@IDENTITY;", conectar);
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                idInsertado = reader[0].ToString();
            }
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                string id = dataGridView1[0, i].Value.ToString();
                string nombre = dataGridView1[1, i].Value.ToString();
                double existencias = Convert.ToDouble(dataGridView1[2, i].Value.ToString());
                double mov = Convert.ToDouble(dataGridView1[3, i].Value.ToString());
                double suma = Convert.ToDouble(dataGridView1[4, i].Value.ToString());
                cmd = new OleDbCommand("UPDATE Inventario SET Existencia='" + suma + "' where Id='" + id + "';", conectar);
                cmd.ExecuteNonQuery();
                cmd = new OleDbCommand("insert into Kardex (IdProducto,Tipo,Descripcion,ExistenciaAntes,ExistenciaDespues,Fecha,Precio) values('" + id + "','ENTRADA','EXISTENCIA POR: "+movimiento+", FOLIO: " + idInsertado + "'," + existencias + ",'" + suma + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "','NA');", conectar);
                cmd.ExecuteNonQuery();
                cmd = new OleDbCommand("INSERT INTO ComprasMovimientos(Folio,IdProducto,Nombre,ExistenciasAntiguas,Movimiento,ExistenciasActuales,Tipo,Fecha) VALUES ('" + idInsertado + "','" + id + "','" + nombre + "','" + existencias + "','" + mov + "','" + suma + "','" + movimiento + "','" + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()) + "');", conectar);
                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("SE HA AGREGADO AL INVENTARIO CON EXITO", "CERRAR ORDEN", MessageBoxButtons.OK, MessageBoxIcon.Information);
            frmCompras com = new frmCompras();
            com.Show();
            com.usuario = usuario;
            this.Close();
        }

        public void formato(string id, double cant)
        {
            cmd = new OleDbCommand("select * from invent where idArticulo=" + id + ";", conectar);
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string salida = "";
                string set = "";
                switch (cmbProveedor.SelectedIndex)
                {
                    case 0:
                        salida = "" + (Convert.ToDouble(Convert.ToString(reader[3].ToString())) + cant);
                        set = "entrada";
                        break;
                    case 1:
                        salida = "" + (Convert.ToDouble(Convert.ToString(reader[3].ToString())) - cant);
                        set = "salida";
                        break;
                    case 2:
                        salida = "" + (Convert.ToDouble(Convert.ToString(reader[3].ToString())) + cant);
                        set = "entrada";
                        break;
                }
                cmd = new OleDbCommand("UPDATE invent set " + set + "='" + salida + "' Where idArticulo=" + id + ";", conectar);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
