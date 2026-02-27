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
using Microsoft.Office.Interop.Excel;
using System.Globalization;

namespace BRUNO
{
    public partial class frmClientes : frmBase
    {
        private DataSet ds;
        //OleDbConnection conectar = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb");
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon); 
        OleDbDataAdapter da;
        OleDbCommand cmd;
        public String usuario = "";

        public frmClientes()
        {
            InitializeComponent();
        }

        private void frmClientes_Load(object sender, EventArgs e)
        {
            EstilizarDataGridView(dataGridView1);
            EstilizarBotonPrimario(button1);
            EstilizarBotonPrimario(button2);
            EstilizarBotonPrimario(button3);
            EstilizarBotonPrimario(button4);
            EstilizarBotonPrimario(button5);
            EstilizarBotonPrimario(button6);
            EstilizarBotonPrimario(button8);
            EstilizarTextBox(textBox1);

            ds = new DataSet();
            conectar.Open();
            da = new OleDbDataAdapter("select * from Clientes where Estatus='ACTIVO';", conectar);
            da.Fill(ds, "Id");
            dataGridView1.DataSource = ds.Tables["Id"];
            dataGridView1.Columns[0].Visible = false;

            double total=0;
            if (usuario == "Invitado")
            {
                lblAdeudo.Hide();
                label2.Hide();
                button3.Hide();
                dataGridView1.Columns[7].Visible = false;
                dataGridView1.Columns[8].Visible = false;
            }
            else
            {
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    total += Convert.ToDouble(dataGridView1[7, i].Value.ToString());
                }
                lblAdeudo.Text = "$" + total.ToString("#,#.00", CultureInfo.InvariantCulture);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmAgregarClientes agregar = new frmAgregarClientes();
            agregar.Text = "Agregar";
            agregar.Show();
            this.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Clientes ORDER BY Nombre and Estatus='ACTIVO';", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
            }
            else
            {

                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Clientes where Estatus='ACTIVO' and Nombre LIKE '%" + textBox1.Text + "%' ORDER BY Nombre ;", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Por favor, seleccione un cliente para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult dialogResult = MessageBox.Show("¿Estas seguro de elimiar al cliente?", "Alto!", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                cmd = new OleDbCommand("UPDATE Clientes set Estatus='SUSPENDIDO' where Id=" + dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString() + ";", conectar);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Se ha eliminado el cliente con exito", "ELIMINADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ds = new DataSet();
                da = new OleDbDataAdapter("select * from Clientes where Estatus='ACTIVO';", conectar);
                da.Fill(ds, "Id");
                dataGridView1.DataSource = ds.Tables["Id"];
                dataGridView1.Columns[0].Visible = false;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
            {
                frmHistorialAbonos histo = new frmHistorialAbonos();
                histo.lblID.Text = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
                histo.lblNombre.Text = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
                histo.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un registro de la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
            {
                frmAbonoCliente abonar = new frmAbonoCliente();
                abonar.txtAdeudo.Text = dataGridView1[7, dataGridView1.CurrentRow.Index].Value.ToString();
                abonar.lblID.Text = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
                abonar.lblCliente.Text = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
                abonar.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un registro de la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmAgregarClientes agregar = new frmAgregarClientes();
            agregar.Text = "Editar";
            agregar.button1.Text = "Editar";
            agregar.lblID.Text = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
            agregar.txtNombre.Text = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
            agregar.txtTelefono.Text = dataGridView1[2, dataGridView1.CurrentRow.Index].Value.ToString();
            agregar.txtDireccion.Text = dataGridView1[3, dataGridView1.CurrentRow.Index].Value.ToString();
            agregar.txtReferencia.Text = dataGridView1[4, dataGridView1.CurrentRow.Index].Value.ToString();
            agregar.txtRFC.Text = dataGridView1[5, dataGridView1.CurrentRow.Index].Value.ToString();
            agregar.txtCorreo.Text = dataGridView1[6, dataGridView1.CurrentRow.Index].Value.ToString();
            agregar.textBox1.Text = dataGridView1[8, dataGridView1.CurrentRow.Index].Value.ToString();
            agregar.txtCP.Text = dataGridView1[11, dataGridView1.CurrentRow.Index].Value.ToString();
            agregar.txtAdeudo.Enabled = false;
            agregar.Show();
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
            {
                frmHistorialCompras histo = new frmHistorialCompras();
                histo.lblID.Text = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
                histo.lblNombre.Text = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
                histo.adeudo = Convert.ToDouble(dataGridView1[7, dataGridView1.CurrentRow.Index].Value.ToString());
                histo.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un registro de la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application xla = new Microsoft.Office.Interop.Excel.Application();
            Workbook wb = xla.Workbooks.Add(XlSheetType.xlWorksheet);
            Worksheet ws = (Worksheet)xla.ActiveSheet;

            xla.Visible = true;

            ws.Cells[1, 1] = "PRODUCTO";
            ws.Cells[1, 2] = "ENTRADAS";
            ws.Cells[1, 3] = "FECHA: " + (DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            int linea = dataGridView1.RowCount;
            int cont=0;
            int hoja=0;
            do{
                 if (Convert.ToDouble(dataGridView1[7, cont].Value.ToString()) > 0)
                {
                    ws.Cells[(hoja + 2), 1] = dataGridView1[1, cont].Value.ToString();
                    ws.Cells[(hoja + 2), 2] = "$"+dataGridView1[7, cont].Value.ToString();
                    hoja++;
                }
                
                cont++;
            }while(linea>cont);
               
                
            }

        private void button8_Click(object sender, EventArgs e)
        {
            frmExpedientes expe = new frmExpedientes();            
            expe.Show();
            this.Close();
        }
        }
    }

