using System;
using System.IO.Ports;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AgenteBasculaTorrey
{
    class Program
    {
        static bool USAR_MOCK = true;
        const string PUERTO_COM = "COM3";

        static async Task Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (bool.TryParse(args[0], out bool paramMock))
                {
                    USAR_MOCK = paramMock;
                }
            }

            Console.Title = USAR_MOCK
                ? "Agente Báscula Torrey - MODO SIMULADOR (MOCK)"
                : "Agente Báscula Torrey - PRODUCCIÓN";

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/peso/");
            listener.Start();

            Console.WriteLine("=============================================");
            if (USAR_MOCK)
            {
                Console.WriteLine("  SIMULADOR DE BÁSCULA INICIADO (MOCK MODE)");
                Console.WriteLine("  Siempre responderá un peso fijo de 0.500 kg");
            }
            else
            {
                Console.WriteLine("  AGENTE BÁSCULA INICIADO (MODO PRODUCCIÓN)");
                Console.WriteLine($"  Intentando leer hardware por el puerto: {PUERTO_COM}");
            }
            Console.WriteLine("=============================================");
            Console.WriteLine("Escuchando en: http://localhost:8080/peso/");
            Console.WriteLine("Si la báscula falla o marca 0, se enviará '1'");
            Console.WriteLine("=============================================\n");

            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                ProcesarPeticion(context);
            }
        }

        static void ProcesarPeticion(HttpListenerContext context)
        {
            HttpListenerResponse response = context.Response;
            response.AppendHeader("Access-Control-Allow-Origin", "*");

            // Valor por defecto: si todo falla, devolvemos 1 para no afectar la venta
            string pesoFinal = "1";

            if (USAR_MOCK)
            {
                pesoFinal = "0.500";
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] - Petición recibida (MOCK). Retornando: {pesoFinal} kg");
            }
            else
            {
                try
                {
                    using (SerialPort puerto = new SerialPort(PUERTO_COM, 9600, Parity.None, 8, StopBits.One))
                    {
                        puerto.ReadTimeout = 1000;
                        puerto.WriteTimeout = 1000;
                        puerto.Open();
                        puerto.Write("P");
                        System.Threading.Thread.Sleep(150);

                        string respuestaBascula = puerto.ReadExisting();
                        Match m = Regex.Match(respuestaBascula, @"[0-9]*\.[0-9]+");

                        if (m.Success)
                        {
                            double valorLeido = double.Parse(m.Value, System.Globalization.CultureInfo.InvariantCulture);

                            // Si el peso es 0 o menor, devolvemos 1
                            if (valorLeido <= 0)
                            {
                                pesoFinal = "1";
                                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] - Peso en cero. Retornando respaldo: 1");
                            }
                            else
                            {
                                pesoFinal = m.Value;
                                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] - Éxito. Retornando: {pesoFinal} kg");
                            }
                        }
                        else
                        {
                            // No se encontró formato numérico (Báscula enviando basura o error)
                            pesoFinal = "1";
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] - Respuesta inválida. Retornando respaldo: 1");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Error de hardware (Cable desconectado, puerto ocupado)
                    pesoFinal = "1";
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] - Error de conexión: {ex.Message}. Retornando respaldo: 1");
                }
            }

            byte[] buffer = Encoding.UTF8.GetBytes(pesoFinal);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }
    }
}