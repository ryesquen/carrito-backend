namespace Carrito.Application.DTOs
{
    public class ArticulosTiendaDto
    {
        public int ArticuloId { get; set; }
        public int TiendaId { get; set; }
        public string? ArticuloCodigo { get; set; } = null;
        public int ArticuloStock { get; set; }
        public string? TiendaSucursal { get; set; } = null;
        public DateTime? Fecha { get; set; }
    }
}