namespace Carrito.Domain.Entities
{
    public partial class ClientesArticulo
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int ArticuloId { get; set; }
        public DateTime Fecha { get; set; }

        public virtual Articulo Articulo { get; set; } = null!;
        public virtual Cliente Cliente { get; set; } = null!;
    }
}
