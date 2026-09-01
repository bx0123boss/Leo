using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using PuntoVentaWeb.Models;

namespace PuntoVentaWeb.Services
{
    public class WhatsAppService
    {
        private readonly HttpClient _http;

        public WhatsAppService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ResultadoEnvio> EnviarMensaje(
            ConfiguracionNegocio config,
            string telefono,
            string mensaje)
        {
            try
            {
                // Normalizamos el teléfono
                telefono = telefono.Replace("+", "")
                                   .Replace(" ", "")
                                   .Replace("-", "")
                                   .Replace("(", "")
                                   .Replace(")", "");

                var url =
                    $"https://graph.facebook.com/{config.WhatsAppApiVersion}/{config.WhatsAppPhoneNumberId}/messages";

                _http.DefaultRequestHeaders.Clear();
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        config.WhatsAppAccessToken);
                telefono = new string(telefono.Where(char.IsDigit).ToArray());

                if (telefono.Length == 10)
                    telefono = "52" + telefono;

                var body = new
                {
                    messaging_product = "whatsapp",
                    to = telefono,
                    type = "template",
                    template = new
                    {
                        name = "hello_world",
                        language = new
                        {
                            code = "en_US"
                        }
                    }
                };

                var json = JsonSerializer.Serialize(body);

                using var content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");

                var response = await _http.PostAsync(url, content);

                var respuesta = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new ResultadoEnvio
                    {
                        Exito = false,
                        Mensaje = respuesta
                    };
                }

                return new ResultadoEnvio
                {
                    Exito = true,
                    Mensaje = respuesta
                };
            }
            catch (Exception ex)
            {
                return new ResultadoEnvio
                {
                    Exito = false,
                    Mensaje = ex.ToString()
                };
            }
        }

        private async Task<string?> SubirDocumento(ConfiguracionNegocio config,byte[] pdf,string nombreArchivo)
        {
            try
            {
                var url =
                    $"https://graph.facebook.com/{config.WhatsAppApiVersion}/{config.WhatsAppPhoneNumberId}/media";

                _http.DefaultRequestHeaders.Clear();
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", config.WhatsAppAccessToken);

                using var form = new MultipartFormDataContent();

                var archivo = new ByteArrayContent(pdf);
                archivo.Headers.ContentType =
                    new MediaTypeHeaderValue("application/pdf");

                form.Add(archivo, "file", nombreArchivo);
                form.Add(new StringContent("whatsapp"), "messaging_product");

                var response = await _http.PostAsync(url, form);

                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return null;

                using var doc = JsonDocument.Parse(json);

                return doc.RootElement
                          .GetProperty("id")
                          .GetString();
            }
            catch
            {
                return null;
            }
        }
        public async Task<ResultadoEnvio> EnviarDocumento(
    ConfiguracionNegocio config,
    string telefono,
    byte[] pdf,
    string nombreArchivo,
    string caption)
        {
            telefono = new string(telefono.Where(char.IsDigit).ToArray());

            if (telefono.Length == 10)
                telefono = "52" + telefono;

            var mediaId =
                await SubirDocumento(config, pdf, nombreArchivo);

            if (string.IsNullOrEmpty(mediaId))
            {
                return new ResultadoEnvio
                {
                    Exito = false,
                    Mensaje = "No se pudo subir el PDF."
                };
            }

            var url =
                $"https://graph.facebook.com/{config.WhatsAppApiVersion}/{config.WhatsAppPhoneNumberId}/messages";

            _http.DefaultRequestHeaders.Clear();

            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    config.WhatsAppAccessToken);

            var body = new
            {
                messaging_product = "whatsapp",
                to = telefono,
                type = "document",
                document = new
                {
                    id = mediaId,
                    filename = nombreArchivo,
                    caption = caption
                }
            };

            var json = JsonSerializer.Serialize(body);

            var response = await _http.PostAsync(
                url,
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"));

            var respuesta = await response.Content.ReadAsStringAsync();

            return new ResultadoEnvio
            {
                Exito = response.IsSuccessStatusCode,
                Mensaje = respuesta
            };
        }
    }
}