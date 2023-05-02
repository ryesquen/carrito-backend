namespace Carrito.Application.DTOs
{
    public class ClientesArticuloDto
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string? ClienteNombre { get; set; }
        public string? ClienteApellido { get; set; }
        public int ArticuloId { get; set; }
        public string? ArticuloCodigo { get; set; }
        public string? ArticuloDescripcion { get; set; }
        public DateTime? Fecha { get; set; }
    }
}