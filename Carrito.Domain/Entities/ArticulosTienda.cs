namespace Carrito.Domain.Entities
{
    public partial class ArticulosTienda
    {
        public int ArticuloId { get; set; }
        public int TiendaId { get; set; }
        public DateTime Fecha { get; set; }

        public virtual Articulo Articulo { get; set; } = null!;
        public virtual Tienda Tienda { get; set; } = null!;
    }
}
