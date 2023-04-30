namespace Carrito.Application.DTOs
{
    public class TiendaDto
    {
        public int Id { get; set; }
        public string Sucursal { get; set; } = null!;
        public string Direccion { get; set; } = null!;
    }
}