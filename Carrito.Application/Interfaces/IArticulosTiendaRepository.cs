using Carrito.Application.Response;
using Carrito.Domain.Entities;

namespace Carrito.Application.Interfaces
{
    public interface IArticulosTiendaRepository
    {
        Task<ResponseService<List<ArticulosTienda>>> GetAllArticulosTiendas();
        Task<ResponseService<ArticulosTienda>> GetArticulosTiendaById(int articuloId, int tiendaId);
        Task<ResponseService<ArticulosTienda>> InsertArticulosTienda(ArticulosTienda articulosTienda);
        Task<ResponseService<bool>> UpdateArticulosTienda(int articuloId, int tiendaId, ArticulosTienda articulosTienda);
        Task<ResponseService<bool>> DeleteArticulosTienda(int articuloId, int tiendaId);
    }
}