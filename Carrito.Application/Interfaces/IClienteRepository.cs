using Carrito.Application.Response;
using Carrito.Domain.Entities;

namespace Carrito.Application.Interfaces
{
    public interface IClienteRepository
    {
        Task<ResponseService<List<Cliente>>> GetAllClientes();
        Task<ResponseService<Cliente>> GetClienteById(int id);
        Task<ResponseService<Cliente>> InsertCliente(Cliente cliente);
        Task<ResponseService<bool>> UpdateCliente(int id, Cliente cliente);
        Task<ResponseService<bool>> DeleteCliente(int id);
    }
}