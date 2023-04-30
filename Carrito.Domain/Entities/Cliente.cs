using System;
using System.Collections.Generic;

namespace Carrito.Domain.Entities
{
    public partial class Cliente
    {
        public Cliente()
        {
            ClientesArticulos = new HashSet<ClientesArticulo>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Direccion { get; set; } = null!;

        public virtual ICollection<ClientesArticulo> ClientesArticulos { get; set; }
    }
}
