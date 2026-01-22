using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRUNO
{
    class Conexion
    {
        public static string CadCon = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Jaeger Soft\Jaeger.accdb; Jet OLEDB:Database Password=75941232";
        public static string CadSQL = @"Data Source=.\SQLEXPRESS;Initial Catalog=PuntoDeVenta;Integrated Security=True;MultipleActiveResultSets=True;";

        //63public static string CadCon = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.1.1\Jaeger Soft\Jaeger.accdb; Jet OLEDB:Database Password=75941232";
        //public static string CadCon = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Respaldo\Joyeria.accdb";
        //public static string CadCon = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\192.168.9.101\Jaeger Soft\Joyeria.accdb";
        //public static string lugar = "LOCAL";

        //public static string lugar = "PLAZA";
        //public static string lugar = "CAPTURA";
        //==================================   
        //SAN JUAN
        //public static string lugar = "SANJUAN";
        //public static int MaxChar = 32;
        //public static int FontSize = 9;
        //public static int MaxCharDescription = 16;
        //public static string[] datosTicket = new string[] { "BÉLGICA ABURTO AMARO", "RFC AUAB790326RD4", "AV. MAXIMINO AVILA CAMACHO #500", "CENTRO SAN JUAN XIUTETELCO", "PUE CP73970 Tel (226) 3183839" };
        //public static string[] pieDeTicket = new string[] { "No sé admiten devoluciones ni cambios", "La mercancía de materiales para la construcción, se dejará a pie de calle", "**GRACIAS POR SU COMPRA**" };
        //public static string Font = "";
        //public static string impresora = "print";
        //SANJUAN2
        //public static string lugar = "SANJUAN";
        //public static int MaxChar = 32;
        //public static int FontSize = 9;
        //public static int MaxCharDescription = 16;
        //public static string[] datosTicket = new string[] { "Refacciones, Muelles y Tornillos en General de San Juan", "Telefono: 2263182925" };
        //public static string[] pieDeTicket = new string[] { "**GRACIAS POR SU COMPRA**" };
        //public static string Font = "";
        //public static string impresora = "print";

        //DEPORTES LEO
        //public static string lugar = "LEO";
        //public static int MaxChar = 58;
        //public static int MaxCharDescription = 20;
        //public static int FontSize = 9;
        //public static string[] datosTicket = new string[] { "  DEPORTES Y NOVEDADES LEO", "    R.F.C. PELR550819AX9", "  AV. Hidalgo #1006 Col. Centro", "    C.P 73800", "    TEL. (231) 31 3 24 84", "    Teziutlán Puebla México" };
        //public static string[] pieDeTicket = new string[] { "**GRACIAS POR SU COMPRA**" };
        //public static string Font = "calibri";
        //public static string impresora = "print";
        //Refacciones, Muelles y Tornillos en General de San Juan

        //ZAPATERIA
        //public static string lugar = "ZAPATERIA";
        //public static int MaxChar = 32;
        //public static int FontSize = 9;
        //public static int MaxCharDescription = 16;
        //public static string[] datosTicket = new string[] { "", "", "", "" };
        //public static string[] pieDeTicket = new string[] { "No sé admiten devoluciones ni cambios", "", "**GRACIAS POR SU COMPRA**" };
        //public static string Font = "";
        //public static string impresora = "print";


        //PASTELES DIAZ
        //public static string lugar = "PASTELES DIAZ";
        //public static int MaxChar = 30;
        //public static int FontSize = 9;
        //public static int MaxCharDescription = 16;
        //public static string[] datosTicket = new string[] { "PASTELES DÍAZ", "", "", "" };
        //public static string[] pieDeTicket = new string[] { "No sé admiten devoluciones ni cambios", "", "**GRACIAS POR SU COMPRA**" };
        //public static string Font = "";
        //public static string impresora = "print";

        //PESCADERIA
        //public static string lugar = "LAGUNA DE TAMIAHUA";
        //public static int MaxChar = 32;
        //public static int FontSize = 9;
        //public static int MaxCharDescription = 16;
        //public static string[] datosTicket = new string[] { "LAGUNA DE TAMIAHUA", "", "", "" };
        //public static string[] pieDeTicket = new string[] { "No sé admiten devoluciones ni cambios", "", "**GRACIAS POR SU COMPRA**" };
        //public static string Font = "";
        //public static string impresora = "print";

        //CROQUETE
        //public static string lugar = "PINTURAS";
        //
        //public static string[] datosTicket = new string[] { "        TERESA REYES HERNANDEZ",
        //                                            "  Av. Cuauhtémoc No. 501 Col. Centro",
        //                                            "   Tels. 231 31 2 0176 y 231 31 223 33",
        //                                            "             Whatsapp 222 586 60 60",
        //                                            "           Teziutlán, Pue. C.P. 73800",
        //                                            "             R.F.C. REHT4908096N1"};
        //public static string[] pieDeTicket = new string[] { "ESTO NO ES UN COMPROBANTE FISCAL", "      **GRACIAS POR SU COMPRA**" };



        //public static string lugar = "CROQUETAS";
        //public static string[] datosTicket = new string[] { "Distribuidora de alimentos Can & cat",
        //                                             "     Hidalgo 3-A barrio de Ahuateno ",
        //                                             "        Móvil 2312262873",
        //                                             "        Teziutlan Puebla"};
        //public static string[] pieDeTicket = new string[] { "ESTO NO ES UN COMPROBANTE FISCAL", "      **GRACIAS POR SU COMPRA**" };

        //public static string lugar = "DEPORTES LEO";
        //public static string[] datosTicket = new string[] { "      DEPORTES Y NOVEDADES LEO",
        //                                             "            R.F.C. PELR550819AX9 ",
        //                                             "         AV. Hidalgo #1006 Col. Centro",
        //                                             "        C.P. 73800 Tel.(231) 31 3 24 84",
        //                                             "              Teziutlan Puebla"};
        //public static string[] pieDeTicket = new string[] { "ESTO NO ES UN COMPROBANTE FISCAL", "      **GRACIAS POR SU COMPRA**" };
        //public static string logoPath = @"C:\Jaeger Soft\logo.jpg";
        //public static string Font = "";
        //public static string impresora = "print";
        //public static int MaxChar = 32;
        //public static int FontSize = 9;
        //public static int MaxCharDescription = 16;
        //public static bool ConIva = false;


        public static string lugar;
        public static string[] datosTicket;
        public static string[] pieDeTicket;
        public static string logoPath;
        public static string Font;
        public static string impresora;
        public static int MaxChar;
        public static int FontSize;
        public static int MaxCharDescription;
        public static bool ConIva;
        public static bool impresionMediaCarta;
        public static string Whatsapp;

        public static void CargarConfiguracion(int id)
        {
            using (var conexion = new OleDbConnection(CadCon))
            {
                conexion.Open();
                string query = "SELECT * FROM Configuracion WHERE Id = @id";
                using (var cmd = new OleDbCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lugar = reader["Lugar"].ToString();
                            Font = reader["Font"].ToString();
                            impresora = reader["Impresora"].ToString();
                            MaxChar = Convert.ToInt32(reader["MaxChar"]);
                            FontSize = Convert.ToInt32(reader["FontSize"]);
                            MaxCharDescription = Convert.ToInt32(reader["MaxCharDescription"]);
                            ConIva = Convert.ToBoolean(reader["ConIva"]);
                            logoPath = reader["logoPath"].ToString();
                            datosTicket = reader["DatosTicket"].ToString().Split('|');
                            pieDeTicket = reader["PieDeTicket"].ToString().Split('|');
                            impresionMediaCarta = Convert.ToBoolean(reader["MediaCarta"]);
                            Whatsapp = reader["PieDeTicket"].ToString();

                        }
                    }
                }
            }
        }
    }
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          