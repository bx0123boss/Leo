using System.ComponentModel.DataAnnotations;

namespace PuntoVentaWeb.Models;

public class Cliente
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "El teléfono es obligatorio")]
    public string Telefono { get; set; }

    public string Direccion { get; set; }

    public string Referencia { get; set; }

    [Required(ErrorMessage = "El RFC es obligatorio")]
    public string RFC { get; set; }

    [EmailAddress(ErrorMessage = "Formato de correo inválido")]
    public string Correo { get; set; }
    public decimal Adeudo { get; set; }
    public decimal Limite { get; set; }
    public DateTime UltimoPago { get; set; }
    public string Estatus { get; set; }
}