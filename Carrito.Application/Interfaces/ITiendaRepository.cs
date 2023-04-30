using Carrito.Application.Response;
using Carrito.Domain.Entities;

namespace Carrito.Application.Interfaces
{
    public interface ITiendaRepository
    {
        Task<ResponseService<List<Tienda>>> GetAllTiendas();
        Task<ResponseService<Tienda>> GetTiendaById(int id);
        Task<ResponseService<Tienda>> InsertTienda(Tienda tienda);
        Task<ResponseService<bool>> UpdateTienda(int id, Tienda tienda);
        Task<ResponseService<bool>> DeleteTienda(int id);
    }
}