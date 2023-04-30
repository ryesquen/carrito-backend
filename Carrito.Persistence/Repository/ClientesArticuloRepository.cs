using Carrito.Application.DTOs;
using Carrito.Application.Interfaces;
using Carrito.Application.Response;
using Carrito.Domain.Entities;
using Carrito.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Carrito.Persistence.Repository
{
    public class ClientesArticuloRepository : IClientesArticuloRepository
    {
        private readonly CarritoContext _carritoContext;
        public ClientesArticuloRepository(CarritoContext carritoContext)
        {
            _carritoContext = carritoContext;
        }
        public async Task<ResponseService<List<ClientesArticulo>>> GetAllClientesArticulos()
        {
            var response = new ResponseService<List<ClientesArticulo>>();
            try
            {
                var list = await _carritoContext.ClientesArticulos.ToListAsync();
                if (list is not null) response.Object = list;
                else response.AddInternalServerError("Se produjo un error.");
            }
            catch (Exception ex)
            {
                response.AddInternalServerError(ex.Message);
            }
            return response;
        }
        public async Task<ResponseService<ClientesArticulo>> GetClientesArticuloById(int id)
        {
            var response = new ResponseService<ClientesArticulo>();
            try
            {
                var clienteArticulo = await _carritoContext.ClientesArticulos.FirstOrDefaultAsync(c => c.Id == id);
                if (clienteArticulo is not null) response.Object = clienteArticulo;
                else response.AddNotFound("No se encontró la relación cliente - artículo.");
                return response;
            }
            catch (Exception ex)
            {
                response.AddInternalServerError(ex.Message);
            }
            return response;
        }
        public async Task<ResponseService<ClientesArticulo>> InsertClientesArticulo(ClientesArticulo clientesArticulo)
        {
            var response = new ResponseService<ClientesArticulo>();
            try
            {
                await _carritoContext.AddAsync(clientesArticulo);
                var result = await _carritoContext.SaveChangesAsync();
                if (result > 0) response.Object = clientesArticulo;
                else response.AddInternalServerError("Error al insertar el cliente - artículo.");
            }
            catch (DbUpdateException ex)
            {
                response.AddInternalServerError(ex.Message);
            }
            return response;
        }
        public async Task<ResponseService<bool>> UpdateClientesArticulo(int clienteId, int articuloId, ClientesArticulo clientesArticulo)
        {
            var response = new ResponseService<bool>();
            try
            {
                if (clientesArticulo is null) { response.AddBadRequest("No se mandó: Cliente - Artículo."); }
                if (clienteId != clientesArticulo?.ClienteId || articuloId != clientesArticulo.ArticuloId) { response.AddBadRequest("Los Id´s no corresponden"); }
                if (clientesArticulo is not null) _carritoContext.Update(clientesArticulo);
                var result = await _carritoContext.SaveChangesAsync();
                if (result > 0) response.Object = clientesArticulo is not null;
                else response.AddInternalServerError("Error al actualizar: Cliente - Artículo..");
            }
            catch (DbUpdateException ex)
            {
                response.AddInternalServerError(ex.Message);
            }
            return response;
        }
        public async Task<ResponseService<bool>> DeleteClientesArticulo(int id)
        {
            var response = new ResponseService<bool>();
            try
            {
                var clienteArticulo = await _carritoContext.ClientesArticulos.FirstOrDefaultAsync(c => c.Id == id);
                if (clienteArticulo is not null) _carritoContext.Remove(clienteArticulo);
                else response.AddBadRequest("No se encontró: cliente - artículo a eliminar.");
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