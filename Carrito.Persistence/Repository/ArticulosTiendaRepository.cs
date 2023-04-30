using Carrito.Application.Interfaces;
using Carrito.Application.Response;
using Carrito.Domain.Entities;
using Carrito.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Carrito.Persistence.Repository
{
    public class ArticulosTiendaRepository : IArticulosTiendaRepository
    {
        private readonly CarritoContext _carritoContext;
        public ArticulosTiendaRepository(CarritoContext carritoContext)
        {
            _carritoContext = carritoContext;
        }
        public async Task<ResponseService<List<ArticulosTienda>>> GetAllArticulosTiendas()
        {
            var response = new ResponseService<List<ArticulosTienda>>();
            try
            {
                var list = await _carritoContext.ArticulosTiendas.ToListAsync();
                if (list is not null) response.Object = list;
                else response.AddInternalServerError("Se produjo un error.");
            }
            catch (Exception ex)
            {
                response.AddInternalServerError(ex.Message);
            }
            return response;
        }
        public async Task<ResponseService<ArticulosTienda>> GetArticulosTiendaById(int articuloId, int tiendaId)
        {
            var response = new ResponseService<ArticulosTienda>();
            try
            {
                var articuloTienda = await _carritoContext.ArticulosTiendas.FirstOrDefaultAsync(c => c.TiendaId == tiendaId && c.ArticuloId == articuloId);
                if (articuloTienda is not null) response.Object = articuloTienda;
                else response.AddNotFound("No se encontró la relación tienda - artículo.");
                return response;
            }
            catch (Exception ex)
            {
                response.AddInternalServerError(ex.Message);
            }
            return response;
        }
        public async Task<ResponseService<ArticulosTienda>> InsertArticulosTienda(ArticulosTienda articulosTienda)
        {
            var response = new ResponseService<ArticulosTienda>();
            try
            {
                await _carritoContext.AddAsync(articulosTienda);
                var result = await _carritoContext.SaveChangesAsync();
                if (result > 0) response.Object = articulosTienda;
                else response.AddInternalServerError("Error al insertar el artículo - tienda.");
            }
            catch (DbUpdateException ex)
            {
                response.AddInternalServerError(ex.Message);
            }
            return response;
        }
        public async Task<ResponseService<bool>> UpdateArticulosTienda(int articuloId, int tiendaId, ArticulosTienda articulosTienda)
        {
            var response = new ResponseService<bool>();
            try
            {
                if (articulosTienda is null) { response.AddBadRequest("No se mandó: Tienda - Artículo."); }
                if (tiendaId != articulosTienda?.TiendaId || articuloId != articulosTienda.ArticuloId) { response.AddBadRequest("Los Id´s no corresponden"); }
                if (articulosTienda is not null) _carritoContext.Update(articulosTienda);
                var result = await _carritoContext.SaveChangesAsync();
                if (result > 0) response.Object = articulosTienda is not null;
                else response.AddInternalServerError("Error al actualizar: Tienda - artículo.");
            }
            catch (DbUpdateException ex)
            {
                response.AddInternalServerError(ex.Message);
            }
            return response;
        }
        public async Task<ResponseService<bool>> DeleteArticulosTienda(int articuloId, int tiendaId)
        {
            var response = new ResponseService<bool>();
            try
            {
                var articuloTienda = await _carritoContext.ArticulosTiendas.FirstOrDefaultAsync(c => c.TiendaId == tiendaId && c.ArticuloId == articuloId);
                if (articuloTienda is not null) _carritoContext.Remove(articuloTienda);
                else response.AddBadRequest("No se encontró: tienda - artículo a eliminar.");
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