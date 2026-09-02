using Microsoft.Extensions.Logging;
using System.Text.Json;
using PuntoVentaWeb.Models;

namespace PuntoVentaWeb.Services;

/// <summary>
/// Reads the local installation identity from C:\ProgramData\MiSistema\installation.json.
/// This file is created manually (or by a simple script) when configuring each business.
/// It is never overwritten by app updates.
/// </summary>
public class InstalacionService
{
    private const string ArchivoInstalacion = @"C:\ProgramData\MiSistema\installation.json";

    private readonly ILogger<InstalacionService> _logger;
    private readonly Lazy<Instalacion> _instalacion;

    public InstalacionService(ILogger<InstalacionService> logger)
    {
        _logger = logger;
        _instalacion = new Lazy<Instalacion>(CargarDesdeArchivo);
    }

    /// <summary>The current installation identity (cached after first load).</summary>
    public Instalacion Actual => _instalacion.Value;

    private Instalacion CargarDesdeArchivo()
    {
        try
        {
            if (File.Exists(ArchivoInstalacion))
            {
                var json = File.ReadAllText(ArchivoInstalacion);
                var instalacion = JsonSerializer.Deserialize<Instalacion>(json);
                if (instalacion != null)
                {
                    return instalacion;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al leer el archivo de instalación {Archivo}", ArchivoInstalacion);
        }

        // Valor por defecto si el archivo no existe o falló la lectura.
        return new Instalacion
        {
            InstallationId = "SIN_CONFIGURAR",
            BusinessName = "Sin configurar",
            CloudflareHostname = ""
        };
    }
}
