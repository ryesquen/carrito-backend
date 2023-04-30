using Carrito.Application.Response;
using Carrito.Domain.Entities;

namespace Carrito.Application.Interfaces
{
    public interface IClientesArticuloRepository
    {
        Task<ResponseService<List<ClientesArticulo>>> GetAllClientesArticulos();
        Task<ResponseService<ClientesArticulo>> GetClientesArticuloById(int id);
        Task<ResponseService<ClientesArticulo>> InsertClientesArticulo(ClientesArticulo clientesArticulo);
        Task<ResponseService<bool>> UpdateClientesArticulo(int clienteId, int articuloId, ClientesArticulo clientesArticulo);
        Task<ResponseService<bool>> DeleteClientesArticulo(int id);
    }
}