using Carrito.Application.Interfaces;
using Carrito.Application.Response;
using Carrito.Domain.Entities;
using Carrito.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Carrito.Persistence.Repository
{
    public class ArticuloRepository : IArticuloRepository
    {
        private readonly CarritoContext _carritoContext;
        public ArticuloRepository(CarritoContext carritoContext)
        {
            _carritoContext = carritoContext;
        }
        public async Task<ResponseService<List<Articulo>>> GetAllArticulos()
        {
            var response = new ResponseService<List<Articulo>>();
            try
            {
                var list = await _carritoContext.Articulos.ToListAsync();
                if (list is not null) response.Object = list;
                else response.AddInternalServerError("Se produjo un error.");
            }
            catch (Exception ex)
            {
                response.AddInternalServerError(ex.Message);
            }
            return response;
        }
        public async Task<ResponseService<Articulo>> GetArticuloById(int id)
        {
            var response = new ResponseService<Articulo>();
            try
            {
                var articulo = await _carritoContext.Articulos.FirstOrDefaultAsync(c => c.Id == id);
                if (articulo is not null) response.Object = articulo;
                else response.AddNotFound("No se encontró el artículo.");
                return response;
            }
            catch (Exception ex)
            {
                response.AddInternalServerError(ex.Message);
            }
            return response;
        }
        public async Task<ResponseService<Articulo>> InsertArticulo(Articulo articulo)
        {
            var response = new ResponseService<Articulo>();
            try
            {
                await _carritoContext.AddAsync(articulo);
                var result = await _carritoContext.SaveChangesAsync();
                if (result > 0) response.Object = articulo;
                else response.AddInternalServerError("Error al insertar el artículo.");
            }
            catch (DbUpdateException ex)
            {
                response.AddInternalServerError(ex.Message);
            }
            return response;
        }
        public async Task<ResponseService<bool>> UpdateArticulo(int id, Articulo articulo)
        {
            var response = new ResponseService<bool>();
            try
            {
                if (articulo is null) { response.AddBadRequest("No se mandó el artículo"); }
                if (id != articulo?.Id) { response.AddBadRequest("Los Id´s no corresponden"); }
                if (articulo is not null) _carritoContext.Update(articulo);
                var result = await _carritoContext.SaveChangesAsync();
                if (result > 0) response.Object = articulo is not null;
                else response.AddInternalServerError("Error al actualizar el artículo.");
            }
            catch (DbUpdateException ex)
            {
                response.AddInternalServerError(ex.Message);
            }
            return response;
        }
        public async Task<ResponseService<bool>> DeleteArticulo(int id)
        {
            var response = new ResponseService<bool>();
            try
            {
                var category = await _carritoContext.Articulos.FirstOrDefaultAsync(c => c.Id == id);
                if (category is not null) _carritoContext.Remove(category);
                else response.AddBadRequest("No se encontró el artículo a eliminar.");
                await _carritoContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                response.AddInternalServerError(ex.Message);
            }
            return response;
        }
    }
}