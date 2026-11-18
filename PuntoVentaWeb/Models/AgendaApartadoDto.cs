namespace PuntoVentaWeb.Models
{
    public class AgendaApartadoDto
    {
        public int Id { get; set; }
        public string Folio { get; set; } = "";
        public int IdCliente { get; set; }
        public string NombreCliente { get; set; } = "";
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal Abono { get; set; }
        public decimal Restante { get; set; }
        public string Estatus { get; set; } = "PENDIENTE";
        public List<DetalleApartadoDto> Productos { get; set; } = new();

        public bool EstaLiquidado => Restante <= 0;
        public bool EstaVencido => FechaEntrega.HasValue && FechaEntrega.Value.Date < DateTime.Today && !EstaLiquidado;
        public bool EsHoy => FechaEntrega.HasValue && FechaEntrega.Value.Date == DateTime.Today;
    }

    public class DetalleApartadoDto
    {
        public int Id { get; set; }
        public string IdProducto { get; set; } = "";
        public string Producto { get; set; } = "";
        public string Cantidad { get; set; } = "1";
        public decimal MontoTotal { get; set; }
        public string Estatus { get; set; } = "";
    }
}