namespace Carrito.Domain.Entities
{
    public partial class Articulo
    {
        public Articulo()
        {
            ArticulosTienda = new HashSet<ArticulosTienda>();
            ClientesArticulos = new HashSet<ClientesArticulo>();
        }

        public int Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public decimal Precio { get; set; }
        public byte[] Imagen { get; set; } = null!;
        public int Stock { get; set; }

        public virtual ICollection<ArticulosTienda> ArticulosTienda { get; set; }
        public virtual ICollection<ClientesArticulo> ClientesArticulos { get; set; }
    }
}
