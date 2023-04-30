using Carrito.Application.Interfaces;
using Carrito.Application.Response;
using Carrito.Domain.Entities;
using Carrito.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Carrito.Persistence.Repository
{
    public class TiendaRepository : ITiendaRepository
    {
        private readonly CarritoContext _carritoContext;
        public TiendaRepository(CarritoContext carritoContext)
        {
            _carritoContext = carritoContext;
        }
        public async Task<ResponseService<List<Tienda>>> GetAllTiendas()
        {
            var response = new ResponseService<List<Tienda>>();
            try
            {
                var list = await _carritoContext.Tiendas.ToListAsync();
                if (list is not null) response.Object = list;
                else response.AddInternalServerError("Se produjo un error.");
            }
            catch (Exception ex)
            {
                response.AddInternalServerError(ex.Message);
            }
            return response;
        }
        public async Task<ResponseService<Tienda>> GetTiendaById(int id)
        {
            var response = new ResponseService<Tienda>();
            try
            {
                var tienda = await _carritoContext.Tiendas.FirstOrDefaultAsync(c => c.Id == id);
                if (tienda is not null) response.Object = tienda;
                else response.AddNotFound("No se encontró la tienda.");
                return response;
            }
            catch (Exception ex)
            {
                response.AddInternalServerError(ex.Message);
            }
            return response;
        }
        public async Task<ResponseService<Tienda>> InsertTienda(Tienda tienda)
        {
            var response = new ResponseService<Tienda>();
            try
            {
                await _carritoContext.AddAsync(tienda);
                var result = await _carritoContext.SaveChangesAsync();
                if (result > 0) response.Object = tienda;
                else response.AddInternalServerError("Error al insertar al tienda.");
            }
            catch (DbUpdateException ex)
            {
                response.AddInternalServerError(ex.Message);
            }
            return response;
        }
        public async Task<ResponseService<bool>> UpdateTienda(int id, Tienda tienda)
        {
            var response = new ResponseService<bool>();
            try
            {
                if (tienda is null) { response.AddBadRequest("No se mandó la tienda"); }
                if (id != tienda?.Id) { response.AddBadRequest("Los Id´s no corresponden"); }
                if (tienda is not null) _carritoContext.Update(tienda);
                var result = await _carritoContext.SaveChangesAsync();
                if (result > 0) response.Object = tienda is not null;
                else response.AddInternalServerError("Error al actualizar la tienda.");
            }
            catch (DbUpdateException ex)
            {
                response.AddInternalServerError(ex.Message);
            }
            return response;
        }
        public async Task<ResponseService<bool>> DeleteTienda(int id)
        {
            var response = new ResponseService<bool>();
            try
            {
                var tienda = await _carritoContext.Tiendas.FirstOrDefaultAsync(c => c.Id == id);
                if (tienda is not null) _carritoContext.Remove(tienda);
                else response.AddBadRequest("No se encontró la tienda a eliminar.");
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