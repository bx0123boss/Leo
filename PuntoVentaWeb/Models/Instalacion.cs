namespace PuntoVentaWeb.Models;

/// <summary>
/// Represents the local installation identity of this deployment.
/// Stored in C:\ProgramData\MiSistema\installation.json
/// This identifies which business/installation this server serves.
/// Does NOT contain sensitive Cloudflare credentials.
/// </summary>
public class Instalacion
{
    /// <summary>Unique identifier for this installation (e.g. A7F3D9C2). Independent of local IP.</summary>
    public string InstallationId { get; set; } = "";

    /// <summary>Name of the business (e.g. "Restaurante ABC").</summary>
    public string BusinessName { get; set; } = "";

    /// <summary>Public hostname assigned to this installation (e.g. app1.jaeger-soft.com).</summary>
    public string CloudflareHostname { get; set; } = "";
}
