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
    public partial class frmKardex : Form
    {
        private DataSet ds;
        OleDbConnection conectar = new OleDbConnection(Conexion.CadCon);
        OleDbDataAdapter da;
        public String idProducto = "";
        public frmKardex()
        {
            InitializeComponent();
        }

        private void frmKardex_Load(object sender, EventArgs e)
        {
            conectar.Open();
            ds = new DataSet();
            da = new OleDbDataAdapter("select id,IdProducto,Tipo, Descripcion, ExistenciaAntes,ExistenciaDespues,ExistenciaDespues - ExistenciaAntes as Movimiento, Fecha,Precio from Kardex where idProducto='" + idProducto + "';", conectar);
            da.Fill(ds, "Id");
            dataGridView2.DataSource = ds.Tables["Id"];
            dataGridView2.Columns[1].Visible = false;
            dataGridView2.Columns[0].Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ds = new DataSet();
            da = new OleDbDataAdapter("select id,IdProducto,Tipo, Descripcion, ExistenciaAntes,ExistenciaDespues,ExistenciaDespues - ExistenciaAntes as Movimiento, Fecha, Precio from Kardex where idProducto='" + idProducto + "' AND Fecha >=#" + dateTimePicker1.Value.Month.ToString() + "/" + dateTimePicker1.Value.Day.ToString() + "/" + dateTimePicker1.Value.Year.ToString() + " 00:00:00# and Fecha <=#" + dateTimePicker2.Value.Month.ToString() + "/" + dateTimePicker2.Value.Day.ToString() + "/" + dateTimePicker2.Value.Year.ToString() + " 23:59:59# ORDER BY Fecha ASC;", conectar);
            da.Fill(ds, "Id");
            dataGridView2.DataSource = ds.Tables["Id"];
            dataGridView2.Columns[1].Visible = false;
            dataGridView2.Columns[0].Visible = false;
            //
        }
    }
}
