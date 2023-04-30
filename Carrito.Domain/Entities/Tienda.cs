namespace Carrito.Domain.Entities
{
    public partial class Tienda
    {
        public Tienda()
        {
            ArticulosTienda = new HashSet<ArticulosTienda>();
        }

        public int Id { get; set; }
        public string Sucursal { get; set; } = null!;
        public string Direccion { get; set; } = null!;

        public virtual ICollection<ArticulosTienda> ArticulosTienda { get; set; }
    }
}
