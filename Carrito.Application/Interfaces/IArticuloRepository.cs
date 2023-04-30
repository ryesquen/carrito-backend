using Carrito.Application.Response;
using Carrito.Domain.Entities;

namespace Carrito.Application.Interfaces
{
    public interface IArticuloRepository
    {
        Task<ResponseService<List<Articulo>>> GetAllArticulos();
        Task<ResponseService<Articulo>> GetArticuloById(int id);
        Task<ResponseService<Articulo>> InsertArticulo(Articulo articulo);
        Task<ResponseService<bool>> UpdateArticulo(int id, Articulo articulo);
        Task<ResponseService<bool>> DeleteArticulo(int id);
    }
}