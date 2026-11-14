namespace PuntoVentaWeb.Models
{
    public class ProduccionRequest
    {
        public string IdProductoFinal { get; set; } = string.Empty;
        public string NombreProductoFinal { get; set; } = string.Empty;
        public double CantidadProducida { get; set; }
        public string IdUsuario { get; set; } = "1";
        public string Usuario { get; set; } = "Panadero";
        public List<InsumoDesgloseDto> InsumosAfectados { get; set; } = new();
    }

    public class InsumoDesgloseDto
    {
        public string IdInsumo { get; set; } = string.Empty;
        public string NombreInsumo { get; set; } = string.Empty;
        public string Unidad { get; set; } = string.Empty;
        public double CantidadUnitaria { get; set; }
        public double CantidadTotalRequerida { get; set; }
        public double StockActual { get; set; }
        public bool StockSuficiente => StockActual >= CantidadTotalRequerida;
    }

    public class ProduccionResultado
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string FolioLote { get; set; } = string.Empty;
    }

    public class ProduccionLoteConsultaDto
    {
        public int Id { get; set; }
        public string Folio { get; set; } = string.Empty;
        public string IdProductoFinal { get; set; } = string.Empty;
        public string ProductoFinal { get; set; } = string.Empty;
        public double CantidadProducida { get; set; }
        public DateTime? Fecha { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public bool Cancelado { get; set; }
        public string Estado => Cancelado ? "CANCELADO" : "ACTIVO";
        public DateTime? FechaCancelado { get; set; }
        public string UsuarioCancelado { get; set; } = string.Empty;
    }
}
