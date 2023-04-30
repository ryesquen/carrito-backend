using Carrito.Application.Interfaces;
using Carrito.Application.Response;
using Carrito.Domain.Entities;
using Carrito.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Carrito.Persistence.Repository
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly CarritoContext _carritoContext;

        public ClienteRepository(CarritoContext carritoContext)
        {
            _carritoContext = carritoContext;
        }
        public async Task<ResponseService<List<Cliente>>> GetAllClientes()
        {
            var response = new ResponseService<List<Cliente>>();
            try
            {
                var list = await _carritoContext.Clientes.ToListAsync();
                if (list is not null) response.Object = list;
                else response.AddInternalServerError("Se produjo un error.");
            }
            catch (Exception ex)
            {
                response.AddInternalServerError(ex.Message);
            }
            return response;
        }
        public async Task<ResponseService<Cliente>> GetClienteById(int id)
        {
            var response = new ResponseService<Cliente>();
            try
            {
                var category = await _carritoContext.Clientes.FirstOrDefaultAsync(c => c.Id == id);
                if (category is not null) response.Object = category;
                else response.AddNotFound("No se encontró el cliente.");
                return response;
            }
            catch (Exception ex)
            {
                response.AddInternalServerError(ex.Message);
            }
            return response;
        }
        public async Task<ResponseService<Cliente>> InsertCliente(Cliente cliente)
        {
            var response = new ResponseService<Cliente>();
            try
            {
                await _carritoContext.AddAsync(cliente);
                var result = await _carritoContext.SaveChangesAsync();
                if (result > 0) response.Object = cliente;
                else response.AddInternalServerError("Error al insertar el Cliente.");
            }
            catch (DbUpdateException ex)
            {
                response.AddInternalServerError(ex.Message);
            }
            return response;
        }
        public async Task<ResponseService<bool>> UpdateCliente(int id, Cliente cliente)
        {
            var response = new ResponseService<bool>();
            try
            {
                if (cliente is null) { response.AddBadRequest("No se mandó el cliente"); }
                if (id != cliente?.Id) { response.AddBadRequest("Los Id´s no corresponden"); }
                if (cliente is not null) _carritoContext.Update(cliente);
                var result = await _carritoContext.SaveChangesAsync();
                if (result > 0) response.Object = cliente is not null;
                else response.AddInternalServerError("Error al actualizar el Cliente.");
            }
            catch (DbUpdateException ex)
            {
                response.AddInternalServerError(ex.Message);
            }
            return response;
        }
        public async Task<ResponseService<bool>> DeleteCliente(int id)
        {
            var response = new ResponseService<bool>();
            try
            {
                var category = await _carritoContext.Clientes.FirstOrDefaultAsync(c => c.Id == id);
                if (category is not null) _carritoContext.Remove(category);
                else response.AddBadRequest("No se encontró el Cliente a eliminar.");
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