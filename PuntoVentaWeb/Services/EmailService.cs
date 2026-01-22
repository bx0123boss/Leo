using System.Net;
using System.Net.Mail;
using System.Net.Mime; // Necesario para incrustar imágenes
using System.Text;
using PuntoVentaWeb.Models;

namespace PuntoVentaWeb.Services;

public class EmailService
{
    private readonly CotizacionService _cotizacionService;

    // RUTAS A TUS ARCHIVOS LOCALES
    private const string RUTA_LOGO_LOCAL = @"C:\Jaeger Soft\logo.png";
    private const string RUTA_WHATSAPP_LOCAL = @"C:\Jaeger Soft\whatsapp.png"; 


    public EmailService(CotizacionService cotizacionService)
    {
        _cotizacionService = cotizacionService;
    }

    public async Task<(bool Exito, string Mensaje)> EnviarCotizacionConPdf(
         string destinatario,
         string nombreCliente,
         string asunto,
         byte[] pdfBytes,
         int folio)
    {
        try
        {
            var config = await _cotizacionService.ObtenerConfiguracion();

            if (string.IsNullOrEmpty(config.CorreoEmisor) || string.IsNullOrEmpty(config.PasswordEmisor))
            {
                return (false, "Falta configurar el correo emisor.");
            }

            // Nota: Si usas Outlook, cambia a smtp.office365.com
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(config.CorreoEmisor, config.PasswordEmisor),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(config.CorreoEmisor, config.NombreLugar ?? "Atención a Clientes"),
                Subject = asunto,
                IsBodyHtml = true
            };

            mailMessage.To.Add(destinatario);

            // 1. Generar el HTML
            string htmlBody = GenerarHtmlProfesional(nombreCliente, folio, config);

            // 2. Crear la Vista HTML
            // CORRECCIÓN AQUÍ: Usamos "text/html" manual o MediaTypeNames.Text.Html si funciona
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, Encoding.UTF8, "text/html");

            // 3. INCRUSTAR LOGO (Si existe)
            if (File.Exists(RUTA_LOGO_LOCAL))
            {
                // CORRECCIÓN: Usamos "image/png" manualmente
                LinkedResource logoRes = new LinkedResource(RUTA_LOGO_LOCAL, "image/png");
                logoRes.ContentId = "LogoImage";
                htmlView.LinkedResources.Add(logoRes);
            }

            // 4. INCRUSTAR WHATSAPP (Si existe)
            if (File.Exists(RUTA_WHATSAPP_LOCAL))
            {
                // CORRECCIÓN: Usamos "image/png" manualmente
                LinkedResource waRes = new LinkedResource(RUTA_WHATSAPP_LOCAL, "image/png");
                waRes.ContentId = "WhatsappImage";
                htmlView.LinkedResources.Add(waRes);
            }

            mailMessage.AlternateViews.Add(htmlView);

            // 5. Adjuntar PDF
            using (var stream = new MemoryStream(pdfBytes))
            {
                var attachment = new Attachment(stream, $"Cotizacion_{folio}.pdf", "application/pdf");
                mailMessage.Attachments.Add(attachment);

                await smtpClient.SendMailAsync(mailMessage);
            }

            return (true, "Enviado correctamente");
        }
        catch (Exception ex)
        {
            return (false, $"Error SMTP: {ex.Message}");
        }
    }
    private string GenerarHtmlProfesional(string cliente, int folio, ConfiguracionNegocio config)
    {
        // Datos del negocio para el pie
        string footerText = "";
        if (config.PieDeTicket != null)
            footerText = string.Join("<br>", config.PieDeTicket.Select(linea => WebUtility.HtmlEncode(linea)));

        string nombreNegocio = WebUtility.HtmlEncode(config.NombreLugar ?? "Su Proveedor de Confianza");
        string nombreCliente = WebUtility.HtmlEncode(string.IsNullOrEmpty(cliente) ? "Cliente" : cliente);
        string numeroWhatsapp = !string.IsNullOrEmpty(config.Whatsapp) ? config.Whatsapp : "520000000000";
        // HTML DISEÑADO
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <style>
                body {{ font-family: 'Segoe UI', Arial, sans-serif; color: #444; background-color: #f6f6f6; margin: 0; padding: 0; }}
                .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
                .header {{ background-color: #0056b3; padding: 20px; text-align: center; color: white; }}
                .content {{ padding: 30px 25px; }}
                .btn-pdf {{ display: inline-block; background-color: #dc3545; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; font-weight: bold; font-size: 14px; }}
                .footer {{ background-color: #f4f4f4; padding: 20px; border-top: 3px solid #333; font-size: 13px; color: #666; }}
                .wa-btn {{ text-decoration: none; color: #25D366; font-weight: bold; display: flex; align-items: center; justify-content: center; }}
            </style>
        </head>
        <body>
            <div style='padding: 20px;'>
                <div class='container'>
                    
                    <div class='header'>
                        <h2 style='margin:0;'>Nueva Cotización</h2>
                        <p style='margin: 5px 0 0 0; opacity: 0.9;'>Folio #{folio}</p>
                    </div>

                    <div class='content'>
                        <h3 style='color: #333; margin-top: 0;'>Hola, {nombreCliente}.</h3>
                        
                        <p>Es un gusto saludarle. Adjunto a este correo encontrará el documento PDF con los detalles de la cotización que solicitó.</p>
                        
                        <div style='background-color: #e9ecef; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                            <strong>Resumen:</strong><br>
                            📄 Cotización #{folio}<br>
                            📅 Fecha: {DateTime.Now:dd/MM/yyyy}<br>
                        </div>

                        <p>Quedamos atentos a cualquier duda o comentario que tenga sobre esta propuesta.</p>
                        
                        <br>
                        <p style='text-align: center; color: #999; font-size: 12px;'>
                            (Abra el archivo adjunto para ver los precios)
                        </p>
                    </div>

                    <div class='footer'>
                        <table width='100%' border='0'>
                            <tr>
                                <td width='30%' align='center' valign='top'>
                                    <img src='cid:LogoImage' width='90' style='border-radius: 50%; border: 2px solid #ddd; display:block;'>
                                    <div style='margin-top:5px; font-weight:bold; color:#333; font-size:11px;'>{nombreNegocio.ToUpper()}</div>
                                </td>
                                
                                <td width='70%' valign='top' style='padding-left: 15px; border-left: 1px solid #ddd;'>
                                    
                                    <div style='margin-bottom: 10px;'>
                                        <a href='https://wa.me/{numeroWhatsapp}?text=Hola,%20tengo%20dudas%20sobre%20la%20cotización%20{folio}' style='text-decoration:none; color:#2E7D32; font-weight:bold; font-size: 14px;'>
                                            <img src='cid:WhatsappImage' width='20' style='vertical-align: middle; margin-right: 5px;' border='0'>
                                            Escríbenos por WhatsApp
                                        </a>
                                    </div>

                                    {footerText}
                                    
                                    <br><br>
                                    <span style='color: #999; font-size: 11px;'>© {DateTime.Now.Year} {nombreNegocio}</span>
                                </td>
                            </tr>
                        </table>
                    </div>

                </div>
            </div>
        </body>
        </html>
        ";
    }
}